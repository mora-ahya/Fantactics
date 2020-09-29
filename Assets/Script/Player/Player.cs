using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;
using MyInitSet;

namespace FantacticsScripts
{
    public class Player : MonoBehaviour
    {
        public int PlayerID { get; set; }
        public bool Signal { get; private set; }
        public int Team { get; private set; }
        System.Action phase;

        int currentSquare;
        [SerializeField] Board board = default;
        [SerializeField] GameObject directionUI = default;

        //MovePhase
        [SerializeField] CameraManager cameraManager = default;
        [SerializeField] UIManager uiManager = default;

        BoardDirection[] moveDirectionHistories = new BoardDirection[6]; //chara毎に大きさ変える
        BoardDirection moveDirection;
        int mobility;
        int NumberOfMoves;
        bool decideDestination;
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
            phase = null;
            character = c;
            for (int i = 0; i < handObjects.Length; i++)
            {
                handObjects[i].Number = i;
            }
            deck = (1 << character.Hp) - 1;
            Debug.Log("山札：" + System.Convert.ToString(deck, 2));
            //DrawCards();
            currentSquare = 5;
            board.GetSquare(currentSquare).PlayerEnter(Team);
            transform.position = new Vector3(currentSquare % Board.Width + 0.5f, 0, currentSquare / Board.Width + 0.5f) * Square.Side;
            transform.position += Vector3.up;
            PlottingPhaseInit();
        }

        public void Act()
        {
            phase?.Invoke();
            cameraManager.Act();
        }

        public void StartTurn(Phase p, int segment = 0)
        {
            switch (p)
            {
                case Phase.PlottingPhase:
                    Debug.Log("SelectSegmentsPhase");
                    PlottingPhaseInit();
                    break;
                case Phase.MovePhase:
                    Debug.Log("Move Phase!");
                    phase = MovePhase;
                    directionUI.SetActive(true);
                    board.GetSquare(currentSquare).PlayerExit();
                    break;

                case Phase.RangePhase:
                case Phase.MeleePhase:
                    Debug.Log("Range Phase!");
                    SetUsedCard(segment);
                    directionUI.SetActive(true);
                    startSquare = currentSquare;
                    targetSquare = startSquare;
                    phase = RangePhase;
                    break;
            }
            uiManager.SwitchUI(p, true);
            Debug.Log("Your Turn!");
            Signal = false;
        }

        /// <summary>
        /// PlottingPhaseの実装セット
        /// </summary>
        #region PlottingPhase

        void PlottingPhaseInit()
        {
            phase = PlottingPhase;
            actions[0] = null;
            actions[1] = null;
            heldCard = null;
            ThrowAwayHands();
            DrawCards();
            ResetHandObjectsPosition();
        }

        void PlottingPhase()
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

