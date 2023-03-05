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

        [SerializeField] BoxCollider2D[] cardFrames = default; //colliderに
        [SerializeField] RectTransform centerOfHandObjects = default;

        PlottingPhaseResult result;

        void Awake()
        {
            mouseButtonLongPressEventHandler = new CardOperation.MouseButtonLongPressEventHandler(LongPressMouseButtonEvent);
            mouseButtonDownEventHandler = new CardOperation.MouseButtonDownEventHandler(PressMouseButtonEvent);
            mouseButtonUpEventHandler = new CardOperation.MouseButtonUpEventHandler(ReleaseMouseButtonEvent);
            result = new PlottingPhaseResult();
        }

        /// <summary>
        /// プロットフェーズの初期処理
        /// </summary>
        public override void Initialize(Player player)
        {
            result.Clear();

            //CameraManager.Instance.SetTarget(manager.GetSelfPlayer().gameObject);
            CardOperation.Instance.ResetHandsObjectsPosition();
            UIManager.Instance.ShowPhaseUI(PhaseEnum.PlottingPhase, true);
            CardOperation.Instance.OnMouseButtonLongPress += mouseButtonLongPressEventHandler;
            CardOperation.Instance.OnMouseButtonDown += mouseButtonDownEventHandler;
            CardOperation.Instance.OnMouseButtonUp += mouseButtonUpEventHandler;
        }

        public override void End()
        {
            GameSceneManager.Instance.SendPhaseResult(result);

            CardOperation.Instance.OnMouseButtonLongPress -= mouseButtonLongPressEventHandler;
            CardOperation.Instance.OnMouseButtonDown -= mouseButtonDownEventHandler;
            CardOperation.Instance.OnMouseButtonUp -= mouseButtonUpEventHandler;
            UIManager.Instance.ShowPhaseUI(PhaseEnum.PlottingPhase, false);
            CardOperation.Instance.ResetHandsObjectsPosition();
        }

        public override bool CanEndPhase()
        {
            if (result.Actions[0] == null || result.Actions[1] == null)
            {
                return false;
            }

            return true;
        }

        public override PhaseResult GetResult()
        {
            if (result.Actions[0] == null || result.Actions[1] == null)
                return null;

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

            if (tmp.OnMouse(clickPoint) && tmp != result.Actions[0] && tmp != result.Actions[1])
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

        protected void ReleaseMouseButtonEvent(ref Card selectedCard, ref Card holdingCard)
        {
            if (holdingCard == null)
                return;

            int index = (Input.mousePosition.x < Screen.width / 2) ? 0 : 1;
            if (cardFrames[index].OverlapPoint(Input.mousePosition))
            {
                if (result.Actions[index] != null)
                {
                    result.Actions[index].ResetPosition((CardOperation.Instance.NumberOfHands - 1) / 2f);
                }
                holdingCard.Emphasize(false);
                result.Actions[index] = holdingCard;
                holdingCard.transform.position = cardFrames[index].transform.position;
                selectedCard = null;
            }
            else
            {
                holdingCard.ResetPosition((CardOperation.Instance.NumberOfHands - 1) / 2f);
                holdingCard.Emphasize(false);
            }

            holdingCard = null;
        }
    }
}
