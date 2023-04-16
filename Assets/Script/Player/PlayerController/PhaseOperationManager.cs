using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantacticsScripts
{
    public interface IPhaseOperationBase
    {
        public void OnStart();
        public void Act();
        public void End();
        public bool CanEnd();
    }

    //public static class IControllerBaseExtension
    //{
    //    public static void OnDestroy(this IControllerBase target)
    //    {
    //        Debug.Log("DestroyTest : IControllerBaseExtension");
    //        PlayerController.Instance.OnDestroyedControllerObject(target);
    //    }
    //}

    public class PhaseOperation : MonoBehaviour, IPhaseOperationBase
    {
        public virtual void OnStart()
        {

        }

        public virtual void Act()
        {

        }

        public virtual void End()
        {

        }

        public virtual bool CanEnd()
        {
            return true;
        }
    }

    public class PhaseOperationManager : MonoBehaviour, IManagerBase
    {
        public static PhaseOperationManager Instance { get; private set; }

        public int ActPriority => 0;

        IPhaseOperationBase currentPhaseOperation;

        [SerializeField] GameObject phaseOperationsObject = default;

        [SerializeField] PlottingOperation plottingOperation = default;
        [SerializeField] MoveOnBoardOperation moveOnBoardOperation = default;
        [SerializeField] AttackOnBoardOperation attackOnBoardOperation = default;

        public void AwakeInitialize()
        {
            Instance = this;
        }

        public void LateAwakeInitialize()
        {
            IPhaseOperationBase[] phaseOperations = phaseOperationsObject.GetComponents<IPhaseOperationBase>();

            foreach (IPhaseOperationBase phaseOperation in phaseOperations)
            {
                phaseOperation.OnStart();
            }
        }

        public void Act()
        {
            currentPhaseOperation?.Act();
        }

        public void EndCurrentController()
        {
            if (currentPhaseOperation != null && currentPhaseOperation.CanEnd())
            {
                currentPhaseOperation.End();
            }
        }

        public void ActivePlottingOperation(Player player, PlottingResult result)
        {
            plottingOperation.Initialize(player, result);
            currentPhaseOperation = plottingOperation;
        }

        public void ActiveMoveOnBoardOperation(Player player, int mobility, int startSquareNum, MoveResult result)
        {
            moveOnBoardOperation.Initialize(player, startSquareNum, mobility, result);
            currentPhaseOperation = moveOnBoardOperation;
        }

        public void ActiveAttackOnBoardOperation(Player player, AttackResult result)
        {
            attackOnBoardOperation.Initialize(player, result);
            currentPhaseOperation = attackOnBoardOperation;
        }
    }
}
