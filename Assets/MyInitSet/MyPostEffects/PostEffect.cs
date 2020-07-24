using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace MyInitSet
{
    public class PostEffect : MonoBehaviour
    {
        [SerializeField]
        protected Shader shader = default;
        protected Material material;
        protected CommandBuffer cb;
        protected bool isActive;

        public CommandBuffer Cb => cb;

        public bool IsActive
        {
            get
            {
                return isActive;
            }

            set
            {
                isActive = value;
            }
        }

        public virtual void Run()
        {

        }

        public virtual void Clear()
        {

        }

    }
}
