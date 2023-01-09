using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantacticsScripts
{
    public class Phase : MonoBehaviour
    {
        [SerializeField] protected PhaseManager manager = default;

        public virtual void Initialize()
        {

        }

        public virtual void End()
        {

        }

        public virtual PhaseResult GetResult()
        {
            return null;
        }

        public virtual PhaseEnum GetPhaseEnum()
        {
            return PhaseEnum.MeleePhase;
        }

        public virtual void Act()
        {

        }
    }
}
