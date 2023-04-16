using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//オブジェクトのローカルポジションを使う

namespace FantacticsScripts
{
    public class MoveAnimation : MonoBehaviour
    {
        public bool IsRunning { get; private set; }

        readonly protected List<int> roadSquares = new List<int>(8);
        Queue<Part> parts;
        GameObject movingObject;
        Board board;

        int numberOfMoves;
        int maxNumberOfMoves;
        float moveAmountBetweenSquares = 0f;
        float characterMoveSpeed = 1f;

        Vector3 prePosition = new Vector3();
        Vector3 nextPosition = new Vector3();
        int currentSquareNumber;

        public void Initialize()
        {
            // movedirectionsからpartsを作成
        }

        public void Act()
        {
            MoveFollowingDirections();
            //if (currentAction == null)
            //{
            //    return;
            //}

            //float moveSpeed = characterMoveSpeed * Time.deltaTime;
            //moveAmountBetweenSquares += moveSpeed;

            //if (moveAmountBetweenSquares > 1.0f)
            //{
            //    moveAmountBetweenSquares = 1.0f;
            //    currentAction.Invoke();
            //    CalculateNextPoint();
            //}
            //else
            //{
            //    currentAction.Invoke();
            //}
        }

        public void SetBoard(Board board)
        {
            this.board = board;
        }

        public void SetAnimation(Player player, MoveResult result)
        {
            movingObject = player.gameObject;
            currentSquareNumber = player.Information.CurrentSquare;
            roadSquares.AddRange(result.roadSquares);
            numberOfMoves = 0;
            IsRunning = true;
            //CameraManager.Instance.SetTarget(movingObject);
        }

        void MoveFollowingDirections()
        {
            // 前後のdirectionを比較して、使う関数を決める(Actionは使用しない)
            float moveSpeed = characterMoveSpeed * Time.deltaTime;
            moveAmountBetweenSquares += moveSpeed;
            if (moveAmountBetweenSquares > Square.Side)
            {
                //MoveBetweenSquares(moveDirections[numberOfMoves], moveSpeed - (moveAmountBetweenSquares - Square.Side));
                CalculateNextPoint();
            }
            else
            {
                //MoveBetweenSquares(moveDirections[numberOfMoves], moveSpeed);
            }
        }

        void GenerateAnimationParts(Player player, MoveResult result)
        {
            //BoardDirection initDir = result.MoveDirections[0];
            //player.transform.LookAt(Board.GetVectorByDirection(initDir));

            //int currentSquareNum = player.Information.CurrentSquare;
            //int nextSquareNum = board.GetSquare(currentSquareNum).GetAdjacentSquare(initDir).Number;

            //Vector3 prePos = Board.SquareNumberToWorldPosition(currentSquareNum);
            //Vector3 nextPos = (prePos + Board.SquareNumberToWorldPosition(nextSquareNum)) / 2.0f;

            //parts.Enqueue(new GoStraight(in prePos, in nextPos, true));

            //for (int i = 1; i < result.MoveDirections.Length; i++)
            //{
            //    prePos = nextPos;
            //    if (result.MoveDirections[i] == result.MoveDirections[i - 1])
            //    {
            //        nextPos = prePos + Board.GetVectorByDirection(result.MoveDirections[i]) * Square.Side;
            //        parts.Enqueue(new GoStraight(in prePos, in nextPos));
            //    }
            //    else
            //    {
            //        Vector3 center = prePos + Board.GetVectorByDirection(result.MoveDirections[i - 1]) * Square.Side / 2.0f;
            //        nextPos = center + Board.GetVectorByDirection(result.MoveDirections[i]) * Square.Side / 2.0f;
            //        parts.Enqueue(new Turn(in prePos, in nextPos, in center));
            //    }
            //}

            //最後の半歩を追加
        }

        void MoveBetweenSquares(BoardDirection dir, float amount)
        {
            switch (dir)
            {
                case BoardDirection.Up:
                    movingObject.gameObject.transform.position += Vector3.forward * amount;
                    break;

                case BoardDirection.Right:
                    movingObject.gameObject.transform.position += Vector3.right * amount;
                    break;

                case BoardDirection.Down:
                    movingObject.gameObject.transform.position += Vector3.back * amount;
                    break;

                case BoardDirection.Left:
                    movingObject.gameObject.transform.position += Vector3.left * amount;
                    break;
            }
        }

        void CalculateNextPoint()
        {
            if (maxNumberOfMoves == numberOfMoves + 1)
            {
                IsRunning = false;
                return;
            }

            prePosition.Set(movingObject.transform.localPosition.x, movingObject.transform.localPosition.y, movingObject.transform.localPosition.z);

            // Boardを参照できるようにする
            //currentSquareNumber = board.GetSquare(currentSquareNumber).GetAdjacentSquare(moveDirections[numberOfMoves]).Number;
            //Board.SquareNumberToWorldPosition(board.GetSquare(currentSquareNumber).GetAdjacentSquare(moveDirections[numberOfMoves + 1]).Number, ref nextPosition);
            numberOfMoves++;
            moveAmountBetweenSquares = 0f;
        }

        abstract class Part
        {
            protected Vector3 prePosition;
            protected Vector3 nextPosition;

            public abstract void PreProcess(Transform transform);

            public abstract void Process(Transform transform, float moveAmountBetweenSquares);

            public abstract void PostProcess(Transform transform);
        }

        class Turn : Part
        {
            protected Vector3 center;
            public Turn(in Vector3 prePos, in Vector3 nextPos, in Vector3 center)
            {
                prePosition = prePos;
                nextPosition = nextPos;
                this.center = center;
            }

            public override void PreProcess(Transform transform)
            {

            }

            public override void Process(Transform transform, float moveAmountBetweenSquares)
            {
                transform.position = Mathf.Pow(1f - moveAmountBetweenSquares, 2) * prePosition;
                transform.position += 2f * (1f - moveAmountBetweenSquares) * moveAmountBetweenSquares * center;
                transform.position += Mathf.Pow(moveAmountBetweenSquares, 2) * nextPosition;

                transform.LookAt((center - prePosition) * (1f - moveAmountBetweenSquares) + (nextPosition - prePosition) * moveAmountBetweenSquares);
            }

            public override void PostProcess(Transform transform)
            {
                transform.position = nextPosition;
            }
        }

        class GoStraight : Part
        {
            bool adjustment;
            public GoStraight(in Vector3 prePos, in Vector3 nextPos, bool adjustment = false)
            {
                prePosition = prePos;
                nextPosition = nextPos;
                this.adjustment = adjustment;
            }

            public override void PreProcess(Transform transform)
            {

            }

            public override void Process(Transform transform, float moveAmountBetweenSquares)
            {
                transform.position = Vector3.Lerp(prePosition, nextPosition, moveAmountBetweenSquares);
            }

            public override void PostProcess(Transform transform)
            {
                transform.position = nextPosition;
            }
        }
    }
}
