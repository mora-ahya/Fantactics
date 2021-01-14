using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantacticsScripts
{
    public class FantacticsBitConverter
    {
        public static void ToBoardDirections(int numOfMoves, int offset, byte[] source, BoardDirection[] destination)
        {
            for (int i = 0; i < numOfMoves; i++)
            {
                destination[i] = BoardDirection.Up + ((source[(i / 4) + offset] >> (2 * i % 8)) & 3);
                Debug.Log(destination[i]);
            }
        }
    }
}
