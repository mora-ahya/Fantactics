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

        public void SetPlots(int cardID1, int cardID2)
        {
            plots[0] = cardID1;
            plots[1] = cardID2;
        }

        public CardInfomation GetPlot(int segmentNumber)
        {
            if (segmentNumber > 2)
                return null;

            return Chara.GetCard(plots[segmentNumber]);
        }
    }
}
