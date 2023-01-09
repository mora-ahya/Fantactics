using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantacticsScripts
{
    public class OldGameScene : MonoBehaviour
    {
        public Card[] testCards;

        System.Action process;

        [SerializeField] Board board = default;
        [SerializeField] CameraManager cameraManager = default;
        [SerializeField] PhaseNotice phaseNotice = default;
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
        PhaseEnum currentPhase;

        void Start()
        {
            board.Initialize();
            board.ChangeRedBitFlag(board.GetSquare(3).GetAdjacentSquare(BoardDirection.Right).Number, true);
            board.ApplayRedBitFlag();
            playerSegments = new System.Tuple<int, int>[6];
            actionOrder = new int[6] { 0, 1, 2, 3, 4, 5 };
            cards = new CardInfomation[6];
            characters = new Character[6];
            characters[0] = new TestCharacter();
            //players[0].Initialize(characters[0]);
            currentPhase = PhaseEnum.PlottingPhase;
            phaseNotice.DisplayPhaseNotice(currentPhase);
            process = WaitPhaseNoticeProcess;
        }

        void Update()
        {
            players[0].Act();
            process();
            //cameraManager.FreeMode();
        }

        void Process()
        {
            process();
        }

        /// <summary>
        /// プレイヤーがアクションを起こすプロセス
        /// </summary>
        void ActionPlayerProcess()
        {
            players[currentPlayerID].Act();

            //if (!players[currentPlayerID].Signal)
            //return;

            if (turn == maxPlayer - 1)
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
                currentSegment = 0;
                process = WaitPhaseNoticeProcess;
                currentPhase = PhaseEnum.PlottingPhase;
                phaseNotice.DisplayPhaseNotice(currentPhase);
                //for (int i = 0; i < maxPlayer; i++)
                    //players[i].StartTurn(currentPhase);

                return;
            }
            turn++;
            AllocateTurnToPlayer();

        }

        /// <summary>
        /// プレイヤーが使用カードを選択するのを待つプロセス
        /// </summary>
        void WaitPlayerSegmentsProcess()
        {
            /*
            for (int i = 0; i < maxPlayer; i++)
                players[i].Act();*/

            if (!WaitPlayers())
                return;

            GetPlayerSegments();
            ConvertCardIDIntoCardInfomation(currentSegment);
            DecideActionOrder();
            AllocateTurnToPlayer();
        }

        void WaitPhaseNoticeProcess()
        {
            if (!phaseNotice.IsActing)
            {
                if (currentPhase == PhaseEnum.PlottingPhase)
                {
                    process = WaitPlayerSegmentsProcess;
                }
                else
                {
                    process = ActionPlayerProcess;
                }
                return;
            }

        }

        bool WaitPlayers()
        {
            for (int i = 0; i < maxPlayer; i++)
            {
                //if (!players[i].Signal)
                //return false;
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
                //playerSegments[i] = players[i].GetSegmentsID();
            }
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
            currentPlayerID = actionOrder[turn];
            if (currentPhase != ChangeCardTypeToPhase(cards[currentPlayerID].Type))
            {
                currentPhase = ChangeCardTypeToPhase(cards[currentPlayerID].Type);
                phaseNotice.DisplayPhaseNotice(currentPhase);
                process = WaitPhaseNoticeProcess;
            }
            //players[currentPlayerID].StartTurn(currentPhase, currentSegment);
            signal = true;
        }

        /// <summary>
        /// CardTypeをPhaseに変える
        /// </summary>
        /// <param name="cardType"></param>
        /// <returns></returns>
        PhaseEnum ChangeCardTypeToPhase(CardType cardType)
        {
            switch (cardType)
            {
                case CardType.Move:
                    return PhaseEnum.MovePhase;

                case CardType.Melee:
                    return PhaseEnum.MeleePhase;

                case CardType.Range:
                    return PhaseEnum.RangePhase;

                default:
                    return 0;
            }
        }
    }
}
