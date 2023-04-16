using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FantacticsScripts
{
    public class AttackOnBoardOperation : PhaseOperation
    {
        [SerializeField] Board board = default;

        AttackResult result;
        Board.BoardEventHandler onClick;

        int squareNumHavingPlayer;
        CardInfomation usedCardInformation;
        readonly HashSet<int> squareNumsInRange = new HashSet<int>();

        public override void OnStart()
        {
            onClick = new Board.BoardEventHandler(OnClickSquare);
        }

        public void Initialize(Player player, AttackResult res)
        {
            result = res;
            squareNumsInRange.Clear();

            squareNumHavingPlayer = player.Information.CurrentSquare;
            result.TargetSquare = squareNumHavingPlayer;
            usedCardInformation = player.GetPlot();
            UIManager.Instance.ShowPhaseUI(PhaseEnum.RangePhase, true);
            Board.AppendSquareNumInRangeToHashSet(squareNumsInRange, squareNumHavingPlayer, usedCardInformation.MinRange, usedCardInformation.MaxRange, player.Information.Direction);
            //前後に撃てる場合を考慮できるようにする

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

        public override bool CanEnd()
        {
            return true;
        }

        void OnClickSquare(PointerEventData eventData, int clickedSquareNum)
        {
            if (result.TargetSquare != squareNumHavingPlayer)
            {
                // キャンセル処理
                UIManager.Instance.ShowDecideButton(false);
            }

            if (clickedSquareNum < 0)
            {
                result.TargetSquare = squareNumHavingPlayer;
                return;
            }

            if (squareNumsInRange.Contains(clickedSquareNum) == false)
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
