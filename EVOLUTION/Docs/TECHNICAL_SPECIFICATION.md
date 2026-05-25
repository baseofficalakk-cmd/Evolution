# EVOLUTION - Game Technical Specification

## Overview
**EVOLUTION** is a MOBA + Hero Shooter + Arena PvP game for PC with 2.5D stylized graphics similar to Brawl Stars.

---

## Engine & Technology Stack

### Primary Choice: Unity 6 LTS + C#
- **Rendering:** Universal Render Pipeline (URP)
- **Networking:** Netcode for GameObjects or Photon Fusion
- **UI:** UI Toolkit + TextMeshPro
- **Input:** New Input System
- **Camera:** Cinemachine

### Alternative: MonoGame + .NET 8
- Custom rendering pipeline
- Lidgren Network or NetCoreServer
- ImGui.NET for UI

---

## Performance Targets

| Metric | Target |
|--------|--------|
| FPS | Stable 60 |
| Timestep | Fixed 0.0166667s |
| Max Active Objects | 300 |
| Network Tick Rate | 60 Hz |
| Load Time | < 5 seconds |
| Memory Usage | < 500 MB |

### Optimization Techniques
- Object pooling for projectiles, VFX
- GPU instancing for decorations
- Texture atlases (2048x2048 max)
- Frustum culling
- Static/dynamic batching
- Async scene loading
- LOD system for heroes

---

## System Requirements

### Minimum
- OS: Windows 10
- CPU: 2 cores @ 1.5 GHz
- RAM: 2 GB
- Storage: 50 MB
- .NET: .NET 8 Runtime
- Resolution: 1024x576

### Recommended
- OS: Windows 10/11
- CPU: 4 cores @ 2.5 GHz
- RAM: 4 GB
- Storage: 100 MB
- .NET: .NET 8 Runtime
- Resolution: 1280x720

---

## Core Gameplay

### Controls
| Action | Input |
|--------|-------|
| Movement | WASD |
| Aim | Mouse cursor |
| Attack | Left Mouse Button |
| Manual Aim | Hold Right Mouse Button |
| Ultimate | Spacebar |
| Ping | Middle Mouse Button (radial menu) |
| Chat | Enter / T |

### Combat Rules
- **LMB without RMB:** Auto-attack (nearest enemy)
- **Hold RMB:** Manual aiming mode (dotted line appears)
- **LMB during RMB:** Aimed attack
- **Auto-aim assist:** Optional toggle in settings

---

## Game Modes

### Battle Royale
- **Players:** 12
- **Teams:** Free-for-all
- **Respawn:** Disabled
- **Mechanic:** Shrinking damage zone
- **Win Condition:** Last player standing
- **Map Size:** Full 2400x1600

### Capture The Flag
- **Players:** 4-6 (2v2 or 3v3)
- **Teams:** Blue vs Red
- **Respawn:** Enabled (10s)
- **Objective:** Capture enemy flag 3 times
- **Win Condition:** Most captures or timeout

### Siege
- **Players:** 10 (5v5)
- **Teams:** Attackers vs Defenders
- **Respawn:** Enabled (10s)
- **Objective:** Attackers capture points
- **Win Condition:** All points captured or timeout

### Titan Trial (Monthly Event)
- **Players:** 5 vs Boss
- **Teams:** Cooperative
- **Respawn:** Enabled (10s)
- **Boss:** Atlas (Unique hero unlock)
- **Win Condition:** Defeat Atlas
- **Reward:** Exclusive hero skin

---

## Map System

### Specifications
- **Dimensions:** 2400 x 1600 units
- **Wall Blocks:** 20 (procedurally placed)
- **Bush Zones:** 15 (stealth areas)
- **Spawn Points:** 12 (team-based or neutral)
- **Decorations:** Procedural (trees, rocks, props)

### Mechanics
- **Bushes:** Hide players from enemies outside
- **Walls:** Block projectiles and movement
- **Destructibles:** Some walls can be destroyed
- **Dynamic Collisions:** Updated on destruction

### Procedural Generation
```json
{
  "seed": "random",
  "wallDensity": 0.3,
  "bushDensity": 0.2,
  "openSpaceRatio": 0.4,
  "symmetricLayout": true,
  "laneCount": 3
}
```

---

## Heroes

### Rarity Distribution
| Rarity | Count | Unlock Method | Price (Cells) |
|--------|-------|---------------|---------------|
| Starter | 3 | Default | 0 |
| Rare | 5 | Chests | 1000 |
| Epic | 4 | Chests | 2500 |
| Legendary | 2 | Chests | 5500 |
| Unique | 1 | Titan Trial | Special |

