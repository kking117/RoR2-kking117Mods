## QueenGlandBuff

The goal of this mod is to buff Beetle Guards spawned by the Queen's Gland so they provide more than just emotional support.

**Skill Changes**

The biggest issue with Beetle Guards is their inability to fight an aerial enemy, to counter this its attacks now launch projectiles.
These extra projectiles also make Beetle Guards deal a lot of extra damage so their damage has been reduced, however the additional projectiles make it higher than before.

<a href="https://imgur.com/wWG1E1P"><img src="https://i.imgur.com/wWG1E1P.png" title="source: imgur.com" /></a>

For its Slam skill it fires out 5 projectiles evenly upwards. This is mostly for crowds and flying enemies that are just out of range.

<a href="https://imgur.com/beAFC77"><img src="https://i.imgur.com/beAFC77.png" title="source: imgur.com" /></a>

Its Sunder skill now fires 5 projectiles in the direction the Guard is aiming. This allows them to hit flying enemies.

**AI Changes**

The AI now prefers to use Sunder over Slam, this is so it doesn't get locked into using Slam against flying enemies that are slightly out of reach.
The Beetle Guard will often target nearby grounded enemies over flying ones.

**Item/Stacking Changes**

Each player can now only have 3 Beetle Guards each, further stacks increase their health and damage.
The 30 seconds respawn timer now actually functions but is reduced to 20 seconds.
Beetle Guards spawn with an elite affix for some extra spice.

**Extra Skills**

The mod also adds 2 new skills to the Beetle Guard to make them useful beyond damage.

A new Utility called "Recall". This skill teleports the user back to their owner, the AI uses this when it's beyond the leash length. This ability is useful for those situations where the Beetle Guard is incapable of navigating back to their owner.

A new Special called "Staunch". This skill gives the user a buff for 10 seconds that forces nearby enemy AI to target it, also during this nearby friendly Beetle Guards gain increased stats. This ability helps keep Beetle Guards useful even after their damage has fallen off.

## Known Issues

- None for now?

## Change Log

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

## Credits & Special Thanks

* **Rob** - HenryMod tutorial and most of the structure being based on it.
* **RoR2 Modding Discord** - For helping me figure out how to make a ContentPack work.
* **StandaloneAncientScepter Devs** - Code used to change the deployable limits is based on their Turret changes.

And probably other people that I can't remember.

## Contact

You can find me as kking117#0370 on discord.