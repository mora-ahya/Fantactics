using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FantacticsScripts
{
    public class PhaseNotice : MonoBehaviour
    {
        readonly static string StringNameSegmentPhase = "Segment Phase";
        readonly static string StringNameMovePhase = "Move Phase";
        readonly static string StringNameRangePhase = "Range Phase";
        readonly static string StringNameMeleePhase = "Melee Phase";
        readonly static Vector3 SmallNoticePosition = new Vector3(200.0f, 800.0f, 0f);
        readonly static Vector3 SmallNoticeScale = new Vector3(0.25f, 0.25f, 0f);
        readonly static float NoticeSpreadingSpeed = 3.0f;
        readonly static float FirstSpeedChangeBound = 1.0f;
        readonly static float SecondSpeedChangeBound = 1.1f;


        [SerializeField] Text text = default;
        [SerializeField] Image image = default;

        Vector3 vector3Tmp;
        public bool IsActing { get; private set; }

        public void Act()
        {
            if (transform.localScale.x < FirstSpeedChangeBound)
            {
                vector3Tmp.Set(NoticeSpreadingSpeed, 0, 0);
                transform.localScale += vector3Tmp * Time.deltaTime;
                return;
            }

            if (transform.localScale.x < SecondSpeedChangeBound)
            {
                vector3Tmp.Set(NoticeSpreadingSpeed / 36.0f, 0, 0);
                transform.localScale += vector3Tmp * Time.deltaTime;
                return;
            }

            if (transform.localScale.y > 0)
            {
                vector3Tmp.Set(0, NoticeSpreadingSpeed, 0);
                transform.localScale -= vector3Tmp * Time.deltaTime;
                return;
            }

            vector3Tmp.Set(SmallNoticeScale.x, SmallNoticeScale.y, 0);
            transform.localScale = vector3Tmp;
            vector3Tmp.Set(SmallNoticePosition.x, SmallNoticePosition.y, 0);
            transform.position = vector3Tmp;
            IsActing = false;
        }

        public void DisplayPhaseNotice(PhaseEnum p)
        {
            switch (p)
            {
                case PhaseEnum.PlottingPhase:
                    text.text = StringNameSegmentPhase;
                    image.color = Color.yellow;
                    break;

                case PhaseEnum.MovePhase:
                    text.text = StringNameMovePhase;
                    image.color = Color.green;
                    break;

                case PhaseEnum.RangePhase:
                    text.text = StringNameRangePhase;
                    image.color = Color.blue;
                    break;

                case PhaseEnum.MeleePhase:
                    text.text = StringNameMovePhase;
                    image.color = Color.red;
                    break;
            }
            vector3Tmp.Set(0, 1f, 1f);
            transform.localScale = vector3Tmp;
            vector3Tmp.Set(Screen.width / 2.0f, Screen.height / 2.0f, 0);
            transform.position = vector3Tmp;
            IsActing = true;
        }
    }
}
