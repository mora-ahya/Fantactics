using UnityEngine;

namespace FantacticsScripts
{
    public enum PhaseEnum
    {
        PlottingPhase,
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
        Left = 3,
    }

    public enum DataType
    {
        Notification = 0,//行動するプレイヤー、プロット内容
        Action = 1,//行動したプレイヤーID,行動内容
        React = 2//仕掛けたプレイヤーのID,対応内容,具体的な内容
    }
}
