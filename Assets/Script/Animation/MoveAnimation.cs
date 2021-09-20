using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantacticsScripts
{
    public class MoveAnimation : MonoBehaviour
    {
        public bool IsRunning { get; private set; }

        Player movingPlayer;
        BoardDirection[] moveDirections = new BoardDirection[8];
        int numberOfMoves;
        int maxNumberOfMoves;
        float moveAmountBetweenSquares = 0f;
        float characterMoveSpeed = 1f;

        public void Act()
        {
            MoveFollowingDirections();
        }

        public void SetAnimation(Player player, int maxNumOfMoves, int offset, byte[] directions)
        {
            movingPlayer = player;
            maxNumberOfMoves = maxNumOfMoves;
            FantacticsBitConverter.ToBoardDirections(maxNumberOfMoves, offset, directions, moveDirections);
            numberOfMoves = 0;
            IsRunning = true;
            CameraManager.Instance.SetTarget(gameObject);
        }

        void MoveFollowingDirections()
        {
            float moveSpeed = characterMoveSpeed * Time.deltaTime;
            moveAmountBetweenSquares += moveSpeed;
            if (moveAmountBetweenSquares > Square.Side)
            {
                MoveBetweenSquares(moveDirections[numberOfMoves], moveSpeed - (moveAmountBetweenSquares - Square.Side));
                moveAmountBetweenSquares = 0f;
                numberOfMoves++;
                if (maxNumberOfMoves == numberOfMoves)
                {
                    IsRunning = false;
                }
            }
            else
            {
                MoveBetweenSquares(moveDirections[numberOfMoves], moveSpeed);
            }
        }

        void MoveBetweenSquares(BoardDirection dir, float amount)
        {
            switch (dir)
            {
                case BoardDirection.Up:
                    movingPlayer.gameObject.transform.position += Vector3.forward * amount;
                    break;

                case BoardDirection.Right:
                    movingPlayer.gameObject.transform.position += Vector3.right * amount;
                    break;

                case BoardDirection.Down:
                    movingPlayer.gameObject.transform.position += Vector3.back * amount;
                    break;

                case BoardDirection.Left:
                    movingPlayer.gameObject.transform.position += Vector3.left * amount;
                    break;
            }
        }
    }
}
