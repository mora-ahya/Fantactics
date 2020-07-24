using System.Collections;
using System.Collections.Generic;

namespace FantacticsScripts
{
    public enum Phase
    {
        SelectSegmentsPhase,
        MovePhase,
        RangePhase,
        MeleePhase,
        SpecialPhase
    }

    public enum CardType
    {
        Move,
        Range,
        Melee,
        Special
    }

    public enum BoardDirection
    {
        Up = 0,
        Right = 1,
        Down = 2,
        Left = 3
    }
}
