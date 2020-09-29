using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantacticsScripts
{
    public class CardInfomation
    {
        public Sprite CardSprite { get; protected set; }
        public int minRange { get; protected set; }
        public int maxRange { get; protected set; }
        public int Blast { get; protected set; }
        public int Damage { get; protected set; }
        public string Name { get; protected set; }
        public int ID { get; protected set; }
        public CardType Type { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sprite"></param>
        /// <param name="minRange"></param>
        /// <param name="maxRange"></param>
        /// <param name="blast"></param>
        /// <param name="damage"></param>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <param name="type"></param>
        public CardInfomation(Sprite sprite = null, int minRan = 1, int maxRan = 1, int blast = 0, int damage = 1, string name = null, int id = -1, CardType type = 0)
        {
            CardSprite = sprite;
            minRange = minRan;
            maxRange = maxRan;
            Blast = blast;
            Damage = damage;
            Name = name;
            ID = id;
            Type = type;
        }
    }
}
