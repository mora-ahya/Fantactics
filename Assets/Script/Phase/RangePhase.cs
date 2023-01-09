using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantacticsScripts
{
    public class RangePhase : Phase
    {

        [SerializeField] Board board = default;
        [SerializeField] GameObject directionUI = default;
        [SerializeField] CameraTarget squareTarget = default;

        RangePhaseResult result;

        int startSquare;
        CardInfomation usedCardInformation;
        bool duringTheMove;

        void Awake()
        {
            result = new RangePhaseResult();
        }

        public override void Initialize()
        {
            Player player = manager.GetSelfPlayer();
            startSquare = player.Information.CurrentSquare;
            result.TargetSquare = startSquare;
            usedCardInformation = player.GetPlot();
            //squareTarget.Initialize(player.transform, MoveAim, EndMove);
            UIManager.Instance.ShowPhaseUI(PhaseEnum.RangePhase, true);
        }

        public override PhaseResult GetResult()
        {
            UIManager.Instance.ShowPhaseUI(PhaseEnum.RangePhase, false);
            directionUI.SetActive(false);
            return result;
        }

        public override void Act()
        {
        }

        void EndMove()
        {
            directionUI.SetActive(true);
            duringTheMove = false;
        }

        public void MoveAim(int dir)
        {
            if (!board.CanMoveToDirection(result.TargetSquare, BoardDirection.Up + dir))
            {
                Debug.Log("Nothing Square!");
                return;
            }

            int tmp = board.GetSquare(result.TargetSquare).GetAdjacentSquare(BoardDirection.Up + dir).Number;

            if (board.GetManhattanDistance(startSquare, tmp) > usedCardInformation.maxRange)
            {
                Debug.Log("Over move!");
                return;
            }

            squareTarget.StartMove(BoardDirection.Up + dir);
            result.TargetSquare = tmp;
        }

        public void DecideTarget()
        {
            int dis = board.GetManhattanDistance(startSquare, result.TargetSquare);
            if (dis > usedCardInformation.maxRange || dis < usedCardInformation.minRange)
            {
                Debug.Log("Over Range!");
                return;
            }
            usedCardInformation = null;
            manager.EndPhase(result);
            //player.EndTurn();
            Debug.Log("You attack this square!!");
        }
    }
}
