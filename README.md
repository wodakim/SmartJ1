# Entropy Syndicate (Unity 6 LTS Mobile F2P Framework)

A production-oriented Unity architecture for a real-time “rule manipulation” arena game where players place **Rule Shards** that alter local physics behavior instead of controlling a character directly.

## Folder Tree

```text
Assets/
  Scenes/
    Main.unity
  Scripts/
    Core/
      GameBootstrapper.cs
      GameStateMachine.cs
      ServiceRegistry.cs
    Gameplay/
      ChaosEntity.cs
      EnergySystem.cs
      EntropyEngine.cs
      RunController.cs
      ShardExecutionPipeline.cs
      SpawnDirector.cs
      TimeModifierEngine.cs
    Meta/
      ForgeTreePresenter.cs
      MissionPresenter.cs
      SeasonalModifierService.cs
    UI/
      AudioIntensityController.cs
      FloatingTextController.cs
      JuiceController.cs
      ScreenDistortionController.cs
      SettingsController.cs
      UIFlowController.cs
    Data/
      ConfigBootstrapper.cs
      GameBalanceConfig.cs
      GameEnums.cs
      MetaProgressionConfig.cs
      PlayerSaveData.cs
      ShardDefinition.cs
      ShopCatalog.cs
    Services/
      AnalyticsService.cs
      EconomyService.cs
      MissionService.cs
      MonetizationService.cs
      ProgressionService.cs
      SaveService.cs
    Utils/
      CryptoUtils.cs
      DeterministicObjectPool.cs
      SaveDataExtensions.cs
```

## Architecture

- **State-driven flow** via `GameStateMachine` (Boot/Home/Run/GameOver/etc.).
- **Service-based composition** in `GameBootstrapper` with explicit registration (save, economy, analytics, monetization, progression).
- **Gameplay runtime engines** are split:
  - `EntropyEngine`
  - `EnergySystem`
  - `SpawnDirector`
  - `TimeModifierEngine`
  - `ShardExecutionPipeline`
- **No God class**: `RunController` orchestrates systems but logic remains in specialized engines.
- **Scalability/performance-first**:
  - pooled entities (`DeterministicObjectPool`)
  - frame budget pinned to 60 FPS
  - non-LINQ gameplay code
  - minimal update footprint

## Core Loop

Boot → Home → Start Run → Place Shards → Build Entropy/Score → Game Over → Rewards → Meta upgrade → Repeat.

In-run:
Observe chaos → Place shard (energy cost) → manipulate trajectories → raise entropy multiplier → survive escalating spawn pressure.

## Meta Systems

- Account level
- Shard mastery slots in save data
- Forge tree framework with 20 default nodes (generated via `ConfigBootstrapper`)
- Daily/Weekly missions (10 daily + 6 weekly in bootstrap generator)
- Recalibration prestige (`ProgressionService.PrestigeRecalibration`)
- Seasonal modifier readiness (`SeasonalModifierService`)
- Battle Pass data structure (`MetaProgressionConfig.BattlePassTier`)

## Monetization Flow (Ethical, non-P2W)

Implemented hooks:
1. Rewarded ads
   - Double end-run rewards
   - Instant energy refill (capped per run)
   - Revive once flow hook
2. IAP framework
   - Starter pack / premium packs / cosmetics / remove ads via `ShopCatalog`
3. Battle Pass framework
   - mission XP + free/premium reward structure

No direct cash-to-power stat multipliers are provided.

## Security

- Save payload protected with base64 wrapper and digest validation (`CryptoUtils` + `SaveService`).
- Currency writes route through `EconomyService` only.
- Tamper detection flag in save (`tamperDetected`) and leaderboard disable (`leaderboardEnabled=false`) on checksum mismatch.
- Local save blob format: protected JSON payload with digest.

## Balancing Logic

`GameBalanceConfig` controls:
- spawn acceleration curves
- entropy gain/decay
- difficulty weighting (time + entropy + stacks)
- drop rate curve
- energy regen scaling by run duration

Use ScriptableObject values to tune without code edits.

## How to Add New Shards

1. Create a `ShardDefinition` asset.
2. Add a new enum value to `ShardType`.
3. Add behavior mapping in `ShardExecutionPipeline.ApplyEffect`.
4. Add the definition to `RunController.shardDefinitions` list.
5. Optional: add cosmetics and unlock rules in meta config.

## Scene Setup (Playable)

The repository includes `Assets/Scenes/Main.unity` as a starter playable scene. In Unity Editor:
1. Add `GameBootstrapper` component to the bootstrap GameObject.
2. Create and assign `GameBalanceConfig` + `MetaProgressionConfig` assets.
3. Add `RunController`, `UIFlowController`, pool prefab, and UI references.
4. Wire Run HUD sliders and score label.
5. Press Play.

## Analytics Events

Recorded locally:
- `session_start`
- `session_end`
- `run_start`
- `run_end`
- `shard_used`
- `entropy_peak`
- `ad_shown`
- `ad_watched`
- `purchase_triggered`
- `prestige_used`

Export via `AnalyticsService.Export()` (hooked to UI debug action).
