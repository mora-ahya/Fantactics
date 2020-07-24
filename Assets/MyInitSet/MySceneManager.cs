using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyInitSet
{
    public class MySceneManager : MonoBehaviour
    {
        static MySceneManager mySceneManagerInstance;
        public static MySceneManager Instance => mySceneManagerInstance;

        delegate SceneProcess SceneProcess();

        SceneProcess scene;
        int result;

        void Awake()
        {
            Screen.SetResolution(800, 640, false);
            mySceneManagerInstance = this;
        }

        void Start()
        {
            scene = TitleScene;
            result = 0;
        }

        SceneProcess TitleScene()
        {

            return null;
        }

        void Update()
        {
            scene = scene();
        }
    }
}
