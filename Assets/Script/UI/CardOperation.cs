using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantacticsScripts
{
    public class CardOperation : MonoBehaviour
    {
        public static CardOperation Instance { get; private set; }

        public delegate void MouseButtonUpEventHandler(Card selectedCard, Card heldCard);
        public event MouseButtonUpEventHandler OnMouseButtonUp;

        public delegate void MouseButtonDownEventHandler(Card selectedCard, Card heldCard, ref Vector3 clickPoint);
        public event MouseButtonDownEventHandler OnMouseButtonDown;

        public delegate void MouseButtonLongPressEventHandler(Card selectedCard, Card heldCard, Vector3 clickPoint);
        public event MouseButtonLongPressEventHandler OnMouseButtonLongPress;

        public int NumberOfHands { get; private set; } = 0;
        Card heldCard;
        Card selectedCard;
        Vector3 clickPoint;
        [SerializeField] Card[] handObjects; //一番手札が多いキャラクターに合わせる

        void Awake()
        {
            Instance = this;
        }

        public void Act()
        {
            if (Input.GetMouseButtonUp(0))
            {
                OnMouseButtonUp?.Invoke(selectedCard, heldCard);
            }

            if (!Input.GetMouseButton(0))
                return;

            if (Input.GetMouseButtonDown(0))
            {
                OnMouseButtonDown?.Invoke(selectedCard, heldCard, ref clickPoint);
            }

            OnMouseButtonLongPress?.Invoke(selectedCard, heldCard, clickPoint);
        }

        public Card GetHandsObject(int index)
        {
            return handObjects[index];
        }
    }
}

