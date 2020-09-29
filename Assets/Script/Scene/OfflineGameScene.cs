using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantacticsScripts
{
    public class OfflineGameScene : MonoBehaviour
    {
        public Card[] testCards;

        System.Action process;

        [SerializeField] Board board = default;
        [SerializeField] CameraManager cameraManager = default;
        [SerializeField] PhaseNotice phaseNotice = default;
        [SerializeField] PhaseBase[] phases = default;
        Player mainPlayer;
        CPU[] cpus;
        PlayerInformation[] playerInformation;
        int currentPlayerID;
        int[] actionOrder;//playersのインデックスを行動順に並べる
        int turn;
        int maxPlayer = 1;
        int currentSegment;
        bool signal;
        Phase currentPhase;

        void Start()
        {
            board.Initialize();
            board.ChangeRedBitFlag(board.GetSquare(3).GetAdjacentSquares(BoardDirection.Right).Number, true);
            board.ApplayRedBitFlag();
            actionOrder = new int[6] { 0, 1, 2, 3, 4, 5 };
            playerInformation = new PlayerInformation[6];
            currentPhase = Phase.PlottingPhase;
            phaseNotice.DisplayPhaseNotice(currentPhase);
            process = WaitPhaseNoticeProcess;
        }

        void Update()
        {
            process();
            CameraManager.Instance.Act();
        }

        void Process()
        {
            process();
        }

        public void PlayerFinishesTurn()
        {
            switch (currentPhase)
            {
                case Phase.PlottingPhase:
                    GetPlayerPlots();
                    DecideActionOrder();
                    AllocateTurnToPlayer();
                    break;
            }
        }

        void WaitPhaseNoticeProcess()
        {
            if (!phaseNotice.IsActing)
            {
                if (currentPhase == Phase.PlottingPhase)
                {
                    //process = WaitPlayerSegmentsProcess;
                }
                else
                {
                    //process = ActionPlayerProcess;
                }
                return;
            }

            phaseNotice.Act();
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

        void GetPlayerPlots()
        {

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
            CardInfomation c1 = playerInformation[playerID1].GetPlot(currentSegment);
            CardInfomation c2 = playerInformation[playerID2].GetPlot(currentSegment);
            Character chara1 = playerInformation[playerID1].Chara;
            Character chara2 = playerInformation[playerID2].Chara;

            return c1.Type > c2.Type ||
                (c1.Type == c2.Type && chara1.Initiative < chara2.Initiative);
        }

        /// <summary>
        /// actionOrderに従って、プレイヤーにターンを割り振る
        /// </summary>
        void AllocateTurnToPlayer()
        {
            currentPlayerID = actionOrder[turn];
            Phase p = ChangeCardTypeToPhase(playerInformation[currentPlayerID].GetPlot(currentSegment).Type);
            if (currentPhase != p)
            {
                currentPhase = p;
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
