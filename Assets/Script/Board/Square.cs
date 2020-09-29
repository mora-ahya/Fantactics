using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantacticsScripts
{
    public class Square
    {
        public static readonly float Side = 10f;
        static readonly string testCase = "{0} : up...{1}, right...{2}, down...{3}, left...{4}";

        public int test;
        public int Number { get; private set; }
        public int TeamOfHavingPlayer { get; private set; }
        public int ConsumptionOfMobility { get; private set; } = 1;

        Square[] adjacentSquares = new Square[4];

        public void Initialize(Square upSquare, Square rightSquare, Square downSquare, Square leftSquare, int num)
        {
            Number = num;
            adjacentSquares[(int)BoardDirection.Up] = upSquare;
            adjacentSquares[(int)BoardDirection.Right] = rightSquare;
            adjacentSquares[(int)BoardDirection.Down] = downSquare;
            adjacentSquares[(int)BoardDirection.Left] = leftSquare;
        }

        public Square GetAdjacentSquares(BoardDirection dir)
        {
            return adjacentSquares[(int)dir];
        }

        public void PlayerEnter(int teamNum)
        {
            TeamOfHavingPlayer = teamNum;
        }

        public void PlayerExit()
        {
            TeamOfHavingPlayer = 0;
        }

        /// <summary>
        /// number番目のSquareの色を変えたいときに使用
        /// </summary>
        public void RaiseBit(int[] board)
        {
            board[Number / 32] |= 1 << Number % 32;
        }

        /// <summary>
        /// number番目のSquareの色を変えたいときに使用
        /// </summary>
        public void LowerBit(int[] board)
        {
            board[Number / 32] &= (board[Number / 32] ^ (1 << Number % 32));
        }

        /// <summary>
        /// Squareの上下左右のマスが正しく登録されているか調べるテスト関数
        /// </summary>
        public void Test()
        {
            Debug.Log(string.Format(testCase, test, adjacentSquares[0]?.test ?? -1, adjacentSquares[1]?.test ?? -1, adjacentSquares[2]?.test ?? -1, adjacentSquares[3]?.test ?? -1));
        }

        public static int ManhattanDistance(Square square1, Square square2)
        {
            return Mathf.Abs(square1.Number / Board.Width - square2.Number / Board.Width)
                + Mathf.Abs(square1.Number % Board.Width - square2.Number % Board.Width);
        }
    }
}
