using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantacticsScripts
{
    public class PlottingPhase : Phase
    {
        CardOperation.MouseButtonLongPressEventHandler mouseButtonLongPressEventHandler;
        CardOperation.MouseButtonDownEventHandler mouseButtonDownEventHandler;
        CardOperation.MouseButtonUpEventHandler mouseButtonUpEventHandler;

        readonly Card[] actions = new Card[2];

        [SerializeField] BoxCollider2D[] cardFrames = default; //colliderに
        [SerializeField] RectTransform centerOfHandObjects = default;

        void Awake()
        {
            mouseButtonLongPressEventHandler = new CardOperation.MouseButtonLongPressEventHandler(LongPressMouseButtonEvent);
            mouseButtonDownEventHandler = new CardOperation.MouseButtonDownEventHandler(PressMouseButtonEvent);
            mouseButtonUpEventHandler = new CardOperation.MouseButtonUpEventHandler(ReleaseMouseButtonEvent);
            result = new byte[3];
        }

        /// <summary>
        /// プロットフェーズの初期処理
        /// </summary>
        public override void Initialize()
        {
            actions[0] = null;
            actions[1] = null;
            result[1] = 0;
            result[2] = 0;
            CameraManager.Instance.SetTarget(manager.GetSelfPlayer().gameObject);
            CardOperation.Instance.ResetHandsObjectsPosition();
            UIManager.Instance.SwitchUI(PhaseEnum.PlottingPhase, true);
            CardOperation.Instance.OnMouseButtonLongPress += mouseButtonLongPressEventHandler;
            CardOperation.Instance.OnMouseButtonDown += mouseButtonDownEventHandler;
            CardOperation.Instance.OnMouseButtonUp += mouseButtonUpEventHandler;
        }

        public override byte[] GetResult()
        {
            CardOperation.Instance.OnMouseButtonLongPress -= mouseButtonLongPressEventHandler;
            CardOperation.Instance.OnMouseButtonDown -= mouseButtonDownEventHandler;
            CardOperation.Instance.OnMouseButtonUp -= mouseButtonUpEventHandler;
            UIManager.Instance.SwitchUI(PhaseEnum.PlottingPhase, false);
            result[1] = (byte)actions[0].CardInfo.ID;
            result[2] = (byte)actions[1].CardInfo.ID;
            return result;
        }

        protected void PressMouseButtonEvent(ref Card selectedCard, ref Card heldCard, ref Vector3 clickPoint)
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

            if (tmp.OnMouse(clickPoint) && tmp != actions[0] && tmp != actions[1])
            {
                selectedCard = tmp;
                selectedCard.Emphasize(true);
            }
        }

        protected void LongPressMouseButtonEvent(ref Card selectedCard, ref Card heldCard, Vector3 clickPoint)
        {
            if (heldCard != null)
            {
                heldCard.transform.position = Input.mousePosition;
                return;
            }

            if ((clickPoint - Input.mousePosition).magnitude > 10.0f)
            {
                heldCard = selectedCard;
            }
        }

        protected void ReleaseMouseButtonEvent(ref Card selectedCard, ref Card heldCard)
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
                heldCard.Emphasize(false);
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
        
        public void DecidePlots()
        {
            if (actions[0] == null || actions[1] == null)
                return;

            Player player = manager.GetSelfPlayer();
            player.Information.SetPlot(0, actions[0].CardInfo.ID);
            player.Information.SetPlot(1, actions[1].CardInfo.ID);
            manager.EndPhase(this);
            //player.EndTurn();
        }
        
    }
}
