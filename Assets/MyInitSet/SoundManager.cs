using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyInitSet
{
    public class SoundManager : MonoBehaviour
    {
        static SoundManager soundManagerInstance;
        public static SoundManager Instance => soundManagerInstance;

        [SerializeField] AudioSource bgmSource = default;
        [SerializeField] AudioSource seSource = default;
        [SerializeField] AudioClip[] ses = default;
        [SerializeField] AudioClip[] bgms = default;

        // Start is called before the first frame update
        void Awake()
        {
            soundManagerInstance = this;
        }

        public void PlaySE(int seNum)
        {
            seSource.PlayOneShot(ses[seNum]);
        }

        public void SetBGM(int bgmNum)
        {
            bgmSource.clip = bgms[bgmNum];
            bgmSource.Play();
        }

        public void StopBGM()
        {
            bgmSource.Stop();
        }

        public void SetSEVolum(float volume)
        {
            seSource.volume = volume;
        }

        public void SetBGMVolum(float volume)
        {
            bgmSource.volume = volume;
        }


    }
}
