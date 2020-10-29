using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantacticsScripts
{
    public class RangePhase : Phase
    {

        [SerializeField] Board board = default;
        [SerializeField] GameObject directionUI = default;
        [SerializeField] SquareTarget squareTarget = default;
        int startSquare;
        int targetSquare;
        CardInfomation usedCardInformation;
        bool duringTheMove;

        void Awake()
        {
            result = new byte[2];
        }

        public override void Initialize()
        {
            startSquare = player.Information.CurrentSquare;
            targetSquare = startSquare;
            usedCardInformation = player.GetPlot();
            squareTarget.Initialize(player.transform, MoveAim, EndMove);
            UIManager.Instance.SwitchUI(PhaseEnum.RangePhase, true);
        }

        public override byte[] EndProcess()
        {
            UIManager.Instance.SwitchUI(PhaseEnum.RangePhase, false);
            directionUI.SetActive(false);
            result[1] = (byte)targetSquare;
            return result;
        }

        public override void Act()
        {
            squareTarget.Act();
        }

        void EndMove()
        {
            directionUI.SetActive(true);
            duringTheMove = false;
        }

        public void MoveAim(int dir)
        {
            if (!board.CanMoveToDirection(targetSquare, BoardDirection.Up + dir))
            {
                Debug.Log("Nothing Square!");
                return;
            }

            int tmp = board.GetSquare(targetSquare).GetAdjacentSquares(BoardDirection.Up + dir).Number;

            if (board.GetManhattanDistance(startSquare, tmp) > usedCardInformation.maxRange)
            {
                Debug.Log("Over move!");
                return;
            }

            squareTarget.StartMove(BoardDirection.Up + dir);
            targetSquare = tmp;
        }

        public void DecideTarget()
        {
            int dis = board.GetManhattanDistance(startSquare, targetSquare);
            if (dis > usedCardInformation.maxRange || dis < usedCardInformation.minRange)
            {
                Debug.Log("Over Range!");
                return;
            }
            usedCardInformation = null;
            player.EndTurn();
            Debug.Log("You attack this square!!");
        }
    }
}
