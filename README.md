# BATTERY RUN: THE LOST ENERGY ISLAND
### A Complete 3D Adventure & Exploration Unity Game

**Developer:** Antigravity Studios  
**Game Engine:** Unity 3D (Compatible with 2021.3 LTS, 2022.3 LTS, Unity 6)  
**Target Platform:** Windows PC (1920 × 1080)  
**Language:** C#  

---

## 1. GAME STORY & LORE

You control **BOLT**, a cute and resilient futuristic robot who crash-lands on the uncharted and mysterious **Energy Island**. 

The island's ancient central generator has shut down, leaving the island unstable and plunging its security systems into rogue protocols. BOLT’s onboard power cells are depleting over time. To survive and escape:
1. Traverse the **Lost Island** beaches and cliffs.
2. Navigate the **Dangerous Jungle** across moving platforms, avoiding rogue patrol drones and rolling boulders.
3. Descend into the **Crystal Cave**, dodge falling stalactites, and decipher an ancient color-glyph puzzle.
4. Scale the ancient **Final Energy Core** temple, recharge the master generator dais, and restore energy to escape the island!

---

## 2. KEY FEATURES & SYSTEMS

### ⚡ Battery & Energy Mechanics
- **Dynamic Battery Gauge:** Starts at 100% and steadily drains over time (~1.2%/s normal, 1.8x when sprinting).
- **Color-Coded Status:**
  - 🟢 **Green (60% – 100%):** Optimal system performance.
  - 🟡 **Yellow (30% – 59%):** Energy caution level.
  - 🔴 **Red (0% – 29%):** Critical power alert with pulsing warning banner and alarm audio.
- **Depletion = Game Over:** If power hits 0%, BOLT powers down, prompting the Game Over retry screen.
- **Collectibles:**
  - 🔋 **Standard Battery:** Restores **+20% power** and awards **+100 score**.
  - 💎 **Rare Energy Crystal:** Restores **+35% power** and awards **+250 score**.

### 🎮 Smooth Player Controls & Cute Robot Visuals
- **Character Controller Physics:** Smooth acceleration, turning toward movement direction, sprinting (Left Shift), and responsive jumping.
- **Procedural Animated Robot Visual (BOLT):**
  - Futuristic capsule chassis with metallic cyan-and-white finish.
  - Dynamic chest battery gauge that shifts color from green to yellow to red in real-time.
  - Digital visor screen with animated blinking cyan eyes.
  - Blinking antenna beacon orb.
  - Articulated swinging arms, walking legs, and back jetpack booster ignition on jumping!
  - Fully procedural: no missing external FBX rigs or animation controller dependencies.

### 🎥 Collision-Aware 3rd Person Camera
- Smooth orbit camera with mouse sensitivity controls.
- Distance smoothing and obstacle raycast/sphere-cast collision avoidance so the camera never clips through walls or ground geometry.
- Automatic cursor lock during gameplay and unlock on pause or menus.

### 🗺️ Four Distinct Playable Levels
1. **Level 1 – The Lost Island:** Tropical archipelago with golden sand, emerald grass, palm trees, stepping stones across water, 5 batteries, 1 hidden crystal, and an exit beacon.
2. **Level 2 – Dangerous Jungle:** Dense jungle foliage, ravines with moving platforms, rolling boulder hazards, 2 patrolling rogue sentry drones, 8 batteries, and 2 crystals.
3. **Level 3 – Crystal Cave:** Glowing underground cavern with blue/purple crystals, falling ceiling stalactites, 10 batteries, 3 crystals, and a 3-pedestal color sequence puzzle (Red → Blue → Green) to open the energy gateway.
4. **Level 4 – The Final Energy Core:** Ancient sci-fi energy temple with floating obsidian dais over an energy void, moving platforms, rogue sentinels, 12 batteries, 4 crystals, and the central Grand Energy Core Generator!

