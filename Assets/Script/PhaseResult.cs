using System.Collections;
using System.Collections.Generic;

namespace FantacticsScripts
{
    public class PhaseResult
    {
        public PhaseEnum PhaseNumber { get; protected set; }

        // ÉvÉåÉCÉÑÅ[ÇéØï Ç≈Ç´ÇÈâΩÇ© int?
        public int PlayerID;

        protected PhaseResult()
        {

        }

        public virtual void Clear()
        {
            PlayerID = 0;
        }
    }

    public class PlottingPhaseResult : PhaseResult
    {
        public readonly Card[] Actions = new Card[2];

        public PlottingPhaseResult()
        {
            PhaseNumber = PhaseEnum.PlottingPhase;
        }

        public override void Clear()
        {
            base.Clear();
            Actions[0] = null;
            Actions[1] = null;
        }
    }

    public class MovePhaseResult : PhaseResult
    {
        public readonly BoardDirection[] MoveDirections = new BoardDirection[8];

        public BoardDirection PlayerForward;
        public int NumOfMove = 0;
        public int DestSquareNum = 0;

        public MovePhaseResult()
        {
            PhaseNumber = PhaseEnum.MovePhase;
        }

        public override void Clear()
        {
            base.Clear();
            NumOfMove = 0;
            DestSquareNum = 0;
            PlayerForward = BoardDirection.Up;
        }

        public void Copy(MovePhaseResult source)
        {
            source.MoveDirections.CopyTo(this.MoveDirections, 0);
            this.NumOfMove = source.NumOfMove;
            this.DestSquareNum = source.DestSquareNum;
            this.PlayerForward = source.PlayerForward;
        }
    }

    public class RangePhaseResult : PhaseResult
    {
        public int TargetSquare = 0;

        public RangePhaseResult()
        {
            PhaseNumber = PhaseEnum.RangePhase;
        }

        public override void Clear()
        {
            base.Clear();
            TargetSquare = 0;
        }
    }
}
