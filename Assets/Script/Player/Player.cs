using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;
using MyInitSet;

namespace FantacticsScripts
{
    public class Player : MonoBehaviour
    {
        int playerID;
        public bool Signal { get; private set; }
        public bool TurnSignal { get;  set; }
        System.Action phase;

        //MovePhase
        static readonly Vector3 cameraOffset = new Vector3(0, 11.5f, -11.5f);
        [SerializeField] GameObject mainCamera;
        [SerializeField] GameObject movePhaseUI;

        BoardDirection[] moveDirectionHistories = new BoardDirection[6]; //chara毎に大きさ変える
        BoardDirection moveDirection;
        int maxMoveAmount;
        int currentMoveAmouont;
        bool decideDestination;
        float moveAmountBetweenSquares;
        float charaMoveSpeed = 1f;

        //SelectSegmentsPhase
        Card holdingCard;
        Card[] segments = new Card[2];
        [SerializeField] Transform[] segmentsUI = default;
        [SerializeField] GameObject selectSegmentsPhaseUI;

        //RangePhase
        Square startSquare;
        Square targetSquare;
        Vector2Int attackRange = new Vector2Int();
        bool decideTarget;

        //CardOperation
        Character character;
        Card[] handObjects;
        int deck;
        int hands;
        int discards;
        int exclusionCards;
        int equipments;

        void Start()
        {
            phase = null;
            if (phase == MovePhase)
            {
                
            }
            deck = ~0;
            DrawCards();
        }

        void Update()
        {
            phase?.Invoke();
        }

        public void Act()
        {

        }

        public void SetPhase(CardType cardType)
        {
            switch (cardType)
            {
                case CardType.Move:
                    Debug.Log("Move Phase!");
                    movePhaseUI.SetActive(true);
                    phase = MovePhase;
                    break;

                case CardType.Range:
                    Debug.Log("Range Phase!");
                    phase = RangePhase;
                    break;

                case CardType.Melee:
                    //phase = MeleePhase;
                    break;
            }
            Debug.Log("Your Turn!");
            Signal = false;
        }

        /// <summary>
        /// SelectSegmentPhaseの実装セット
        /// </summary>
        #region SelectSegmentsPhase

        /// <summary>
        /// カードを選択する。
        /// </summary>
        /// <param name="target"></param>
        public void GrabCard(Card target)
        {
            if (holdingCard == target)
            {
                holdingCard.transform.localScale /= 1.1f;
                holdingCard = null;
                return;
            }

            if (holdingCard != null)
                holdingCard.transform.localScale /= 1.1f;

            holdingCard = target;
            holdingCard.transform.localScale *= 1.1f;
        }

        public void SetSegment(int n)
        {
            if (holdingCard == null)
                return;

            if (segments[n] != null)
            {

                //カードを元の場所に戻す
            }
            segments[n] = holdingCard;
            holdingCard.transform.localScale /= 1.1f;
            holdingCard.transform.position = segmentsUI[n].position;
            holdingCard = null;
        }

        public void DecideSegments()
        {
            if (segments[0] == null || segments[1] == null)
                return;

            Signal = true;
            selectSegmentsPhaseUI.SetActive(false);
        }

        /// <summary>
        /// 選んだカードのIDをタプルでサーバに送る
        /// </summary>
        /// <returns></returns>
        public System.Tuple<int, int> GetSegmentsID()
        {
            Debug.Log("getSegmentID");
            return System.Tuple.Create(segments[0].CardInfo.ID, segments[1].CardInfo.ID);
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

            if (currentMoveAmouont != 0 && (int)moveDirectionHistories[currentMoveAmouont - 1] == (dir + 2) % 4)
            {
                currentMoveAmouont--;
                moveDirection = BoardDirection.Up + dir;
                decideDestination = true;
                Debug.Log("Go Back");
                return;
            }

            if (currentMoveAmouont == maxMoveAmount)
            {
                Debug.Log("Cannot Move!");
                return;
            }

            Debug.Log("Go Forward!");
            moveDirectionHistories[currentMoveAmouont] = BoardDirection.Up + dir;
            moveDirection = moveDirectionHistories[currentMoveAmouont];
            currentMoveAmouont++;
            decideDestination = true;
        }

