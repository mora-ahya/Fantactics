using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantacticsScripts
{
    public class SquareTarget : MonoBehaviour
    {
        public delegate void SetDirectionDelegate(int dir);
        public delegate void NotifyEndMove();

        SetDirectionDelegate setDirection;
        NotifyEndMove notify;
        BoardDirection moveDirection;
        bool duringTheMove;
        float moveAmountBetweenSquares;
        float objectMoveSpeed = 1f;

        public void Act()
        {
            if (!duringTheMove)
                return;

            MoveToDestination();
        }

        public void SetDirection(int dir)
        {
            if (duringTheMove)
                return;

            setDirection?.Invoke(dir);
        }

        public void Initialize(Transform t, SetDirectionDelegate setDirDelegate, NotifyEndMove n)
        {
            transform.position = t.position;
            setDirection = setDirDelegate;
            notify = n;
            CameraManager.Instance.SetTarget(gameObject);
        }

        public void StartMove(BoardDirection dir)
        {
            moveDirection = dir;
            duringTheMove = true;
        }

        void MoveToDestination()
        {
            moveAmountBetweenSquares += objectMoveSpeed;
            if (moveAmountBetweenSquares > Square.Side)
            {
                MoveBetweenSquares(moveDirection, objectMoveSpeed - (moveAmountBetweenSquares - Square.Side));
                moveAmountBetweenSquares = 0f;
                notify?.Invoke();
                duringTheMove = false;
            }
            else
            {
                MoveBetweenSquares(moveDirection, objectMoveSpeed);
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
    }
}
