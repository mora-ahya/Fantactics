using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FantacticsScripts
{
    public class RangePhase : Phase
    {
        // 間借りしているが、UI表示処理などとは必ず分ける
        protected override IEnumerator Act(List<Player> players)
        {
            yield return DisplayPhaseNotice();

            foreach (Player player in players)
            {
                player.StartAction(PhaseEnum.RangePhase, 0);

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
