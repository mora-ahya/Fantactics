using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantacticsScripts
{
    public class CameraManager : MonoBehaviour
    {
        public enum CameraModeEnum
        {
            FreeMode,
            FollowMode,
            ShakeMode,
        }

        public static CameraManager Instance { get; private set; }

        readonly Vector2 yRange = new Vector2(12.5f, 60f);
        readonly float minXRange = 0f;
        readonly float minZRange = -11f;

        public CameraTarget cameraTarget { get; private set; } = null;
        Vector3 positoinTmp = new Vector3();
        Vector3 prePosition = new Vector3();
        Vector3 posOffset = new Vector3(0, 20.0f, -20.0f);
        System.Action mode;

        void Awake()
        {
            Instance = this;
            mode = FollowMode;
        }

        public void SetPosition(int squareNumber)
        {
            positoinTmp.Set(Square.Side * (squareNumber % Board.Width), 0, Square.Side * (squareNumber / Board.Width));
            positoinTmp += posOffset;
            transform.position = positoinTmp;
        }

        public void SetPosition(Vector3 setPos)
        {
            transform.position = setPos + posOffset;
        }

        public void SetOffset(float x, float y, float z)
        {
            posOffset.Set(x, y, z);
            if (cameraTarget)
            {
                transform.position = cameraTarget.transform.position + posOffset;
                transform.LookAt(cameraTarget.transform);
            }
        }

        public void SetTarget(CameraTarget target)
        {
            cameraTarget = target;
            if (cameraTarget)
            {
                transform.position = cameraTarget.transform.position + posOffset;
                transform.LookAt(cameraTarget.transform);
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
                positoinTmp.y -= posOffset.normalized.y * Input.mouseScrollDelta.y;

                if (positoinTmp.y >= yRange.x && positoinTmp.y <= yRange.y)
                {
                    positoinTmp.z -= posOffset.normalized.z * Input.mouseScrollDelta.y;
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
            if (cameraTarget == null)
                return;

            transform.position = cameraTarget.transform.position + posOffset;
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
            if (cameraTarget == null)
                return;

            float difX = Random.Range(-1.0f, 1.0f);
            float difY = Random.Range(-1.0f, 1.0f);
            positoinTmp.Set(difX, difY, 0.0f);

            transform.position = cameraTarget.transform.position + posOffset + positoinTmp;
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

        public void SetMode(CameraModeEnum modeEnum)
        {
            switch (modeEnum)
            {
                case CameraModeEnum.FreeMode:
                    mode = FreeMode;
                    break;

                case CameraModeEnum.FollowMode:
                    mode = FollowMode;
                    break;

                case CameraModeEnum.ShakeMode:
                    break;
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
