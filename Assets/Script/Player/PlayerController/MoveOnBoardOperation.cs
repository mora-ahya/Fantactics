using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FantacticsScripts
{
    public class MoveOnBoardOperation : PhaseOperation
    {
        struct FootPrint
        {
            int squareNum;
            int usingCardId;
        }

        public string Tag
        {
            get;
            set;
        }

        [SerializeField] Board board = default;
        [SerializeField] CameraTarget cameraTarget = default;
        [SerializeField] GameObject decideButton = default;

        readonly HashSet<int> alreadyRoadSquares = new HashSet<int>();
        MoveResult moveResult;
        Board.BoardEventHandler onClick;

        Player controlledPlayer;
        bool isDecidingForward;
        int mobility = 0;

        public override void OnStart()
        {
            onClick = new Board.BoardEventHandler(OnClickSquare);
        }

        public override void End()
        {
            if (isDecidingForward)
            {
                ResetAroundGameObjectsTest();
                UIManager.Instance.ShowPhaseUI(PhaseEnum.MovePhase, false);
                decideButton.SetActive(false);
                GameSceneManager.Instance.SendPhaseResult(moveResult);
                board.OnPointClickEvent -= onClick;
            }
            else
            {
                isDecidingForward = true;
                decideButton.SetActive(false);
                ResetRoadGameObjectsTest();
                ColorAround();
            }
        }

        public override bool CanEnd()
        {
            if (CanMove())
            {
                Debug.Log("You Can Still Move!");
                return false;
            }

            return true;
        }

        public void Initialize(Player player, int startSquareNum, int mobility, MoveResult result)
        {
            controlledPlayer = player;
            moveResult = result;
            //カードの情報をもらう
            isDecidingForward = false;
            this.mobility = mobility;
            UIManager.Instance.SetRemainMovePower(mobility);

            board.OnPointClickEvent += onClick;

            moveResult.roadSquares.Clear();
            moveResult.roadSquares.Add(startSquareNum);

            alreadyRoadSquares.Clear();
            alreadyRoadSquares.Add(startSquareNum);

            cameraTarget.SetPosition(player.GetCharacterHeadPosition());

            CameraManager.Instance.SetTarget(cameraTarget);
            CameraManager.Instance.SetOffset(0.0f, 30.0f, -10.0f);
            CameraManager.Instance.SetTarget(null);

            if (CanMove())
            {
                UIManager.Instance.ShowPhaseUI(PhaseEnum.MovePhase, true);
                return;
            }

            // 1歩も動けないとき何かしらやり取りを入れたい
            decideButton.SetActive(true);
        }

        void OnClickSquare(PointerEventData eventData, int clickedSquareNum)
        {
            if (isDecidingForward)
            {
                DecideForward(eventData);
            }
            else
            {
                DecideMove(clickedSquareNum);
            }
        }

        void DecideForward(PointerEventData eventData)
        {
            Vector3 destPos = Board.SquareNumberToWorldPosition(moveResult.roadSquares[moveResult.roadSquares.Count - 1]);
            Vector2 destPosToClickPos = new Vector2(eventData.pointerCurrentRaycast.worldPosition.x - destPos.x, eventData.pointerCurrentRaycast.worldPosition.z - destPos.z);
            float cos = Vector2.Dot(Vector2.one, destPosToClickPos);
            float sin = Vector2.Dot(Vector2.up + Vector2.left, destPosToClickPos);

            if (cos > 0)
            {
                if (sin > 0)
                {
                    moveResult.PlayerForward = BoardDirection.Up;
                }
                else
                {
                    moveResult.PlayerForward = BoardDirection.Right;
                }
            }
            else
            {
                if (sin > 0)
                {
                    moveResult.PlayerForward = BoardDirection.Left;
                }
                else
                {
                    moveResult.PlayerForward = BoardDirection.Down;
                }
            }

            decideButton.SetActive(true);
        }

        void DecideMove(int clickedSquareNum)
        {
            if (clickedSquareNum < 0)
            {
                Debug.Log("Nothing Square!");
                return;
            }

            int numOfMoves = moveResult.roadSquares.Count - 1;
            int currentSquare = moveResult.roadSquares[numOfMoves];

            if (board.CheckNeighborSquare(currentSquare, clickedSquareNum, out _) == false)
            {
                Debug.Log("Cannot Move There!");
                return;
            }

            if (alreadyRoadSquares.Contains(clickedSquareNum) && moveResult.roadSquares[numOfMoves - 1] != clickedSquareNum)
            {
                Debug.Log("The square has already road!");
                return;
            }

            if (board.CheckPlayerIsInSquare(clickedSquareNum) && controlledPlayer.Information.CurrentSquare != clickedSquareNum)
            {
                Debug.Log("The square has already other player!");
                return;
            }

            if (mobility < board.GetSquare(clickedSquareNum).ConsumptionOfMobility)
            {
                Debug.Log("Cannot Move!");
                return;
            }

            // 周囲の色を消す
            ResetAroundGameObjectsTest();

            if (numOfMoves > 0 && moveResult.roadSquares[numOfMoves - 1] == clickedSquareNum)
            {
                // 戻るならここの色を消す
                DeleteRoadGameObjectsTest();
                Debug.Log("Go Back");
                moveResult.roadSquares.RemoveAt(numOfMoves);
                mobility += board.GetSquare(currentSquare).ConsumptionOfMobility;
            }
            else
            {
                // 進むならここの色をつける
                GeneRoadObjInsteadOfColorTest(currentSquare);
                Debug.Log("Go Forward!");
                moveResult.roadSquares.Add(clickedSquareNum);
                mobility -= board.GetSquare(clickedSquareNum).ConsumptionOfMobility;
            }

            UIManager.Instance.SetRemainMovePower(mobility);

            if (CanMove())
            {
                
            }
            else
            {
                decideButton.SetActive(true);
            }
        }

        // 周囲に色をつける機能をつけたい
        // すでに通ったマスは通れることにはするが、色はつけない
        bool CanMove()
        {
            if (mobility == 0)
            {
                return false;
            }

            int numOfMove = moveResult.roadSquares.Count - 1;
            int lastSquare = -1;
            bool canMove = false;

            if (numOfMove > 0)
            {
                lastSquare = moveResult.roadSquares[numOfMove - 1];
            }

            Square currentSquare = board.GetSquare(moveResult.roadSquares[numOfMove]);

            for (int i = 0; i < 4; i++)
            {
                Square nextSquare = currentSquare.GetAdjacentSquare(BoardDirection.Up + i);

                if (nextSquare == null)
                {
                    continue;
                }

                if (lastSquare == nextSquare.Number)
                {
                    // 違う色
                    GeneAroundObjInsteadOfColorTest(nextSquare.Number);
                    continue;
                }

                if (nextSquare.ConsumptionOfMobility <= mobility && nextSquare.HavingPlayerID == -1)
                {
                    GeneAroundObjInsteadOfColorTest(nextSquare.Number);
                    canMove = true;
                }
            }

            return canMove;
        }

        void ColorAround()
        {
            Square currentSquare = board.GetSquare(moveResult.roadSquares[moveResult.roadSquares.Count - 1]);

            for (int i = 0; i < 4; i++)
            {
                Square nextSquare = currentSquare.GetAdjacentSquare(BoardDirection.Up + i);
                GeneAroundObjInsteadOfColorTest(nextSquare.Number);
            }
        }

        #region Test
        List<GameObject> TestAroundGameObjects = new List<GameObject>();
        List<GameObject> TestRoadGameObjects = new List<GameObject>();

        public void GeneAroundObjInsteadOfColorTest(int squareNum)
        {
            GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
            g.transform.position = Board.SquareNumberToWorldPosition(squareNum);
            TestAroundGameObjects.Add(g);
        }

        public void GeneRoadObjInsteadOfColorTest(int squareNum)
        {
            GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
            g.transform.position = Board.SquareNumberToWorldPosition(squareNum);
            TestRoadGameObjects.Add(g);
        }

        public void ResetAroundGameObjectsTest()
        {
            foreach (GameObject a in TestAroundGameObjects)
            {
                Destroy(a);
            }
            TestAroundGameObjects.Clear();
        }

        public void ResetRoadGameObjectsTest()
        {
            foreach (GameObject a in TestRoadGameObjects)
            {
                Destroy(a);
            }
            TestRoadGameObjects.Clear();
        }

        public void DeleteRoadGameObjectsTest()
        {
            GameObject a = TestRoadGameObjects[TestRoadGameObjects.Count - 1];
            TestRoadGameObjects.RemoveAt(TestRoadGameObjects.Count - 1);
            Destroy(a);
        }

        /// <summary>
        /// テストコマンド
        /// </summary>
        public void SetMoveAmount()
        {
            mobility = 1;
            Debug.Log("You Got 4 Mobilities!");
        }
        #endregion
    }
}
