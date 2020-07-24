using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using UnityEngine;

namespace MyInitSet
{
    public class TextLoader : MonoBehaviour
    {
        private static TextLoader tlInstance;

        public static TextLoader Instance => tlInstance;

        private readonly char[] dels = { '\r', '\n' };
        private readonly char rep = '>'; //改行文字
        private readonly string columnBreak = ">>"; //改段落文字
        private readonly char indention = '\n'; //改行置き換え

        private StringBuilder sb = new StringBuilder();
        private TextAsset ta;

        void Awake()
        {
            tlInstance = this;
        }

        public void LoadText(string s, List<string> l)
        {
            ta = Resources.Load(s) as TextAsset;
            if (ta == null)
                return;

            string[] tmp = ta.text.Split(dels);
            foreach (string str in tmp)
            {
                if (str == null)
                    continue;

                if (str.Length > 1 && str.Substring(0, 2) == columnBreak)
                {
                    l.Add(sb.ToString());
                    sb.Clear();
                }
                else
                {
                    sb.Replace(rep, indention);
                    sb.Append(str);
                }
            }

            if (sb.Length != 0)
            {
                l.Add(sb.ToString());
                sb.Clear();
            }
            ta = null;

        }
    }
}
