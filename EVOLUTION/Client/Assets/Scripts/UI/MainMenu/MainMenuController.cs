using UnityEngine;
using UnityEngine.UI;
using Shared.Enums;
using Shared.Models;
using System.Collections.Generic;

namespace EVOLUTION.UI.MainMenu
{
    /// <summary>
    /// Main Menu UI Controller
    /// Handles navigation between menu screens, hero selection, and game mode selection
    /// </summary>
    public class MainMenuController : MonoBehaviour
    {
        [Header("UI Panels")]
        [SerializeField] private GameObject mainPanel;
        [SerializeField] private GameObject heroSelectionPanel;
        [SerializeField] private GameObject gameModePanel;
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private GameObject profilePanel;
        
        [Header("Buttons")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button heroesButton;
        [SerializeField] private Button shopButton;
        [SerializeField] private Button battlePassButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button profileButton;
        [SerializeField] private Button backButton;
        
        [Header("Hero Selection")]
        [SerializeField] private Transform heroListContainer;
        [SerializeField] private GameObject heroListItemPrefab;
        [SerializeField] private Image heroPortraitImage;
        [SerializeField] private Text heroNameText;
        [SerializeField] private Text heroDescriptionText;
        [SerializeField] private Button selectHeroButton;
        [SerializeField] private Button lockInfoButton;
        
        [Header("Game Mode")]
        [SerializeField] private Button battleRoyaleButton;
        [SerializeField] private Button captureTheFlagButton;
        [SerializeField] private Button siegeButton;
        [SerializeField] private Button titanTrialButton;
        
        // State
        private List<HeroData> _availableHeroes;
        private HeroData _selectedHero;
        private PlayerData _playerData;
        private GameMode _selectedGameMode;
        
        // Events
        public event System.Action<GameMode, HeroData> OnPlayClicked;
        public event System.Action<HeroData> OnHeroSelected;
        public event System.Action OnSettingsOpened;
        public event System.Action OnProfileOpened;
        
        private void Awake()
        {
            SetupButtonListeners();
        }
        
        private void Start()
        {
            InitializePlayerData();
            LoadAvailableHeroes();
            ShowMainPanel();
        }
        
        #region Initialization
        
        private void SetupButtonListeners()
        {
            if (playButton != null) playButton.onClick.AddListener(OnPlayButtonClick);
            if (heroesButton != null) heroesButton.onClick.AddListener(OnHeroesButtonClick);
            if (shopButton != null) shopButton.onClick.AddListener(OnShopButtonClick);
            if (battlePassButton != null) battlePassButton.onClick.AddListener(OnBattlePassButtonClick);
            if (settingsButton != null) settingsButton.onClick.AddListener(OnSettingsButtonClick);
            if (profileButton != null) profileButton.onClick.AddListener(OnProfileButtonClick);
            if (backButton != null) backButton.onClick.AddListener(OnBackButtonClick);
            
            if (selectHeroButton != null) selectHeroButton.onClick.AddListener(OnSelectHeroClick);
            
            if (battleRoyaleButton != null) battleRoyaleButton.onClick.AddListener(() => SelectGameMode(GameMode.BattleRoyale));
            if (captureTheFlagButton != null) captureTheFlagButton.onClick.AddListener(() => SelectGameMode(GameMode.CaptureTheFlag));
            if (siegeButton != null) siegeButton.onClick.AddListener(() => SelectGameMode(GameMode.Siege));
            if (titanTrialButton != null) titanTrialButton.onClick.AddListener(() => SelectGameMode(GameMode.TitanTrial));
        }
        
        private void InitializePlayerData()
        {
            // Load player data from local storage or server
            _playerData = new PlayerData
            {
                Id = "player_" + System.Guid.NewGuid().ToString().Substring(0, 8),
                Name = "Player",
                Level = 1,
                XP = 0,
                CurrencyCells = 500,
                CurrencyGems = 100
            };
        }
        
        private void LoadAvailableHeroes()
        {
            _availableHeroes = new List<HeroData>();
            
            // Load starter heroes (free)
            _availableHeroes.Add(CreateStarterHero("nova", "Nova", "Cosmic Defender"));
            _availableHeroes.Add(CreateStarterHero("titan", "Titan", "Iron Guardian"));
            _availableHeroes.Add(CreateStarterHero("zephyr", "Zephyr", "Wind Assassin"));
            
            // Add locked heroes based on player progression
            // These would be unlocked through chests or purchases
        }
        
        private HeroData CreateStarterHero(string id, string name, string title)
        {
            return new HeroData
            {
                Id = id,
                Name = name,
                Title = title,
                Rarity = HeroRarity.Starter,
                IsUnlocked = true,
                Level = 1,
                XP = 0,
                Trophies = 0,
                BaseStats = new HeroStats
                {
                    MaxHealth = 1000f,
                    MoveSpeed = 6f,
                    AttackDamage = 100f,
                    Armor = 10f
                }
            };
        }
        
        #endregion
        
        #region Panel Navigation
        
        private void ShowMainPanel()
        {
            HideAllPanels();
            if (mainPanel != null) mainPanel.SetActive(true);
            if (backButton != null) backButton.gameObject.SetActive(false);
        }
        
        private void ShowHeroSelectionPanel()
        {
            HideAllPanels();
            if (heroSelectionPanel != null) heroSelectionPanel.SetActive(true);
            if (backButton != null) backButton.gameObject.SetActive(true);
            
            PopulateHeroList();
        }
        
        private void ShowGameModePanel()
        {
            HideAllPanels();
            if (gameModePanel != null) gameModePanel.SetActive(true);
            if (backButton != null) backButton.gameObject.SetActive(true);
        }
        
        private void ShowSettingsPanel()
        {
            HideAllPanels();
            if (settingsPanel != null) settingsPanel.SetActive(true);
            if (backButton != null) backButton.gameObject.SetActive(true);
            
            OnSettingsOpened?.Invoke();
        }
        
        private void ShowProfilePanel()
        {
            HideAllPanels();
            if (profilePanel != null) profilePanel.SetActive(true);
            if (backButton != null) backButton.gameObject.SetActive(true);
            
            UpdateProfileUI();
            OnProfileOpened?.Invoke();
        }
        
        private void HideAllPanels()
        {
            if (mainPanel != null) mainPanel.SetActive(false);
            if (heroSelectionPanel != null) heroSelectionPanel.SetActive(false);
            if (gameModePanel != null) gameModePanel.SetActive(false);
            if (settingsPanel != null) settingsPanel.SetActive(false);
            if (profilePanel != null) profilePanel.SetActive(false);
        }
        
        #endregion
        
        #region Button Handlers
        
        private void OnPlayButtonClick()
        {
            ShowGameModePanel();
        }
        
        private void OnHeroesButtonClick()
        {
            ShowHeroSelectionPanel();
        }
        
        private void OnShopButtonClick()
        {
            Debug.Log("Opening shop...");
            // TODO: Open shop panel
        }
        
        private void OnBattlePassButtonClick()
        {
            Debug.Log("Opening Battle Pass...");
            // TODO: Open Battle Pass panel
        }
        
        private void OnSettingsButtonClick()
        {
            ShowSettingsPanel();
        }
        
        private void OnProfileButtonClick()
        {
            ShowProfilePanel();
        }
        
        private void OnBackButtonClick()
        {
            ShowMainPanel();
        }
        
        private void OnSelectHeroClick()
        {
            if (_selectedHero != null)
            {
                OnHeroSelected?.Invoke(_selectedHero);
                Debug.Log($"Selected hero: {_selectedHero.Name}");
            }
        }
        
        #endregion
        
        #region Hero Selection
        
        private void PopulateHeroList()
        {
            if (heroListContainer == null || heroListItemPrefab == null) return;
            
            // Clear existing items
            foreach (Transform child in heroListContainer)
            {
                Destroy(child.gameObject);
            }
            
            // Create hero list items
            foreach (var hero in _availableHeroes)
            {
                GameObject item = Instantiate(heroListItemPrefab, heroListContainer);
                
                HeroListItem listItem = item.GetComponent<HeroListItem>();
                if (listItem == null)
                {
                    listItem = item.AddComponent<HeroListItem>();
                }
                
                listItem.Initialize(hero, OnHeroListItemClick);
            }
            
            // Select first hero by default
            if (_availableHeroes.Count > 0)
            {
                SelectHero(_availableHeroes[0]);
            }
        }
        
        private void OnHeroListItemClick(HeroData hero)
        {
            SelectHero(hero);
        }
        
        private void SelectHero(HeroData hero)
        {
            _selectedHero = hero;
            
            // Update hero details UI
            if (heroPortraitImage != null)
            {
                // Load portrait sprite (would be from Resources in production)
                Debug.Log($"Loading portrait for {hero.Name}");
            }
            
            if (heroNameText != null)
            {
                heroNameText.text = $"{hero.Name}\n<size=60%>{hero.Title}</size>";
            }
            
            if (heroDescriptionText != null)
            {
                heroDescriptionText.text = hero.Lore;
            }
            
            // Update lock button state
            if (lockInfoButton != null)
            {
                lockInfoButton.interactable = !hero.IsUnlocked;
            }
            
            OnHeroSelected?.Invoke(hero);
        }
        
        #endregion
        
        #region Game Mode Selection
        
        private void SelectGameMode(GameMode mode)
        {
            _selectedGameMode = mode;
            Debug.Log($"Selected game mode: {mode}");
            
            // Start matchmaking
            StartMatchmaking();
        }
        
        private void StartMatchmaking()
        {
            Debug.Log($"Starting matchmaking for {_selectedGameMode} with {_selectedHero?.Name ?? "No hero"}");
            
            // Notify game to start matchmaking
            OnPlayClicked?.Invoke(_selectedGameMode, _selectedHero);
        }
        
        #endregion
        
        #region Profile
        
        private void UpdateProfileUI()
        {
            // Update profile UI with player stats
            // This would show level, trophies, rank, etc.
            Debug.Log($"Updating profile for {_playerData?.Name}");
        }
        
        #endregion
        
        #region Public API
        
        public void SetPlayerData(PlayerData data)
        {
            _playerData = data;
        }
        
        public PlayerData GetPlayerData() => _playerData;
        public HeroData GetSelectedHero() => _selectedHero;
        public GameMode GetSelectedGameMode() => _selectedGameMode;
        
        #endregion
    }
    
