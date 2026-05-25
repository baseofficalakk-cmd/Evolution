using UnityEngine;
using Evolution.AI.BehaviorTrees;
using Evolution.AI.Actions;
using Evolution.AI.Decisions;

namespace Evolution.AI
{
    /// <summary>
    /// Решения для дерева поведения AI
    /// </summary>
    
    /// <summary>
    /// Базовый класс для решений (Decisions)
    /// </summary>
    public abstract class DecisionNode : BehaviorNode
    {
        protected AIBotController botController;
        
        public void SetBotController(AIBotController controller)
        {
            this.botController = controller;
        }
    }
    
    /// <summary>
    /// Решение: Проверка здоровья
    /// </summary>
    public class HealthCheckDecision : DecisionNode
    {
        private float healthThreshold;
        private bool checkLessThan;
        
        public HealthCheckDecision(float threshold, bool lessThan = true)
        {
            healthThreshold = threshold;
            checkLessThan = lessThan;
            NodeName = "HealthCheck";
        }
        
        public override BehaviorNodeStatus Execute(float deltaTime)
        {
            if (botController == null)
                return BehaviorNodeStatus.Failure;
            
            float healthPercent = botController.CurrentHealth / botController.MaxHealth;
            
            bool conditionMet = checkLessThan ? 
                (healthPercent < healthThreshold) : 
                (healthPercent >= healthThreshold);
            
            Status = conditionMet ? BehaviorNodeStatus.Success : BehaviorNodeStatus.Failure;
            return Status;
        }
    }
    
    /// <summary>
    /// Решение: Есть ли цель в радиусе
    /// </summary>
    public class EnemyInRangeDecision : DecisionNode
    {
        private float detectionRadius;
        private GameObject detectedEnemy;
        
        public EnemyInRangeDecision(float radius = 15f)
        {
            detectionRadius = radius;
            NodeName = "EnemyInRange";
        }
        
        public GameObject DetectedEnemy => detectedEnemy;
        
        public override BehaviorNodeStatus Execute(float deltaTime)
        {
            if (botController == null)
                return BehaviorNodeStatus.Failure;
            
            detectedEnemy = FindClosestEnemy();
            
            if (detectedEnemy != null)
            {
                float distance = Vector3.Distance(botController.transform.position, detectedEnemy.transform.position);
                
                if (distance <= detectionRadius)
                {
                    Status = BehaviorNodeStatus.Success;
                    return BehaviorNodeStatus.Success;
                }
            }
            
            Status = BehaviorNodeStatus.Failure;
            return BehaviorNodeStatus.Failure;
        }
        
        private GameObject FindClosestEnemy()
        {
            // Упрощенная реализация - в полной версии использовать Physics.OverlapSphere
            var enemies = GameObject.FindGameObjectsWithTag("Enemy");
            
            GameObject closest = null;
            float minDistance = float.MaxValue;
            
            foreach (var enemy in enemies)
            {
                float dist = Vector3.Distance(botController.transform.position, enemy.transform.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    closest = enemy;
                }
            }
            
            return closest;
        }
    }
    
    /// <summary>
    /// Решение: Проверка доступности способности
    /// </summary>
    public class AbilityReadyDecision : DecisionNode
    {
        private string abilityId;
        
        public AbilityReadyDecision(string ability)
        {
            abilityId = ability;
            NodeName = $"AbilityReady_{ability}";
        }
        
        public override BehaviorNodeStatus Execute(float deltaTime)
        {
            if (botController == null)
                return BehaviorNodeStatus.Failure;
            
            bool isReady = botController.IsAbilityReady(abilityId);
            
            Status = isReady ? BehaviorNodeStatus.Success : BehaviorNodeStatus.Failure;
            return Status;
        }
    }
    
    /// <summary>
    /// Решение: Находимся ли в кусте
    /// </summary>
    public class InBushDecision : DecisionNode
    {
        public InBushDecision()
        {
            NodeName = "InBush";
        }
        
        public override BehaviorNodeStatus Execute(float deltaTime)
        {
            if (botController == null)
                return BehaviorNodeStatus.Failure;
            
            bool inBush = botController.IsInBush();
            
            Status = inBush ? BehaviorNodeStatus.Success : BehaviorNodeStatus.Failure;
            return Status;
        }
    }
    
    /// <summary>
    /// Решение: Количество врагов в радиусе
    /// </summary>
    public class EnemyCountDecision : DecisionNode
    {
        private float radius;
        private int minEnemies;
        private int detectedCount;
        
        public EnemyCountDecision(int minimum, float detectionRadius = 20f)
        {
            minEnemies = minimum;
            radius = detectionRadius;
            NodeName = "EnemyCount";
        }
        
        public int DetectedCount => detectedCount;
        
        public override BehaviorNodeStatus Execute(float deltaTime)
        {
            if (botController == null)
                return BehaviorNodeStatus.Failure;
            
            detectedCount = CountEnemiesInRadius();
            
            Status = detectedCount >= minEnemies ? BehaviorNodeStatus.Success : BehaviorNodeStatus.Failure;
            return Status;
        }
        
        private int CountEnemiesInRadius()
        {
            var enemies = GameObject.FindGameObjectsWithTag("Enemy");
            int count = 0;
            
            foreach (var enemy in enemies)
            {
                float dist = Vector3.Distance(botController.transform.position, enemy.transform.position);
                if (dist <= radius)
                {
                    count++;
                }
            }
            
            return count;
        }
    }
    
    /// <summary>
    /// Решение: Случайный выбор (для разнообразия поведения)
    /// </summary>
    public class RandomDecision : DecisionNode
    {
        private float probability;
        
        public RandomDecision(float successProbability = 0.5f)
        {
            probability = Mathf.Clamp01(successProbability);
            NodeName = "Random";
        }
        
        public override BehaviorNodeStatus Execute(float deltaTime)
        {
            float roll = Random.value;
            
            Status = roll < probability ? BehaviorNodeStatus.Success : BehaviorNodeStatus.Failure;
            return Status;
        }
    }
    
    /// <summary>
    /// Решение: Проверка дистанции до цели
    /// </summary>
    public class DistanceCheckDecision : DecisionNode
    {
        private GameObject target;
        private float minDistance;
        private float maxDistance;
        
        public DistanceCheckDecision(GameObject targetObj, float min = 0f, float max = float.MaxValue)
        {
            target = targetObj;
            minDistance = min;
            maxDistance = max;
            NodeName = "DistanceCheck";
        }
        
        public override BehaviorNodeStatus Execute(float deltaTime)
        {
            if (botController == null || target == null)
                return BehaviorNodeStatus.Failure;
            
            float distance = Vector3.Distance(botController.transform.position, target.transform.position);
            
            bool inRange = distance >= minDistance && distance <= maxDistance;
            
            Status = inRange ? BehaviorNodeStatus.Success : BehaviorNodeStatus.Failure;
            return Status;
        }
    }
}
