using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantacticsScripts
{
    public class CardInfomation
    {
        public Sprite CardSprite { get; protected set; }
        public int MinRange { get; protected set; }
        public int MaxRange { get; protected set; }
        public int Blast { get; protected set; }
        public int Power { get; protected set; }
        public string Name { get; protected set; }
        public int ID { get; protected set; }
        public CardType Type { get; protected set; }
        public int AttackEffectID { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sprite">カードの画像</param>
        /// <param name="minRan">射程の最低範囲</param>
        /// <param name="maxRan">射程の最大範囲</param>
        /// <param name="blast">爆風</param>
        /// <param name="power">ダメージ量、もしくは移動量</param>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <param name="type"></param>
        public CardInfomation(Sprite sprite = null, int minRan = 1, int maxRan = 1, int blast = 0, int power = 1, string name = null, int id = -1, CardType type = 0, int effectID = -1)
        {
            CardSprite = sprite;
            MinRange = minRan;
            MaxRange = maxRan;
            Blast = blast;
            Power = power;
            Name = name;
            ID = id;
            Type = type;
            AttackEffectID = effectID;
        }
    }
}
