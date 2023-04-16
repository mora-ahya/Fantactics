using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantacticsScripts
{
    public class Phase : MonoBehaviour
    {
        [SerializeField] protected PhaseManager manager = default;

        public bool IsCompleted { get; protected set; }

        public virtual void Initialize(Player player)
        {

        }

        public void Initialize(List<Player> players)
        {
            IsCompleted = false;
            StartCoroutine(Act(players));
        }

        protected virtual IEnumerator Act(List<Player> players)
        {
            yield return null;
        }

        public virtual PhaseEnum GetPhaseEnum()
        {
            return PhaseEnum.MeleePhase;
        }

        public virtual void Act()
        {

        }

        protected IEnumerator DisplayPhaseNotice()
        {
            UIManager.Instance.DisplayPhaseNotice(GetPhaseEnum());

            while (UIManager.Instance.GetPhaseNoticeIsActing())
            {
                yield return null;
            }
        }

        protected IEnumerator WaitPlayersAction(List<Player> players)
        {
            while (CheckAllActingPlayerIsCompleting(players) == false)
            {
                yield return null;
            }

            //OnCompleteWaitPlayers();
        }

        protected bool CheckAllActingPlayerIsCompleting(List<Player> players)
        {
            foreach (Player player in players)
            {
                if (player.IsActing)
                {
                    return false;
                }
            }

            return true;
        }

        protected IEnumerator WaitPlayersMove(List<Player> players)
        {
            while (CheckAllMovingPlayerIsCompleting(players) == false)
            {
                yield return null;
            }

            //OnCompleteWaitPlayers();
        }

        protected bool CheckAllMovingPlayerIsCompleting(List<Player> players)
        {
            foreach (Player player in players)
            {
                if (player.IsMoving)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
