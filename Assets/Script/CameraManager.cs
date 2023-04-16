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

        Vector2 rangeLimitX = new Vector2();
        Vector2 rangeLimitY = new Vector2();
        Vector2 rangeLimitZ = new Vector2();

        float moveSpeed = 0.1f;

        public CameraTarget cameraTarget { get; private set; } = null;
        Vector3 positoinTmp = new Vector3();
        Vector3 prePosition = new Vector3();
        Vector3 posOffset = new Vector3(0, 20.0f, -20.0f);
        System.Action mode;

        void Awake()
        {
            Instance = this;
            mode = FollowMode;

            rangeLimitX.Set(0.0f, Board.Width * Square.Side);
            rangeLimitY.Set(12.5f, 60.0f);
            rangeLimitZ.Set(-11.0f, Board.Height * Square.Side + -11.0f);
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

                if (positoinTmp.y >= rangeLimitY.x && positoinTmp.y <= rangeLimitY.y)
                {
                    positoinTmp.z -= posOffset.normalized.z * Input.mouseScrollDelta.y;
                    positoinTmp.z = Mathf.Clamp(positoinTmp.z, rangeLimitZ.x, Board.Height * Square.Side + rangeLimitZ.x);
                }
                positoinTmp.y = Mathf.Clamp(positoinTmp.y, rangeLimitY.x, rangeLimitY.y);
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
                positoinTmp.x = Mathf.Clamp(positoinTmp.x, rangeLimitX.x, rangeLimitX.y);
                positoinTmp.z = Mathf.Clamp(positoinTmp.z, rangeLimitZ.x, rangeLimitZ.y);
            }

            if (transform.position != positoinTmp)
                transform.position = positoinTmp;
        }

        public void Zoom(float zoomDelta)
        {
            positoinTmp = transform.position;

            if (zoomDelta != 0)//まだ未完成
            {
                positoinTmp.y -= posOffset.normalized.y * zoomDelta;

                if (positoinTmp.y >= rangeLimitY.x && positoinTmp.y <= rangeLimitY.y)
                {
                    positoinTmp.z -= posOffset.normalized.z * zoomDelta;
                    positoinTmp.z = Mathf.Clamp(positoinTmp.z, rangeLimitZ.x, rangeLimitZ.y);
                }
                positoinTmp.y = Mathf.Clamp(positoinTmp.y, rangeLimitY.x, rangeLimitY.y);
            }

            transform.position = positoinTmp;
        }

        public void MoveWithinLimits(float moveDeltaX, float moveDeltaY, float moveDeltaZ)
        {
            Vector3 posTmp = new Vector3();

            posTmp.x = Mathf.Clamp(transform.position.x + moveDeltaX * moveSpeed, rangeLimitX.x, rangeLimitX.y);
            posTmp.y = Mathf.Clamp(transform.position.y + moveDeltaY * moveSpeed, rangeLimitY.x, rangeLimitY.y);
            posTmp.z = Mathf.Clamp(transform.position.z + moveDeltaZ * moveSpeed, rangeLimitZ.x, rangeLimitZ.y);

            transform.position = posTmp;
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
            return (Board.Width * Square.Side / 2) / (rangeLimitY.y - rangeLimitY.x) * (y - rangeLimitY.x);
        }

        float CalculateZAxisOfMinReachableRange(float y)
        {
            return (Board.Height * Square.Side / 2) / (rangeLimitY.y - rangeLimitY.x) * (y - rangeLimitY.x) - 6.5f;
        }
    }
}
