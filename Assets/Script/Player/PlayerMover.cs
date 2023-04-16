using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantacticsScripts
{
    public class PlayerMover : MonoBehaviour
    {
        struct MovePack
        {
            public Vector3 startPosition;
            public Vector3 endPosition;
            public Vector3 center;
            public float distance;
            public Part part;
        }

        abstract class Part
        {
            public abstract void Process(Transform transform, ref MovePack movePack, float moveAmountBetweenSquares);
        }

        class Turn : Part
        {
            public override void Process(Transform transform, ref MovePack movePack, float moveAmountBetweenSquares)
            {
                transform.position = Mathf.Pow(1.0f - moveAmountBetweenSquares, 2.0f) * movePack.startPosition;
                transform.position += 2.0f * (1.0f - moveAmountBetweenSquares) * moveAmountBetweenSquares * movePack.center;
                transform.position += Mathf.Pow(moveAmountBetweenSquares, 2.0f) * movePack.endPosition;

                //Vector3 tmp = (movePack.center - movePack.startPosition) * (1.0f - movePack.moveAmountBetweenSquares) + (movePack.endPosition - movePack.startPosition) * movePack.moveAmountBetweenSquares;
                transform.forward = (movePack.center - movePack.startPosition) * (1.0f - moveAmountBetweenSquares) + (movePack.endPosition - movePack.center) * moveAmountBetweenSquares;
            }
        }

        class GoStraight : Part
        {
            public override void Process(Transform transform, ref MovePack movePack, float moveAmountBetweenSquares)
            {
                transform.position = Vector3.Lerp(movePack.startPosition, movePack.endPosition, moveAmountBetweenSquares);
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

        readonly List<MovePack> movePacks = new List<MovePack>(8);
        
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

        public bool SetAnimationForDirection(List<int> roadSquaresSource, BoardDirection lastDirection)
        {
            if (IsMoving)
            {
                return false;
            }

            IsMoving = true;
            GenerateMovePacks(roadSquaresSource);
            StartCoroutine(MoveFollowingDirections(lastDirection));
            //CameraManager.Instance.SetTarget(movingObject);
            return true;
        }

        IEnumerator MoveFollowingDirections(BoardDirection lastDirection)
        {
            if (movePacks.Count > 0)
            {
                player.transform.forward = (movePacks[0].endPosition - movePacks[0].startPosition).normalized;
            }

            for (int i = 0; i < movePacks.Count; i++)
            {
                float moveAmountBetweenSquares = 0.0f;
                bool isContinue = true;
                MovePack movePack = movePacks[i];

                while (isContinue)
                {
                    yield return null;
                    moveAmountBetweenSquares += characterMoveSpeed / movePack.distance * Square.Side * Time.deltaTime;

                    if (moveAmountBetweenSquares > 1.0f)
                    {
                        moveAmountBetweenSquares = 1.0f;
                        isContinue = false;
                    }

                    movePack.part.Process(player.transform, ref movePack, moveAmountBetweenSquares);
                }
            }

            IsMoving = false;
            player.transform.forward = Board.GetVectorByDirection(lastDirection);
        }

        void GenerateMovePacks(List<int> roadSquaresSource) // ˆÚ“®•ûŽ®‚ð•Ô‚·
        {
            movePacks.Clear();

            if (roadSquaresSource.Count == 1)
            {
                return;
            }

            int preSquare = roadSquaresSource[0];
            int currentSquare = roadSquaresSource[1];
            int diffSquare = currentSquare - preSquare;
            MovePack movePack = new MovePack();
            movePack.startPosition = Board.SquareNumberToWorldPosition(preSquare);

            for (int i = 2; i < roadSquaresSource.Count; i++)
            {
                if (roadSquaresSource[i] - currentSquare != diffSquare)
                {
                    movePack.endPosition = (Board.SquareNumberToWorldPosition(preSquare) + Board.SquareNumberToWorldPosition(currentSquare)) / 2.0f;
                    movePack.distance = Vector3.Distance(movePack.startPosition, movePack.endPosition);
                    if (movePack.distance > Mathf.Epsilon)
                    {
                        movePack.part = parts[(int)MoveType.GoStraight];
                        movePacks.Add(movePack);
                    }

                    movePack.startPosition = movePack.endPosition;
                    movePack.center = Board.SquareNumberToWorldPosition(currentSquare);
                    movePack.endPosition = (Board.SquareNumberToWorldPosition(currentSquare) + Board.SquareNumberToWorldPosition(roadSquaresSource[i])) / 2.0f;
                    movePack.part = parts[(int)MoveType.Turn];
                    movePack.distance = Square.Side;
                    movePacks.Add(movePack);

                    movePack.startPosition = movePack.endPosition;
                    diffSquare = roadSquaresSource[i] - currentSquare;
                }

                preSquare = currentSquare;
                currentSquare = roadSquaresSource[i];
            }

            movePack.endPosition = Board.SquareNumberToWorldPosition(roadSquaresSource[roadSquaresSource.Count - 1]);
            movePack.distance = Vector3.Distance(movePack.startPosition, movePack.endPosition);

            if (movePack.distance > Mathf.Epsilon)
            {
                movePack.part = parts[(int)MoveType.GoStraight];
                movePacks.Add(movePack);
            }
        }
    }
}
