using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FantacticsScripts
{
    public class PlottingOperation : PhaseOperation
    {
        CardOperation.MouseButtonUpEventHandler mouseButtonUpEventHandler;

        [SerializeField] BoxCollider2D[] cardFrames = default; //colliderに
        [SerializeField] RectTransform centerOfHandObjects = default;

        PlottingResult result;

        public override void OnStart()
        {
            mouseButtonUpEventHandler = new CardOperation.MouseButtonUpEventHandler(ReleaseMouseButtonEvent);
        }

        /// <summary>
        /// プロットフェーズの初期処理
        /// </summary>
        public void Initialize(Player player, PlottingResult res)
        {
            result = res;

            //CameraManager.Instance.SetTarget(manager.GetSelfPlayer().gameObject);
            UIManager.Instance.ShowPhaseUI(PhaseEnum.PlottingPhase, true);
            CardOperation.Instance.ResetHandsObjectsPosition();
            CardOperation.Instance.OnMouseButtonUp += mouseButtonUpEventHandler;
        }

        public override void End()
        {
            GameSceneManager.Instance.SendPhaseResult(result);

            UIManager.Instance.ShowPhaseUI(PhaseEnum.PlottingPhase, false);
            CardOperation.Instance.OnMouseButtonUp -= mouseButtonUpEventHandler;
            CardOperation.Instance.ResetHandsObjectsPosition();
        }

        public override bool CanEnd()
        {
            if (result.Actions[0] == null || result.Actions[1] == null)
            {
                return false;
            }

            return true;
        }

        protected void ReleaseMouseButtonEvent(Card targetCard, PointerEventData eventData, ref bool isResetPosition)
        {
            for (int actionIndex = 0; actionIndex < result.Actions.Length; actionIndex++)
            {
                if (result.Actions[actionIndex] == targetCard)
                {
                    result.Actions[actionIndex] = null;
                    break;
                }
            }

            for (int frameIndex = 0; frameIndex < cardFrames.Length; frameIndex++)
            {
                if (cardFrames[frameIndex].OverlapPoint(eventData.position))
                {
                    if (result.Actions[frameIndex] != null)
                    {
                        result.Actions[frameIndex].ResetPosition((CardOperation.Instance.NumberOfHands - 1) / 2f);
                    }

                    result.Actions[frameIndex] = targetCard;
                    targetCard.transform.position = cardFrames[frameIndex].transform.position;
                    isResetPosition = false;
                    break;
                }
            }
        }
    }
}