### 🏆 Scoring & Persistent Save System
- Score breakdown:
  - Battery collected: **+100 pts**
  - Rare crystal found: **+250 pts**
  - Objective completed: **+300 pts**
  - Finish level: **+500 pts**
  - Finish without dying (Deathless bonus): **+200 bonus pts**
  - Finish with >50% battery remaining: **+200 bonus pts**
- **1 to 3 Star Performance Rating** calculated per level based on collectibles, health, and survival.
- **Unity PlayerPrefs Save System:** Automatically records level high scores, unlocked levels, star achievements, cumulative score, and volume preferences.

### 🎵 Rich Sound & Music System
- Real 16-bit 44.1kHz PCM `.wav` audio files included:
  - 7 Musical Themes: Main Menu, Level 1, Level 2, Level 3, Level 4, Victory Fanfare, and Game Over.
  - 11 Sound Effects: Walking, Jumping, Battery Pickup, Crystal Pickup, Low Battery Warning, Button Click, Door Unlock, Obstacle Hit, Enemy Alert, Level Complete, and Power Down.
- Runtime procedural audio synthesizer fallback (`ProceduralAudioGenerator.cs`) ensures music and SFX never fail even if files are relocated.
- In-game Settings menu with sliders for Master Volume, Music Volume, SFX Volume, and Mute toggle.

---

## 3. HOW TO OPEN & RUN IN UNITY

1. Extract `Battery_Run_The_Lost_Energy_Island.zip`.
2. Open **Unity Hub**.
3. Click **Add > Add project from disk** and select the extracted folder `Battery_Run_The_Lost_Energy_Island`.
4. Open the project with any Unity version (2021.3 LTS, 2022.3 LTS, or Unity 6).
5. In the Project window, navigate to `Assets/Scenes/` and double-click `MainMenu.unity`.
6. Press the **Play ▶** button in the Unity Editor toolbar!
7. Alternatively, use the top menu bar shortcut:
   - Click **Battery Run > Play Game** to launch immediately.
   - Click **Battery Run > Open Level 1 / 2 / 3 / 4** to test any level directly!

---

## 4. PROJECT STRUCTURE

```
Battery_Run_The_Lost_Energy_Island/
├── Assets/
│   ├── Scenes/
│   │   ├── MainMenu.unity
│   │   ├── Level1_LostIsland.unity
│   │   ├── Level2_DangerousJungle.unity
│   │   ├── Level3_CrystalCave.unity
│   │   └── Level4_FinalCore.unity
│   ├── Scripts/
│   │   ├── Player/ (PlayerController, BatterySystem, PlayerVisualRobot)
│   │   ├── Camera/ (ThirdPersonCameraController)
│   │   ├── Collectibles/ (CollectibleItem, BatteryPickup, CrystalPickup)
│   │   ├── Managers/ (GameManager, LevelManager, ScoreManager, CheckpointManager)
│   │   ├── UI/ (UIManager, HUDController, MainMenuUI, PauseMenuUI, LevelCompleteUI, GameOverUI, VictoryUI, SettingsMenuUI)
│   │   ├── Audio/ (SoundManager, ProceduralAudioGenerator)
│   │   ├── Enemies/ (EnemyPatrol, RollingHazard)
│   │   ├── Environment/ (MovingPlatform, FallingRock, ColorPuzzle, EnergyGenerator, Checkpoint, GoalBeacon)
│   │   ├── Levels/ (LevelBuilder, MaterialUtil)
│   │   └── SaveSystem/ (SaveManager)
│   ├── Resources/
│   │   ├── Materials/ (14 stylized standard materials)
│   │   └── Audio/ (Music/ 7 BGM tracks, SFX/ 11 sound effects) - loaded at runtime via Resources
│   └── Editor/ (BatteryRunProjectSetup)
├── Packages/
│   └── manifest.json
├── ProjectSettings/
│   ├── EditorBuildSettings.asset
│   ├── TagManager.asset
│   ├── InputManager.asset
│   ├── AudioManager.asset
│   ├── ProjectSettings.asset
│   └── ProjectVersion.txt
├── README.md
└── HOW_TO_PLAY.txt
```

Enjoy the adventure and save the Lost Energy Island with BOLT!
