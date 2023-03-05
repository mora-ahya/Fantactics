using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FantacticsScripts
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [SerializeField] GameObject handUI = default;

        [SerializeField] GameObject selectSegmentsPhaseUI = default;

        readonly string RemainMovePowerStringFormat = "{0,2:00}";
        [SerializeField] GameObject movePhaseUI = default;
        [SerializeField] Text movePhaseUIRemainMove = default;

        [SerializeField] GameObject rangeMeleePhaseUI = default;

        [SerializeField] GameObject directionButtons = default;
        [SerializeField] Button decideButton = default;

        [SerializeField] PhaseNotice phaseNotice = default;

        void Awake()
        {
            Instance = this;
        }

        public void ShowPhaseUI(PhaseEnum p, bool on)
        {
            switch (p)
            {
                case PhaseEnum.PlottingPhase:
                    selectSegmentsPhaseUI.SetActive(on);
                    ShowHandUI(on);
                    break;

                case PhaseEnum.MovePhase:
                    movePhaseUI.SetActive(on);
                    directionButtons.SetActive(on);
                    break;

                case PhaseEnum.RangePhase:
                case PhaseEnum.MeleePhase:
                    rangeMeleePhaseUI.SetActive(on);
                    break;
            }
        }

        public void ShowDirectionUI(bool isShow)
        {
            directionButtons.SetActive(isShow);
        }

        public void ShowHandUI(bool isShow)
        {
            handUI.SetActive(isShow);
        }

        // 位置の設定をできるように
        public void ShowDecideButton(bool isShow)
        {
            decideButton.gameObject.SetActive(isShow);
        }

        #region MovePhaseUI
        public void SetRemainMovePower(int movePower)
        {
            movePhaseUIRemainMove.text = string.Format(RemainMovePowerStringFormat, movePower);
        }
        #endregion

        public void DisplayPhaseNotice(PhaseEnum p)
        {
            phaseNotice.DisplayPhaseNotice(p);
        }
    }
}
