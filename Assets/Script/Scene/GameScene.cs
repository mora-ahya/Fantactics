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
        public virtual void OnReceivedPlottingPhaseResult(PlottingResult result)
        {

        }

        /// <summary>
        /// MovePhaseResultを受け取ったときの処理
        /// </summary>
        public virtual void OnReceivedMovePhaseResult(MoveResult result)
        {

        }

        /// <summary>
        /// RangePhaseResultを受け取ったときの処理
        /// </summary>
        public virtual void OnReceivedRangePhaseResult(AttackResult result)
        {

        }
    }
}
