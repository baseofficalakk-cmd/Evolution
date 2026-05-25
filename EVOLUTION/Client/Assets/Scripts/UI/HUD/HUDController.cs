using UnityEngine;
using UnityEngine.UI;
using Shared.Enums;
using Shared.Models;
using System.Collections.Generic;

namespace EVOLUTION.UI.HUD
{
    /// <summary>
    /// In-Game HUD Controller
    /// Displays health, abilities, cooldowns, minimap, score, and match timer
    /// </summary>
    public class HUDController : MonoBehaviour
    {
        [Header("Health Bar")]
        [SerializeField] private Slider healthSlider;
        [SerializeField] private Text healthText;
        [SerializeField] private Image healthFillImage;
        
        [Header("Ability Buttons")]
        [SerializeField] private Button basicAttackButton;
        [SerializeField] private Button skill1Button;
        [SerializeField] private Button skill2Button;
        [SerializeField] private Button ultimateButton;
        
        [Header("Cooldown Overlays")]
        [SerializeField] private Image skill1Cooldown;
        [SerializeField] private Image skill2Cooldown;
        [SerializeField] private Image ultimateCooldown;
        
        [Header("Match Info")]
        [SerializeField] private Text matchTimerText;
        [SerializeField] private Text scoreText;
        [SerializeField] private Text teamScoreText;
        
        [Header("Minimap")]
        [SerializeField] private RawImage minimapImage;
        [SerializeField] private RectTransform minimapPlayerIcon;
        [SerializeField] private Camera minimapCamera;
        
        [Header("Kill Feed")]
        [SerializeField] private Transform killFeedContainer;
        [SerializeField] private GameObject killFeedItemPrefab;
        [SerializeField] private int maxKillFeedItems = 5;
        
        [Header("Ping & Network")]
        [SerializeField] private Text pingText;
        [SerializeField] private Text fpsText;
        
        // State
        private Hero _localHero;
        private MatchData _matchData;
        private List<GameObject> _killFeedItems = new();
        
        // Events
        public event System.Action OnPingActivated;
        public event System.Action<AbilityType> OnAbilityButtonPressed;
        
        private void Awake()
        {
            SetupAbilityButtons();
        }
        
        private void Start()
        {
            InitializeHUD();
        }
        
        private void Update()
        {
            UpdateFPSCounter();
            UpdateHeroAbilities();
        }
        
        #region Initialization
        
        private void SetupAbilityButtons()
        {
            if (basicAttackButton != null)
                basicAttackButton.onClick.AddListener(() => OnAbilityButtonPressed?.Invoke(AbilityType.Basic));
            
            if (skill1Button != null)
                skill1Button.onClick.AddListener(() => OnAbilityButtonPressed?.Invoke(AbilityType.Skill1));
            
            if (skill2Button != null)
                skill2Button.onClick.AddListener(() => OnAbilityButtonPressed?.Invoke(AbilityType.Skill2));
            
            if (ultimateButton != null)
                ultimateButton.onClick.AddListener(() => OnAbilityButtonPressed?.Invoke(AbilityType.Ultimate));
        }
        
