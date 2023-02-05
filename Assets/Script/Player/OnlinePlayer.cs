using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyInitSet;
using System;

namespace FantacticsScripts
{
    public class OnlinePlayer : MonoBehaviour
    {
        public int PlayerID { get; set; }
        public int Team { get; private set; }
        Action process;
        Client client;

        bool signal = false;
        int currentSquare;
        int currentSegment;
        PhaseEnum currentPhase;
        [SerializeField] PhaseNotice phaseNotice = default;
        [SerializeField] Board board = default;
        [SerializeField] GameObject directionUI = default;

        //MovePhase
        [SerializeField] CameraManager cameraManager = default;
        [SerializeField] UIManager uiManager = default;

        BoardDirection[] moveDirectionHistories = new BoardDirection[6]; //chara毎に大きさ変える
        BoardDirection moveDirection;
        int mobility;
        int numberOfMoves;
        bool IsDuringTheMove;
        float moveAmountBetweenSquares;
        float charaMoveSpeed = 1f;

        //PlottingPhase
        int numberOfHands = 0;
        Card heldCard;
        Card selectedCard;
        Card[] actions = new Card[2];
        [SerializeField] BoxCollider2D[] cardFrames = default; //colliderに
        [SerializeField] RectTransform centerOfHandObjects = default;
        Vector3 clickPoint;

        //RangePhase
        int startSquare;
        int targetSquare;
        CardInfomation usedCardInformation;
        bool decideTarget;

        //CardOperation
        Character character;
        [SerializeField] Card[] handObjects; //一番手札が多いキャラクターに合わせる
        int deck;
        int hands;
        int discards;
        int exclusionCards;
        int equipments;

        public void Initialize(Character c)
        {
            process = null;
            character = c;
            for (int i = 0; i < handObjects.Length; i++)
            {
                handObjects[i].Number = i;
            }
            deck = (1 << character.Hp) - 1;
            currentSquare = 5;
            board.GetSquare(currentSquare).AddPlayer(Team);
            transform.position = new Vector3(currentSquare % Board.Width + 0.5f, 0, currentSquare / Board.Width + 0.5f) * Square.Side;
            transform.position += Vector3.up;
            client = new Client();
            client.Initialize();
            
        }

        public void Act()
        {
            process?.Invoke();
            cameraManager.Act();
        }

        void DisplayPhaseNoticeProcess()
        {
            if (signal)
            {
                phaseNotice.DisplayPhaseNotice(currentPhase);
            }

            if (phaseNotice.IsActing)
                return;

            StartTurn(currentPhase);
        }

        void StartTurn(PhaseEnum p)
        {
            switch (p)
            {
                case PhaseEnum.PlottingPhase:
                    PlottingPhaseInit();
                    break;
                case PhaseEnum.MovePhase:
                    process = MovePhaseProcess;
                    directionUI.SetActive(true);
                    board.GetSquare(currentSquare).RemovePlayer();
                    break;

                case PhaseEnum.RangePhase:
                case PhaseEnum.MeleePhase:
                    SetUsedCard(currentSegment);
                    directionUI.SetActive(true);
                    startSquare = currentSquare;
                    targetSquare = startSquare;
                    process = RangePhaseProcess;
                    break;
            }
            uiManager.ShowPhaseUI(p, true);
        }

        /// <summary>
        /// PlottingPhaseの実装セット
        /// </summary>
        #region PlottingPhase
        
        /// <summary>
        /// OnReceiveDataに加える。サーバから手札を受け取るときに使う。
        /// データ形式...data0～Imagination番目:手札のカードID
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        void ReceiveHandsEvent(object sender, byte[] data)
        {
            ThrowAwayHands();
            int currentIndex = 0;
            for (int i = 0; i < character.Imagination; i++)
            {
                if (data[i] == 255)
                    break;

                hands |= (1 << data[i]);
                handObjects[i].SetCardInfo(character.GetCard(data[i]));
            }
            currentIndex = character.Imagination;
            if (BitCalculation.BitCount(deck) < character.Imagination)
            {
                deck = ((1 << character.Hp) - 1);
                discards = 0;
            }
            
            deck &= ~hands;
            currentPhase = PhaseEnum.PlottingPhase;
            process = PlottingPhaseProcess;
            signal = true;
        }

