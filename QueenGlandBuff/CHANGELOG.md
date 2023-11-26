**1.4.0**

```
- Updated dependancies.

- Added slightly more granular configurations for the Beetle Guard's base stats.
- Beetle Guard's health regen is now 5 (+1 per level), the same as mobile drones.

- Added "Elite Skills" configuration to disable the skill changes Beetle Guard's get when Perfected.
- Reduced the amount of rocks thrown from its skills from 5x75% to 3x125%.
- Reduced the proc coefficient of the rock projectiles from 0.5 to 0.3.
- Reduced the blast radius of the rock projectiles from 10.5m to 8m.
- Rock projectiles use Sweetspot falloff instead of linear (should be a damage buff overall).
- Rock projectiles from Sunder now launch in a fan instead of a cone.

- Renamed Staunch to Valor.
- Valor no longer grabs aggro every second, it instead happens when the buff is gained.
- Valor can no longer draw the aggro of bosses, the respective configuration has been removed.
- Valor will always draw aggro instead of being chance based, the respective configuration has been removed.

- Removed the Recall skill, as this should be handled with minion leash changes from other mods.

- Beetle Guards should now be marked as an ally for RiskyMod.
```

**1.3.8**

```
- Fixed "Regen" configuration doing nothing.
- Changed "Regen" default configuration. (3 -> 4)
- Changed "Max Summons" default configuration. (3 -> 1)
- Changed "Respawn Time" default configuration. (20 -> 30)
- Allowed Swarms to double the total summons.
```

**1.3.7**

```
- Changed "Aggro Range" default configuration to match the teleporter radius. (100 -> 60)
- Changed "Aggro Boss Chance" default configuration. (0.025 -> 0)
- Added "Armor" configuration, controls how much armor the Staunch buff gives.
```

**1.3.6**

```
- Removed the Beetle Frenzy buff as it made the difference between 1 or 2 Beetle Guards quite large.
- Removed "Buff Range" configuration, as Beetle Frenzy buff has been removed.
- Removed "Targeting Changes" configuration and behaviour, as they seem to be competent enough without it.
- Removed "Regen" configuration.

- Added an extra configuration category to tweak a few of Beetle Guard Ally's base stats, specifically health, damage and regen.
- Recategorized some configurations and added "Enable Changes" to enable/disable changes to the Queen's Gland item specifically.
- Changed "StackDamage" default configuration. (15 -> 20)
```

**1.3.5**

```
- Small fix (hopefully) to a compatability issue.
```

**1.3.4**

```
General:
- Summons now ignore Swarms.
```

**1.3.3**

```
General:
- Removed excessive debug logging during load.
```

**1.3.2**

```
General:
- Beetle Guards are no longer summoned in the Bazaar.

Staunch Buff:
- Improved how the code selects which allies get the BeetleFrenzy buff.

AI Changes:
- Removed "Leash Length" configuration, added new configurations for greater control over leash distance.
- Added "Base Recall Distance" configuration.
- Added "Max Recall Distance" configuration.
- Added "Recall Distance Scaler" configuration.
```

**1.3.1**

````
General:
- Fixed Staunch and BeetleFrenzy buffs being classed as debuffs.
- Fixed "Aggro Boss Chance" doing effectively nothing.

Queen's Gland:
- Fixed Beetle Guards not being killed off when losing all your Queen's Glands.

Beetle Guard Ally:
- Skill cooldowns are no longer influenced by Attack Speed.
- Recall now fails if their owner isn't on the same team.
- Added back passive regen at 1/s(+0.2/s per level).
````

**1.3.0**

_Just a quick fix to get it running with the new DLC. Give me a yell if the Beetle Guards are spawning as Mending without the DLC._

````
- Updated for SotV
````

**1.2.0**

````
- Added "Regen" configuration, controls the out of combat health regen Beetle Guards are given.

- "Become Elite" configuration now allows for Elite Beetle Guards only during Artifact of Honor.
- Now uses a more accurate method of collecting the elites types it can use.
````

**1.1.1**

_While the Beetle Guards are much more powerful, their reduced durability along with respawn times gave them way too much downtime. These adjustments should improve uptime by allowing them to recover in-between fights._

````
- Beetle Guards now regenerate health outside of combat.

- "BaseHealth" default configuration changed. (0->10)
- "StackHealth" default configuration changed. (5->10)
````

**1.1.0**

````
- Added "Targeting Changes" configuration, toggles the ai changes that makes Beetle Guards go for grounded enemies.
- Added "Respawn Time" configuration, changes the respawn timer for Beetle Guards.

- Actually replaced the SkillDrivers properly.
- "Aggro Boss Chance" default value changed (0.1 -> 0.025)
- Fixed stacking health and damage bonuses not syncing for clients.
- "StackHealth" configuration now uses integers default value changed (0.25 -> 5)
- "StackDamage" configuration now uses integers default value changed (0.5 -> 15)
- Beetle Frenzy buff no longer stacks, stat increases have been improved to compensate.
- Added a potential fix for buffs being out of sync in multiplayer with certain mods. (If this fails I'll just use R2API for the buffs.)

- Removed "Upgrade Beetle Guard" configuration, was mostly just for messing around.
````

**1.0.0**

````
- First public release.
````