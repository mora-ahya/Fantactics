using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantacticsScripts
{
    public class PlottingPhase : PhaseBase
    {
        CardOperation.MouseButtonLongPressEventHandler mouseButtonLongPressEventHandler;
        CardOperation.MouseButtonDownEventHandler mouseButtonDownEventHandler;
        CardOperation.MouseButtonUpEventHandler mouseButtonUpEventHandler;

        Card[] actions = new Card[2];
        [SerializeField] BoxCollider2D[] cardFrames = default; //colliderに
        [SerializeField] RectTransform centerOfHandObjects = default;

        void Start()
        {
            mouseButtonLongPressEventHandler = new CardOperation.MouseButtonLongPressEventHandler(LongPressMouseButtonEvent);
            mouseButtonDownEventHandler = new CardOperation.MouseButtonDownEventHandler(PressMouseButtonEvent);
            mouseButtonUpEventHandler = new CardOperation.MouseButtonUpEventHandler(ReleaseMouseButtonEvent);
        }

        /// <summary>
        /// プロットフェーズの初期処理
        /// </summary>
        public override void Initialize()
        {
            actions[0] = null;
            actions[1] = null;
            CardOperation.Instance.OnMouseButtonLongPress += mouseButtonLongPressEventHandler;
            CardOperation.Instance.OnMouseButtonDown += mouseButtonDownEventHandler;
            CardOperation.Instance.OnMouseButtonUp += mouseButtonUpEventHandler;
        }

        public override void EndProcess()
        {
            CardOperation.Instance.OnMouseButtonLongPress -= mouseButtonLongPressEventHandler;
            CardOperation.Instance.OnMouseButtonDown -= mouseButtonDownEventHandler;
            CardOperation.Instance.OnMouseButtonUp -= mouseButtonUpEventHandler;
        }

        protected void PressMouseButtonEvent(Card selectedCard, Card heldCard, ref Vector3 clickPoint)
        {
            if (selectedCard != null)
            {
                selectedCard.Emphasize(false);
                selectedCard = null;
            }

            clickPoint = Input.mousePosition;
            int index = (int)Mathf.Floor((clickPoint.x - Screen.width / 2) / (Card.Width + Card.Padding) + CardOperation.Instance.NumberOfHands / 2f);
            index = Mathf.Clamp(index, 0, CardOperation.Instance.NumberOfHands - 1);
            Card tmp = CardOperation.Instance.GetHandsObject(index);

            if (tmp.OnMouse(clickPoint))
            {
                selectedCard = tmp;
                selectedCard.Emphasize(true);
            }
        }

        protected void LongPressMouseButtonEvent(Card selectedCard, Card heldCard, Vector3 clickPoint)
        {
            if (heldCard != null)
            {
                heldCard.transform.position = Input.mousePosition;
                return;
            }

            if ((clickPoint - Input.mousePosition).magnitude > 10.0f)
                heldCard = selectedCard;
        }

        protected void ReleaseMouseButtonEvent(Card selectedCard, Card heldCard)
        {
            if (heldCard == null)
                return;

            int index = (Input.mousePosition.x < Screen.width / 2) ? 0 : 1;
            if (cardFrames[index].OverlapPoint(Input.mousePosition))
            {
                if (actions[index] != null)
                {
                    actions[index].ResetPosition((CardOperation.Instance.NumberOfHands - 1) / 2f);
                }
                actions[index] = heldCard;
                heldCard.transform.position = cardFrames[index].transform.position;
                selectedCard = null;
            }
            else
            {
                heldCard.ResetPosition((CardOperation.Instance.NumberOfHands - 1) / 2f);
                heldCard.Emphasize(false);
            }

            heldCard = null;
        }
        /*
        public void DecideSegments()
        {
            if (actions[0] == null || actions[1] == null)
                return;

            uiManager.SwitchUI(Phase.PlottingPhase, false);
            byte[] tmp = { (byte)PlayerID, (byte)actions[0].CardInfo.ID, (byte)actions[1].CardInfo.ID };
            client.StartSend(tmp);
        }
        */
    }
}
