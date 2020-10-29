using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantacticsScripts
{
    public class Phase : MonoBehaviour
    {
        [SerializeField] protected Player player = default;
        protected byte[] result;

        public virtual void Initialize()
        {

        }

        public virtual byte[] EndProcess()
        {
            return null;
        }

        public virtual void Act()
        {

        }

        public void UpdatePlayerID()
        {
            result[0] = (byte)player.Information.PlayerID;
        }
    }
}
