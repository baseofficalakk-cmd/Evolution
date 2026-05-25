using UnityEngine;
using Evolution.AI.BehaviorTrees;
using Evolution.AI.Actions;
using Evolution.AI.Decisions;

namespace Evolution.AI
{
    /// <summary>
    /// Контроллер AI бота - управляет поведением бота на основе дерева поведения
    /// </summary>
    public class AIBotController : MonoBehaviour
    {
        [Header("AI Settings")]
        [SerializeField] private float updateInterval = 0.1f;
        [SerializeField] private float reactionTime = 0.2f;
        [SerializeField] private float accuracyModifier = 0.8f;
        
        [Header("Combat Settings")]
        [SerializeField] private float attackRange = 10f;
        [SerializeField] private float retreatHealthPercent = 0.3f;
        [SerializeField] private float bushSearchRadius = 20f;
        
        [Header("References")]
        [SerializeField] private GameObject heroPrefab;
        
        // Состояние бота
        private BehaviorNode behaviorTree;
        private float updateTimer;
        private float currentHealth;
        private float maxHealth;
        private bool isDead;
        
        // Компоненты героя
        private CharacterController characterController;
        private CombatSystem combatSystem;
        private Hero heroComponent;
        
        // Данные о окружении
        private Vector3[] patrolPoints;
        private Vector3[] bushPositions;
        private GameObject currentTarget;
        private GameObject[] allies;
        
        // Ввод для героя
        private Vector2 movementInput;
        private Vector2 aimDirection;
        private bool isAttacking;
        private bool isUsingAbility;
        
        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;
        public bool IsDead => isDead;
        
        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            combatSystem = GetComponent<CombatSystem>();
            heroComponent = GetComponent<Hero>();
            
            if (heroComponent != null)
            {
                maxHealth = heroComponent.MaxHealth;
                currentHealth = maxHealth;
            }
        }
        
        private void Start()
        {
            InitializeBehaviorTree();
            SetupEnvironmentData();
        }
        
        private void Update()
        {
            if (isDead) return;
            
            updateTimer += Time.deltaTime;
            
            if (updateTimer >= updateInterval)
            {
                updateTimer = 0;
                UpdateBehavior();
            }
            
            ApplyInputs();
        }
        
        /// <summary>
        /// Инициализация дерева поведения
        /// </summary>
        private void InitializeBehaviorTree()
        {
            // Создаем главное дерево: Selector между различными состояниями
            var mainSelector = new SelectorNode { NodeName = "MainBehavior" };
            
            // 1. Отступление при низком HP
            var retreatSequence = CreateRetreatBehavior();
            mainSelector.AddChild(retreatSequence);
            
            // 2. Атака врагов
            var attackSequence = CreateAttackBehavior();
            mainSelector.AddChild(attackSequence);
            
            // 3. Патрулирование/следование за целью
            var patrolSequence = CreatePatrolBehavior();
            mainSelector.AddChild(patrolSequence);
            
            behaviorTree = mainSelector;
            
            // Устанавливаем ссылки на botController для всех узлов
            SetBotControllerReferences(behaviorTree);
            
            behaviorTree.Initialize();
        }
        
        private void SetBotControllerReferences(BehaviorNode node)
        {
            if (node is ActionNode action)
            {
                action.SetBotController(this);
            }
            
            if (node is DecisionNode decision)
            {
                decision.SetBotController(this);
            }
            
            // Рекурсивно для дочерних узлов
            if (node is SequenceNode seq)
            {
                // Нужно использовать рефлексию или добавить метод для получения детей
            }
            else if (node is SelectorNode sel)
            {
                // Аналогично
            }
        }
        
        /// <summary>
        /// Поведение отступления
        /// </summary>
        private BehaviorNode CreateRetreatBehavior()
        {
            var sequence = new SequenceNode { NodeName = "RetreatBehavior" };
            
            // Проверка: HP ниже порога
            var healthCheck = new HealthCheckDecision(retreatHealthPercent, true);
            sequence.AddChild(healthCheck);
            
            // Проверка: есть ли враг поблизости
            var enemyCheck = new EnemyInRangeDecision(15f);
            sequence.AddChild(enemyCheck);
            
            // Действие: отступление
            var retreatAction = new RetreatAction(20f);
            sequence.AddChild(retreatAction);
            
            return sequence;
        }
        
        /// <summary>
        /// Поведение атаки
        /// </summary>
        private BehaviorNode CreateAttackBehavior()
        {
            var sequence = new SequenceNode { NodeName = "AttackBehavior" };
            
            // Проверка: есть ли враг в радиусе
            var enemyCheck = new EnemyInRangeDecision(attackRange);
            sequence.AddChild(enemyCheck);
            
            // Действие: атака
            var attackAction = new AttackAction(attackRange, 1f);
            sequence.AddChild(attackAction);
            
            return sequence;
        }
        
