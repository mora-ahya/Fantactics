using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantacticsScripts
{
    public class GameScene : MonoBehaviour
    {
        public Card[] testCards;

        System.Action process;

        public Player[] players;//オンライン形式ならintのプレイヤーidで良いかも
        Character[] characters;//playerと辞書形式にできるかも
        System.Tuple<int, int>[] playerSegments; //カードのidを受け取る。オンラインでやる場合は...
        int currentPlayerID;
        int[] actionOrder;//playersのインデックスを行動順に並べる
        CardInfomation[] cards;
        int turn;
        int maxPlayer = 1;
        int currentSegment;
        bool signal;
        Phase currentPhase;

        private void Start()
        {
            process = WaitPlayerSegments;
            playerSegments = new System.Tuple<int, int>[6];
            actionOrder = new int[6] {0, 1, 2, 3, 4, 5 };
            cards = new CardInfomation[6];
            characters = new Character[6];
            characters[0] = new TestCharacter();

            for (int i = 0; i < testCards.Length; i++)
            {
                testCards[i].SetCardInfo(characters[0].GetCard(i));
                Debug.Log(testCards[i].CardInfo);
            }
        }

        private void Update()
        {
            process();
        }

        void Process()
        {
            process();
        }

        void ActionPlayer()
        {
            //players[currentPlayerID].Act();

            if (!players[currentPlayerID].Signal)
                return;

            if (turn == maxPlayer - 1)
            {
                turn = 0;
                if (currentSegment == 0)
                {
                    currentSegment++;
                    ConvertSegmentIDIntoCard(currentSegment);
                    DecideActionOrder();
                    AllocateTurnToPlayer();
                    return;
                }
                currentSegment = 0;
                process = WaitPlayerSegments;
                return;
            }
            turn++;
            AllocateTurnToPlayer();

        }

        void WaitPlayerSegments()
        {
            if (!WaitPlayers())
                return;

            GetPlayerSegments();
            ConvertSegmentIDIntoCard(currentSegment);
            DecideActionOrder();
            AllocateTurnToPlayer();
            process = ActionPlayer;
        }

        bool WaitPlayers()
        {
            foreach (Player player in players)
            {
                if (!player.Signal)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 全プレイヤーが選んだカードを受け取る
        /// タプルでカードIDを受け取る
        /// </summary>
        void GetPlayerSegments()
        {
            for (int i = 0; i < maxPlayer; i++)
            {
                playerSegments[i] = players[i].GetSegmentsID();
            }
        }

        /// <summary>
        /// 受け取ったセグメントをカード情報に変換する
        /// </summary>
        /// <param name="segment"></param>
        void ConvertSegmentIDIntoCard(int segment)
        {
            if (segment == 0)
            {
                for (int i = 0; i < maxPlayer; i++)
                {
                    cards[i] = characters[i].GetCard(playerSegments[i].Item1);
                }
                return;
            }

            for (int i = 0; i < maxPlayer; i++)
            {
                cards[i] = characters[i].GetCard(playerSegments[i].Item2);
            }
        }

        /// <summary>
        /// セグメントから行動順を決定する
        /// </summary>
        void DecideActionOrder()
        {
            int tmp;
            int j;
            for (int i = 1; i < maxPlayer; i++)//挿入ソート
            {
                tmp = actionOrder[i];
                j = i;
                for (; j > 0 && JudgeOrder(actionOrder[j - 1], tmp); j--)
                {
                    actionOrder[j] = actionOrder[j - 1];
                }
                actionOrder[j] = tmp;
            }
        }

        /// <summary>
        /// playerID2の方がplayerID1より行動順が早いときtrueを返す
        /// 双方の選んだカードのタイプを比較する。
        /// どちらも同じときcharaのイニシアティブで判断する
        /// </summary>
        /// <param name="playerID1"></param>
        /// <param name="playerID2"></param>
        /// <returns></returns>
        bool JudgeOrder(int playerID1, int playerID2)
        {
            return cards[playerID1].Type > cards[playerID2].Type || 
                (cards[playerID1].Type == cards[playerID2].Type && characters[playerID1].Initiative < characters[playerID2].Initiative);
        }

        /// <summary>
        /// プレイヤーにactionOrder順にターンを割り振る
        /// </summary>
        void AllocateTurnToPlayer()
        {
            currentPlayerID = actionOrder[turn];
            players[currentPlayerID].SetPhase(cards[currentPlayerID].Type);
            signal = true;
        }
    }
}