### Hero Structure
Each hero has:
- Base stats (Health, Speed, Damage, Range, Attack Speed)
- Basic Ability (auto-attack)
- Special Ability (cooldown-based)
- Ultimate Ability (charge-based)
- Passive Ability (always active)
- 3-5 gameplay tips
- Lore description
- Color scheme
- Available skins

### Implemented Heroes (Session 1)
1. **Nova** (Starter) - Cosmic ranged DPS
2. **Titan** (Starter) - Tank with CC
3. **Zephyr** (Starter) - Mobile assassin

---

## Currencies

### Cells 🧬
- Earned from matches, quests, events
- Used to unlock heroes
- Cannot be purchased with real money

### Gems 💎
- Premium currency
- Purchased with real money or earned slowly
- Used for:
  - Chests
  - Cosmetics
  - Battle Pass
  - Special offers

---

## Chest System

### Standard Chest
- **Cost:** 50 Gems
- **Items:** 3-12
- **Drop Rates:**
  - Rare: 50%
  - Epic: 30%
  - Legendary: 5%
  - Unique: 0.5%

### Golden Chest
- **Cost:** 200 Gems
- **Items:** 7-25
- **Drop Rates:**
  - Rare: 75%
  - Epic: 50%
  - Legendary: 7.5%
  - Unique: 3%

### Opening Sequence
1. Cinematic camera zoom
2. Chest shake animation
3. Items revealed one by one
4. Rarity glow effect
5. Sound effects per rarity
6. Particle burst on legendary/unique

---

## Progression Systems

### Hero Level & Mastery
- **Max Level:** 100
- **Max XP:** 30,000
- **XP per Match:** 30 (base)
- **Rewards at milestones:**
  - Sprays (levels 5, 15, 25...)
  - Emotes (levels 10, 20, 30...)
  - Titles (levels 25, 50, 75, 100)
  - Frames (levels 50, 100)
  - Skin variants (level 100)

### Profile Level
- Sum of all hero levels
- Displayed on profile card
- Unlocks global rewards

### Trophy System
- Gain trophies on victory
- Lose trophies on defeat
- Amount depends on rank
- Profile trophies = sum of all hero trophies

### Ranked System
- **Tiers:** Bronze, Silver, Gold, Platinum, Diamond, Master, Grandmaster
- **Seasonal:** 3-month seasons
- **Reset:** 50% trophy retention
- **Features:**
  - Competitive matchmaking
  - Hero bans (3 per team)
  - Leaderboards
  - Seasonal rewards

---

## Multiplayer Architecture

### Network Model
- **Topology:** Client-Server (dedicated)
- **Protocol:** UDP with reliability layer
- **Tick Rate:** 60 Hz
- **Authority:** Server-authoritative

### Features
- Matchmaking (ranked and casual)
- Lag compensation
- Client-side prediction
- Server reconciliation
- Entity interpolation
- Reconnect support
- Basic anti-cheat

### Message Types (64+)
- Connection (Connect, Disconnect, Heartbeat)
- Authentication (Login, Register, Logout)
- Matchmaking (Queue, MatchFound, Cancelled)
- Match Control (Start, End, Join, Leave)
- Gameplay (Input, State, Projectile, Damage, Death, Respawn)
- Chat & Ping
- Progression (XP, LevelUp, Quests)
- Social (Friends, Clan)
- Economy (Purchase, Chest)
- Admin Commands
- Replay Data
- Spectator
- Ban Phase
- Voting
- AFK Detection
- Seasonal Updates
- Tournament

---

## AI Bots

### When Used
- Fill matches when players unavailable
- Tutorial matches
- Practice mode
- Offline play

### Bot Behavior
- Use abilities appropriately
- Intelligent aiming (with accuracy based on difficulty)
- Dodge incoming projectiles
- Use bushes for ambush/escape
- Retreat on low HP
- Follow objectives (CTF, Siege)
- Scale difficulty dynamically

### AI Configuration Per Hero
```json
{
  "aggressionLevel": 0.5,
  "retreatHPThreshold": 0.3,
  "bushUsagePreference": 0.5,
  "ultimateUsagePriority": 0.7,
  "prefersObjectives": true,
  "reactionTime": 0.2,
  "accuracyBase": 0.8,
  "dodgeChance": 0.3,
  "teamPlayScore": 0.6
}
```

