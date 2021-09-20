using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantacticsScripts
{
    public class AttackEffect : MonoBehaviour //攻撃エフェクトのベースクラス
    {
        public bool IsEnd { get; protected set; } = false;

        public virtual void Initialize()
        {

        }

        public virtual void Act()
        {

        }
    }
}
