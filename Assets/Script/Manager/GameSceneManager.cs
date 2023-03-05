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

        // Start is called before the first frame update
        void Start()
        {

        }

        public void SetGameScene(GameScene gameScene)
        {
            this.gameScene = gameScene;
        }

        public void SendPhaseResult(PlottingPhaseResult plottingPhaseResult)
        {
            gameScene.OnReceivedPlottingPhaseResult(plottingPhaseResult);
        }

        public void SendPhaseResult(MovePhaseResult movePhaseResult)
        {
            gameScene.OnReceivedMovePhaseResult(movePhaseResult);
        }

        public void SendPhaseResult(RangePhaseResult rangePhaseResult)
        {
            gameScene.OnReceivedRangePhaseResult(rangePhaseResult);
        }

        public void SendPhaseResult(MeleePhaseResult meleePhaseResult)
        {
            gameScene.OnReceivedMeleePhaseResult(meleePhaseResult);
        }
    }
}