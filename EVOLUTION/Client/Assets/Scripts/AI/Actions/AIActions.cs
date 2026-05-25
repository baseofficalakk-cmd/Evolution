using UnityEngine;
using Evolution.AI.BehaviorTrees;
using Evolution.Shared.Enums;

namespace Evolution.AI.Actions
{
    /// <summary>
    /// Базовый класс для действий AI
    /// </summary>
    public abstract class ActionNode : BehaviorNode
    {
        protected AIBotController botController;
        
        public void SetBotController(AIBotController controller)
        {
            this.botController = controller;
        }
    }
    
    /// <summary>
    /// Действие: Движение к точке
    /// </summary>
    public class MoveToAction : ActionNode
    {
        private Vector3 targetPosition;
        private float stopDistance;
        private bool useNavMesh;
        
        public MoveToAction(float stopDistance = 0.5f, bool useNavMesh = true)
        {
            this.stopDistance = stopDistance;
            this.useNavMesh = useNavMesh;
            NodeName = "MoveTo";
        }
        
        public void SetTarget(Vector3 position)
        {
            targetPosition = position;
        }
        
        public override BehaviorNodeStatus Execute(float deltaTime)
        {
            if (botController == null)
                return BehaviorNodeStatus.Failure;
            
            float distance = Vector3.Distance(botController.transform.position, targetPosition);
            
            if (distance <= stopDistance)
            {
                botController.SetMovementInput(Vector2.zero);
                Status = BehaviorNodeStatus.Success;
                return BehaviorNodeStatus.Success;
            }
            
            Vector3 direction = (targetPosition - botController.transform.position).normalized;
            botController.SetMovementInput(new Vector2(direction.x, direction.z));
            
            Status = BehaviorNodeStatus.Running;
            return BehaviorNodeStatus.Running;
        }
        
        public override void Terminate()
        {
            botController?.SetMovementInput(Vector2.zero);
            base.Terminate();
        }
    }
    
    /// <summary>
    /// Действие: Атака цели
    /// </summary>
    public class AttackAction : ActionNode
    {
        private GameObject target;
        private float attackRange;
        private float fireRate;
        private float timeSinceLastAttack;
        
        public AttackAction(float attackRange = 10f, float fireRate = 1f)
        {
            this.attackRange = attackRange;
            this.fireRate = fireRate;
            NodeName = "Attack";
        }
        
        public void SetTarget(GameObject newTarget)
        {
            this.target = newTarget;
        }
        
        public override BehaviorNodeStatus Execute(float deltaTime)
        {
            if (botController == null || target == null)
                return BehaviorNodeStatus.Failure;
            
            timeSinceLastAttack += deltaTime;
            
            float distance = Vector3.Distance(botController.transform.position, target.transform.position);
            
            if (distance > attackRange)
            {
                Status = BehaviorNodeStatus.Failure;
                return BehaviorNodeStatus.Failure;
            }
            
            if (timeSinceLastAttack >= fireRate)
            {
                Vector3 direction = (target.transform.position - botController.transform.position).normalized;
                botController.SetAimDirection(new Vector2(direction.x, direction.z));
                botController.PerformAttack();
                timeSinceLastAttack = 0;
            }
            
            Status = BehaviorNodeStatus.Running;
            return BehaviorNodeStatus.Running;
        }
    }
    
    /// <summary>
    /// Действие: Использование способности
    /// </summary>
    public class UseAbilityAction : ActionNode
    {
        private string abilityId;
        private Vector3 targetPosition;
        private Vector2 targetDirection;
        private float cooldown;
        private float timeSinceLastUse;
        
        public UseAbilityAction(string abilityId, float cooldown = 10f)
        {
            this.abilityId = abilityId;
            this.cooldown = cooldown;
            NodeName = $"UseAbility_{abilityId}";
        }
        
        public void SetTargetPosition(Vector3 position)
        {
            targetPosition = position;
        }
        
        public void SetTargetDirection(Vector2 direction)
        {
            targetDirection = direction;
        }
        
        public override BehaviorNodeStatus Execute(float deltaTime)
        {
            if (botController == null)
                return BehaviorNodeStatus.Failure;
            
            timeSinceLastUse += deltaTime;
            
            if (timeSinceLastUse < cooldown)
            {
                Status = BehaviorNodeStatus.Failure;
                return BehaviorNodeStatus.Failure;
            }
            
            botController.SetAimDirection(targetDirection);
            botController.UseAbility(abilityId, targetPosition, targetDirection);
            
            timeSinceLastUse = 0;
            Status = BehaviorNodeStatus.Success;
            return BehaviorNodeStatus.Success;
        }
    }
    
    /// <summary>
    /// Действие: Отступление (бегство)
    /// </summary>
    public class RetreatAction : ActionNode
    {
        private GameObject threatSource;
        private float retreatDistance;
        
        public RetreatAction(float retreatDistance = 15f)
        {
            this.retreatDistance = retreatDistance;
            NodeName = "Retreat";
        }
        
        public void SetThreat(GameObject threat)
        {
            this.threatSource = threat;
        }
        
        public override BehaviorNodeStatus Execute(float deltaTime)
        {
            if (botController == null || threatSource == null)
                return BehaviorNodeStatus.Failure;
            
            Vector3 retreatDirection = (botController.transform.position - threatSource.transform.position).normalized;
            float currentDistance = Vector3.Distance(botController.transform.position, threatSource.transform.position);
            
            if (currentDistance >= retreatDistance)
            {
                botController.SetMovementInput(Vector2.zero);
                Status = BehaviorNodeStatus.Success;
                return BehaviorNodeStatus.Success;
            }
            
            botController.SetMovementInput(new Vector2(retreatDirection.x, retreatDirection.z));
            
            Status = BehaviorNodeStatus.Running;
            return BehaviorNodeStatus.Running;
        }
        
