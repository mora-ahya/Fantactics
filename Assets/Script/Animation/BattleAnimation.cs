using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantacticsScripts
{
    public class BattleAnimation : MonoBehaviour
    {
        public GameScene GameScene { get; private set; }
        public bool IsRunning { get; private set; }
        AttackEffect attackEffect;
        Player attacker;
        int victimIDs;

        public void SetAnimation(Player atk, AttackEffect effect, int targetSquare)
        {
            IsRunning = true;
            attacker = atk;
            attackEffect = effect;
            attackEffect.transform.position = Board.SquareNumberToWorldPosition(targetSquare);
            attackEffect.Initialize();
        }

        public void Act()
        {
            if (attackEffect == null)
                return;

            attackEffect.Act();

            if (attackEffect.IsEnd)
            {
                attackEffect = null;
                IsRunning = false;
            }
        }

        void Phase1()
        {
            //アタッカーにカメラを向ける
        }

        void Phase2()
        {
            //エフェクトにカメラを向ける
        }
    }
}
