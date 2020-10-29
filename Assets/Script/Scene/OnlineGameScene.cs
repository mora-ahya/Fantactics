using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyInitSet;

namespace FantacticsScripts
{
    public class OnlineGameScene : GameScene
    {
        public Card[] testCards;

        System.Action process;
        Client client;

        [SerializeField] Board board = default;
        [SerializeField] PhaseNotice phaseNotice = default;
        [SerializeField] Phase[] phases = default;
        [SerializeField] Player[] players = default;
        int currentPlayerID;
        int[] actionOrder;//playersのインデックスを行動順に並べる
        int turn;
        int maxPlayer = 1;
        PhaseEnum currentPhase;
        CharacterAnimation currentAnimation;

        void Start()
        {
            board.Initialize();
            board.ChangeRedBitFlag(board.GetSquare(3).GetAdjacentSquares(BoardDirection.Right).Number, true);
            board.ApplayRedBitFlag();
            actionOrder = new int[6] { 0, 1, 2, 3, 4, 5 };
            players[0].Initialize(new PlayerInformation(new TestCharacter()));
            DrawCards();
            currentPhase = PhaseEnum.PlottingPhase;
            phaseNotice.DisplayPhaseNotice(currentPhase);
            players[0].StartTurn(phases[(int)currentPhase]);
            process = MainPlayerTurnProcess;
        }

        void Update()
        {
            Process();

        }

        void Process()
        {
            process();

            CameraManager.Instance.Act();
            CardOperation.Instance.Act();
            if (phaseNotice.IsActing)
                phaseNotice.Act();
        }

        public override void NotifyPhaseEnd(byte[] result)
        {
            client.StartSend(result);
        }

        void SendPlayerActionEvent(object sender)
        {
            switch (currentPhase)
            {
                case PhaseEnum.PlottingPhase:
                    DecideActionOrder();
                    AllocateTurnToPlayer();
                    break;

                case PhaseEnum.MovePhase:
                    process = WaitCharacterAnimation;
                    break;

                case PhaseEnum.RangePhase:
                    process = WaitCharacterAnimation;
                    break;

                case PhaseEnum.MeleePhase:
                    process = WaitCharacterAnimation;
                    break;
            }
        }

        void ReceiveDataFromServerEvent(object sender, byte[] data)
        {

        }

        void TransitionToTheNextTurn()
        {
            if (turn == maxPlayer - 1)
            {
                CurrentSegment++;
                turn = 0;
                if (CurrentSegment == 2)
                {
                    CurrentSegment = 0;
                    ThrowAwayHands();
                    DrawCards();
                    currentPhase = PhaseEnum.PlottingPhase;
                    players[0].StartTurn(phases[(int)currentPhase]);
                    phaseNotice.DisplayPhaseNotice(currentPhase);
                    process = MainPlayerTurnProcess;
                    return;
                }
                DecideActionOrder();
            }

            AllocateTurnToPlayer();
        }

        void MainPlayerTurnProcess()
        {
            players[0].Act();
        }

        void WaitCharacterAnimation()
        {
            currentAnimation.Act();
            if (!currentAnimation.GetAnimationFinish())
                return;

            TransitionToTheNextTurn();
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
            CardInfomation c1 = players[playerID1].Information.GetPlot(CurrentSegment);
            CardInfomation c2 = players[playerID2].Information.GetPlot(CurrentSegment);
            Character chara1 = players[playerID1].Information.Chara;
            Character chara2 = players[playerID2].Information.Chara;

            return c1.Type > c2.Type ||
                (c1.Type == c2.Type && chara1.Initiative < chara2.Initiative);
        }

