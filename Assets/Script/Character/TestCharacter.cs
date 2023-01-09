using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantacticsScripts
{
    public class TestCharacter : Character
    {
        public TestCharacter()
        {
            Hp = 3;
            Initiative = 3;
            Mobility = 1;
            DefensePower = 0;//防御可能回数
            Imagination = 3;
            AssaultPower = 0;//突進ダメージ
            AssaultCounterPower = 0;//被突進ダメージ
            EquipmentsMask = 0;
            HeadPosition = new Vector3(0f, 11.45f, 0f);

            cards = new CardInfomation[Hp];
            MakeCard();
        }

        void MakeCard()
        {
            cards[0] = new CardInfomation(null, 1, 1, 0, 3, "moveTest", 0, CardType.Move);
            cards[1] = new CardInfomation(null, 1, 3, 2, 3, "rangeTest", 1, CardType.Range);
            cards[2] = new CardInfomation(null, 1, 1, 0, 6, "meleeTest", 2, CardType.Melee);
        }
    }
}
