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

        public virtual PhaseResult GetResult()
        {
            return null;
        }

        public virtual void Act()
        {

        }
    }
}