        private void InitializeHUD()
        {
            // Find local hero
            _localHero = FindObjectOfType<Hero>();
            
            if (_localHero != null)
            {
                SubscribeToHeroEvents();
                UpdateHealthBar(_localHero.GetCurrentHealth(), _localHero.GetMaxHealth());
            }
            
            // Hide cursor for gameplay
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        
        private void SubscribeToHeroEvents()
        {
            if (_localHero != null)
            {
                _localHero.OnHealthChanged += HandleHealthChanged;
                _localHero.OnHeroDied += HandleHeroDied;
            }
            
            // Subscribe to match manager events
            if (MatchManager.Instance != null)
            {
                MatchManager.Instance.OnMatchTimerUpdated += HandleMatchTimerUpdated;
                MatchManager.Instance.OnKillOccurred += HandleKillOccurred;
            }
        }
        
        private void OnDestroy()
        {
            if (_localHero != null)
            {
                _localHero.OnHealthChanged -= HandleHealthChanged;
                _localHero.OnHeroDied -= HandleHeroDied;
            }
            
            if (MatchManager.Instance != null)
            {
                MatchManager.Instance.OnMatchTimerUpdated -= HandleMatchTimerUpdated;
                MatchManager.Instance.OnKillOccurred -= HandleKillOccurred;
            }
        }
        
        #endregion
        
        #region Health Bar
        
        private void HandleHealthChanged(Hero hero, float currentHealth, float maxHealth)
        {
            if (hero != _localHero) return;
            UpdateHealthBar(currentHealth, maxHealth);
        }
        
        private void UpdateHealthBar(float current, float max)
        {
            if (healthSlider == null || healthText == null) return;
            
            healthSlider.maxValue = max;
            healthSlider.value = current;
            healthText.text = $"{Mathf.CeilToInt(current)}/{Mathf.CeilToInt(max)}";
            
            // Color based on health percentage
            float percent = current / max;
            if (percent > 0.6f)
            {
                healthFillImage.color = Color.green;
            }
            else if (percent > 0.3f)
            {
                healthFillImage.color = Color.yellow;
            }
            else
            {
                healthFillImage.color = Color.red;
            }
        }
        
        #endregion
        
        #region Abilities & Cooldowns
        
        private void UpdateHeroAbilities()
        {
            if (_localHero == null) return;
            
            UpdateCooldownOverlay(skill1Cooldown, _localHero.GetRemainingCooldown(AbilityType.Skill1), 8f);
            UpdateCooldownOverlay(skill2Cooldown, _localHero.GetRemainingCooldown(AbilityType.Skill2), 12f);
            UpdateCooldownOverlay(ultimateCooldown, _localHero.GetRemainingCooldown(AbilityType.Ultimate), 30f);
            
            // Update button interactability
            if (skill1Button != null)
                skill1Button.interactable = _localHero.IsAbilityReady(AbilityType.Skill1);
            
            if (skill2Button != null)
                skill2Button.interactable = _localHero.IsAbilityReady(AbilityType.Skill2);
            
            if (ultimateButton != null)
                ultimateButton.interactable = _localHero.IsAbilityReady(AbilityType.Ultimate);
        }
        
        private void UpdateCooldownOverlay(Image overlay, float remaining, float total)
        {
            if (overlay == null) return;
            
            if (remaining > 0f && total > 0f)
            {
                overlay.enabled = true;
                overlay.fillAmount = remaining / total;
            }
            else
            {
                overlay.enabled = false;
            }
        }
        
        #endregion
        
        #region Match Info
        
        private void HandleMatchTimerUpdated(float timer)
        {
            if (matchTimerText == null) return;
            
            int minutes = Mathf.FloorToInt(timer / 60f);
            int seconds = Mathf.FloorToInt(timer % 60f);
            matchTimerText.text = $"{minutes:00}:{seconds:00}";
        }
        
        public void UpdateScore(int team1Score, int team2Score)
        {
            if (teamScoreText == null) return;
            teamScoreText.text = $"{team1Score} - {team2Score}";
        }
        
        #endregion
        
        #region Kill Feed
        
        private void HandleKillOccurred(Hero victim, Hero killer)
        {
            if (killFeedContainer == null || killFeedItemPrefab == null) return;
            
            AddKillFeedEntry(killer, victim);
        }
        
        private void AddKillFeedEntry(Hero killer, Hero victim)
        {
            // Create kill feed item
            GameObject item = Instantiate(killFeedItemPrefab, killFeedContainer);
            _killFeedItems.Add(item);
            
            // Setup item text
            Text textComponent = item.GetComponent<Text>();
            if (textComponent == null)
            {
                textComponent = item.AddComponent<Text>();
            }
            
            string killerName = killer != null ? killer.HeroData?.Name ?? "Unknown" : "Unknown";
            string victimName = victim != null ? victim.HeroData?.Name ?? "Unknown" : "Unknown";
            
            textComponent.text = $"<color=#00ff00>{killerName}</color> eliminated <color=#ff0000>{victimName}</color>";
            textComponent.alignment = TextAnchor.MiddleRight;
            textComponent.fontSize = 14;
            
            // Limit items
            while (_killFeedItems.Count > maxKillFeedItems)
            {
                GameObject oldItem = _killFeedItems[0];
                _killFeedItems.RemoveAt(0);
                Destroy(oldItem);
            }
            
            // Auto-remove after delay
            Destroy(item, 5f);
        }
        
        #endregion
        
        #region Hero Death
        
        private void HandleHeroDied(Hero hero)
        {
            if (hero != _localHero) return;
            
            // Show death screen or spectator mode
            Debug.Log("You died! Entering spectator mode...");
            
            // Disable HUD elements
            SetHUDActive(false);
        }
        
        #endregion
        
        #region Network Info
        
        private void UpdateFPSCounter()
        {
            if (fpsText == null) return;
            
            float fps = 1f / Time.unscaledDeltaTime;
            fpsText.text = $"FPS: {Mathf.CeilToInt(fps)}";
            
            // Color based on FPS
            if (fps >= 55f)
            {
                fpsText.color = Color.green;
            }
            else if (fps >= 30f)
            {
                fpsText.color = Color.yellow;
            }
            else
            {
                fpsText.color = Color.red;
            }
        }
        
        public void UpdatePing(int pingMs)
        {
            if (pingText == null) return;
            
            pingText.text = $"{pingMs}ms";
            
            // Color based on ping
            if (pingMs < 50)
            {
                pingText.color = Color.green;
            }
            else if (pingMs < 100)
            {
                pingText.color = Color.yellow;
            }
            else
            {
                pingText.color = Color.red;
            }
        }
        
        #endregion
        
        #region Minimap
        
        private void UpdateMinimapPosition(Vector3 playerPosition)
        {
            if (minimapPlayerIcon == null || minimapCamera == null) return;
            
            // Convert world position to minimap UV coordinates
            // This is simplified - actual implementation would use render texture
            Vector3 viewportPos = minimapCamera.WorldToViewportPoint(playerPosition);
            minimapPlayerIcon.anchoredPosition = new Vector2(
                (viewportPos.x - 0.5f) * 200f,
                (viewportPos.y - 0.5f) * 200f
            );
        }
        
        #endregion
        
        #region Utility
        
        private void SetHUDActive(bool active)
        {
            gameObject.SetActive(active);
        }
        
        public void ShowPingWheel()
        {
            OnPingActivated?.Invoke();
            // Show radial ping menu
        }
        
        #endregion
    }
}
