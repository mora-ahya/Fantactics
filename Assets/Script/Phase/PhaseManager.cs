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
        GameScene gameScene;
        Player selfPlayer;
        Phase currentPhase;

        List<Player> phasePlayers = new List<Player>();

        public void Initialize(GameScene scene, Player sPlayer)
        {
            gameScene = scene;
            selfPlayer = sPlayer;
        }

        public void Act()
        {
            
        }

        public void ActPhase()
        {
            currentPhase?.Act();
        }

        public Phase GetPhase(PhaseEnum phaseEnum)
        {
            return phases[(int)phaseEnum];
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
