using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantacticsScripts
{
    public class RangePhase : MonoBehaviour
    {

        [SerializeField] Board board = default;
        int startSquare;
        int targetSquare;
        CardInfomation usedCardInformation;
        bool decideTarget;

        public virtual void Initialize()
        {

        }

        public virtual void EndProcess()
        {

        }

        public virtual void Act()
        {

        }

        public void MoveAim(int n)
        {
            if (!board.CanMoveToDirection(targetSquare, BoardDirection.Up + n))
            {
                Debug.Log("Nothing Square!");
                return;
            }

            int tmp = board.GetSquare(targetSquare).GetAdjacentSquares(BoardDirection.Up + n).Number;

            if (board.GetManhattanDistance(startSquare, tmp) > usedCardInformation.maxRange)
            {
                Debug.Log("Over move!");
                return;
            }

            targetSquare = tmp;
            CameraManager.Instance.MoveSquare(BoardDirection.Up + n);
        }

        public void DecideTarget()
        {
            int dis = board.GetManhattanDistance(startSquare, targetSquare);
            if (dis > usedCardInformation.maxRange || dis < usedCardInformation.minRange)
            {
                Debug.Log("Over Range!");
                return;
            }

            CameraManager.Instance.SetPosition(transform);
            UIManager.Instance.SwitchUI(Phase.RangePhase, false);
            usedCardInformation = null;
            Debug.Log("You attack this square!!");
        }
    }
}
