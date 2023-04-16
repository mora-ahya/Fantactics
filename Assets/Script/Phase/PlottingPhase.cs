using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FantacticsScripts
{
    public class PlottingPhase : Phase
    {
        protected override IEnumerator Act(List<Player> players)
        {
            yield return DisplayPhaseNotice();

            foreach (Player player in players)
            {
                player.StartAction(PhaseEnum.PlottingPhase, 0);
            }

            yield return WaitPlayersAction(players);

            IsCompleted = true;
        }
    }
}
