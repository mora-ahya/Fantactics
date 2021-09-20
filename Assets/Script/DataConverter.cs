using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantacticsScripts
{
    public class DataConverter
    {
        public static int PlottingPhaseResultToByteArray(PlottingPhaseResult source)
        {
            //result[1] = (byte)actions[0].CardInfo.ID;
            //result[2] = (byte)actions[1].CardInfo.ID;

            return 0;
        }

        public static int MovePhaseResultToByteArray(MovePhaseResult source)
        {
            //プレイヤーID,移動数,移動方向1~4,移動方向5~8,移動先のマス番号
            //result[1] = (byte)numberOfMoves;
            //for (int i = 0; i < numberOfMoves; i++)
            //{
            //    result[(i / 4) + 2] |= (byte)((int)moveDirectionHistories[i] << (2 * i) % 8);
            //}
            //result[4] = (byte)currentSquare;

            return 0;
        }
    }
}
