using System.Collections;
using System.Collections.Generic;

namespace FantacticsScripts
{
    public class PlayerInformation
    {
        public int PlayerID { get; private set; }
        public int Team { get; private set; }
        public int CurrentHP;
        public int Deck;
        public int Hands;
        public int Discards;
        public int ExclusionCards;
        public int Equipments;
        public int CurrentSquare { get; private set; }
        public int AmountOfDamage = 0;
        public bool IsDamaged = false;
        public readonly Character Chara;
        readonly int[] plots;

        public PlayerInformation(Character chara)
        {
            Chara = chara;
            plots = new int[2];
        }

        public void SetCurrentSquare(int num)
        {
            CurrentSquare = num;
        }

        public void SetPlot(int segmentNum, int cardID)
        {
            if (segmentNum > 2)
                return;

            plots[segmentNum] = cardID;
        }

        public CardInfomation GetPlot(int segmentNumber)
        {
            if (segmentNumber > 2)
                return null;

            return Chara.GetCard(plots[segmentNumber]);
        }
    }
}
