using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantacticsScripts
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] GameObject selectSegmentsPhaseUI = default;
        [SerializeField] GameObject movePhaseUI = default;
        [SerializeField] GameObject rangePhaseUI = default;
        [SerializeField] GameObject meleePhaseUI = default;

        public void SwitchUI(Phase p, bool on)
        {
            switch (p)
            {
                case Phase.SelectSegmentsPhase:
                    selectSegmentsPhaseUI.SetActive(on);
                    break;

                case Phase.MovePhase:
                    movePhaseUI.SetActive(on);
                    break;

                case Phase.RangePhase:
                    rangePhaseUI.SetActive(on);
                    break;

                case Phase.MeleePhase:
                    meleePhaseUI.SetActive(on);
                    break;
            }
        }
    }
}
