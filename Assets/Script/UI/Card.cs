using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace FantacticsScripts
{
    public class Card : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        public readonly static float Width = 200f;
        public readonly static float Height = 300f;
        public readonly static float Padding = 25f;
        public readonly static float CenterY = Height / 2;

        static readonly string rangeTextTemplate = "{0}～{1}";

        public CardInfomation CardInfo { get; private set; }
        bool isEmphasized = false;
        [SerializeField] Image image = default;
        [SerializeField] Text cardName = default;
        [SerializeField] Text damage = default;
        [SerializeField] Text range = default;
        [SerializeField] BoxCollider2D boxCollider2D = default;
        public int Number { get; set; }

        public void SetCardInfo(CardInfomation cardInfomation)
        {
            CardInfo = cardInfomation;
            cardName.text = cardInfomation.Name;
        }

        public void ResetPosition(float half)
        {
            gameObject.transform.position = Vector3.up * CenterY + Vector3.right * (((float)Number - half) * (Width + Padding) + Screen.width / 2);
        }
        
        public bool OnMouse(in Vector3 pos)
        {
            return boxCollider2D.OverlapPoint(pos);
        }

        /// <summary>
        /// カードを強調する
        /// </summary>
        public void Emphasize(bool on)
        {
            if (on)
            {
                if (!isEmphasized)
                {
                    gameObject.transform.localScale *= 1.1f;
                    isEmphasized = true;
                }
            }
            else
            {
                if (isEmphasized)
                {
                    gameObject.transform.localScale /= 1.1f;
                    isEmphasized = false;
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            CardOperation.Instance.OnPointerDown(this, eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            CardOperation.Instance.OnDrag(this, eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            CardOperation.Instance.OnPointerUp(this, eventData);
        }
    }
}
