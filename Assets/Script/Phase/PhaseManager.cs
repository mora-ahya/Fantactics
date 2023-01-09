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

        public void StartPhase(PhaseEnum phaseEnum)
        {
            currentPhase = phases[(int)phaseEnum];
            currentPhase.Initialize();
        }

        public void EndCurrentPhase()
        {
            currentPhase.End();
        }

        public void EndPhase(PhaseResult phaseResult)
        {
            switch (phaseResult.PhaseNumber)
            {
                case PhaseEnum.PlottingPhase:
                    break;

                case PhaseEnum.MovePhase:
                    break;

                case PhaseEnum.RangePhase:
                    break;

                case PhaseEnum.MeleePhase:
                    break;
            }
        }
    }
}
