using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantacticsScripts
{
    public class Card : MonoBehaviour
    {
        public CardInfomation CardInfo { get; private set; }
        int number;

        public void SetCardInfo(CardInfomation cardInfomation)
        {
            CardInfo = cardInfomation;
        }
    }
}
