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
        readonly static float SmallNoticePositionX = 200f;
        readonly static float SmallNoticePositionY = 800f;


        [SerializeField] Text text = default;
        [SerializeField] Image image = default;

        Vector3 vector3Tmp;
        public bool IsActing { get; private set; }

        public void Act()
        {
            if (transform.localScale.x < 1f)
            {
                vector3Tmp.Set(0.1f, 0, 0);
                transform.localScale += vector3Tmp;
                return;
            }

            if (transform.localScale.x < 1.1f)
            {
                vector3Tmp.Set(0.0015f, 0, 0);
                transform.localScale += vector3Tmp;
                return;
            }

            if (transform.localScale.y > 0)
            {
                vector3Tmp.Set(0, 0.1f, 0);
                transform.localScale -= vector3Tmp;
                return;
            }

            vector3Tmp.Set(0.25f, 0.25f, 0);
            transform.localScale = vector3Tmp;
            vector3Tmp.Set(SmallNoticePositionX, SmallNoticePositionY, 0);
            transform.position = vector3Tmp;
            IsActing = false;
        }

        public void DisplayPhaseNotice(Phase p)
        {
            switch (p)
            {
                case Phase.PlottingPhase:
                    text.text = StringNameSegmentPhase;
                    image.color = Color.yellow;
                    break;

                case Phase.MovePhase:
                    text.text = StringNameMovePhase;
                    image.color = Color.green;
                    break;

                case Phase.RangePhase:
                    text.text = StringNameRangePhase;
                    image.color = Color.blue;
                    break;

                case Phase.MeleePhase:
                    text.text = StringNameMovePhase;
                    image.color = Color.red;
                    break;
            }
            vector3Tmp.Set(0, 1f, 1f);
            transform.localScale = vector3Tmp;
            vector3Tmp.Set(Screen.width / 2, Screen.height / 2, 0);
            transform.position = vector3Tmp;
            IsActing = true;
        }
    }
}
