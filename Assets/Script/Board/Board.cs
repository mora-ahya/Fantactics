using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantacticsScripts
{
    public class Board : MonoBehaviour
    {
        public readonly static int Width = 11;
        public readonly static int Height = 14;
        readonly static int PropertyNameRedBitFlag = Shader.PropertyToID("_RedBitFlag");

        Square[] squares = new Square[Width * Height];
        Material material;

        ///ボードのマスを赤くするときに使用
        ///赤くしたいマスの番目のbitを立てる
        int[] redBitFlag = new int[((Width * Height) >> 5) + 1];
        float[] redBitFlagF = new float[((Width * Height) >> 5) + 1];

        public void Initialize()
        {
            material = GetComponent<MeshRenderer>().material;
            GenerateBoard();
            //BoardTest();
        }

        void GenerateBoard()
        {
            for (int i = 0; i < (Height + 1) * Width; i++)
            {
                if (i < Width * Height)
                {
                    squares[i] = new Square();
                    squares[i].test = i;
                }

                int j = i - Width;

                if (j < 0)
                    continue;

                squares[j].Initialize(
                    j >= (Height - 1) * Width ? null : squares[j + Width],
                    j % Width == Width - 1 ? null : squares[j + 1],
                    j < Width ? null : squares[j - Width],
                    j % Width == 0 ? null : squares[j - 1],
                    j
                    );

            }
        }

        public Square GetSquare(int num)
        {
            return squares[num];
        }

        public bool CanMoveToDirection(int squareNum, BoardDirection dir)
        {
            return squares[squareNum].GetAdjacentSquares(dir) != null;
        }

        public bool PlayerIsInSquare(int squareNum)
        {
            return squares[squareNum].TeamOfHavingPlayer != 0;
        }

        public bool PlayerIsInSquare(int squareNum, BoardDirection dir)
        {
            return squares[squareNum].GetAdjacentSquares(dir).TeamOfHavingPlayer != 0;
        }

        public int GetManhattanDistance(int squareNum1, int squareNum2)
        {
            return Mathf.Abs(squareNum1 / Width - squareNum2 / Width)
                + Mathf.Abs(squareNum1 % Width - squareNum2 % Width);
        }

        public void ChangeRedBitFlag(int squareNum, bool raise)
        {
            if (raise)
            {
                redBitFlag[squareNum / 32] |= 1 << squareNum % 32;
                redBitFlagF[squareNum / 32] = redBitFlag[squareNum / 32];
            }
            else
            {
                redBitFlag[squareNum / 32] &= (redBitFlag[squareNum / 5] ^ (1 << squareNum % 32));
                redBitFlagF[squareNum / 32] = redBitFlag[squareNum / 32];
            }
        }

        public void ApplayRedBitFlag()
        {
            material.SetFloatArray(PropertyNameRedBitFlag, redBitFlagF);
        }

        /// <summary>
        /// すべてのSquareの上下左右のマスが正しく登録されているか調べるテスト関数
        /// </summary>
        void BoardTest()
        {
            for (int i = 0; i < 154; i++)
            {
                squares[i].Test();
            }
        }
    }
}
