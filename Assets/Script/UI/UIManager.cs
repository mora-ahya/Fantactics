using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantacticsScripts
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [SerializeField] GameObject selectSegmentsPhaseUI = default;
        [SerializeField] GameObject movePhaseUI = default;
        [SerializeField] GameObject rangeMeleePhaseUI = default;
        [SerializeField] GameObject directoinButtons = default;

        void Awake()
        {
            Instance = this;
        }

        public void SwitchUI(PhaseEnum p, bool on)
        {
            switch (p)
            {
                case PhaseEnum.PlottingPhase:
                    selectSegmentsPhaseUI.SetActive(on);
                    break;

                case PhaseEnum.MovePhase:
                    movePhaseUI.SetActive(on);
                    directoinButtons.SetActive(on);
                    break;

                case PhaseEnum.RangePhase:
                case PhaseEnum.MeleePhase:
                    rangeMeleePhaseUI.SetActive(on);
                    directoinButtons.SetActive(on);
                    break;
            }
        }
    }
}