        /// <summary>
        /// プロットフェーズの初期処理
        /// </summary>
        void PlottingPhaseInit()
        {
            process = PlottingPhaseProcess;
            actions[0] = null;
            actions[1] = null;
            heldCard = null;
            ResetHandObjectsPosition();
        }

        /// <summary>
        /// プロットフェーズ中に実行し続ける関数。processに代入する
        /// </summary>
        void PlottingPhaseProcess()
        {
            if (Input.GetMouseButtonUp(0) && heldCard != null)
                SetActions();

            if (!Input.GetMouseButton(0))
                return;

            if (Input.GetMouseButtonDown(0))
                GrabCard();

            if (heldCard != null)
            {
                heldCard.transform.position = Input.mousePosition;
                return;
            }

            if ((clickPoint - Input.mousePosition).magnitude > 10.0f)
                heldCard = selectedCard;

        }

        /// <summary>
        /// カードを選択状態にするかしないかの関数
        /// </summary>
        void GrabCard()
        {
            if (selectedCard != null)
            {
                selectedCard.transform.position -= Vector3.up * Card.Height / 4;
                selectedCard = null;
            }

            clickPoint = Input.mousePosition;
            int index = (int)Mathf.Floor((clickPoint.x - Screen.width / 2) / (Card.Width + Card.Padding) + numberOfHands / 2f);
            index = Mathf.Clamp(index, 0, numberOfHands - 1);
            Card tmp = handObjects[index];

            if (tmp.OnMouse(clickPoint) && tmp.CardInfo != actions[0]?.CardInfo && tmp.CardInfo != actions[1]?.CardInfo)
            {
                selectedCard = tmp;
                selectedCard.transform.position += Vector3.up * Card.Height / 4;
            }
        }

        void SetActions()
        {
            int index = (Input.mousePosition.x < Screen.width / 2) ? 0 : 1;
            if (cardFrames[index].OverlapPoint(Input.mousePosition))
            {
                if (actions[index] != null)
                {
                    actions[index].ResetPosition((numberOfHands - 1) / 2f);
                }
                actions[index] = heldCard;
                heldCard.transform.position = cardFrames[index].transform.position;
                selectedCard = null;
            }
            else
            {
                heldCard.ResetPosition((numberOfHands - 1) / 2f);
                heldCard.transform.position += Vector3.up * Card.Height / 4;
            }

            heldCard = null;
        }

        public void DecideSegments()
        {
            if (actions[0] == null || actions[1] == null)
                return;

            uiManager.ShowPhaseUI(PhaseEnum.PlottingPhase, false);
            byte[] tmp = {(byte)PlayerID,(byte)actions[0].CardInfo.ID, (byte)actions[1].CardInfo.ID };
            client.StartSend(tmp);
        }

        #endregion

        /// <summary>
        /// MovePhaseの実装セット
        /// </summary>
        #region MovePhase

