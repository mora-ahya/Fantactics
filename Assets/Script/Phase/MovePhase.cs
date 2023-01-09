using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantacticsScripts
{
    public class MovePhase : Phase
    {
        [SerializeField] Board board = default;
        [SerializeField] CameraTarget cameraTarget = default;

        CameraTarget.EndMoveHandler endMoveHandler;
        MovePhaseResult result;
        int mobility = 0;
        float moveAmountBetweenSquares;
        float objectMoveSpeed = 1f;

        void Awake()
        {
            result = new MovePhaseResult();
            endMoveHandler = new CameraTarget.EndMoveHandler(DisplayDirectionUI);
        }

        public override void Initialize()
        {
            //カードの情報をもらう
            Player player = manager.GetSelfPlayer();

            mobility = player.GetPlot().Power;

            result.Clear();
            result.DestSquare = player.Information.CurrentSquare;

            UIManager.Instance.ShowPhaseUI(PhaseEnum.MovePhase, true);

            cameraTarget.SetPosition(player.GetCharacterHeadPosition());
            cameraTarget.AddEndMoveHandler(endMoveHandler);

            CameraManager.Instance.SetMode(CameraManager.CameraModeEnum.FollowMode);
            CameraManager.Instance.SetTarget(cameraTarget);
            CameraManager.Instance.SetOffset(0.0f, 30.0f, -10.0f);
        }

        public override void End()
        {
            UIManager.Instance.ShowPhaseUI(PhaseEnum.MovePhase, false);
        }

        public override PhaseResult GetResult()
        {
            if (cameraTarget.IsMoving)
            {
                return null;
            }

            if (CanPlayerStillMove())
            {
                Debug.Log("You Can Still Move!");
                return null;
            }

            return result;
        }

        public override void Act()
        {

        }

        public void SetDirection(int dir)
        {
            if (!board.CanMoveToDirection(result.DestSquare, BoardDirection.Up + dir))
            {
                Debug.Log("Nothing Square!");
                return;
            }

            int nextSquare = board.GetSquare(result.DestSquare).GetAdjacentSquare(BoardDirection.Up + dir).Number;

            if (board.PlayerIsInSquare(nextSquare) && nextSquare != manager.GetSelfPlayer().Information.CurrentSquare)
            {
                Debug.Log("The square has already other player!");
                return;
            }

            if (result.NumOfMoves != 0 && (int)result.MoveDirections[result.NumOfMoves - 1] == (dir + 2) % 4)
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

        void DisplayDirectionUI()
        {
            UIManager.Instance.ShowDirectionUI(true);
        }

        void GoNextSquare(int dir, int nextSquare)
        {
            Debug.Log("Go Forward!");
            UIManager.Instance.ShowDirectionUI(false);

            result.MoveDirections[result.NumOfMoves] = BoardDirection.Up + dir;
            result.DestSquare = board.GetSquare(result.DestSquare).GetAdjacentSquare(result.MoveDirections[result.NumOfMoves]).Number;
            result.NumOfMoves++;

            cameraTarget.StartMove(BoardDirection.Up + dir);

            mobility -= board.GetSquare(nextSquare).ConsumptionOfMobility;
        }

        void ReturnTheLastSquare(int dir, int lastSquare)
        {
            Debug.Log("Go Back");
            UIManager.Instance.ShowDirectionUI(false);

            mobility += board.GetSquare(result.DestSquare).ConsumptionOfMobility;

            result.NumOfMoves--;
            result.DestSquare = lastSquare;

            cameraTarget.StartMove(BoardDirection.Up + dir);
        }

        /// <summary>
        /// テストコマンド
        /// </summary>
        public void SetMoveAmount()
        {
            CameraManager.Instance.SetPosition(transform.position);
            mobility = 4;
            result.NumOfMoves = 0;
            Debug.Log("You Got 4 Mobilities!");
        }

        bool CanPlayerStillMove()
        {
            if (result.NumOfMoves == 0)
                return true;

            Square currentSquare = board.GetSquare(result.DestSquare);
            Square nextSquare;

            for (int i = 0; i < 4; i++)
            {
                if (result.MoveDirections[result.NumOfMoves - 1] == BoardDirection.Up + (i + 2) % 4)
                    continue;

                nextSquare = currentSquare.GetAdjacentSquare(BoardDirection.Up + i);

                if (nextSquare == null)
                    continue;

                if (nextSquare.ConsumptionOfMobility <= mobility && nextSquare.HavingPlayerID == -1)
                    return true;
            }

            return false;
        }
    }
}
