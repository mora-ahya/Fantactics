using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantacticsScripts
{
    public class PhaseManager : MonoBehaviour
    {
        [SerializeField] Phase[] phases = default;
        GameScene gameScene;
        Player selfPlayer;

        public void Initialize(GameScene scene, Player sPlayer)
        {
            gameScene = scene;
            selfPlayer = sPlayer;
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

        public void EndPhase(Phase phase)
        {
            gameScene.NotifyPhaseEnd(phase.GetResult());
        }
    }
}