        public void SetDirection(int dir)
        {
            if (IsDuringTheMove)
                return;

            if (!board.ExistsSquare(currentSquare, BoardDirection.Up + dir))
            {
                Debug.Log("Nothing Square!");
                return;
            }
            int nextSquare = board.GetSquare(currentSquare).GetAdjacentSquare(BoardDirection.Up + dir).Number;

            if (board.CheckPlayerIsInSquare(nextSquare))
            {
                Debug.Log("The square has already player!");
                return;
            }

            if (numberOfMoves != 0 && (int)moveDirectionHistories[numberOfMoves - 1] == (dir + 2) % 4)
            {
                numberOfMoves--;
                mobility += board.GetSquare(nextSquare).ConsumptionOfMobility;
                moveDirection = BoardDirection.Up + dir;
                IsDuringTheMove = true;
                currentSquare = nextSquare;
                directionUI.SetActive(false);
                Debug.Log("Go Back");
                return;
            }

            if (mobility < board.GetSquare(nextSquare).ConsumptionOfMobility)
            {
                Debug.Log("Cannot Move!");
                return;
            }

            Debug.Log("Go Forward!");
            directionUI.SetActive(false);
            moveDirectionHistories[numberOfMoves] = BoardDirection.Up + dir;
            moveDirection = moveDirectionHistories[numberOfMoves];
            currentSquare = board.GetSquare(currentSquare).GetAdjacentSquare(BoardDirection.Up + dir).Number;
            numberOfMoves++;
            mobility -= board.GetSquare(nextSquare).ConsumptionOfMobility;
            IsDuringTheMove = true;
        }

        public void SetMoveAmount()
        {
            cameraManager.SetPosition(transform.position);
            mobility = 4;
            numberOfMoves = 0;
            Debug.Log("You Got 4 Mobilities!");
            IsDuringTheMove = false;
        }

        public void DecideDestination()
        {
            if (IsDuringTheMove)
                return;

            if (WhetherCanPlayerStillMove())
            {
                return;
            }

            byte[] tmp = new byte[3];
            tmp[0] = (byte)PlayerID;
            tmp[0] |= (byte)(numberOfMoves << 4);
            for (int i = 0; i < numberOfMoves; i++)
            {
                tmp[(i / 4) + 1] |= (byte)((int)moveDirectionHistories[i] << (2 * i) % 8);
            }
            client.StartSend(tmp);
            directionUI.SetActive(false);
            board.GetSquare(currentSquare).AddPlayer(Team);
            uiManager.ShowPhaseUI(PhaseEnum.MovePhase, false);
            numberOfMoves = 0;
            mobility = 0;
            process = null;
        }

        bool WhetherCanPlayerStillMove()
        {
            if (numberOfMoves == 0)
                return true;

            int adjacentSquareNumber;
            for (int i = 0; i < 4; i++)
            {
                if (moveDirectionHistories[numberOfMoves - 1] == BoardDirection.Up + i)
                    continue;

                adjacentSquareNumber = board.GetSquare(currentSquare).GetAdjacentSquare(BoardDirection.Up + i).Number;
                if (board.GetSquare(adjacentSquareNumber).ConsumptionOfMobility > mobility || board.CheckPlayerIsInSquare(adjacentSquareNumber))
                    return false;
            }

            return true;
        }

        void MoveToDestination()
        {
            moveAmountBetweenSquares += charaMoveSpeed;
            if (moveAmountBetweenSquares > Square.Side)
            {
                MoveBetweenSquares(transform, moveDirection, charaMoveSpeed - (moveAmountBetweenSquares - Square.Side));
                moveAmountBetweenSquares = 0f;
                directionUI.SetActive(true);
                IsDuringTheMove = false;
            }
            else
            {
                MoveBetweenSquares(transform, moveDirection, charaMoveSpeed);
            }
            cameraManager.SetPosition(transform.position);
        }

        void MoveBetweenSquares(Transform t, BoardDirection dir, float amount)
        {
            switch (dir)
            {
                case BoardDirection.Up:
                    t.position += Vector3.forward * amount;
                    break;

                case BoardDirection.Right:
                    t.position += Vector3.right * amount;
                    break;

                case BoardDirection.Down:
                    t.position += Vector3.back * amount;
                    break;

                case BoardDirection.Left:
                    t.position += Vector3.left * amount;
                    break;
            }
        }

        void MovePhaseProcess()
        {
            if (!IsDuringTheMove)
                return;

            MoveToDestination();
        }

