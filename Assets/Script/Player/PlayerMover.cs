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
                transform.position = Mathf.Pow(1f - movePack.moveAmountBetweenSquares, 2) * movePack.nextPosition;
                transform.position += 2f * (1f - movePack.moveAmountBetweenSquares) * movePack.moveAmountBetweenSquares * movePack.center;
                transform.position += Mathf.Pow(movePack.moveAmountBetweenSquares, 2) * movePack.nextPosition;

                transform.LookAt((movePack.center - movePack.nextPosition) * (1f - movePack.moveAmountBetweenSquares) + (movePack.nextPosition - movePack.nextPosition) * movePack.moveAmountBetweenSquares);
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

        public bool IsRunning { get; private set; }

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

        public bool SetAnimation(MovePhaseResult result)
        {
            if (IsRunning)
            {
                return false;
            }

            result.MoveDirections.CopyTo(moveDirections, 0);
            IsRunning = true;
            StartCoroutine(MoveFollowingDirections(result.NumOfMoves, result.LastDirection));
            //CameraManager.Instance.SetTarget(movingObject);
            return true;
        }

        IEnumerator MoveFollowingDirections(int numOfMove, BoardDirection lastDirection)
        {
            float moveSpeed;
            MovePack movePack = new MovePack();
            int nextSquareNum = player.Information.CurrentSquare;
            bool isContinue;
            Part part;

            if (numOfMove != 0)
            {
                player.transform.LookAt(Board.GetVectorByDirection(moveDirections[0]));
            }

            for (int i = 0; i < numOfMove; i++)
            {
                nextSquareNum = board.GetSquare(nextSquareNum).GetAdjacentSquare(moveDirections[i]).Number;
                part = CalculateNextMove(i, nextSquareNum, ref movePack);

                movePack.moveAmountBetweenSquares = 0.0f;
                isContinue = true;
                // アニメーションステートの変更

                while (isContinue)
                {
                    moveSpeed = characterMoveSpeed * Time.deltaTime;
                    movePack.moveAmountBetweenSquares += moveSpeed;

                    if (movePack.moveAmountBetweenSquares >= 1.0f)
                    {
                        isContinue = false;
                        movePack.moveAmountBetweenSquares = 1.0f;
                    }

                    // 移動式の分岐の仕方
                    part.Process(player.transform, ref movePack);
                    yield return null;
                }
            }

            IsRunning = false;
            player.transform.LookAt(Board.GetVectorByDirection(lastDirection));
        }

        Part CalculateNextMove(int moveNum, int nextSquareNum, ref MovePack movePack) // 移動方式を返す
        {
            BoardDirection direction = moveDirections[moveNum];

            if (moveNum == 0 || moveNum == moveDirections.Length)
            {
                movePack.nextPosition = movePack.currentPosition + Board.GetVectorByDirection(direction) * Square.Side / 2.0f;
                return parts[(int)MoveType.GoStraight];
            }

            // Boardの情報からアクションが必要か調べる nextSquareNumを使用 別関数にするかもしれない

            BoardDirection nextDirection = moveDirections[moveNum + 1];

            if (direction == nextDirection)
            {
                movePack.nextPosition = movePack.currentPosition + Board.GetVectorByDirection(direction) * Square.Side;
                return parts[(int)MoveType.GoStraight];
            }
            else
            {
                movePack.center = movePack.currentPosition + Board.GetVectorByDirection(nextDirection) * Square.Side / 2.0f;
                movePack.nextPosition = movePack.center + Board.GetVectorByDirection(direction) * Square.Side / 2.0f;
                return parts[(int)MoveType.Turn];
            }
        }
    }
}
