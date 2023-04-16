using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantacticsScripts
{
    public class FieldAnimation : MonoBehaviour
    {
        MoveAnimation moveAnimation;
        BattleAnimation battleAnimation;

        System.Action func;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Act()
        {
            func?.Invoke();
        }

        void MoveAnimationFunc()
        {
            moveAnimation.Act();

            if (!moveAnimation.IsRunning)
            {
                func = null;
            }
        }

        void BattleAnimationFunc()
        {
            battleAnimation.Act();

            if (!battleAnimation.IsRunning)
            {
                func = null;
            }
        }

        public void SetMoveAnimation(Player player, MoveResult result)
        {
            moveAnimation.SetAnimation(player, result);
            func = MoveAnimationFunc;
        }

        public void SetBattleAnimation(Player player, AttackEffect attackEffect, int targetSquare)
        {
            battleAnimation.SetAnimation(player, attackEffect, targetSquare);
            func = BattleAnimationFunc;
        }

        public bool IsEnd()
        {
            return func == null;
        }
    }
}
