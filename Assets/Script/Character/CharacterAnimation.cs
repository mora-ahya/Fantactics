using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantacticsScripts
{
    public class CharacterAnimation : MonoBehaviour
    {
        BoardDirection[] moveDirections = new BoardDirection[8];
        int numberOfMoves;
        int maxNumberOfMoves;
        float moveAmountBetweenSquares = 0f;
        float characterMoveSpeed = 1f;
        System.Action func;

        public void Act()
        {
            func?.Invoke();
            Debug.Log(true);
        }

        public void SetMovement(int maxNumOfMoves, int offset, byte[] directions)
        {
            maxNumberOfMoves = maxNumOfMoves;
            FantacticsBitConverter.ToBoardDirections(maxNumberOfMoves, offset, directions, moveDirections);
            numberOfMoves = 0;
            func = MoveAlongDirections;
            CameraManager.Instance.SetTarget(gameObject);
        }

        void MoveAlongDirections()
        {
            moveAmountBetweenSquares += characterMoveSpeed;
            if (moveAmountBetweenSquares > Square.Side)
            {
                MoveBetweenSquares(moveDirections[numberOfMoves], characterMoveSpeed - (moveAmountBetweenSquares - Square.Side));
                moveAmountBetweenSquares = 0f;
                numberOfMoves++;
                if (maxNumberOfMoves == numberOfMoves)
                {
                    func = null;
                }
            }
            else
            {
                MoveBetweenSquares(moveDirections[numberOfMoves], characterMoveSpeed);
            }
        }

        void MoveBetweenSquares(BoardDirection dir, float amount)
        {
            switch (dir)
            {
                case BoardDirection.Up:
                    transform.position += Vector3.forward * amount;
                    break;

                case BoardDirection.Right:
                    transform.position += Vector3.right * amount;
                    break;

                case BoardDirection.Down:
                    transform.position += Vector3.back * amount;
                    break;

                case BoardDirection.Left:
                    transform.position += Vector3.left * amount;
                    break;
            }
        }

        public bool IsAnimationOver()
        {
            return func == null;
        }
    }
}
