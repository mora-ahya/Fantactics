using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantacticsScripts
{ 
    public class GameSceneManager : MonoBehaviour
    {
        public static GameSceneManager Instance { get; private set; }

        GameScene gameScene = null;

        void Awake()
        {
            Instance = this;
        }

        public void SetGameScene(GameScene gameScene)
        {
            this.gameScene = gameScene;
        }

        public void SendPhaseResult(PlottingResult plottingPhaseResult)
        {
            gameScene.OnReceivedPlottingPhaseResult(plottingPhaseResult);
        }

        public void SendPhaseResult(MoveResult movePhaseResult)
        {
            gameScene.OnReceivedMovePhaseResult(movePhaseResult);
        }

        public void SendPhaseResult(AttackResult rangePhaseResult)
        {
            gameScene.OnReceivedRangePhaseResult(rangePhaseResult);
        }
    }
}