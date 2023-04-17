using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

// そのPhaseを行うプレイヤーがいるか確認
// いないならそのPhaseは飛ばす、いるならPhaseのInitializeを行う


namespace FantacticsScripts
{
    public class PhaseManager : MonoBehaviour
    {
        [SerializeField] Phase[] phases = default;
        Phase currentPhase;

        public void Initialize()
        {
            
        }

        public void ActPhase()
        {
            currentPhase?.Act();
        }

        public bool CheckCurrentPhaseIsCompleted()
        {
            return currentPhase.IsCompleted;
        }

        public void StartPhase(List<Player> players, PhaseEnum phaseEnum)
        {
            currentPhase = phases[(int)phaseEnum];
            currentPhase.Initialize(players);
        }
    }
}