            Signal = true;
            uiManager.SwitchUI(Phase.PlottingPhase, false);
        }

        /// <summary>
        /// 選んだカードのIDをタプルでサーバに送る
        /// </summary>
        /// <returns></returns>
        public System.Tuple<int, int> GetSegmentsID()
        {
            return System.Tuple.Create(actions[0].CardInfo.ID, actions[1].CardInfo.ID);
        }

        #endregion

        /// <summary>
        /// MovePhaseの実装セット
        /// </summary>
        #region MovePhase

        public void SetDirection(int dir)
        {
            if (decideDestination)
                return;

            if (!board.CanMoveToDirection(currentSquare, BoardDirection.Up + dir))
            {
                Debug.Log("Nothing Square!");
                return;
            }
            int nextSquare = board.GetSquare(currentSquare).GetAdjacentSquares(BoardDirection.Up + dir).Number;

            if (board.PlayerIsInSquare(nextSquare))
            {
                Debug.Log("The square has already player!");
                return;
            }

            if (NumberOfMoves != 0 && (int)moveDirectionHistories[NumberOfMoves - 1] == (dir + 2) % 4)
            {
                NumberOfMoves--;
                mobility += board.GetSquare(nextSquare).ConsumptionOfMobility;
                moveDirection = BoardDirection.Up + dir;
                decideDestination = true;
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
            moveDirectionHistories[NumberOfMoves] = BoardDirection.Up + dir;
            moveDirection = moveDirectionHistories[NumberOfMoves];
            currentSquare = board.GetSquare(currentSquare).GetAdjacentSquares(BoardDirection.Up + dir).Number;
            NumberOfMoves++;
            mobility -= board.GetSquare(nextSquare).ConsumptionOfMobility;
            decideDestination = true;
        }

        public void SetMoveAmount()
        {
            cameraManager.SetPosition(transform);
            mobility = 4;
            NumberOfMoves = 0;
            Debug.Log("You Got 4 Mobilities!");
            decideDestination = false;
        }

        public void DecideDestination()
        {
            if (decideDestination)
                return;

            if (WhetherPlayerCanStillMove())
            {
                Debug.Log("You Can Still Move!");
                return;
            }

            directionUI.SetActive(false);
            board.GetSquare(currentSquare).PlayerEnter(Team);
            uiManager.SwitchUI(Phase.MovePhase, false);
            NumberOfMoves = 0;
            mobility = 0;
            Signal = true;
            phase = null;
            Debug.Log("Let's Go!");
        }

        bool WhetherPlayerCanStillMove()
        {
            if (NumberOfMoves == 0)
                return true;

            int adjacentSquareNumber;
            for (int i = 0; i < 4; i++)
            {
                if (moveDirectionHistories[NumberOfMoves - 1] == BoardDirection.Up + i)
                    continue;

                adjacentSquareNumber = board.GetSquare(currentSquare).GetAdjacentSquares(BoardDirection.Up + i).Number;
                if (board.GetSquare(adjacentSquareNumber).ConsumptionOfMobility > mobility || board.PlayerIsInSquare(adjacentSquareNumber))
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
                decideDestination = false;
            }
            else
            {
                MoveBetweenSquares(transform, moveDirection, charaMoveSpeed);
            }
            cameraManager.SetPosition(transform);
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

        void MovePhase()
        {
            if (!decideDestination)
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
            if (!board.CanMoveToDirection(targetSquare, BoardDirection.Up + n))
            {
                Debug.Log("Nothing Square!");
                return;
            }

            int tmp = board.GetSquare(targetSquare).GetAdjacentSquares(BoardDirection.Up + n).Number;

            if (board.GetManhattanDistance(startSquare, tmp) > usedCardInformation.maxRange)
            {
                Debug.Log("Over move!");
                return;
            }

            targetSquare = tmp;
            cameraManager.MoveSquare(BoardDirection.Up + n);
        }

        public void DecideTarget()
        {
            int dis = board.GetManhattanDistance(startSquare, targetSquare);
            if (dis > usedCardInformation.maxRange || dis < usedCardInformation.minRange)
            {
                Debug.Log("Over Range!");
                return;
            }

            cameraManager.SetPosition(transform);
            uiManager.SwitchUI(Phase.RangePhase, false);
            usedCardInformation = null;
            Signal = true;
            phase = null;
            Debug.Log("You attack this square!!");
        }

        void RangePhase()
        {
            if (!decideTarget)
                return;

        }

        #endregion

        #region CardOperation

        /// <summary>
        /// 山札から規定数のカードを手札に加える(ランダム)
        /// </summary>
        void DrawCards()
        {
            int i;
            int count = 0;
            while (count != character.Imagination)
            {
                if (deck == 0)
                    ReturnDeckFromDiscards();

                if (deck == 0)
                    break;
                i = BitCalculation.GetNthBit(deck, Random.Range(0, BitCalculation.BitCount(deck)));
                deck &= ~i;
                hands |= i;
                handObjects[count].SetCardInfo(character.GetCard(BitCalculation.CheckWhichBitIsOn(i)));
                count++;
            }
            numberOfHands = count;
        }

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

                i = BitCalculation.GetNthBit(deck, Random.Range(0, BitCalculation.BitCount(deck)));
                deck &= ~i;
                exclusionCards |= i;

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
            int i = BitCalculation.GetNthBit(exclusionCards, Random.Range(0, BitCalculation.BitCount(exclusionCards)));
            while (amount-- > 0 && exclusionCards != 0)
            {
                exclusionCards &= ~i;
                deck |= i;

                i = BitCalculation.GetNthBit(exclusionCards, Random.Range(0, BitCalculation.BitCount(exclusionCards)));
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
