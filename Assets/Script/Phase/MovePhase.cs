using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantacticsScripts
{
    public class MovePhase : Phase
    {
        // 間借りしているが、UI表示処理などとは必ず分ける
        protected override IEnumerator Act(List<Player> players)
        {
            yield return DisplayPhaseNotice();

            foreach (Player player in players)
            {
                player.StartAction(PhaseEnum.MovePhase, 0);

                while (player.IsActing)
                {
                    yield return null;
                }

                player.ApplyMoveResult();

                while (player.IsMoving)
                {
                    yield return null;
                }
            }

            IsCompleted = true;
        }
    }
}