---

## UI Systems

### Main Menu
- Play button (mode selection)
- Heroes screen
- Shop/Chests
- Battle Pass
- Profile
- Settings
- Social (Friends, Clan)
- News/Events

### Hero Selection
- Grid of owned heroes
- Hero preview with animation
- Lore and abilities display
- Ban phase integration (ranked)
- Lock-in confirmation
- Team composition display

### HUD (In-Game)
- Minimap
- Health bar
- Ability icons with cooldowns
- Ultimate charge indicator
- Score/KDA
- Chat window
- Ping wheel
- Match timer
- Objective indicators

### Additional Screens
- Chest opening
- Battle Pass progression
- Quest log
- Leaderboards
- Replay browser
- Tournament bracket
- Clan management

---

## Audio System

### SFX Categories
- Attacks (per hero/ability)
- Ultimates (cinematic)
- UI (clicks, notifications)
- Chests (opening, reveals)
- Explosions
- Footsteps (surface-based)
- Announcer voice lines

### Music
- Menu theme
- Battle theme (dynamic intensity)
- Victory fanfare
- Defeat melody
- Event themes

### Volume Controls
- Music: 0-100%
- SFX: 0-100%
- Voice: 0-100%

---

## VFX System

### Effect Types
- Explosions
- Projectile trails
- Healing particles
- Gravity wells
- Electricity arcs
- Fire burns
- Poison clouds
- Shadow effects
- Hit impacts
- Shield bubbles
- Speed trails
- Stealth reveal

### Optimization
- Particle limit: 50 simultaneous
- GPU particles where possible
- LOD for distant effects
- Culling when off-screen

---

## Animation System

### Per-Hero Animations
- Idle (with variations)
- Walk/Run (8-directional)
- Attack (basic and abilities)
- Hurt/Recoil
- Death (multiple variants)
- Ultimate cast
- Victory pose
- Defeat pose
- Emotes (4 slots)

### Technical
- Animator Controller per hero
- Blend trees for movement
- Root motion for abilities
- IK for aiming

---

## Database Schema

### Tables

**Players**
```sql
id (PK), username, email, password_hash, role,
cells, gems, profile_level, total_trophies,
current_rank, rank_trophies, created_at, last_login
```

**Heroes**
```sql
id (PK), player_id (FK), hero_id, level, xp,
trophies, matches_played, matches_won,
unlocked_at, favorite_skin
```

**Matches**
```sql
id (PK), mode, map, start_time, duration,
winning_team, replay_data
```

**Match_Players**
```sql
match_id (FK), player_id (FK), hero_id, team,
kills, deaths, assists, damage_dealt, healing_done,
trophy_change, xp_gained, is_mvp
```

**Friends**
```sql
player_id (FK), friend_id (FK), status, since
```

**Clans**
```sql
id (PK), name, owner_id, created_at, trophies, level
```

**Clan_Members**
```sql
clan_id (FK), player_id (FK), role, joined_at
```

**Quests**
```sql
id (PK), player_id (FK), type, target, progress,
completed, collected, expires_at
```

**Inventory**
```sql
id (PK), player_id (FK), item_type, item_id, acquired_at
```

**Battle_Pass**
```sql
player_id (PK), season, level, xp, is_premium,
claimed_rewards[]
```

---

## Admin Panel

### Access Levels
- Guest (no access)
- Player (no access)
- Moderator (ban/mute)
- Admin (full access except owner)
- Owner (everything)

### Features
- Player management (ban, mute, warn)
- Currency granting
- Item unlocking
- Season management
- Analytics dashboard
- Match control (force end, restart)
- Server logs viewer
- Database queries (read-only)

---

## Daily & Weekly Systems

### Daily Quests
- 3 quests per day
- Reset at 00:00 UTC
- Examples:
  - Win 2 matches
  - Deal 5000 damage
  - Heal 2000 HP
  - Get 10 kills

### Weekly Quests
- 5 quests per week
- Reset on Monday 00:00 UTC
- Larger rewards

### Daily Login Rewards
- 30-day cycle
- Rewards escalate
- Day 7, 14, 21, 30: special rewards
- Streak bonus for consecutive days

---

## Achievements

### Categories
- Hidden achievements
- Hero-specific achievements
- Ranked achievements
- Event achievements
- Mastery achievements
- Social achievements