        public void SetMoveAmount()
        {
            mainCamera.transform.position = transform.position + cameraOffset;
            maxMoveAmount = 4;
            currentMoveAmouont = 0;
            Debug.Log("You Can Move 4 Squares!");
            decideDestination = false;
        }

        public void DecideDestination()
        {
            if (decideDestination)
                return;

            if (maxMoveAmount != currentMoveAmouont)
            {
                Debug.Log("You Can Still Move!");
                return;
            }
            currentMoveAmouont = 0;
            maxMoveAmount = 0;
            Signal = true;
            phase = null;
            Debug.Log("Let's Go!");
        }

        void MoveToDestination()
        {
            moveAmountBetweenSquares += charaMoveSpeed;
            if (moveAmountBetweenSquares > Square.Side)
            {
                MoveBetweenSquares(transform, moveDirection, charaMoveSpeed - (moveAmountBetweenSquares - Square.Side));
                moveAmountBetweenSquares = 0f;
                decideDestination = false;
            }
            else
            {
                MoveBetweenSquares(transform, moveDirection, charaMoveSpeed);
            }

            mainCamera.transform.position = transform.position + cameraOffset;
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

        void MoveAim(int n)
        {
            Square squareTmp = targetSquare.AdjacentSquares[n];
            if (squareTmp == null)
            {
                Debug.Log("Nothing Square!");
                return;
            }

            if (Square.ManhattanDistance(startSquare, squareTmp) > attackRange.y)
            {
                Debug.Log("Over move!");
                return;
            }

            targetSquare = squareTmp;
            MoveBetweenSquares(mainCamera.transform, BoardDirection.Up + n, 1f);
        }

        void DecideTarget()
        {
            int dis = Square.ManhattanDistance(startSquare, targetSquare);
            if (dis > attackRange.y || dis < attackRange.x)
            {
                Debug.Log("Over Range!");
                return;
            }

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
            if (deck == 0)
                ReturnDeckFromDiscards();

            Debug.Log("山札：" + System.Convert.ToString(deck, 2));
            int i = BitCalculation.GetNthBit(deck, Random.Range(0, BitCalculation.BitCount(deck) - 1));
            while (BitCalculation.BitCount(hands) != /*character.Imagination*/ 7)
            {
                deck &= ~i;
                hands |= i;
                if (deck == 0)
                    ReturnDeckFromDiscards();
                
                i = BitCalculation.GetNthBit(deck, Random.Range(0, BitCalculation.BitCount(deck) - 1));
            }
            Debug.Log("山札：" + System.Convert.ToString(deck, 2));
            Debug.Log("手札：" + System.Convert.ToString(hands, 2));
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
            if (deck == 0)
                ReturnDeckFromDiscards();

            int i = BitCalculation.GetNthBit(deck, Random.Range(0, BitCalculation.BitCount(deck) - 1));
            int count = 0;
            while (count++ != num)
            {
                deck &= ~i;
                hands |= i;
                if (deck == 0)
                    ReturnDeckFromDiscards();

                i = BitCalculation.GetNthBit(deck, Random.Range(0, BitCalculation.BitCount(deck) - 1));
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
            int i = BitCalculation.GetNthBit(exclusionCards, Random.Range(0, BitCalculation.BitCount(exclusionCards) - 1));
            while (amount-- > 0 && exclusionCards != 0)
            {
                exclusionCards &= ~i;
                deck |= i;

                i = BitCalculation.GetNthBit(exclusionCards, Random.Range(0, BitCalculation.BitCount(exclusionCards - 1)));
            }

            Debug.Log("山札：" + System.Convert.ToString(deck, 2));
        }
        #endregion
    }
}
