using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantacticsScripts
{
    public class Board : MonoBehaviour
    {
        public static readonly int BoardWidth = 11;
        public static readonly int BoardHeight = 14;
        static readonly int PropertyNameRedBitFlag = Shader.PropertyToID("_RedBitFlag");

        Square[] squares = new Square[BoardWidth * BoardHeight];
        Material material;

        ///ボードのマスを赤くするときに使用
        ///赤くしたいマスの番目のbitを立てる
        int[] redBitFlag = new int[((BoardWidth * BoardHeight) >> 5) + 1];
        float[] redBitFlagF = new float[((BoardWidth * BoardHeight) >> 5) + 1];

        void Start()
        {
            //transform.position += new Vector3(Square.Side / 2f * BoardWidth, 0, Square.Side / 2f * BoardHeight);
            transform.Rotate(Vector3.up * 180f);
            material = GetComponent<MeshRenderer>().material;
            GenerateBoard();
            ChangeRedBitFlag(1, true);
            ApplayRedBitFlag();
            //BoardTest();
        }

        void GenerateBoard()
        {
            for (int i = 0; i < (BoardHeight + 1) * BoardWidth; i++)
            {
                if (i < BoardWidth * BoardHeight)
                {
                    squares[i] = new Square();
                    squares[i].test = i;
                }

                int j = i - BoardWidth;

                if (j < 0)
                    continue;

                squares[j].Initialize(
                    j < BoardWidth ? null : squares[j - BoardWidth],
                    j % BoardWidth == BoardWidth - 1 ? null : squares[j + 1],
                    j >= (BoardHeight - 1) * BoardWidth ? null : squares[j + BoardWidth],
                    j % BoardWidth == 0 ? null : squares[j - 1],
                    j
                    );

            }
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
