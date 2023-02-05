using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantacticsScripts
{
    public class Phase : MonoBehaviour
    {
        [SerializeField] protected PhaseManager manager = default;

        public virtual void Initialize(Player player)
        {

        }

        public virtual void End()
        {

        }

        public virtual bool CanEndPhase()
        {
            return false;
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
