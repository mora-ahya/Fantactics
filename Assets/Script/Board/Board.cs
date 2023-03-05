using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FantacticsScripts
{
    public class Board : MonoBehaviour, IPointerClickHandler
    {
        public readonly static int Width = 11;
        public readonly static int Height = 14;
        readonly static int PropertyNameRedBitFlag = Shader.PropertyToID("_RedBitFlag");

        readonly Square[] squares = new Square[Width * Height];
        Material material;
        Texture2D squareColors;

        GameObject basePosition;

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
            dest.Set((squareNum % Width + 0.5f) * Square.Side, 0f, (squareNum / Width + 0.5f) * Square.Side);
        }

        public static int ManhattanDistance(Square square1, Square square2)
        {
            return ManhattanDistance(square1.Number, square2.Number);
        }

        public static int ManhattanDistance(int squareNum1, int SquareNum2)
        {
            return Mathf.Abs(squareNum1 / Width - SquareNum2 / Width)
                + Mathf.Abs(squareNum1 % Width - SquareNum2 % Width);
        }

        /// <summary>
        /// forward方向を向いているとき時の、x,y方向を求めるときの係数を取得できる
        /// </summary>
        /// <param name="factorX">xの係数</param>
        /// <param name="factorY">yの係数</param>
        /// <param name="forward">向いている方向</param>
        public static void GetRotationFactor(out int factorX, out int factorY, BoardDirection forward)
        {
            switch (forward)
            {
                case BoardDirection.Up:
                    factorX = 1;
                    factorY = Board.Width;
                    return;

                case BoardDirection.Right:
                    factorX = -Board.Width;
                    factorY = 1;
                    return;

                case BoardDirection.Down:
                    factorX = -1;
                    factorY = -Board.Width;
                    return;

                case BoardDirection.Left:
                    factorX = Board.Width;
                    factorY = -1;
                    return;

                default:
                    factorX = 1;
                    factorY = Board.Width;
                    return;
            }
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

        // 場合によってはstaticから普通の関数に変更する
        public static void AppendSquareNumInRangeToHashSet(ICollection<int> squareNums, int originSquareNum, int minRange, int maxRange, BoardDirection forward)
        {
            Board.GetRotationFactor(out int factorX, out int factorY, forward);

            for (int y = 0; y <= maxRange; y++)
            {
                for (int x = Mathf.Max(minRange - y, 0); x <= maxRange - y; x++)
                {
                    squareNums.Add(originSquareNum + x * factorX + y * factorY);
                    squareNums.Add(originSquareNum - x * factorX + y * factorY);
                }
            }
        }

        public int WorldPositionToSquareNumber(in Vector3 worldPos)
        {
            Vector3 vectorTmp = worldPos - basePosition.transform.position;

            int width = Mathf.FloorToInt(vectorTmp.x) / (int)Square.Side;
            int height = Mathf.FloorToInt(vectorTmp.z) / (int)Square.Side;

            return width + height * Board.Width;
        }

        public int GetSquareNumberFollowedForward(int SquareNum, int dx, int dy, BoardDirection forward = BoardDirection.Up)
        {
            Board.GetRotationFactor(out int factorX, out int factorY, forward);

            SquareNum += dx * factorX + dy * factorY;

            if (0 <= SquareNum && SquareNum < Width * Height)
            {
                return SquareNum;
            }

            return -1;
        }

        public void Initialize()
        {
            material = GetComponent<MeshRenderer>().material;
            basePosition = transform.Find("BasePosition").gameObject;
            GenerateBoard();
            squareColors = new Texture2D(Board.Width, Board.Height, TextureFormat.RGB24, false);
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

        public void SetSquareColor(int squareNum, Color color)
        {
            squareColors.SetPixel(squareNum % Board.Width, squareNum / Board.Width, color);
        }

        public void SetSquareColor(ICollection<int> squareNums, Color color)
        {
            foreach (int squareNum in squareNums)
            {
                SetSquareColor(squareNum, color);
            }
        }

        public void ApplySquareColor()
        {
            squareColors.Apply();
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

        public delegate void BoardEventHandler(PointerEventData eventData, int squareNum);
        public event BoardEventHandler OnPointUpEvent;
        public event BoardEventHandler OnPointDownEvent;
        public event BoardEventHandler OnPointClickEvent;

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("Click");
            Debug.Log("squareNum : " + WorldPositionToSquareNumber(eventData.pointerCurrentRaycast.worldPosition));
            OnPointClickEvent?.Invoke(eventData, WorldPositionToSquareNumber(eventData.pointerCurrentRaycast.worldPosition));
        }
    }
}
