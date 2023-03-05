using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantacticsScripts
{
    public class MovePhase : Phase
    {
        struct FootPrint
        {
            BoardDirection direction;
            int usingCardId;
        }

        [SerializeField] Board board = default;
        [SerializeField] CameraTarget cameraTarget = default;
        [SerializeField] GameObject decideButton = default;

        CameraTarget.EndMoveHandler endMoveHandler;
        MovePhaseResult result;
        bool isDecidingForward;
        int mobility = 0;
        float moveAmountBetweenSquares;
        float objectMoveSpeed = 1f;

        void Awake()
        {
            result = new MovePhaseResult();
            endMoveHandler = new CameraTarget.EndMoveHandler(OnEndCameraTargetMove);
        }

        public override void Initialize(Player player)
        {
            //カードの情報をもらう
            isDecidingForward = false;
            mobility = player.GetPlot().Power;
            UIManager.Instance.SetRemainMovePower(mobility);

            result.Clear();
            result.DestSquareNum = player.Information.CurrentSquare;
            result.PlayerID = player.Information.PlayerID;
            //board.ChangeRedBitFlag(result.DestSquare, true);
            //board.ApplayRedBitFlag();

            cameraTarget.SetPosition(player.GetCharacterHeadPosition());
            cameraTarget.AddEndMoveHandler(endMoveHandler);

            CameraManager.Instance.SetMode(CameraManager.CameraModeEnum.FollowMode);
            CameraManager.Instance.SetTarget(cameraTarget);
            CameraManager.Instance.SetOffset(0.0f, 30.0f, -10.0f);

            if (CanStillMove())
            {
                UIManager.Instance.ShowPhaseUI(PhaseEnum.MovePhase, true);
                return;
            }

            // 1歩も動けないとき何かしらやり取りを入れたい
            isDecidingForward = true;
            End();
        }

        public override void End()
        {
            if (isDecidingForward)
            {
                UIManager.Instance.ShowPhaseUI(PhaseEnum.MovePhase, false);
                GameSceneManager.Instance.SendPhaseResult(result);
            }
            else
            {
                isDecidingForward = true;
                decideButton.SetActive(false);
                UIManager.Instance.ShowDirectionUI(true);
            }
        }

        public override PhaseResult GetResult() // End形式が出来たら消す
        {
            if (cameraTarget.IsMoving)
            {
                return null;
            }

            if (CanStillMove())
            {
                Debug.Log("You Can Still Move!");
                return null;
            }

            return result;
        }

        public override bool CanEndPhase()
        {
            if (cameraTarget.IsMoving)
            {
                return false;
            }

            if (CanStillMove())
            {
                Debug.Log("You Can Still Move!");
                return false;
            }

            return true;
        }

        public override void Act()
        {

        }

        public void SetDirection(int dir)
        {
            if (isDecidingForward)
            {
                result.PlayerForward = BoardDirection.Up + dir;
                End();
                return;
            }

            if (!board.ExistsSquare(result.DestSquareNum, BoardDirection.Up + dir))
            {
                Debug.Log("Nothing Square!");
                return;
            }

            int nextSquare = board.GetSquare(result.DestSquareNum).GetAdjacentSquare(BoardDirection.Up + dir).Number;

            if (board.CheckPlayerIsInSquare(nextSquare) && nextSquare != manager.GetSelfPlayer().Information.CurrentSquare)
            {
                Debug.Log("The square has already other player!");
                return;
            }

            if (result.NumOfMove != 0 && (int)result.MoveDirections[result.NumOfMove - 1] == (dir + 2) % 4)
            {
                ReturnTheLastSquare(dir, nextSquare);
                return;
            }

            if (mobility < board.GetSquare(nextSquare).ConsumptionOfMobility)
            {
                Debug.Log("Cannot Move!");
                return;
            }

            GoNextSquare(dir, nextSquare);
        }

        void OnEndCameraTargetMove()
        {
            if (CanStillMove())
            {
                UIManager.Instance.ShowDirectionUI(true);
                return;
            }

            decideButton.SetActive(true);
        }

        void GoNextSquare(int dir, int nextSquare)
        {
            Debug.Log("Go Forward!");
            UIManager.Instance.ShowDirectionUI(false);

            result.MoveDirections[result.NumOfMove] = BoardDirection.Up + dir;
            result.DestSquareNum = board.GetSquare(result.DestSquareNum).GetAdjacentSquare(result.MoveDirections[result.NumOfMove]).Number;
            result.NumOfMove++;

            //board.ChangeRedBitFlag(result.DestSquare, true);
            //board.ApplayRedBitFlag();

            cameraTarget.StartMove(BoardDirection.Up + dir);

            mobility -= board.GetSquare(nextSquare).ConsumptionOfMobility;
            UIManager.Instance.SetRemainMovePower(mobility);
        }

        void ReturnTheLastSquare(int dir, int lastSquare)
        {
            Debug.Log("Go Back");
            UIManager.Instance.ShowDirectionUI(false);

            mobility += board.GetSquare(result.DestSquareNum).ConsumptionOfMobility;
            UIManager.Instance.SetRemainMovePower(mobility);

            //board.ChangeRedBitFlag(result.DestSquare, false);
            //board.ApplayRedBitFlag();

            result.NumOfMove--;
            result.DestSquareNum = lastSquare;

            cameraTarget.StartMove(BoardDirection.Up + dir);
        }

        bool CanStillMove()
        {
            if (mobility == 0)
            {
                return false;
            }

            Square currentSquare = board.GetSquare(result.DestSquareNum);
            Square nextSquare;

            for (int i = 0; i < 4; i++)
            {
                if (result.NumOfMove != 0 && result.MoveDirections[result.NumOfMove - 1] == BoardDirection.Up + (i + 2) % 4)
                    continue;

                nextSquare = currentSquare.GetAdjacentSquare(BoardDirection.Up + i);

                if (nextSquare == null)
                    continue;

                if (nextSquare.ConsumptionOfMobility <= mobility && nextSquare.HavingPlayerID == -1)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// テストコマンド
        /// </summary>
        public void SetMoveAmount()
        {
            CameraManager.Instance.SetPosition(transform.position);
            mobility = 4;
            result.NumOfMove = 0;
            Debug.Log("You Got 4 Mobilities!");
        }
    }
}
