using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;
using MyInitSet;

namespace FantacticsScripts
{
    public class Player : MonoBehaviour
    {
        public PlayerInformation Information { get; private set; }
        public bool IsMoving { get { return mover.IsMoving; } }

        public bool IsAttacking { get; protected set; } = false;
        public bool IsActing { get; protected set; } = false;
        public bool IsClient { get; protected set; } = true;
        [SerializeField] Board board = default;
        [SerializeField] OfflineGameScene gameScene = default;
        [SerializeField] PlayerMover mover = default;

        public void Initialize(PlayerInformation pInfo)
        {
            Information = pInfo;
            Information.Deck = (1 << Information.Chara.Hp) - 1;
            Information.SetCurrentSquare(5);
            board.GetSquare(Information.CurrentSquare).AddPlayer(Information.PlayerID);
            transform.position = Board.SquareNumberToWorldPosition(Information.CurrentSquare);
            mover.Initialize(this, board);
        }

        public void Act()
        {
            //phase?.Invoke();
            
        }

        public void SetIsActing(bool isAc)
        {
            IsActing = isAc;
        }

        public void SetMoveAnimation(MoveResult movePhaseResult)
        {
            this.SetMoveAnimation(movePhaseResult.roadSquares, movePhaseResult.PlayerForward);
        }

        public void SetMoveAnimation(List<int> roadSquaresSource, BoardDirection lastDirection)
        {
            mover.SetAnimationForDirection(roadSquaresSource, lastDirection);
        }

        public void StorePlottingResult(PlottingResult result)
        {
            Information.SetPlot(0, result.Actions[0].CardInfo.ID);
            Information.SetPlot(1, result.Actions[1].CardInfo.ID);

            IsActing = false;
        }

        public void StoreMoveResult(MoveResult result)
        {
            if (Information.moveResult != result)
            {
                Information.moveResult.Copy(result);
            }
            IsActing = false;
        }

        public void ApplyMoveResult()
        {
            //fieldAnimation.SetMoveAnimation(this, result);
            //プレイヤーによってマス位置を変える(向き次第)
            List<int> listTmp = Information.moveResult.roadSquares;
            SetMoveAnimation(Information.moveResult);
            board.GetSquare(Information.CurrentSquare).RemovePlayer();
            Information.SetCurrentSquare(listTmp[listTmp.Count - 1]);
            board.GetSquare(listTmp[listTmp.Count - 1]).AddPlayer(Information.PlayerID);
            Information.Direction = Information.moveResult.PlayerForward;
            //process = AnimationProcess;
        }

        public void StoreAttackResult(AttackResult result)
        {
            if (Information.attackResult != result)
            {
                Information.attackResult.Copy(result);
            }
            IsActing = false;
            //process = AnimationProcess;
        }

        public void ApplyAttackResult()
        {
            IsAttacking = true;
            IsAttacking = false;
        }

        public CardInfomation GetPlot()
        {
            return Information.GetPlot(gameScene.CurrentSegment);
        }

        public void SetDamage(int amount)
        {

        }

        public Vector3 GetCharacterHeadPosition()
        {
            return Information.Chara.HeadPosition + transform.position;
        }

        public bool CanMoveBeforeMeleeAttack()
        {
            return true;
        }

        public void StartAction(PhaseEnum phaseEnum, int step)
        {
            switch (phaseEnum)
            {
                case PhaseEnum.PlottingPhase:
                    Information.plottingResult.Clear();
                    Information.plottingResult.Initialize(phaseEnum);
                    PhaseOperationManager.Instance.ActivePlottingOperation(this, Information.plottingResult);
                    break;

                case PhaseEnum.MovePhase:
                    Information.moveResult.Clear();
                    Information.moveResult.Initialize(phaseEnum);
                    PhaseOperationManager.Instance.ActiveMoveOnBoardOperation(this, GetPlot().Power, Information.CurrentSquare, Information.moveResult);
                    break;

                case PhaseEnum.RangePhase:
                    Information.attackResult.Clear();
                    Information.attackResult.Initialize(phaseEnum);
                    PhaseOperationManager.Instance.ActiveAttackOnBoardOperation(this, Information.attackResult);
                    break;

                case PhaseEnum.MeleePhase:
                    if (step == 0)
                    {
                        Information.moveResult.Clear();
                        Information.moveResult.Initialize(phaseEnum);
                        PhaseOperationManager.Instance.ActiveMoveOnBoardOperation(this, 0, Information.CurrentSquare, Information.moveResult);
                    }
                    else if (step == 1)
                    {
                        Information.attackResult.Clear();
                        Information.attackResult.Initialize(phaseEnum);
                        PhaseOperationManager.Instance.ActiveAttackOnBoardOperation(this, Information.attackResult);
                    }
                    break;
            }
            IsActing = true;
        }

        /// <summary>
        /// 山札から規定数のカードを手札に加える(seedを元に擬似ランダム)
        /// </summary>
        public void DrawCards(int seed)
        {
            int i;
            int count = 0;
            while (count != Information.Chara.Imagination)
            {
                if (Information.Deck == 0)
                    ReturnDeckFromDiscards();

                i = BitCalculation.GetNthBit(Information.Deck, Random.Range(0, BitCalculation.BitCount(Information.Deck)));
                Information.Deck &= ~i;
               Information.Hands |= i;
                CardOperation.Instance.SetCardInfo(count, Information.Chara.GetCard(BitCalculation.CheckWhichBitIsOn(i)));
                count++;
            }
            CardOperation.Instance.NumberOfHands = count;
            Debug.Log("山札：" + System.Convert.ToString(Information.Deck, 2));
            Debug.Log("手札：" + System.Convert.ToString(Information.Hands, 2));
        }

        /// <summary>
        /// 捨て札をすべて山札に戻す
        /// </summary>
        public void ReturnDeckFromDiscards()
        {
            Information.Deck = Information.Discards;
            Information.Deck = Information.Discards = 0;
        }

        /// <summary>
        /// 手札を捨てる
        /// </summary>
        public void ThrowAwayHands()
        {
            Information.Deck = Information.Discards |= Information.Deck = Information.Hands;
            Information.Hands = 0;
        }
    }
}
