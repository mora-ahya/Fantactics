using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;

namespace FantacticsScripts
{
    public class ServerMain : MonoBehaviour
    {
        readonly object syncLock = new object();

        Server server;
        Server.RecieveEventHandler receivePlayerPlottingHandler;
        Server.RecieveEventHandler receivePlayerActionHandler;
        Board board;
        Socket[] clients;

        PlayerInformation[] players;
        Character[] characters;//playerと辞書形式にできるかも
        Tuple<int, int>[] playerActions; //カードのidを受け取る。オンラインでやる場合は...
        int currentPlayerID;
        int[] actionOrder;//playersのインデックスを行動順に並べる
        CardInfomation[] cards;
        int turn;
        int maxPlayer = 1;
        int currentSegment;
        int numberOfSignals = 0;
        Phase currentPhase;

        void Start()
        {
            receivePlayerPlottingHandler = new Server.RecieveEventHandler(ReceivePlayerPlottingEvent);
            receivePlayerActionHandler = new Server.RecieveEventHandler(ReceivePlayerActionEvent);
            board.Initialize();
            actionOrder = new int[6] { 0, 1, 2, 3, 4, 5 };
            cards = new CardInfomation[6];
            characters = new Character[6];
            server = new Server();
            server.Initialize();
            
        }

        void Update()
        {
            
        }

        void StartPlottingPhase()
        {
            currentSegment = 0;
            currentPhase = Phase.PlottingPhase;
            server.OnRecieveData += receivePlayerPlottingHandler;
        }

        /// <summary>
        /// プレイヤーがプロットしたカードのIDを受け取るときのイベント
        /// serverのOnReceiveDataに加える
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        void ReceivePlayerPlottingEvent(object sender, byte[] data)
        {
            playerActions[data[0]] = Tuple.Create((int)data[1], (int)data[2]);
            lock (syncLock)
            {
                numberOfSignals++;
                if (numberOfSignals == maxPlayer)
                {
                    //次の処理に進む
                    numberOfSignals = 0;
                    server.OnRecieveData -= receivePlayerPlottingHandler;
                    StartActionPhase();
                }
            }
        }

        /// <summary>
        /// アクションフェーズを開始する
        /// </summary>
        void StartActionPhase()
        {
            server.OnRecieveData += receivePlayerActionHandler;
            currentSegment = 0;
            ConvertCardIDIntoCardInfomation(currentSegment);
            DecideActionOrder();
            AllocateTurnToPlayer();
        }

        /// <summary>
        /// プレイヤーがアクションフェーズで行った結果を受け取るときのイベント
        /// serverのOnReceiveDataに加える
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        void ReceivePlayerActionEvent(object sender, byte[] data)
        {
            switch (currentPhase)
            {
                case Phase.MovePhase:
                    break;

                case Phase.RangePhase:
                    break;

                case Phase.MeleePhase:
                    break;

                default:
                    break;
            }
            //受け取ったデータを他のプレイヤーにも送る

            if (turn == maxPlayer - 1)
            {
                EndSegment();
                return;
            }
            turn++;
            AllocateTurnToPlayer();
        }

        void EndSegment()
        {
            turn = 0;
            if (currentSegment == 0)
            {
                currentSegment++;
                ConvertCardIDIntoCardInfomation(currentSegment);
                DecideActionOrder();
                AllocateTurnToPlayer();
                return;
            }
            server.OnRecieveData -= receivePlayerPlottingHandler;
            StartActionPhase();
        }

        /// <summary>
        /// 受け取ったカードIDをカード情報に変換する
        /// </summary>
        /// <param name="segment"></param>
        void ConvertCardIDIntoCardInfomation(int segment)
        {
            if (segment == 0)
            {
                for (int i = 0; i < maxPlayer; i++)
                {
                    cards[i] = characters[i].GetCard(playerActions[i].Item1);
                }
                return;
            }

            for (int i = 0; i < maxPlayer; i++)
            {
                cards[i] = characters[i].GetCard(playerActions[i].Item2);
            }
        }

        /// <summary>
        /// カード情報から行動順を決定する
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
        /// actionOrderに従って、プレイヤーにターンを割り振る
        /// </summary>
        void AllocateTurnToPlayer()
        {
            byte[] data = new byte[2];
            currentPlayerID = actionOrder[turn];
            data[0] = (byte)currentPlayerID;
            if (currentPhase != ChangeCardTypeToPhase(cards[currentPlayerID].Type))
            {
                currentPhase = ChangeCardTypeToPhase(cards[currentPlayerID].Type);
            }
            data[1] = (byte)currentPhase;
            for (int i = 0; i < maxPlayer; i++)
            {
                server.StartSend(clients[i], data);
            }
            server.StartReceive(clients[currentPlayerID], 1024);//データサイズ変更
        }

        /// <summary>
        /// CardTypeをPhaseに変える
        /// </summary>
        /// <param name="cardType"></param>
        /// <returns></returns>
        Phase ChangeCardTypeToPhase(CardType cardType)
        {
            switch (cardType)
            {
                case CardType.Move:
                    return Phase.MovePhase;

                case CardType.Melee:
                    return Phase.MeleePhase;

                case CardType.Range:
                    return Phase.RangePhase;

                default:
                    return 0;
            }
        }
    }
}