### Examples
- "First Blood" - Get first kill in a match
- "Legendary" - Get 10 kills without dying
- "Collector" - Own 10 heroes
- "Veteran" - Play 100 matches
- "Champion" - Reach Grandmaster

---

## Battle Pass

### Structure
- Free track (available to all)
- Premium track (requires purchase)
- 50 levels per season
- Season duration: 3 months

### Rewards
- **Free:** Cells, common items, sprays
- **Premium:** Gems, exclusive skins, emotes, frames

### Progression
- Earn BP XP from matches
- Daily/weekly BP missions
- Bonus XP for first win of day

---

## Clans/Guilds

### Features
- Create clan (1000 Cells cost)
- Invite members (max 50)
- Roles: Owner, Officer, Member
- Clan chat
- Clan trophies (sum of member trophies)
- Clan wars (future feature)
- Clan perks (bonus XP, Cells)

---

## Tournament System

### Features
- Scheduled tournaments
- Bracket generation
- Spectator support
- Commentary slot
- Tournament currency/rewards
- Registration system
- Check-in phase

### Formats
- Single elimination
- Double elimination
- Round robin (groups)

---

## Replay System

### Recording
- Record all player inputs
- Record initial state
- Deterministic simulation
- Compressed storage

### Playback
- Watch own matches
- Watch pro matches
- Timeline controls (play, pause, scrub)
- Free camera
- Player perspective toggle
- Speed control (0.5x, 1x, 2x, 4x)

### Storage
- Local replays (unlimited)
- Cloud replays (last 20)
- Max duration: 10 minutes
- File size: ~100 KB per minute

---

## Spectator Mode

### Features
- Follow specific player
- Free camera mode
- UI toggle (clean view)
- Live spectating friends
- Delay option (prevent stream sniping)
- Multiple observer slots

---

## AFK Detection

### Detection Criteria
- No movement for 30 seconds
- No attacks for 30 seconds
- No input for 30 seconds
- Repeated disconnects

### Punishments
- Warning (first offense)
- Temporary queue ban (15 min, 1 hour, 24 hours)
- Trophy penalty (loss not applied)
- Reduced rewards

### Grace Period
- 60 seconds at match start
- Reconnection support (2 minutes)

---

## Settings

### Graphics
- Fullscreen toggle
- Resolution selector
- VSync toggle
- Shadows quality (Low/Medium/High/Off)
- Particles quality
- Anti-aliasing

### Audio
- Music volume slider
- SFX volume slider
- Voice volume slider
- Mute all toggle

### Gameplay
- Auto-aim toggle
- Mini-map size
- HUD scale
- Colorblind mode
- Damage numbers toggle

### Language
- Russian
- English
- (More planned)

---

## Localization

### Structure
- JSON files per language
- Runtime switching
- Key-value pairs
- Support for plurals
- Support for gender

### Files
- `localization_en.json`
- `localization_ru.json`

### Example
```json
{
  "hero.nova.name": "Nova",
  "hero.nova.lore": "A cosmic warrior...",
  "ui.play": "Play",
  "ui.quit": "Quit",
  "notification.level_up": "You reached level {level}!"
}
```

---

## Tutorial

### Match 1: Movement
- WASD to move
- Camera follows player
- Reach the target zone
- Reward: 100 Cells

### Match 2: Combat
- Auto-attack with LMB
- Manual aim with RMB + LMB
- Use ultimate (Space)
- Defeat training bots
- Reward: 100 Gems

### Match 3: Tactics
- Hide in bushes
- Use walls for cover
- Objective-based gameplay
- Reward: Starter Chest

### Completion Reward
- 500 Cells
- 50 Gems
- Tutorial badge

---

## Security

### Anti-Cheat Basics
- Server authority on damage
- Movement validation
- Cooldown enforcement
- Statistics anomaly detection
- Report system

### Account Security
- Password hashing (BCrypt)
- Session tokens
- Email verification
- Two-factor authentication (planned)

---

## Build & Deployment

### Client Build
- Platform: Windows x64
- IL2CPP scripting backend
- .NET Standard 2.1
- Stripping level: High

### Server Build
- .NET 8 console application
- Linux/Windows compatible
- Docker support (planned)

### Update System
- Version checking
- Forced update for major patches
- Hotfix capability
- CDN distribution

---

## Future Roadmap

### Post-Launch
- More heroes (monthly)
- New game modes
- Clan wars
- Enhanced tournament system
- Cross-platform play
- Mobile version (potential)
- Esports integration

---

*This document serves as the technical specification for EVOLUTION development.*
