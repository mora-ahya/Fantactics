using System.Collections;
using System.Collections.Generic;

namespace FantacticsScripts
{
    public class PlayerInformation
    {
        public int CurrentHP;
        public int Deck;
        public int Hands;
        public int Discards;
        public int ExclusionCards;
        public int Equipments;
        public Character Chara;

        readonly CardInfomation[] plots;

        PlayerInformation()
        {
            plots = new CardInfomation[2];
        }

        public void Initialize(Character chara)
        {
            Chara = chara;
            plots[0] = null;
            plots[1] = null;
        }

        public void SetPlots(int cardID1, int cardID2)
        {
            plots[0] = Chara.GetCard(cardID1);
            plots[1] = Chara.GetCard(cardID2);
        }

        public CardInfomation GetPlot(int segmentNumber)
        {
            if (segmentNumber > 2)
                return null;

            return plots[segmentNumber];
        }
    }
}