    /// <summary>
    /// Hero list item component for the hero selection list
    /// </summary>
    public class HeroListItem : MonoBehaviour
    {
        [SerializeField] private Image heroIcon;
        [SerializeField] private Text heroName;
        [SerializeField] private Image rarityIndicator;
        [SerializeField] private GameObject lockIcon;
        
        private HeroData _heroData;
        private System.Action<HeroData> _onClick;
        
        public void Initialize(HeroData hero, System.Action<HeroData> onClick)
        {
            _heroData = hero;
            _onClick = onClick;
            
            if (heroName != null)
            {
                heroName.text = hero.Name;
            }
            
            if (lockIcon != null)
            {
                lockIcon.SetActive(!hero.IsUnlocked);
            }
            
            // Set rarity color
            if (rarityIndicator != null)
            {
                rarityIndicator.color = GetRarityColor(hero.Rarity);
            }
            
            // Add click listener
            var button = GetComponent<Button>();
            if (button == null)
            {
                button = gameObject.AddComponent<Button>();
            }
            button.onClick.AddListener(() => _onClick?.Invoke(_heroData));
        }
        
        private Color GetRarityColor(HeroRarity rarity)
        {
            return rarity switch
            {
                HeroRarity.Starter => Color.white,
                HeroRarity.Rare => new Color(0f, 0.5f, 1f),
                HeroRarity.Epic => new Color(0.6f, 0f, 1f),
                HeroRarity.Legendary => new Color(1f, 0.8f, 0f),
                HeroRarity.Unique => new Color(1f, 0f, 0.5f),
                _ => Color.white
            };
        }
    }
}
