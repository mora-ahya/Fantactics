using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantacticsScripts
{
    public class CameraManager : MonoBehaviour
    {
        public static CameraManager Instance { get; private set; }

        readonly static Vector3 Offset = new Vector3(0, 11.5f, -11.5f);
        readonly static Vector2 YRange = new Vector2(12.5f, 60f);
        readonly static float MinXRange = 0f;
        readonly static float MinZRange = -11f;

        public GameObject Target { get; private set; } = null;
        Vector3 positoinTmp = new Vector3();
        Vector3 prePosition = new Vector3();
        System.Action mode;

        void Awake()
        {
            Instance = this;
        }

        public void SetPosition(int squareNumber)
        {
            positoinTmp.Set(Square.Side * (squareNumber % Board.Width), 0, Square.Side * (squareNumber / Board.Width));
            positoinTmp += Offset;
            transform.position = positoinTmp;
        }

        public void SetPosition(Transform trans)
        {
            transform.position = trans.position + Offset;
        }

        public void SetTarget(GameObject t)
        {
            Target = t;
        }

        public void MoveSquare(BoardDirection dir)
        {
            switch (dir)
            {
                case BoardDirection.Up:
                    transform.position += Vector3.forward * Square.Side;
                    break;

                case BoardDirection.Right:
                    transform.position += Vector3.right * Square.Side;
                    break;

                case BoardDirection.Down:
                    transform.position += Vector3.back * Square.Side;
                    break;

                case BoardDirection.Left:
                    transform.position += Vector3.left * Square.Side;
                    break;
            }
        }

        public void Act()
        {
            mode?.Invoke();
        }

        void FreeMode()
        {
            positoinTmp = transform.position;

            if (Input.mouseScrollDelta.y != 0)//まだ未完成
            {
                positoinTmp.y -= Offset.normalized.y * Input.mouseScrollDelta.y;

                if (positoinTmp.y >= YRange.x && positoinTmp.y <= YRange.y)
                {
                    positoinTmp.z -= Offset.normalized.z * Input.mouseScrollDelta.y;
                    positoinTmp.z = Mathf.Clamp(positoinTmp.z, MinZRange, Board.Height * Square.Side + MinZRange);
                }
                positoinTmp.y = Mathf.Clamp(positoinTmp.y, YRange.x, YRange.y);
            }

            if (Input.GetMouseButtonDown(0))
            {
                prePosition.Set(Input.mousePosition.x, 0, Input.mousePosition.y);
            }

            if (Input.GetMouseButton(0))
            {
                prePosition.x -= Input.mousePosition.x;
                prePosition.z -= Input.mousePosition.y;
                positoinTmp += prePosition / 10f;
                prePosition.Set(Input.mousePosition.x, 0, Input.mousePosition.y);
                positoinTmp.x = Mathf.Clamp(positoinTmp.x, MinXRange, Board.Width * Square.Side);
                positoinTmp.z = Mathf.Clamp(positoinTmp.z, MinZRange, Board.Height * Square.Side + MinZRange);
            }

            if (transform.position != positoinTmp)
                transform.position = positoinTmp;
        }

        void FollowMode()
        {
            if (Target == null)
                return;

            transform.position = Target.transform.position + Offset;
        }

        public void ChangeMode()
        {
            if (mode == FollowMode)
            {
                mode = FreeMode;
            }
            else
            {
                mode = FollowMode;
            }
        }

        float CalculateXAxisOfMinReachableRange(float y)
        {
            return (Board.Width * Square.Side / 2) / (YRange.y - YRange.x) * (y - YRange.x);
        }

        float CalculateZAxisOfMinReachableRange(float y)
        {
            return (Board.Height * Square.Side / 2) / (YRange.y - YRange.x) * (y - YRange.x) - 6.5f;
        }
    }
}