        #endregion

        /// <summary>
        /// RangePhaseの実装セット
        /// </summary>
        #region RangePhase

        public void SetUsedCard(int segment)
        {
            usedCardInformation = actions[segment].CardInfo;
        }

        public void MoveAim(int n)
        {
            if (!board.ExistsSquare(targetSquare, BoardDirection.Up + n))
            {
                Debug.Log("Nothing Square!");
                return;
            }

            int tmp = board.GetSquare(targetSquare).GetAdjacentSquare(BoardDirection.Up + n).Number;

            if (board.GetManhattanDistance(startSquare, tmp) > usedCardInformation.maxRange)
            {
                Debug.Log("Over move!");
                return;
            }

            targetSquare = tmp;
            //cameraManager.MoveSquare(BoardDirection.Up + n);
        }

        public void DecideTarget()
        {
            int dis = board.GetManhattanDistance(startSquare, targetSquare);
            if (dis > usedCardInformation.maxRange || dis < usedCardInformation.minRange)
            {
                Debug.Log("Over Range!");
                return;
            }

            cameraManager.SetPosition(transform.position);
            uiManager.ShowPhaseUI(PhaseEnum.RangePhase, false);
            usedCardInformation = null;
            process = null;
            Debug.Log("You attack this square!!");
        }

        void RangePhaseProcess()
        {
            if (!decideTarget)
                return;

        }

        #endregion

        #region CardOperation

        /// <summary>
        /// 捨て札をすべて山札に戻す
        /// </summary>
        void ReturnDeckFromDiscards()
        {
            deck = discards;
            discards = 0;
        }

        /// <summary>
        /// 手札を捨てる
        /// </summary>
        void ThrowAwayHands()
        {
            discards |= hands;
            hands = 0;
        }

        /// <summary>
        /// 山札からランダムなカードを除外する
        /// </summary>
        /// <param name="num"></param>
        void ExcludeFromDeck(int num)
        {
            int i;
            int count = 0;
            while (count != num)
            {
                if (deck == 0)
                    ReturnDeckFromDiscards();

                i = BitCalculation.GetNthBit(deck, UnityEngine.Random.Range(0, BitCalculation.BitCount(deck)));
                deck &= ~i;
                exclusionCards |= i;
                count++;
            }
        }

        /// <summary>
        /// 手札から指定したカードを除外する
        /// </summary>
        /// <param name="num"></param>
        void ExcludeFromHands(int num)
        {
            int tmp = BitCalculation.GetNthBit(hands, num);
            hands &= ~tmp;
            exclusionCards |= tmp;
        }

        /// <summary>
        /// 装備から指定したものを除外する
        /// </summary>
        /// <param name="num"></param>
        void ExcludeFromEquipments(int num)
        {
            equipments &= ~(1 << num);
        }

        /// <summary>
        /// 除外カードを指定数山札に戻す(ランダム)
        /// </summary>
        /// <param name="amount"></param>
        void ReturnDeckFromExclusionCards(int amount)
        {
            if (exclusionCards == 0)
                return;
            Debug.Log("山札：" + System.Convert.ToString(deck, 2));
            int i = BitCalculation.GetNthBit(exclusionCards, UnityEngine.Random.Range(0, BitCalculation.BitCount(exclusionCards)));
            while (amount-- > 0 && exclusionCards != 0)
            {
                exclusionCards &= ~i;
                deck |= i;

                i = BitCalculation.GetNthBit(exclusionCards, UnityEngine.Random.Range(0, BitCalculation.BitCount(exclusionCards)));
            }

            Debug.Log("山札：" + System.Convert.ToString(deck, 2));
        }

        void ResetHandObjectsPosition()
        {
            float half = (numberOfHands - 1) / 2f;
            for (int j = 0; j < numberOfHands; j++)
            {
                handObjects[j].ResetPosition(half);
            }
        }
        #endregion
    }
}
