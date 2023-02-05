﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;
using MyInitSet;

namespace FantacticsScripts
{
    public class Player : MonoBehaviour
    {
        System.Action phase;

        public PlayerInformation Information { get; private set; }
        public bool IsMoving { get { return mover.IsMoving; } }
        [SerializeField] Board board = default;
        [SerializeField] OfflineGameScene gameScene = default;
        [SerializeField] PhaseManager phaseManager = default;
        [SerializeField] PlayerMover mover = default;

        public void Initialize(PlayerInformation pInfo)
        {
            phase = null;
            //DrawCards();
            Information = pInfo;
            Information.Deck = (1 << Information.Chara.Hp) - 1;
            Information.SetCurrentSquare(5);
            board.GetSquare(Information.CurrentSquare).AddPlayer(Information.PlayerID);
            transform.position = Board.SquareNumberToWorldPosition(Information.CurrentSquare);
            mover.Initialize(this, board);
            //PlottingPhaseInit();
        }

        public void Act()
        {
            //phase?.Invoke();
            
        }

        public void SetPlot(int plotNum, int cardId)
        {
            Information.SetPlot(plotNum, cardId);
        }

        public void SetMoveAnimation(MovePhaseResult movePhaseResult)
        {
            this.SetMoveAnimation(movePhaseResult.MoveDirections, movePhaseResult.NumOfMove, movePhaseResult.PlayerForward);
        }

        public void SetMoveAnimation(BoardDirection[] moveDirectionsSource, int numOfMove, BoardDirection lastDirection)
        {
            mover.SetAnimationForDirection(moveDirectionsSource, numOfMove, lastDirection);
        }

        public void StorePlottingPhaseResult()
        {
            PhaseResult resultTmp = phaseManager.GetPhaseResult(PhaseEnum.PlottingPhase);

            if (resultTmp == null)
            {
                return;
            }

            PlottingPhaseResult result = (PlottingPhaseResult)resultTmp;

            Information.SetPlot(0, result.Actions[0].CardInfo.ID);
            Information.SetPlot(1, result.Actions[1].CardInfo.ID);

            gameScene.OnDecidedPlayerPlotting();
        }

        public void StoreMovePhaseResult(MovePhaseResult result)
        {
            //fieldAnimation.SetMoveAnimation(this, result);
            //プレイヤーによってマス位置を変える(向き次第)
            board.GetSquare(Information.CurrentSquare).RemovePlayer();
            Information.SetCurrentSquare(result.DestSquareNum);
            board.GetSquare(result.DestSquareNum).AddPlayer(Information.PlayerID);
            //process = AnimationProcess;
        }

        public void StoreRangePhaseResult(RangePhaseResult result)
        {
            //process = AnimationProcess;
        }