        public override void Terminate()
        {
            botController?.SetMovementInput(Vector2.zero);
            base.Terminate();
        }
    }
    
    /// <summary>
    /// Действие: Поиск укрытия в кустах
    /// </summary>
    public class SeekCoverAction : ActionNode
    {
        private Vector3[] bushPositions;
        private int currentBushIndex;
        private float searchRadius;
        
        public SeekCoverAction(float searchRadius = 20f)
        {
            this.searchRadius = searchRadius;
            NodeName = "SeekCover";
        }
        
        public void SetBushPositions(Vector3[] positions)
        {
            bushPositions = positions;
        }
        
        public override BehaviorNodeStatus Execute(float deltaTime)
        {
            if (botController == null || bushPositions == null || bushPositions.Length == 0)
                return BehaviorNodeStatus.Failure;
            
            Vector3 closestBush = FindClosestBush();
            float distance = Vector3.Distance(botController.transform.position, closestBush);
            
            if (distance <= 1f)
            {
                botController.SetMovementInput(Vector2.zero);
                Status = BehaviorNodeStatus.Success;
                return BehaviorNodeStatus.Success;
            }
            
            Vector3 direction = (closestBush - botController.transform.position).normalized;
            botController.SetMovementInput(new Vector2(direction.x, direction.z));
            
            Status = BehaviorNodeStatus.Running;
            return BehaviorNodeStatus.Running;
        }
        
        private Vector3 FindClosestBush()
        {
            Vector3 closest = bushPositions[0];
            float minDistance = float.MaxValue;
            
            foreach (var bush in bushPositions)
            {
                float dist = Vector3.Distance(botController.transform.position, bush);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    closest = bush;
                }
            }
            
            return closest;
        }
        
        public override void Terminate()
        {
            botController?.SetMovementInput(Vector2.zero);
            base.Terminate();
        }
    }
    
    /// <summary>
    /// Действие: Патрулирование между точками
    /// </summary>
    public class PatrolAction : ActionNode
    {
        private Vector3[] patrolPoints;
        private int currentPointIndex;
        private float waitTime;
        private float currentWaitTime;
        private bool isWaiting;
        
        public PatrolAction(float waitTime = 2f)
        {
            this.waitTime = waitTime;
            NodeName = "Patrol";
        }
        
        public void SetPatrolPoints(Vector3[] points)
        {
            patrolPoints = points;
            currentPointIndex = 0;
        }
        
        public override BehaviorNodeStatus Execute(float deltaTime)
        {
            if (botController == null || patrolPoints == null || patrolPoints.Length == 0)
                return BehaviorNodeStatus.Failure;
            
            if (isWaiting)
            {
                currentWaitTime += deltaTime;
                
                if (currentWaitTime >= waitTime)
                {
                    isWaiting = false;
                    currentWaitTime = 0;
                    currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
                }
                
                Status = BehaviorNodeStatus.Running;
                return BehaviorNodeStatus.Running;
            }
            
            Vector3 targetPoint = patrolPoints[currentPointIndex];
            float distance = Vector3.Distance(botController.transform.position, targetPoint);
            
            if (distance <= 0.5f)
            {
                isWaiting = true;
                botController.SetMovementInput(Vector2.zero);
                Status = BehaviorNodeStatus.Running;
                return BehaviorNodeStatus.Running;
            }
            
            Vector3 direction = (targetPoint - botController.transform.position).normalized;
            botController.SetMovementInput(new Vector2(direction.x, direction.z));
            
            Status = BehaviorNodeStatus.Running;
            return BehaviorNodeStatus.Running;
        }
        
        public override void Terminate()
        {
            botController?.SetMovementInput(Vector2.zero);
            isWaiting = false;
            currentWaitTime = 0;
            base.Terminate();
        }
    }
    
    /// <summary>
    /// Действие: Следование за союзником
    /// </summary>
    public class FollowAllyAction : ActionNode
    {
        private GameObject ally;
        private float followDistance;
        private float stopDistance;
        
        public FollowAllyAction(float followDistance = 8f, float stopDistance = 2f)
        {
            this.followDistance = followDistance;
            this.stopDistance = stopDistance;
            NodeName = "FollowAlly";
        }
        
        public void SetAlly(GameObject newAlly)
        {
            this.ally = newAlly;
        }
        
        public override BehaviorNodeStatus Execute(float deltaTime)
        {
            if (botController == null || ally == null)
                return BehaviorNodeStatus.Failure;
            
            float distance = Vector3.Distance(botController.transform.position, ally.transform.position);
            
            if (distance <= stopDistance)
            {
                botController.SetMovementInput(Vector2.zero);
                Status = BehaviorNodeStatus.Running;
                return BehaviorNodeStatus.Running;
            }
            
            if (distance > followDistance)
            {
                Vector3 direction = (ally.transform.position - botController.transform.position).normalized;
                botController.SetMovementInput(new Vector2(direction.x, direction.z));
            }
            else
            {
                botController.SetMovementInput(Vector2.zero);
            }
            
            Status = BehaviorNodeStatus.Running;
            return BehaviorNodeStatus.Running;
        }
        
        public override void Terminate()
        {
            botController?.SetMovementInput(Vector2.zero);
            base.Terminate();
        }
    }
}
