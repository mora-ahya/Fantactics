﻿using System.Collections;
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
        uint[] redBitFlag = new uint[((Width * Height) >> 5) + 1];
        float[] redBitFlagF = new float[((Width * Height) >> 5) + 1];

        public static Vector3 SquareNumberToWorldPosition(int squareNum)
        {
            return new Vector3((squareNum % Width + 0.5f) * Square.Side, 0f, (squareNum / Width + 0.5f) * Square.Side);
        }

        public static void SquareNumberToWorldPosition(int squareNum, ref Vector3 dest)
        {
            dest.Set((squareNum % Width + 0.5f) * Square.Side, 0f, (squareNum / Width + 0.5f) * Square.Side);;
        }

        public static Vector3 GetVectorByDirection(BoardDirection dir)
        {
            switch (dir)
            {
                case BoardDirection.Up:
                    return Vector3.forward;

                case BoardDirection.Right:
                    return Vector3.right;

                case BoardDirection.Down:
                    return Vector3.back;

                case BoardDirection.Left:
                    return Vector3.left;
            }

            return Vector3.zero;
        }

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

        public bool ExistsSquare(int squareNum, BoardDirection dir)
        {
            return squares[squareNum].GetAdjacentSquare(dir) != null;
        }

        public bool CheckPlayerIsInSquare(int squareNum)
        {
            return squares[squareNum].HavingPlayerID != -1;
        }

        public bool CheckPlayerIsInSquare(int squareNum, BoardDirection dir)
        {
            return squares[squareNum].GetAdjacentSquare(dir).HavingPlayerID != -1;
        }

        public bool AddPlayerIntoSquare(Player player, int squareNum)
        {
            squares[player.Information.CurrentSquare].RemovePlayer();
            return squares[squareNum].AddPlayer(player.Information.PlayerID);
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
                redBitFlag[squareNum / 32] |= 1U << squareNum % 32;
                redBitFlagF[squareNum / 32] = redBitFlag[squareNum / 32];
            }
            else
            {
                redBitFlag[squareNum / 32] &= (redBitFlag[squareNum / 5] ^ (1U << squareNum % 32));
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

        /// <summary>
        /// すべてのSquareを赤くするテスト関数
        /// </summary>
        public void RedBitAllOnTest()
        {
            for (int i = 0; i < 16; i++)
            {
                ChangeRedBitFlag(i, true);
            }
        }

        /// <summary>
        /// すべてのSquareを赤じゃなくするテスト関数
        /// </summary>
        public void RedBitAllOffTest()
        {
            for (int i = 0; i < redBitFlag.Length; i++)
            {
                redBitFlag[i] = ~0U;
                redBitFlagF[i] = redBitFlag[i];
            }
        }
    }
}
