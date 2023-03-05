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
        [SerializeField] PhaseManager phaseManager = default;
        [SerializeField] Player[] players = default;
        [SerializeField] FieldAnimation fieldAnimation = default;

        int currentPlayerID;
        int[] actionOrder;//playersのインデックスを行動順に並べる
        int turn;
        int maxPlayer = 1;

        void Start()
        {
            board.Initialize();
            //board.ChangeRedBitFlag(board.GetSquare(3).GetAdjacentSquare(BoardDirection.Right).Number, true);
            //board.ApplayRedBitFlag();
            actionOrder = new int[6] { 0, 1, 2, 3, 4, 5 };

            players[0].Initialize(new PlayerInformation(new TestCharacter()));
            players[0].DrawCards(10);

            phaseManager.Initialize(this, players[0]);

            GameSceneManager.Instance.SetGameScene(this);

            // フェーズの設定
            StartPhase(PhaseEnum.PlottingPhase);

            //players[0].StartTurn(phases[(int)currentPhase]);
            //RangePhase.ColorSquareInRangeTest(Board.Width * Board.Height / 2 + 5, 3, 5, BoardDirection.Left);
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
        }

        void PhaseProcess()
        {
            phaseManager.ActPhase();
        }

        void AnimationProcess()
        {
            fieldAnimation.Act();
            if (fieldAnimation.IsEnd())
            {
                process = null;
                TransitionToNextTurn();
            }
        }

        void StartPhase(PhaseEnum phaseEnum)
        {
            phaseManager.StartPhase(players[0], phaseEnum);
            UIManager.Instance.DisplayPhaseNotice(phaseEnum);
            process = PhaseProcess;
        }

        public void SetCharacter(int charaNum)
        {

        }

        /// <summary>
        /// プロットが決定したときに飛び出す関数
        /// </summary>
        public void OnDecidedPlayerPlotting()
        {
            //AutoPlayerのPlotを決める (もしくは別スレッドの完了を待つ)
            CurrentSegment = 0;
            turn = 0;
            DecideActionOrder(CurrentSegment);
            AllocateTurn();
        }

        /// <summary>
        /// PlottingPhaseResultを受け取ったときの処理
        /// </summary>
        public override void OnReceivedPlottingPhaseResult(PlottingPhaseResult result)
        {
            Player player = players[result.PlayerID]; // 適切なプレイヤーをセットする。今は仮

            player.SetPlot(0, result.Actions[0].CardInfo.ID);
            player.SetPlot(1, result.Actions[1].CardInfo.ID);

            // 本来はAutoPlayerのPlotを決める
            OnDecidedPlayerPlotting();
        }

        /// <summary>
        /// MovePhaseResultを受け取ったときの処理
        /// </summary>
        public override void OnReceivedMovePhaseResult(MovePhaseResult result)
        {
            Player player = players[result.PlayerID];

            player.SetMoveAnimation(result);
            board.AddPlayerIntoSquare(player, result.DestSquareNum);
            player.Information.SetCurrentSquare(result.DestSquareNum);
            player.Information.Direction = result.PlayerForward;
            StartCoroutine(WaitPlayerMoving(player, 1.0f));
        }

        /// <summary>
        /// RangePhaseResultを受け取ったときの処理
        /// </summary>
        public override void OnReceivedRangePhaseResult(RangePhaseResult result)
        {
            Player player = players[result.PlayerID];

            // Animationの設定
            // ダメージ計算
            StartCoroutine(WaitPlayerMoving(player, 1.0f));
        }

        /// <summary>
        /// MeleePhaseResultを受け取ったときの処理
        /// </summary>
        public override void OnReceivedMeleePhaseResult(MeleePhaseResult result)
        {
            Player player = players[result.PlayerID];

            // Animationの設定
            // ダメージ計算
            StartCoroutine(WaitPlayerMoving(player, 1.0f));
        }

        // いずれアニメーション一つにまとめる
        IEnumerator WaitPlayerMoving(Player player, float adjustTime = 0.0f)
        {
            while(player.IsMoving)
            {
                yield return null;
            }

            float delta = 0.0f;

            while (delta < adjustTime)
            {
                delta += Time.deltaTime;
                yield return null;
            }

            TransitionToNextTurn();
        }

        public void OnCompletedPlayerTurn()
        {

        }

        /// <summary>
        /// プレイヤーの行動が終了して、次のプレイヤーにターンを渡す関数
        /// </summary>
        void TransitionToNextTurn()
        {
            if (turn != maxPlayer - 1)
            {
                AllocateTurn();
                return;
            }

            CurrentSegment++;
            turn = 0;
            if (CurrentSegment == 2)
            {
                CurrentSegment = 0;
                StartPlottingPhase();
                return;
            }
            DecideActionOrder(CurrentSegment);
            AllocateTurn();
        }

        /// <summary>
        /// カード情報から行動順を決定する
        /// </summary>
        void DecideActionOrder(int segmentNum)
        {
            int tmp;
            int j;
            for (int i = 1; i < maxPlayer; i++)//挿入ソート
            {
                tmp = actionOrder[i];
                j = i;
                for (; j > 0 && JudgeOrder(actionOrder[j - 1], tmp, segmentNum); j--)
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
        /// <param name="segmentNum"></param>
        /// <returns></returns>
        bool JudgeOrder(int playerID1, int playerID2, int segmentNum)
        {
            CardInfomation c1 = players[playerID1].Information.GetPlot(segmentNum);
            CardInfomation c2 = players[playerID2].Information.GetPlot(segmentNum);
            Character chara1 = players[playerID1].Information.Chara;
            Character chara2 = players[playerID2].Information.Chara;

            return c1.Type > c2.Type ||
                (c1.Type == c2.Type && chara1.Initiative < chara2.Initiative);
        }

        /// <summary>
        /// actionOrderに従って、プレイヤーにターンを割り振る
        /// </summary>
        void AllocateTurn()
        {
            currentPlayerID = actionOrder[turn];
            PhaseEnum p = ChangeCardTypeToPhase(players[currentPlayerID].Information.GetPlot(CurrentSegment).Type);
            PhaseEnum currentPhaseEnum = phaseManager.GetCurrentPhaseEnum();
            if (currentPhaseEnum != p)
            {
                currentPhaseEnum = p;
                UIManager.Instance.DisplayPhaseNotice(currentPhaseEnum);
            }

            if (currentPlayerID == 0)
            {
                phaseManager.StartPhase(players[0], currentPhaseEnum);
            }
            else
            {
                //autoPlayers[currentPlayerID]の行動
            }
        }

        void StartPlottingPhase()
        {
            players[0].ThrowAwayHands();
            players[0].DrawCards(10);
            phaseManager.StartPhase(players[0], PhaseEnum.PlottingPhase);
            UIManager.Instance.DisplayPhaseNotice(PhaseEnum.PlottingPhase);
            // 手札を捨てる、ドローのアニメーション起動
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