        /// <summary>
        /// 自分以外のプレイヤー用
        /// 移動アニメーションのセットから
        /// プレイヤーの位置情報の更新まで行う
        /// </summary>
        /// <param name="maxNumOfMoves"></param>
        /// <param name="offset"></param>
        /// <param name="directions"></param>
        /// <param name="isInversion"></param>
        public void CalculateCurrentSquare(int maxNumOfMoves, int offset, byte[] directions, bool isInversion)
        {
            board.GetSquare(Information.CurrentSquare).RemovePlayer();

            int x = 0, y = 0;
            BoardDirection tmp;
            int inversionOffset = isInversion ? 2 : 0;

            for (int i = 0; i < maxNumOfMoves; i++)
            {
                tmp = BoardDirection.Up + (((directions[(i / 4) + offset] >> (2 * i % 8)) & 3) ^ inversionOffset);
                switch (tmp)
                {
                    case BoardDirection.Up:
                        y += 1;
                        break;

                    case BoardDirection.Right:
                        x += 1;
                        break;

                    case BoardDirection.Down:
                        y -= 1;
                        break;

                    case BoardDirection.Left:
                        x -= 1;
                        break;
                }
            }

            Information.SetCurrentSquare(Information.CurrentSquare + Board.Width * y + x);
            board.GetSquare(Information.CurrentSquare).AddPlayer(Information.PlayerID);
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

        /// <summary>
        /// MovePhaseの実装セット
        /// </summary>
        #region MovePhase
        /*
        public void SetDirection(int dir)
        {
            if (decideDestination)
                return;

            if (!board.CanMoveToDirection(currentSquare, BoardDirection.Up + dir))
            {
                Debug.Log("Nothing Square!");
                return;
            }
            int nextSquare = board.GetSquare(currentSquare).GetAdjacentSquares(BoardDirection.Up + dir).Number;

            if (board.PlayerIsInSquare(nextSquare))
            {
                Debug.Log("The square has already player!");
                return;
            }

            if (NumberOfMoves != 0 && (int)moveDirectionHistories[NumberOfMoves - 1] == (dir + 2) % 4)
            {
                NumberOfMoves--;
                mobility += board.GetSquare(nextSquare).ConsumptionOfMobility;
                moveDirection = BoardDirection.Up + dir;
                decideDestination = true;
                currentSquare = nextSquare;
                directionUI.SetActive(false);
                Debug.Log("Go Back");
                return;
            }

            if (mobility < board.GetSquare(nextSquare).ConsumptionOfMobility)
            {
                Debug.Log("Cannot Move!");
                return;
            }

            Debug.Log("Go Forward!");
            directionUI.SetActive(false);
            moveDirectionHistories[NumberOfMoves] = BoardDirection.Up + dir;
            moveDirection = moveDirectionHistories[NumberOfMoves];
            currentSquare = board.GetSquare(currentSquare).GetAdjacentSquares(BoardDirection.Up + dir).Number;
            NumberOfMoves++;
            mobility -= board.GetSquare(nextSquare).ConsumptionOfMobility;
            decideDestination = true;
        }

        public void SetMoveAmount()
        {
            mobility = 4;
            NumberOfMoves = 0;
            Debug.Log("You Got 4 Mobilities!");
            decideDestination = false;
        }

        public void DecideDestination()
        {
            if (decideDestination)
                return;

            if (WhetherPlayerCanStillMove())
            {
                Debug.Log("You Can Still Move!");
                return;
            }

            directionUI.SetActive(false);
            board.GetSquare(currentSquare).PlayerEnter(Team);
            UIManager.Instance.SwitchUI(PhaseEnum.MovePhase, false);
            NumberOfMoves = 0;
            mobility = 0;
            Signal = true;
            phase = null;
            Debug.Log("Let's Go!");
        }

        bool WhetherPlayerCanStillMove()
        {
            if (NumberOfMoves == 0)
                return true;

            int adjacentSquareNumber;
            for (int i = 0; i < 4; i++)
            {
                if (moveDirectionHistories[NumberOfMoves - 1] == BoardDirection.Up + i)
                    continue;

                adjacentSquareNumber = board.GetSquare(currentSquare).GetAdjacentSquares(BoardDirection.Up + i).Number;
                if (board.GetSquare(adjacentSquareNumber).ConsumptionOfMobility > mobility || board.PlayerIsInSquare(adjacentSquareNumber))
                    return false;
            }

            return true;
        }

        void MoveToDestination()
        {
            moveAmountBetweenSquares += charaMoveSpeed;
            if (moveAmountBetweenSquares > Square.Side)
            {
                MoveBetweenSquares(transform, moveDirection, charaMoveSpeed - (moveAmountBetweenSquares - Square.Side));
                moveAmountBetweenSquares = 0f;
                directionUI.SetActive(true);
                decideDestination = false;
            }
            else
            {
                MoveBetweenSquares(transform, moveDirection, charaMoveSpeed);
            }
        }

        void MoveBetweenSquares(Transform t, BoardDirection dir, float amount)
        {
            switch (dir)
            {
                case BoardDirection.Up:
                    t.position += Vector3.forward * amount;
                    break;

                case BoardDirection.Right:
                    t.position += Vector3.right * amount;
                    break;

                case BoardDirection.Down:
                    t.position += Vector3.back * amount;
                    break;

                case BoardDirection.Left:
                    t.position += Vector3.left * amount;
                    break;
            }
        }

        void MovePhase()
        {
            if (!decideDestination)
                return;

            MoveToDestination();
        }
        */
        #endregion

        /// <summary>
        /// RangePhaseの実装セット
        /// </summary>
        #region RangePhase
        /*
    public void SetUsedCard(int segment)
    {
        usedCardInformation = actions[segment].CardInfo;
    }

    public void MoveAim(int n)
    {
        if (!board.CanMoveToDirection(targetSquare, BoardDirection.Up + n))
        {
            Debug.Log("Nothing Square!");
            return;
        }

        int tmp = board.GetSquare(targetSquare).GetAdjacentSquares(BoardDirection.Up + n).Number;

        if (board.GetManhattanDistance(startSquare, tmp) > usedCardInformation.maxRange)
        {
            Debug.Log("Over move!");
            return;
        }

        targetSquare = tmp;
    }

    public void DecideTarget()
    {
        int dis = board.GetManhattanDistance(startSquare, targetSquare);
        if (dis > usedCardInformation.maxRange || dis < usedCardInformation.minRange)
        {
            Debug.Log("Over Range!");
            return;
        }

        UIManager.Instance.SwitchUI(PhaseEnum.RangePhase, false);
        usedCardInformation = null;
        Signal = true;
        phase = null;
        Debug.Log("You attack this square!!");
    }

    void RangePhase()
    {
        if (!decideTarget)
            return;

    }

    #endregion

    #region CardOperation
    /*
    /// <summary>
    /// 山札から規定数のカードを手札に加える(ランダム)
    /// </summary>
    void DrawCards()
    {
        int i;
        int count = 0;
        while (count != character.Imagination)
        {
            if (deck == 0)
                ReturnDeckFromDiscards();

            if (deck == 0)
                break;
            i = BitCalculation.GetNthBit(deck, Random.Range(0, BitCalculation.BitCount(deck)));
            deck &= ~i;
            hands |= i;
            handObjects[count].SetCardInfo(character.GetCard(BitCalculation.CheckWhichBitIsOn(i)));
            count++;
        }
        numberOfHands = count;
    }

    /// <summary>
    /// 捨て札をすべて山札に戻す
    /// </summary>
    void ReturnDeckFromDiscards()
    {
        deck = discards;
        discards = 0;
    }

    /// <summary>
    /// 手札を捨てる
    /// </summary>
    void ThrowAwayHands()
    {
        discards |= hands;
        hands = 0;
    }

    /// <summary>
    /// 山札からランダムなカードを除外する
    /// </summary>
    /// <param name="num"></param>
    void ExcludeFromDeck(int num)
    {
        int i;
        int count = 0;
        while (count != num)
        {
            if (deck == 0)
                ReturnDeckFromDiscards();

            i = BitCalculation.GetNthBit(deck, Random.Range(0, BitCalculation.BitCount(deck)));
            deck &= ~i;
            exclusionCards |= i;

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
        if (exclusionCards == 0)
            return;
        Debug.Log("山札：" + System.Convert.ToString(deck, 2));
        int i = BitCalculation.GetNthBit(exclusionCards, Random.Range(0, BitCalculation.BitCount(exclusionCards)));
        while (amount-- > 0 && exclusionCards != 0)
        {
            exclusionCards &= ~i;
            deck |= i;

            i = BitCalculation.GetNthBit(exclusionCards, Random.Range(0, BitCalculation.BitCount(exclusionCards)));
        }

        Debug.Log("山札：" + System.Convert.ToString(deck, 2));
    }

    void ResetHandObjectsPosition()
    {
        float half = (numberOfHands - 1) / 2f;
        for (int j = 0; j < numberOfHands; j++)
        {
            handObjects[j].ResetPosition(half);
        }
    }
    */
        #endregion
    }
}
