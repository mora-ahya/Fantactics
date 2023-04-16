using System.Collections;
using System.Collections.Generic;

namespace FantacticsScripts
{
    public class ActionResult
    {
        public PhaseEnum PhaseNumber { get; protected set; }

        // ÉvÉåÉCÉÑÅ[ÇéØï Ç≈Ç´ÇÈâΩÇ© int?
        public int PlayerID { get; protected set; }

        protected ActionResult(int playerID)
        {
            PlayerID = playerID;
        }

        public virtual void Initialize(PhaseEnum phaseEnum)
        {
            PhaseNumber = phaseEnum;
        }

        public virtual void Clear()
        {
            
        }

        public virtual void Copy(ActionResult source)
        {
            this.PhaseNumber = source.PhaseNumber;
        }
    }

    public class PlottingResult : ActionResult
    {
        public readonly Card[] Actions = new Card[2];

        public PlottingResult(int playerID) : base(playerID)
        {

        }

        public override void Clear()
        {
            base.Clear();
            Actions[0] = null;
            Actions[1] = null;
        }

        public void Copy(PlottingResult source)
        {
            base.Copy(source);
            source.Actions.CopyTo(this.Actions, 0);
        }
    }

    public class MoveResult : ActionResult
    {
        public readonly List<int> roadSquares = new List<int>(8);

        public BoardDirection PlayerForward;

        public MoveResult(int playerID) : base(playerID)
        {

        }

        public override void Clear()
        {
            base.Clear();
            roadSquares.Clear();
            PlayerForward = BoardDirection.Up;
        }

        public void Copy(MoveResult source)
        {
            base.Copy(source);
            this.roadSquares.AddRange(source.roadSquares);
            this.PlayerForward = source.PlayerForward;
        }
    }

    public class AttackResult : ActionResult
    {
        public int TargetSquare = 0;

        public AttackResult(int playerID) : base(playerID)
        {

        }

        public override void Clear()
        {
            base.Clear();
            TargetSquare = 0;
        }

        public void Copy(AttackResult source)
        {
            base.Copy(source);
            this.TargetSquare = source.TargetSquare;
        }
    }
}
