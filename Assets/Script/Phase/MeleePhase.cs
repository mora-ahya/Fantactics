using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FantacticsScripts
{
    public class MeleePhase : Phase
    {
        protected override IEnumerator Act(List<Player> players)
        {
            yield return DisplayPhaseNotice();

            // 近接フェーズ最初の移動タイミング
            foreach (Player player in players)
            {
                player.StartAction(PhaseEnum.MeleePhase, 0);
            }

            yield return WaitPlayersAction(players);

            // 移動結果の確認と計算を行う
            
            foreach (Player player in players)
            {
                player.ApplyMoveResult();
            }

            yield return WaitPlayersMove(players);
            
            // 攻撃を行うタイミング
            foreach (Player player in players)
            {
                player.StartAction(PhaseEnum.MeleePhase, 1);

                while (player.IsActing)
                {
                    yield return null;
                }

                player.ApplyAttackResult();

                while (player.IsAttacking)
                {
                    yield return null;
                }
            }

            IsCompleted = true;
        }
    }
}
