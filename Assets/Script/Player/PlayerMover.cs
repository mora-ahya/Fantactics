using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantacticsScripts
{
    public class PlayerMover : MonoBehaviour
    {
        struct MovePack
        {
            public Vector3 currentPosition;
            public Vector3 nextPosition;
            public Vector3 center;
            public float moveAmountBetweenSquares;
        }

        abstract class Part
        {
            public abstract void Process(Transform transform, ref MovePack movePack);
        }

        class Turn : Part
        {
            public override void Process(Transform transform, ref MovePack movePack)
            {
                transform.position = Mathf.Pow(1.0f - movePack.moveAmountBetweenSquares, 2.0f) * movePack.currentPosition;
                transform.position += 2.0f * (1.0f - movePack.moveAmountBetweenSquares) * movePack.moveAmountBetweenSquares * movePack.center;
                transform.position += Mathf.Pow(movePack.moveAmountBetweenSquares, 2.0f) * movePack.nextPosition;

                Vector3 tmp = (movePack.center - movePack.currentPosition) * (1.0f - movePack.moveAmountBetweenSquares) + (movePack.nextPosition - movePack.currentPosition) * movePack.moveAmountBetweenSquares;
                transform.forward = (movePack.center - movePack.currentPosition) * (1.0f - movePack.moveAmountBetweenSquares) + (movePack.nextPosition - movePack.center) * movePack.moveAmountBetweenSquares;
            }
        }

        class GoStraight : Part
        {
            public override void Process(Transform transform, ref MovePack movePack)
            {
                transform.position = Vector3.Lerp(movePack.currentPosition, movePack.nextPosition, movePack.moveAmountBetweenSquares);
            }
        }

        enum MoveType
        {
            GoStraight,
            Turn,
        }

        static readonly Part[] parts =
        {
            new GoStraight(),
            new Turn(),
        };

        public bool IsMoving { get; private set; }

        readonly protected BoardDirection[] moveDirections = new BoardDirection[8];
        
        Player player;
        Board board;

        float characterMoveSpeed = 1f;

        public void Initialize(Player p, Board b)
        {
            player = p;
            board = b;
        }

        public void SetCharacterMoveSpeed(float speed)
        {
            characterMoveSpeed = speed;
        }

        public void Act()
        {
            
        }

        public bool SetAnimationForDirection(BoardDirection[] moveDirectionsSource, int numOfMove, BoardDirection lastDirection)
        {
            if (IsMoving)
            {
                return false;
            }

            moveDirectionsSource.CopyTo(moveDirections, 0);
            IsMoving = true;
            StartCoroutine(MoveFollowingDirections(player.Information.CurrentSquare, numOfMove, lastDirection));
            //CameraManager.Instance.SetTarget(movingObject);
            return true;
        }

        IEnumerator MoveFollowingDirections(int firstSquareNum, int numOfMove, BoardDirection lastDirection)
        {
            float moveDelta;
            float speedRate = 1.0f;
            MovePack movePack = new MovePack();
            movePack.currentPosition = Board.SquareNumberToWorldPosition(firstSquareNum);
            int nextSquareNum = firstSquareNum;
            bool isContinue;
            Part part;

            if (numOfMove != 0)
            {
                player.transform.forward = (Board.GetVectorByDirection(moveDirections[0]));
            }

            for (int i = -1; i < numOfMove; i++)
            {
                part = CalculateNextMove(i, numOfMove, ref speedRate, ref nextSquareNum, ref movePack);
                
                movePack.moveAmountBetweenSquares = 0.0f;
                isContinue = true;
                // アニメーションステートの変更

                while (isContinue)
                {
                    moveDelta = characterMoveSpeed * speedRate * Time.deltaTime;
                    movePack.moveAmountBetweenSquares += moveDelta;

                    if (movePack.moveAmountBetweenSquares >= 1.0f)
                    {
                        isContinue = false;
                        movePack.moveAmountBetweenSquares = 1.0f;
                    }

                    // 移動式の分岐の仕方
                    part.Process(player.transform, ref movePack);
                    yield return null;
                }

                movePack.currentPosition = movePack.nextPosition;
            }

            IsMoving = false;
            player.transform.forward = Board.GetVectorByDirection(lastDirection);
        }

        Part CalculateAdjustMove(BoardDirection direction, int nextSquareNum, ref MovePack movePack)
        {
            // Boardの情報からアクションが必要か調べる nextSquareNumを使用 別変数にするかもしれない
            movePack.nextPosition = movePack.currentPosition + Board.GetVectorByDirection(direction) * Square.Side / 2.0f;
            return parts[(int)MoveType.GoStraight];
        }

        Part CalculateNextMove(int moveNum, int numOfMove, ref float speedRate, ref int nextSquareNum, ref MovePack movePack) // 移動方式を返す
        {
            BoardDirection direction = moveDirections[Mathf.Clamp(moveNum, 0, moveDirections.Length)];

            if (moveNum == -1 || moveNum == numOfMove - 1)
            {
                movePack.nextPosition = movePack.currentPosition + Board.GetVectorByDirection(direction) * Square.Side / 2.0f;
                speedRate = 2.0f;
                return parts[(int)MoveType.GoStraight];
            }

            nextSquareNum = board.GetSquare(nextSquareNum).GetAdjacentSquare(direction).Number;
            // Boardの情報からアクションが必要か調べる nextSquareNumを使用 別変数にするかもしれない

            BoardDirection nextDirection = moveDirections[moveNum + 1];

            if (direction == nextDirection)
            {
                movePack.nextPosition = movePack.currentPosition + Board.GetVectorByDirection(direction) * Square.Side;
                speedRate = 1.0f;
                return parts[(int)MoveType.GoStraight];
            }
            else
            {
                movePack.center = movePack.currentPosition + Board.GetVectorByDirection(direction) * Square.Side / 2.0f;
                movePack.nextPosition = movePack.center + Board.GetVectorByDirection(nextDirection) * Square.Side / 2.0f;
                speedRate = 1.0f;
                return parts[(int)MoveType.Turn];
            }
        }
    }
}
