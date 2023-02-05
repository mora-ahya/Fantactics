using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantacticsScripts
{
    public class GameScene : MonoBehaviour
    {
        public int CurrentSegment { get; protected set; }

        public virtual void NotifyPhaseEnd(int phaseEnum)
        {
            
        }

        /// <summary>
        /// PlottingPhaseResultを受け取ったときの処理
        /// </summary>
        public virtual void OnReceivedPlottingPhaseResult(PlottingPhaseResult result)
        {

        }

        /// <summary>
        /// MovePhaseResultを受け取ったときの処理
        /// </summary>
        public virtual void OnReceivedMovePhaseResult(MovePhaseResult result)
        {

        }
    }
}
