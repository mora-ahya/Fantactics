using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantacticsScripts
{
    public class MovePhase : Phase
    {
        int currentSquare;
        [SerializeField] Board board = default;
        [SerializeField] GameObject directionUI = default;
        [SerializeField] SquareTarget squareTarget = default;

        BoardDirection[] moveDirectionHistories = new BoardDirection[8]; //chara毎に大きさ変える
        int mobility;
        int numberOfMoves;
        bool duringTheMove;
        float moveAmountBetweenSquares;
        float objectMoveSpeed = 1f;

        void Awake()
        {
            result = new byte[4];//プレイヤーID,移動数,移動方向1~4,移動方向5~8
        }

        public override void Initialize()
        {
            //カードの情報をもらう
            Player player = manager.GetSelfPlayer();
            CardInfomation tmp = player.GetPlot();
            mobility = tmp.Power;
            numberOfMoves = 0;
            directionUI.SetActive(true);
            currentSquare = player.Information.CurrentSquare;
            board.GetSquare(currentSquare).PlayerExit();
            UIManager.Instance.SwitchUI(PhaseEnum.MovePhase, true);
            squareTarget.Initialize(player.transform, SetDirection, EndMove);
            result[1] = 0;
            result[2] = 0;
            result[3] = 0;
        }

        public override byte[] GetResult()
        {
            manager.GetSelfPlayer().Information.SetCurrentSquare(currentSquare);
            board.GetSquare(currentSquare).PlayerEnter(result[0]);
            result[1] = (byte)numberOfMoves;
            for (int i = 0; i < numberOfMoves; i++)
            {
                result[(i / 4) + 2] |= (byte)((int)moveDirectionHistories[i] << (2 * i) % 8);
            }
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
                squareTarget.StartMove(BoardDirection.Up + dir);
                duringTheMove = true;
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
            squareTarget.StartMove(moveDirectionHistories[numberOfMoves]);
            currentSquare = board.GetSquare(currentSquare).GetAdjacentSquares(moveDirectionHistories[numberOfMoves]).Number;
            numberOfMoves++;
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
            numberOfMoves = 0;
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
            if (numberOfMoves == 0)
                return true;

            int adjacentSquareNumber;
            for (int i = 0; i < 4; i++)
            {
                if (moveDirectionHistories[numberOfMoves - 1] == BoardDirection.Up + (i + 2) % 4)
                    continue;

                if (!board.CanMoveToDirection(currentSquare, BoardDirection.Up + i))
                    continue;

                adjacentSquareNumber = board.GetSquare(currentSquare).GetAdjacentSquares(BoardDirection.Up + i).Number;
                if (board.GetSquare(adjacentSquareNumber).ConsumptionOfMobility <= mobility && !board.PlayerIsInSquare(adjacentSquareNumber))
                    return true;
            }

            return false;
        }
    }
}
