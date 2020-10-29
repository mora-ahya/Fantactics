using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantacticsScripts
{
    public class FantacticsBitConverter
    {
        public static void ToBoardDirections(byte[] source, BoardDirection[] destination)
        {
            int numOfMoves = source[0] >> 4;
            for (int i = 0; i < numOfMoves; i++)
            {
                destination[i] = BoardDirection.Up + ((source[(i / 4) + 1] >> (2 * i % 8)) & 3);
                Debug.Log(destination[i]);
            }
        }
    }
}
