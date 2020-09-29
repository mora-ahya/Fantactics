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

        void Awake()
        {
            Instance = this;
        }

        public void SwitchUI(Phase p, bool on)
        {
            switch (p)
            {
                case Phase.PlottingPhase:
                    selectSegmentsPhaseUI.SetActive(on);
                    break;

                case Phase.MovePhase:
                    movePhaseUI.SetActive(on);
                    break;

                case Phase.RangePhase:
                case Phase.MeleePhase:
                    rangeMeleePhaseUI.SetActive(on);
                    break;
            }
        }
    }
}
