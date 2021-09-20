using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantacticsScripts
{
    public class AttackEffectManager : MonoBehaviour
    {
        [SerializeField] AttackEffect[] attackEffects = default;

        public AttackEffect GetAttackEffect(int id)
        {
            return attackEffects[id];
        }
    }
}
