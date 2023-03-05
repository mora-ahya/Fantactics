using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FantacticsScripts
{
    public class RangePhase : Phase
    {

        [SerializeField] Board board = default;
        [SerializeField] GameObject directionUI = default;
        [SerializeField] CameraTarget squareTarget = default;

        RangePhaseResult result;
        Board.BoardEventHandler onClick;

        int squareNumHavingPlayer;
        CardInfomation usedCardInformation;
        bool duringTheMove;
        readonly HashSet<int> squareNumsInRange = new HashSet<int>();

        void Awake()
        {
            result = new RangePhaseResult();
            onClick = new Board.BoardEventHandler(OnClickSquare);
        }

        public override void Initialize(Player player)
        {
            squareNumsInRange.Clear();

            squareNumHavingPlayer = player.Information.CurrentSquare;
            result.TargetSquare = squareNumHavingPlayer;
            usedCardInformation = player.GetPlot();
            //squareTarget.Initialize(player.transform, MoveAim, EndMove);
            UIManager.Instance.ShowPhaseUI(PhaseEnum.RangePhase, true);
            Board.AppendSquareNumInRangeToHashSet(squareNumsInRange, squareNumHavingPlayer, usedCardInformation.MinRange, usedCardInformation.MaxRange, player.Information.Direction);
            
            board.SetSquareColor(squareNumsInRange, Color.green);
            board.OnPointClickEvent += onClick;

            GeneObjInsteadOfColorTest(); // test
        }

        public override void End()
        {
            board.OnPointClickEvent -= onClick;
            UIManager.Instance.ShowPhaseUI(PhaseEnum.RangePhase, false);
            UIManager.Instance.ShowDecideButton(false);
            GameSceneManager.Instance.SendPhaseResult(result);

            ResetTestGameObjectsTest(); // test
        }

        public override bool CanEndPhase()
        {
            return true;
        }

        public override PhaseEnum GetPhaseEnum()
        {
            return PhaseEnum.RangePhase;
        }

        void OnClickSquare(PointerEventData eventData, int clickedSquareNum)
        {
            if (result.TargetSquare != squareNumHavingPlayer)
            {
                // キャンセル処理
                UIManager.Instance.ShowDecideButton(false);
            }

            int ManhDis = Board.ManhattanDistance(squareNumHavingPlayer, clickedSquareNum);

            if (ManhDis < usedCardInformation.MinRange || usedCardInformation.MaxRange < ManhDis)
            {
                result.TargetSquare = squareNumHavingPlayer;
                return;
            }

            result.TargetSquare = clickedSquareNum;

            // 影響範囲に色をつける
            // 可能であればダメージ計算も

            UIManager.Instance.ShowDecideButton(true);
        }

        #region Test
        List<GameObject> TestGameObjects = new List<GameObject>();

        public void GeneObjInsteadOfColorTest()
        {
            foreach (int a in squareNumsInRange)
            {
                GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
                g.transform.position = Board.SquareNumberToWorldPosition(a);
                TestGameObjects.Add(g);
            }
        }

        public void ResetTestGameObjectsTest()
        {
            foreach (GameObject a in TestGameObjects)
            {
                Destroy(a);
            }
            TestGameObjects.Clear();
        }

        /// <summary>
        /// test関数
        /// </summary>
        /// <param name="sNum"></param>
        /// <param name="minRange"></param>
        /// <param name="maxRange"></param>
        /// <param name="forward"></param>
        public static void ColorSquareInRangeTest(int sNum, int minRange, int maxRange, BoardDirection forward)
        {
            Board.GetRotationFactor(out int factorX, out int factorY, forward);
            int squareNum;

            for (int y = 0; y <= maxRange; y++)
            {
                for (int x = Mathf.Max(minRange - y, 0); x <= maxRange - y; x++)
                {
                    squareNum = sNum + x * factorX + y * factorY;
                    GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    g.transform.position = Board.SquareNumberToWorldPosition(squareNum);

                    if (x != 0)
                    {
                        squareNum = sNum - x * factorX + y * factorY;
                        g = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        g.transform.position = Board.SquareNumberToWorldPosition(squareNum);
                    }
                }
            }
        }

        #endregion
    }
}
