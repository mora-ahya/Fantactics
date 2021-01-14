using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantacticsScripts
{
    public class CameraManager : MonoBehaviour
    {
        public static CameraManager Instance { get; private set; }

        readonly Vector2 yRange = new Vector2(12.5f, 60f);
        readonly float minXRange = 0f;
        readonly float minZRange = -11f;

        public GameObject Target { get; private set; } = null;
        Vector3 positoinTmp = new Vector3();
        Vector3 prePosition = new Vector3();
        Vector3 offset = new Vector3(0, 11.5f, -11.5f);
        System.Action mode;

        void Awake()
        {
            Instance = this;
            mode = FollowMode;
        }

        public void SetPosition(int squareNumber)
        {
            positoinTmp.Set(Square.Side * (squareNumber % Board.Width), 0, Square.Side * (squareNumber / Board.Width));
            positoinTmp += offset;
            transform.position = positoinTmp;
        }

        public void SetPosition(Transform trans)
        {
            transform.position = trans.position + offset;
        }

        public void SetOffset(float x, float y, float z)
        {
            offset.Set(x, y, z);
        }

        public void SetTarget(GameObject t)
        {
            Target = t;
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
                positoinTmp.y -= offset.normalized.y * Input.mouseScrollDelta.y;

                if (positoinTmp.y >= yRange.x && positoinTmp.y <= yRange.y)
                {
                    positoinTmp.z -= offset.normalized.z * Input.mouseScrollDelta.y;
                    positoinTmp.z = Mathf.Clamp(positoinTmp.z, minZRange, Board.Height * Square.Side + minZRange);
                }
                positoinTmp.y = Mathf.Clamp(positoinTmp.y, yRange.x, yRange.y);
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
                positoinTmp.x = Mathf.Clamp(positoinTmp.x, minXRange, Board.Width * Square.Side);
                positoinTmp.z = Mathf.Clamp(positoinTmp.z, minZRange, Board.Height * Square.Side + minZRange);
            }

            if (transform.position != positoinTmp)
                transform.position = positoinTmp;
        }

        void FollowMode()
        {
            if (Target == null)
                return;

            transform.position = Target.transform.position + offset;
        }

        public void Test(bool b)
        {
            if (b)
            {
                mode = ShakeMode;
            }
            else
            {
                mode = FollowMode;
            }
        }

        void ShakeMode()
        {
            if (Target == null)
                return;

            float difX = Random.Range(-1.0f, 1.0f);
            float difY = Random.Range(-1.0f, 1.0f);
            positoinTmp.Set(difX, difY, 0.0f);

            transform.position = Target.transform.position + offset + positoinTmp;
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
            return (Board.Width * Square.Side / 2) / (yRange.y - yRange.x) * (y - yRange.x);
        }

        float CalculateZAxisOfMinReachableRange(float y)
        {
            return (Board.Height * Square.Side / 2) / (yRange.y - yRange.x) * (y - yRange.x) - 6.5f;
        }
    }
}
