# Fixes & Test Report

Verification was static (code review, namespace resolution check, brace balance, jump-reachability simulation).
Unity itself was not available, so please do one play-through in the Editor (see checklist at the bottom).

## Bugs fixed
| # | Problem | Fix |
|---|---|---|
| 1 | Compile error: `ColorPuzzle` not found in LevelBuilder.cs (project would not build) | Qualified as `Environment.ColorPuzzle` |
| 2 | `Arial.ttf` built-in font throws in Unity 2022.3+/6 -> all UI fails | `MaterialUtil.GetUIFont()` tries LegacyRuntime.ttf first |
| 3 | Squashed cylinder colliders become spheres: Level 1 stone battery unreachable, Level 4 goal sealed inside a dome | Convex MeshColliders (`FixCylinderCollider`) |
| 4 | Level 4 core platform unreachable even when sprinting | Added two bridge pads |
| 5 | Level 2 final gap only crossable by sprinting; middle platform tight | Added moving platform, re-centred middle one |
| 6 | Level 3 gate only 10m wide in a 24m cave (puzzle bypassable) | Gate spans whole cave |
| 7 | HUD hidden / pause dead / wrong music after Menu -> Play (GameManager persists, Start runs once) | `sceneLoaded` hook |
| 8 | Score leaked between levels and retries | Reset at level start |
| 9 | Moving platforms did not carry the player | Platform riding in PlayerController |
| 10 | Rolling boulders (tag Hazard) caused instant respawn + damage | Tag changed to Enemy |
| 11 | No kill plane (Level 1/3 edges = endless fall) | Respawn below y=-40 |
| 12 | Main-menu camera had no AudioListener (silent menu) | Added |
| 13 | Robot visual built twice (duplicate model) | Guard in BuildProceduralRobot |
| 14 | Battery bar never shrank (Image fill needs a sprite) | White sprite assigned |
| 15 | Real WAV files never loaded (not in Resources, SFX_ prefix mismatch) | Moved to Resources, aliases added, double-volume fixed |
| 16 | Shader.Find("Standard") null in builds | Materials in Resources + fallback chain |
| 17 | UI parented with worldPositionStays=true | `SetParent(x, false)` |
| 18 | Goal gave no feedback without all batteries; goal only on enter | Message + OnTriggerStay |
| 19 | Level 4 generator never activated; settings modal stuck after pause; no mute toggle; water had no transparency | Fixed/added |

## Manual play-test checklist
1. Open MainMenu, Play: HUD visible, music plays, ESC pauses.
2. L1: collect 5 batteries (incl. stone one), reach beacon. L2: ride 3 moving platforms. L3: step RED, BLUE, GREEN. L4: tower -> pad -> core.
3. Let battery hit 0: Game Over, Retry works. Level Complete -> Next Level. Settings sliders + Mute.
