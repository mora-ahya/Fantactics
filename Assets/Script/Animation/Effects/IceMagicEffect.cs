using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantacticsScripts
{
    public class IceMagicEffect : AttackEffect
    {
        readonly int vanishID = Shader.PropertyToID("_Vanish");
        readonly float initIceHeight = -36.6f;
        [SerializeField] GameObject[] iceModels = default;
        [SerializeField] Material iceMaterial = default;
        [SerializeField] Material frostMaterial = default;
        int count = -30;

        System.Action phase;

        void Start()
        {
            phase = AppeareFrostPhase;
            iceMaterial.SetFloat(vanishID, 0f);
            //CameraManager.Instance.SetOffset(0, 70f, -30f);
        }

        public override void Initialize()
        {
            IsEnd = false;
            count = 0;
            phase = AppeareFrostPhase;
            iceMaterial.SetFloat(vanishID, 0f);
            frostMaterial.SetFloat(vanishID, 0f);
        }

        void Update()
        {
            //CameraManager.Instance.SetOffset(20, 20f, -30f);
            //CameraManager.Instance.SetTarget(iceModels[0]);
            //phase?.Invoke();
        }

        public override void Act()
        {
            phase?.Invoke();
        }

        void AppeareFrostPhase()
        {
            count++;
            if (count < 0)
                return;

            frostMaterial.SetFloat(vanishID, count / 100.0f);

            if (count == 75)
            {
                phase = AppeareIcePhase;
                count = -1;
                frostMaterial.SetFloat(vanishID, 1.0f);
            }
        }

        void AppeareIcePhase()
        {
            count++;
            if (count < 0)
                return;

            CameraManager.Instance.Test(true);
            iceModels[count / 3].transform.position -= Vector3.up * (initIceHeight / 3.0f);

            if ((count + 1) / 3 == iceModels.Length)
            {
                CameraManager.Instance.Test(false);
                phase = VanishingIcePhase;
                count = -120;
            }

        }

        void VanishingIcePhase()
        {
            count++;
            if (count < 0)
                return;

            iceMaterial.SetFloat(vanishID, count / 100.0f);
            frostMaterial.SetFloat(vanishID, 1.0f - count / 100.0f);

            if (count == 100)
            {
                IsEnd = true;
                phase = null;
            }
        }
    }
}
