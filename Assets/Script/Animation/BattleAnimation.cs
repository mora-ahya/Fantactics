using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantacticsScripts
{
    public class BattleAnimation : MonoBehaviour
    {
        public GameScene GameScene { get; private set; }
        AttackEffect attackEffect;
        Player attacker;
        int victimIDs;

        public void StartAnimation(AttackEffect effect, int squarePos)
        {
            attackEffect = effect;
            attackEffect.transform.position = Board.SquareNuberToWorldPosition(squarePos);
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
            }
        }
    }
}
