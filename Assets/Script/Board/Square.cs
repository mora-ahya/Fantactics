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
        int number;

        Square[] adjacentSquares = new Square[4];

        public int Number => number;
        public Square[] AdjacentSquares => adjacentSquares;

        public void Initialize(Square upSquare, Square rightSquare, Square downSquare, Square leftSquare, int num)
        {
            number = num;
            adjacentSquares[(int)BoardDirection.Up] = upSquare;
            adjacentSquares[(int)BoardDirection.Right] = rightSquare;
            adjacentSquares[(int)BoardDirection.Down] = downSquare;
            adjacentSquares[(int)BoardDirection.Left] = leftSquare;
        }

        /// <summary>
        /// number番目のSquareの色を変えたいときに使用
        /// </summary>
        public void RaiseBit(int[] board)
        {
            board[number / 32] |= 1 << number % 32;
        }

        /// <summary>
        /// number番目のSquareの色を変えたいときに使用
        /// </summary>
        public void LowerBit(int[] board)
        {
            board[number / 32] &= (board[number / 32] ^ (1 << number % 32));
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
            return Mathf.Abs(square1.number / Board.BoardWidth - square2.number / Board.BoardWidth)
                + Mathf.Abs(square1.number % Board.BoardWidth - square2.number % Board.BoardWidth);
        }
    }
}
