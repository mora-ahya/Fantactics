using System.Collections;
using System.Collections.Generic;

namespace FantacticsScripts
{
    public class PhaseResult
    {
        public PhaseEnum PhaseNumber { get; protected set; }

        protected PhaseResult()
        {

        }

        public virtual void Clear()
        {

        }
    }

    public class PlottingPhaseResult : PhaseResult
    {
        public readonly Card[] Actions = new Card[2];

        public override void Clear()
        {
            Actions[0] = null;
            Actions[1] = null;
        }
    }

    public class MovePhaseResult : PhaseResult
    {
        public readonly BoardDirection[] MoveDirections = new BoardDirection[8];

        public int NumOfMoves = 0;
        public int CurrentSquare = 0;

        public override void Clear()
        {
            NumOfMoves = 0;
            CurrentSquare = 0;
        }
    }

    public class RangePhaseResult : PhaseResult
    {
        public int TargetSquare = 0;

        public override void Clear()
        {
            TargetSquare = 0;
        }
    }
}
