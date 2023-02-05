using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace FantacticsScripts
{
    public class PhaseManager : MonoBehaviour
    {
        [SerializeField] Phase[] phases = default;
        GameScene gameScene;
        Player selfPlayer;
        Phase currentPhase;

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

        public Player GetSelfPlayer()
        {
            return selfPlayer;
        }

        public GameScene GetGameScene()
        {
            return gameScene;
        }

        public Phase GetPhase(PhaseEnum phaseEnum)
        {
            return phases[(int)phaseEnum];
        }

        public PhaseResult GetPhaseResult(PhaseEnum phaseEnum)
        {
            return phases[(int)phaseEnum].GetResult();
        }

        public Phase GetCurrentPhase()
        {
            return currentPhase;
        }

        public PhaseEnum GetCurrentPhaseEnum()
        {
            return currentPhase.GetPhaseEnum();
        }

        public PhaseResult GetCurrentPhaseResult()
        {
            return currentPhase.GetResult();
        }

        public void StartPhase(Player player, PhaseEnum phaseEnum)
        {
            currentPhase = phases[(int)phaseEnum];
            currentPhase.Initialize(player);
        }

        public void EndCurrentPhase()
        {
            if (currentPhase.CanEndPhase())
            {
                currentPhase.End();
            }
        }

        public void EndPhase(int phaseNum)
        {
            if (phaseNum < 0 || phaseNum > phases.Length)
            {
                return;
            }

            Phase phase = phases[phaseNum];

            if (phase.CanEndPhase() == false)
            {
                return;
            }

            phase.End();
        }
    }
}