        /// <summary>
        /// actionOrderに従って、プレイヤーにターンを割り振る
        /// </summary>
        void AllocateTurnToPlayer()
        {
            currentPlayerID = actionOrder[turn];
            PhaseEnum p = ChangeCardTypeToPhase(players[currentPlayerID].Information.GetPlot(CurrentSegment).Type);
            if (currentPhase != p)
            {
                currentPhase = p;
                phaseNotice.DisplayPhaseNotice(currentPhase);
            }

            if (currentPlayerID == 0)
            {
                players[0].StartTurn(phases[(int)currentPhase]);
                process = MainPlayerTurnProcess;
            }
            else
            {
                //autoPlayers[currentPlayerID]の行動
            }
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


        /// <summary>
        /// 山札から規定数のカードを手札に加える(ランダム)
        /// </summary>
        void DrawCards()
        {
            int i;
            int count = 0;
            while (count != players[0].Information.Chara.Imagination)
            {
                if (players[0].Information.Deck == 0)
                    ReturnDeckFromDiscards();

                i = BitCalculation.GetNthBit(players[0].Information.Deck, Random.Range(0, BitCalculation.BitCount(players[0].Information.Deck)));
                players[0].Information.Deck &= ~i;
                players[0].Information.Hands |= i;
                CardOperation.Instance.SetCardInfo(count, players[0].Information.Chara.GetCard(BitCalculation.CheckWhichBitIsOn(i)));
                count++;
            }
            CardOperation.Instance.NumberOfHands = count;
            Debug.Log("山札：" + System.Convert.ToString(players[0].Information.Deck, 2));
            Debug.Log("手札：" + System.Convert.ToString(players[0].Information.Hands, 2));
        }

        /// <summary>
        /// 捨て札をすべて山札に戻す
        /// </summary>
        void ReturnDeckFromDiscards()
        {
            players[0].Information.Deck = players[0].Information.Discards;
            players[0].Information.Deck = players[0].Information.Discards = 0;
        }

        /// <summary>
        /// 手札を捨てる
        /// </summary>
        void ThrowAwayHands()
        {
            players[0].Information.Deck = players[0].Information.Discards |= players[0].Information.Deck = players[0].Information.Hands;
            players[0].Information.Hands = 0;
        }
        /*
        /// <summary>
        /// 山札からランダムなカードを除外する
        /// </summary>
        /// <param name="num"></param>
        void ExcludeFromDeck(int num)
        {
            if (mainPlayer.Information.Deck == 0)
                ReturnDeckFromDiscards();

            int i = BitCalculation.GetNthBit(mainPlayer.Information.Deck, Random.Range(0, BitCalculation.BitCount(mainPlayer.Information.Deck) - 1));
            int count = 0;
            while (count++ != num)
            {
                mainPlayer.Information.Deck &= ~i;
                hands |= i;
                if (mainPlayer.Information.Deck == 0)
                    ReturnDeckFromDiscards();

                i = BitCalculation.GetNthBit(mainPlayer.Information.Deck, Random.Range(0, BitCalculation.BitCount(mainPlayer.Information.Deck) - 1));
            }
        }

        /// <summary>
        /// 手札から指定したカードを除外する
        /// </summary>
        /// <param name="num"></param>
        void ExcludeFromHands(int num)
        {
            int tmp = BitCalculation.GetNthBit(hands, num);
            hands &= ~tmp;
            exclusionCards |= tmp;
        }

        /// <summary>
        /// 装備から指定したものを除外する
        /// </summary>
        /// <param name="num"></param>
        void ExcludeFromEquipments(int num)
        {
            equipments &= ~(1 << num);
        }

        /// <summary>
        /// 除外カードを指定数山札に戻す(ランダム)
        /// </summary>
        /// <param name="amount"></param>
        void ReturnDeckFromExclusionCards(int amount)
        {
            if (mainPlayer.Information.ExclusionCards == 0)
                return;
            Debug.Log("山札：" + System.Convert.ToString(mainPlayer.Information.Deck, 2));
            int i = BitCalculation.GetNthBit(mainPlayer.Information.ExclusionCards, Random.Range(0, BitCalculation.BitCount(emainPlayer.Information.ExclusionCards) - 1));
            while (amount-- > 0 && mainPlayer.Information.ExclusionCards != 0)
            {
                mainPlayer.Information.ExclusionCards &= ~i;
                mainPlayer.Information.Deck |= i;

                i = BitCalculation.GetNthBit(mainPlayer.Information.ExclusionCards, Random.Range(0, BitCalculation.BitCount(mainPlayer.Information.ExclusionCards - 1)));
            }

            Debug.Log("山札：" + System.Convert.ToString(mainPlayer.Information.Deck, 2));
        }*/
    }
}
