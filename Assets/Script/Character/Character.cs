using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantacticsScripts
{
    public class Character
    {
        public int Hp { get; protected set; }//体力、総カード数
        public int Initiative { get; protected set; }//プレイヤー処理順
        public int Mobility { get; protected set; }//移動力
        public int DefensePower { get; protected set; }//防御可能回数
        public int Imagination { get; protected set; }//手札数
        public int AssaultPower { get; protected set; }//突進ダメージ
        public int AssaultCounterPower { get; protected set; }//被突進ダメージ
        public int EquipmentsMask { get; protected set; } //デッキから装備カードを分けるためのbitマスク
        protected CardInfomation[] cards;

        public CardInfomation GetCard(int num)
        {
            return cards[num];
        }

        System.Action action;
    }
}
