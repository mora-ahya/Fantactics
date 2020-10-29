using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantacticsScripts
{
    public class CharacterSelectScene : MonoBehaviour
    {
        [SerializeField] GameObject[] charaIcons = default;

        GameScene gameScene;
        int selectableChara;//選択可能なキャラを1、不可能なキャラを0で表す => 247 = 0b11110111 = 3番目のキャラは選択不可
        int selectedCharaNumber;//選択されているキャラの番号を保存、複数選択に対応するため4bit区切りで表示 => 2と4を選択 = 0b1111/0010/0100

        public void Initialized(GameScene gScene)
        {
            gameScene = gScene;
        }

        public void Act()
        {
            if (Input.GetMouseButtonUp(0))
            {

            }
        }

        public void DecideCharacter()
        {
            //gameSceneに投げる
        }

        public void RejectCharacter(int unselectableChara)
        {
            //gameSceneから投げられる
        }
    }
}
