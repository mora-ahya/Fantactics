using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyInitSet
{
    public class BitCalculation
    {
        public static int BitCount(int bit)
        {
            bit = bit - ((bit >> 1) & 0x55555555);
            bit = (bit & 0x33333333) + ((bit >> 2) & 0x33333333);
            bit = (bit + (bit >> 4)) & 0x0f0f0f0f;
            bit = bit + (bit >> 8);
            bit = bit + (bit >> 16);
            return bit & 0x3f;
        }

        /// <summary>
        /// bitのnumber番目に立ってる1のみ立っているbitを返す
        /// numberがbitの1の総数より大きいとき一番左のものを返す
        /// number,bitが0以下のとき0を返す
        /// (例)bit=10110,number=2のとき10000を返す
        /// </summary>
        /// <param name="bit"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public static int GetNthBit(int bit, int number)
        {
            if (BitCount(bit) <= number)
                number = BitCount(bit) - 1;

            if (number < 0)
                return 0;
            
            for (int i = 0; i < number; i++)
            {
                bit = bit & (bit - 1);
            }

            return bit & -bit;
        }

        /// <summary>
        /// 1が何番目に立っているかを返す関数
        /// 立っている1は1つだけである必要がある
        /// (例)bit=01000のとき3を返す(0番目から数える)
        /// </summary>
        /// <param name="bit"></param>
        /// <returns></returns>
        public static int CheckWhichBitIsOn(int bit)
        {
            if (bit == 0)
                return -1;

            int result = 0;
            result += (bit & 0xffff0000) != 0 ? 16 : 0;
            result += (bit & 0xff00ff00) != 0 ? 8 : 0;
            result += (bit & 0xf0f0f0f0) != 0 ? 4 : 0;
            result += (bit & 0xcccccccc) != 0 ? 2 : 0;
            result += (bit & 0xaaaaaaaa) != 0 ? 1 : 0;
            return result;
        }
    }
}
