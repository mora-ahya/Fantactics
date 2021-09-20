using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyInitSet;

namespace FantacticsScripts
{
    public class OfflineGameScene : GameScene
    {
        public Card[] testCards;

        System.Action process;

        [SerializeField] Board board = default;
        [SerializeField] PhaseNotice phaseNotice = default;
        [SerializeField] PhaseManager phaseManager = default;
        [SerializeField] Player[] players = default;
        [SerializeField] FieldAnimation fieldAnimation = default;
        int currentPlayerID;
        int[] actionOrder;//playersのインデックスを行動順に並べる
        int turn;
        int maxPlayer = 1;
        PhaseEnum currentPhaseEnum;
        Phase currentPhase;

        void Start()
        {
            board.Initialize();
            board.ChangeRedBitFlag(board.GetSquare(3).GetAdjacentSquares(BoardDirection.Right).Number, true);
            board.ApplayRedBitFlag();
            actionOrder = new int[6] { 0, 1, 2, 3, 4, 5 };
            players[0].Initialize(new PlayerInformation(new TestCharacter()));
            DrawCards();
            currentPhaseEnum = PhaseEnum.PlottingPhase;
            currentPhase = phaseManager.GetPhase(currentPhaseEnum);
            phaseManager.Initialize(this, players[0]);
            currentPhase.Initialize();
            phaseNotice.DisplayPhaseNotice(currentPhaseEnum);
            //players[0].StartTurn(phases[(int)currentPhase]);
        }

        void Update()
        {
            Process();
            
        }

        void Process()
        {
            CameraManager.Instance.Act();
            CardOperation.Instance.Act();
            process?.Invoke();
            if (phaseNotice.IsActing)
                phaseNotice.Act();
        }

        void PhaseProcess()
        {
            currentPhase?.Act();
        }

        void AnimationProcess()
        {
            fieldAnimation.Act();
            if (fieldAnimation.IsEnd())
            {
                process = null;
                TransitionToTheNextTurn();
            }
        }

        public void SetCharacter(int charaNum)
        {

        }

        public override void NotifyPhaseEnd(PhaseResult result)
        {
            Player playerTmp = players[currentPlayerID];
            currentPhase = null;
            //switch (currentPhaseEnum)
            //{
            //    case PhaseEnum.PlottingPhase:
            //        for (int i = 0; i < maxPlayer - 1; i++)
            //        {
            //            //autoPlayerがプロットを決める
            //        }
            //        DecideActionOrder();
            //        AllocateTurnToPlayer();
            //        break;

            //    case PhaseEnum.MovePhase:
            //        fieldAnimation.SetMoveAnimation(playerTmp, result[1], 2, result);
            //        //プレイヤーによってマス位置を変える(向き次第)
            //        board.GetSquare(playerTmp.Information.CurrentSquare).PlayerExit();
            //        playerTmp.Information.SetCurrentSquare(result[4]);
            //        board.GetSquare(result[4]).PlayerEnter(currentPlayerID);
            //        process = AnimationProcess;
            //        break;

            //    case PhaseEnum.RangePhase:
            //        //battleAnimation;
            //        CalculateRangeDamage(result[1]);
            //        process = AnimationProcess;
            //        break;

            //    case PhaseEnum.MeleePhase:
            //        process = AnimationProcess;
            //        break;
            //}

            //void CalculateRangeDamage(int targetSquare)
            //{
            //    CardInfomation cardTmp = playerTmp.GetPlot();
            //    int dis = 0;
            //    foreach(Player p in players)
            //    {
            //        dis = board.GetManhattanDistance(targetSquare, p.Information.CurrentSquare);
            //        if (dis > cardTmp.Blast)
            //        {
            //            continue;
            //        }
            //        //そのプレイヤーのダメージフラグを立てて、ダメージを保存
            //    }
            //}
        }

        void TransitionToTheNextTurn()
        {
            if (turn != maxPlayer - 1)
            {
                AllocateTurnToPlayer();
                return;
            }

            CurrentSegment++;
            turn = 0;
            if (CurrentSegment == 2)
            {
                CurrentSegment = 0;
                ThrowAwayHands();
                DrawCards();
                currentPhaseEnum = PhaseEnum.PlottingPhase;
                currentPhase = phaseManager.GetPhase(currentPhaseEnum);
                currentPhase.Initialize();
                //players[0].StartTurn(phases[(int)currentPhase]);
                phaseNotice.DisplayPhaseNotice(currentPhaseEnum);
                return;
            }
            DecideActionOrder();
            AllocateTurnToPlayer();
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
            if (currentPhaseEnum != p)
            {
                currentPhaseEnum = p;
                phaseNotice.DisplayPhaseNotice(currentPhaseEnum);
            }

            if (currentPlayerID == 0)
            {
                //players[0].StartTurn(phases[(int)currentPhase]);
                currentPhase = phaseManager.GetPhase(currentPhaseEnum);
                currentPhase.Initialize();
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
