using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantacticsScripts
{
    public class CardOperation : MonoBehaviour
    {
        public static CardOperation Instance { get; private set; }

        public delegate void MouseButtonUpEventHandler(ref Card selectedCard, ref Card heldCard);
        public event MouseButtonUpEventHandler OnMouseButtonUp;

        public delegate void MouseButtonDownEventHandler(ref Card selectedCard, ref Card heldCard, ref Vector3 clickPoint);
        public event MouseButtonDownEventHandler OnMouseButtonDown;

        public delegate void MouseButtonLongPressEventHandler(ref Card selectedCard, ref Card heldCard, Vector3 clickPoint);
        public event MouseButtonLongPressEventHandler OnMouseButtonLongPress;

        public int NumberOfHands { get; set; } = 0;
        Card heldCard;
        Card selectedCard;
        Vector3 clickPoint;
        [SerializeField] Card[] handObjects = default; //一番手札が多いキャラクターに合わせる

        void Awake()
        {
            Instance = this;

            for (int i = 0; i < handObjects.Length; i++)
            {
                handObjects[i].Number = i;
            }
        }

        public void Act()
        {
            if (Input.GetMouseButtonUp(0))
            {
                OnMouseButtonUp?.Invoke(ref selectedCard, ref heldCard);
            }

            if (!Input.GetMouseButton(0))
                return;

            if (Input.GetMouseButtonDown(0))
            {
                OnMouseButtonDown?.Invoke(ref selectedCard, ref heldCard, ref clickPoint);
            }

            OnMouseButtonLongPress?.Invoke(ref selectedCard, ref heldCard, clickPoint);
        }

        public Card GetHandsObject(int index)
        {
            return handObjects[index];
        }

        public void SetCardInfo(int num, CardInfomation info)
        {
            handObjects[num].SetCardInfo(info);
        }

        public void ResetHandsObjectsPosition()
        {
            float half = (NumberOfHands - 1) / 2f;
            for (int i = 0; i < NumberOfHands; i++)
            {
                handObjects[i].ResetPosition(half);
            }
        }

        /*
        /// <summary>
        /// 山札から規定数のカードを手札に加える(ランダム)
        /// </summary>
        void DrawCards()
        {
            if (deck == 0)
                ReturnDeckFromDiscards();

            Debug.Log("山札：" + System.Convert.ToString(deck, 2));
            int i = BitCalculation.GetNthBit(deck, Random.Range(0, BitCalculation.BitCount(deck) - 1));
            while (BitCalculation.BitCount(hands) !=  7)
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
        }*/
    }
}

