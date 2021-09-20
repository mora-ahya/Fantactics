using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantacticsScripts
{
    public class GameScene : MonoBehaviour
    {
        public int CurrentSegment { get; protected set; }

        public virtual void NotifyPhaseEnd(PhaseResult result)
        {
            
        }
    }
}