        /// <summary>
        /// Поведение патрулирования
        /// </summary>
        private BehaviorNode CreatePatrolBehavior()
        {
            var sequence = new SequenceNode { NodeName = "PatrolBehavior" };
            
            // Действие: патрулирование по точкам
            var patrolAction = new PatrolAction(2f);
            if (patrolPoints != null && patrolPoints.Length > 0)
            {
                patrolAction.SetPatrolPoints(patrolPoints);
            }
            sequence.AddChild(patrolAction);
            
            return sequence;
        }
        
        /// <summary>
        /// Настройка данных об окружении
        /// </summary>
        private void SetupEnvironmentData()
        {
            // Получаем точки патрулирования из карты
            var patrolObjects = GameObject.FindGameObjectsWithTag("PatrolPoint");
            patrolPoints = new Vector3[patrolObjects.Length];
            
            for (int i = 0; i < patrolObjects.Length; i++)
            {
                patrolPoints[i] = patrolObjects[i].transform.position;
            }
            
            // Получаем позиции кустов
            var bushObjects = GameObject.FindGameObjectsWithTag("Bush");
            bushPositions = new Vector3[bushObjects.Length];
            
            for (int i = 0; i < bushObjects.Length; i++)
            {
                bushPositions[i] = bushObjects[i].transform.position;
            }
        }
        
        /// <summary>
        /// Обновление поведения
        /// </summary>
        private void UpdateBehavior()
        {
            if (behaviorTree == null) return;
            
            behaviorTree.Execute(Time.deltaTime);
        }
        
        /// <summary>
        /// Применение введенных данных к герою
        /// </summary>
        private void ApplyInputs()
        {
            if (characterController == null || isDead) return;
            
            // Применяем движение
            if (movementInput != Vector2.zero)
            {
                Vector3 moveDirection = new Vector3(movementInput.x, 0, movementInput.y);
                characterController.Move(moveDirection * GetMovementSpeed() * Time.deltaTime);
            }
            
            // Применяем прицеливание
            if (aimDirection != Vector2.zero && combatSystem != null)
            {
                combatSystem.SetAimDirection(aimDirection);
            }
            
            // Применяем атаку
            if (isAttacking && combatSystem != null)
            {
                combatSystem.PerformAttack();
                isAttacking = false; // Сбрасываем после выполнения
            }
        }
        
        private float GetMovementSpeed()
        {
            return heroComponent != null ? heroComponent.MovementSpeed : 5f;
        }
        
        // ============================================
        // Публичные методы для действий AI
        // ============================================
        
        public void SetMovementInput(Vector2 input)
        {
            movementInput = input;
        }
        
        public void SetAimDirection(Vector2 direction)
        {
            aimDirection = direction.normalized;
        }
        
        public void PerformAttack()
        {
            isAttacking = true;
        }
        
        public void UseAbility(string abilityId, Vector3 targetPosition, Vector2 targetDirection)
        {
            if (!IsAbilityReady(abilityId)) return;
            
            SetAimDirection(targetDirection);
            
            if (combatSystem != null)
            {
                combatSystem.UseAbility(abilityId);
            }
        }
        
        public bool IsAbilityReady(string abilityId)
        {
            // Проверка готовности способности
            // В полной версии проверять cooldown через CombatSystem
            return true;
        }
        
        public bool IsInBush()
        {
            // Проверка нахождения в кусте
            // В полной версии использовать Physics.OverlapSphere с тегом Bush
            return false;
        }
        
        public void TakeDamage(float damage)
        {
            currentHealth = Mathf.Max(0, currentHealth - damage);
            
            if (currentHealth <= 0 && !isDead)
            {
                Die();
            }
        }
        
        private void Die()
        {
            isDead = true;
            movementInput = Vector2.zero;
            aimDirection = Vector2.zero;
            isAttacking = false;
            
            // Вызвать событие смерти
            if (heroComponent != null)
            {
                heroComponent.Die();
            }
        }
        
        public void Respawn(Vector3 spawnPosition, float newMaxHealth)
        {
            isDead = false;
            currentHealth = newMaxHealth;
            maxHealth = newMaxHealth;
            
            transform.position = spawnPosition;
        }
        
        public void SetCurrentTarget(GameObject target)
        {
            currentTarget = target;
        }
        
        public GameObject GetCurrentTarget()
        {
            return currentTarget;
        }
        
        public void SetAllies(GameObject[] allyObjects)
        {
            allies = allyObjects;
        }
        
        // ============================================
        // Методы для отладки
        // ============================================
        
        private void OnDrawGizmosSelected()
        {
            // Рисуем радиус обнаружения
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, attackRange);
            
            // Рисуем радиус отступления
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 20f);
            
            // Рисуем точки патрулирования
            if (patrolPoints != null)
            {
                Gizmos.color = Color.blue;
                foreach (var point in patrolPoints)
                {
                    Gizmos.DrawSphere(point, 0.5f);
                }
            }
        }
    }
}
