using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantacticsScripts
{
    public class MovePhase : PhaseBase
    {
        System.Action phase;

        int currentSquare;
        [SerializeField] Board board = default;
        [SerializeField] GameObject directionUI = default;

        BoardDirection[] moveDirectionHistories = new BoardDirection[6]; //chara毎に大きさ変える
        BoardDirection moveDirection;
        int mobility;
        int numberOfMoves;
        bool decideDestination;
        float moveAmountBetweenSquares;
        float charaMoveSpeed = 1f;

        public override void Initialize()
        {
            //カードの情報をもらう
            numberOfMoves = 0;
            directionUI.SetActive(true);
            board.GetSquare(currentSquare).PlayerExit();
        }

        public override void EndProcess()
        {

        }

        public override void Act()
        {
            if (!decideDestination)
                return;

            MoveToDestination();
        }

        public void SetDirection(int dir)
        {
            if (decideDestination)
                return;

            if (!board.CanMoveToDirection(currentSquare, BoardDirection.Up + dir))
            {
                Debug.Log("Nothing Square!");
                return;
            }
            int nextSquare = board.GetSquare(currentSquare).GetAdjacentSquares(BoardDirection.Up + dir).Number;

            if (board.PlayerIsInSquare(nextSquare))
            {
                Debug.Log("The square has already player!");
                return;
            }

            if (numberOfMoves != 0 && (int)moveDirectionHistories[numberOfMoves - 1] == (dir + 2) % 4)
            {
                numberOfMoves--;
                mobility += board.GetSquare(nextSquare).ConsumptionOfMobility;
                moveDirection = BoardDirection.Up + dir;
                decideDestination = true;
                currentSquare = nextSquare;
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
            moveDirectionHistories[numberOfMoves] = BoardDirection.Up + dir;
            moveDirection = moveDirectionHistories[numberOfMoves];
            currentSquare = board.GetSquare(currentSquare).GetAdjacentSquares(BoardDirection.Up + dir).Number;
            numberOfMoves++;
            mobility -= board.GetSquare(nextSquare).ConsumptionOfMobility;
            decideDestination = true;
        }

        /// <summary>
        /// テストコマンド
        /// </summary>
        public void SetMoveAmount()
        {
            CameraManager.Instance.SetPosition(transform);
            mobility = 4;
            numberOfMoves = 0;
            Debug.Log("You Got 4 Mobilities!");
            decideDestination = false;
        }

        public void DecideDestination()
        {
            if (decideDestination)
                return;

            if (CanPlayerStillMove())
            {
                Debug.Log("You Can Still Move!");
                return;
            }

            directionUI.SetActive(false);
            //board.GetSquare(currentSquare).PlayerEnter(Team);
            UIManager.Instance.SwitchUI(Phase.MovePhase, false);
            numberOfMoves = 0;
            mobility = 0;
            phase = null;
            Debug.Log("Let's Go!");
        }

        bool CanPlayerStillMove()
        {
            if (numberOfMoves == 0)
                return true;

            int adjacentSquareNumber;
            for (int i = 0; i < 4; i++)
            {
                if (moveDirectionHistories[numberOfMoves - 1] == BoardDirection.Up + i)
                    continue;

                adjacentSquareNumber = board.GetSquare(currentSquare).GetAdjacentSquares(BoardDirection.Up + i).Number;
                if (board.GetSquare(adjacentSquareNumber).ConsumptionOfMobility > mobility || board.PlayerIsInSquare(adjacentSquareNumber))
                    return false;
            }

            return true;
        }

        void MoveToDestination()
        {
            moveAmountBetweenSquares += charaMoveSpeed;
            if (moveAmountBetweenSquares > Square.Side)
            {
                MoveBetweenSquares(transform, moveDirection, charaMoveSpeed - (moveAmountBetweenSquares - Square.Side));
                moveAmountBetweenSquares = 0f;
                directionUI.SetActive(true);
                decideDestination = false;
            }
            else
            {
                MoveBetweenSquares(transform, moveDirection, charaMoveSpeed);
            }
            CameraManager.Instance.SetPosition(transform);
        }

        void MoveBetweenSquares(Transform t, BoardDirection dir, float amount)
        {
            switch (dir)
            {
                case BoardDirection.Up:
                    t.position += Vector3.forward * amount;
                    break;

                case BoardDirection.Right:
                    t.position += Vector3.right * amount;
                    break;

                case BoardDirection.Down:
                    t.position += Vector3.back * amount;
                    break;

                case BoardDirection.Left:
                    t.position += Vector3.left * amount;
                    break;
            }
        }
    }
}
