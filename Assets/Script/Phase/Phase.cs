using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantacticsScripts
{
    public class Phase : MonoBehaviour
    {
        [SerializeField] protected PhaseManager manager = default;
        protected byte[] result;

        public virtual void Initialize()
        {

        }

        public virtual byte[] GetResult()
        {
            return null;
        }

        public virtual void Act()
        {

        }

        public void UpdatePlayerID()
        {
            result[0] = (byte)manager.GetSelfPlayer().Information.PlayerID;
        }
    }
}
