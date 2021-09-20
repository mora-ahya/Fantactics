using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantacticsScripts
{
    public class MovePhase : Phase
    {
        [SerializeField] Board board = default;
        [SerializeField] GameObject directionUI = default;
        [SerializeField] SquareTarget squareTarget = default;

        MovePhaseResult result;
        int mobility = 0;
        bool duringTheMove;
        float moveAmountBetweenSquares;
        float objectMoveSpeed = 1f;

        void Start()
        {
            result = new MovePhaseResult();
        }

        public override void Initialize()
        {
            //カードの情報をもらう
            Player player = manager.GetSelfPlayer();
            result.Clear();
            mobility = player.GetPlot().Power;
            directionUI.SetActive(true);
            result.CurrentSquare = player.Information.CurrentSquare;

            //board.GetSquare(currentSquare).PlayerExit();
            UIManager.Instance.SwitchUI(PhaseEnum.MovePhase, true);
            squareTarget.Initialize(player.transform, SetDirection, EndMove);
        }

        public override PhaseResult GetResult()
        {
            //manager.GetSelfPlayer().Information.SetCurrentSquare(currentSquare);
            //board.GetSquare(currentSquare).PlayerEnter(result[0]);

            directionUI.SetActive(false);
            UIManager.Instance.SwitchUI(PhaseEnum.MovePhase, false);
            return result;
        }

        public override void Act()
        {
            squareTarget.Act();
        }

        public void SetDirection(int dir)
        {
            if (!board.CanMoveToDirection(result.CurrentSquare, BoardDirection.Up + dir))
            {
                Debug.Log("Nothing Square!");
                return;
            }
            int nextSquare = board.GetSquare(result.CurrentSquare).GetAdjacentSquares(BoardDirection.Up + dir).Number;

            if (board.PlayerIsInSquare(nextSquare) && nextSquare != manager.GetSelfPlayer().Information.CurrentSquare)
            {
                Debug.Log("The square has already other player!");
                return;
            }

            if (result.NumOfMoves != 0 && (int)result.MoveDirections[result.NumOfMoves - 1] == (dir + 2) % 4)
            {
                result.NumOfMoves--;
                mobility += board.GetSquare(nextSquare).ConsumptionOfMobility;
                squareTarget.StartMove(BoardDirection.Up + dir);
                duringTheMove = true;
                result.CurrentSquare = nextSquare;
                directionUI.SetActive(false);
                Debug.Log("Go Back");
                return;
            }

            if (mobility < board.GetSquare(nextSquare).ConsumptionOfMobility)
            {
                Debug.Log("Cannot Move!");
                return;
            }

            Debug.Log("Go Forward!");
            directionUI.SetActive(false);
            result.MoveDirections[result.NumOfMoves] = BoardDirection.Up + dir;
            squareTarget.StartMove(result.MoveDirections[result.NumOfMoves]);
            result.CurrentSquare = board.GetSquare(result.CurrentSquare).GetAdjacentSquares(result.MoveDirections[result.NumOfMoves]).Number;
            result.NumOfMoves++;
            mobility -= board.GetSquare(nextSquare).ConsumptionOfMobility;
            duringTheMove = true;
        }

        void EndMove()
        {
            directionUI.SetActive(true);
            duringTheMove = false;
        }

        /// <summary>
        /// テストコマンド
        /// </summary>
        public void SetMoveAmount()
        {
            CameraManager.Instance.SetPosition(transform);
            mobility = 4;
            result.NumOfMoves = 0;
            Debug.Log("You Got 4 Mobilities!");
            duringTheMove = false;
        }

        public void DecideDestination()
        {
            if (duringTheMove)
                return;

            if (CanPlayerStillMove())
            {
                Debug.Log("You Can Still Move!");
                return;
            }
            Debug.Log("Let's Go!");
            manager.EndPhase(this);
            //player.EndTurn();
        }

        bool CanPlayerStillMove()
        {
            if (result.NumOfMoves == 0)
                return true;

            int adjacentSquareNumber;
            for (int i = 0; i < 4; i++)
            {
                if (result.MoveDirections[result.NumOfMoves - 1] == BoardDirection.Up + (i + 2) % 4)
                    continue;

                if (!board.CanMoveToDirection(result.CurrentSquare, BoardDirection.Up + i))
                    continue;

                adjacentSquareNumber = board.GetSquare(result.CurrentSquare).GetAdjacentSquares(BoardDirection.Up + i).Number;
                if (board.GetSquare(adjacentSquareNumber).ConsumptionOfMobility <= mobility && !board.PlayerIsInSquare(adjacentSquareNumber))
                    return true;
            }

            return false;
        }
    }
}
