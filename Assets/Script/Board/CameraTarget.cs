using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantacticsScripts
{
    public class CameraTarget : MonoBehaviour
    {
        public delegate void EndMoveHandler();
        public event EndMoveHandler OnEndMove;
        public bool IsMoving { get; private set; }

        Vector3 startPosition;
        Vector3 destPosition;
        Vector3 slerpCenter;

        float moveAmount;
        float objectMoveSpeed = 1f;

        bool isSlerp = false;

        public void SetPosition(Vector3 initPos)
        {
            transform.position = initPos;
        }

        public void AddEndMoveHandler(EndMoveHandler handler)
        {
            OnEndMove += handler;
        }

        public void RemoveEndMoveHandler(EndMoveHandler handler)
        {
            OnEndMove -= handler;
        }

        public void StartMove(Vector3 destPos)
        {
            if (IsMoving)
            {
                return;
            }
            moveAmount = 0f;
            startPosition = transform.position;
            destPosition = destPos;
            isSlerp = false;
            IsMoving = true;
            StartCoroutine(MoveToDestination());
        }

        public void StartMove(BoardDirection dir)
        {
            if (IsMoving)
            {
                return;
            }
            StartMove(transform.position + Board.GetVectorByDirection(dir) * Square.Side);
        }

        public void StartMove(Vector3 destPos, Vector3 centerPos)
        {
            if (IsMoving)
            {
                return;
            }
            moveAmount = 0f;
            startPosition = transform.position - centerPos;
            destPosition = destPos - centerPos;
            slerpCenter = centerPos;
            isSlerp = true;
            IsMoving = true;
            StartCoroutine(MoveToDestination());
        }

        IEnumerator MoveToDestination()
        {

            while(moveAmount <= 1.0f)
            {
                moveAmount += objectMoveSpeed * Time.deltaTime;

                if (isSlerp)
                {
                    transform.position = Vector3.Slerp(startPosition, destPosition, moveAmount) + slerpCenter;
                }
                else
                {
                    transform.position = Vector3.Lerp(startPosition, destPosition, moveAmount);
                }

                if (moveAmount >= 1.0f)
                {
                    OnEndMove?.Invoke();
                    IsMoving = false;
                    break;
                }

                yield return null;
            }
        }
    }
}
