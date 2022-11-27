## Overview

Newly Hatched Zoea is difficult to obtain and suffers from being incredibly underwhelming compared to when you get it and what it takes.
The goal of this mod is to make the item more exciting and interesting to match its scarcity.
There's a choice for a buff which improves the item but keeps it close to the original function, and there's a rework which aims to be a more radical change.

Configs are provided to tweak most of the changes.

## Buff

```
- Only corrupts Queen's Glands.
- Cannot be inherited. (Engineer turrets, etc)
- Summons every 30 seconds instead of 60 (-50% per stack) seconds.
- Can have up to 3 allies at once, instead of 1 (+1 per stack) allies.
- Summoned creature is now determined by a fixed list, instead of a random chance with Void Devastators being the rarest.
- Summons can teleport back to their owner if they're too far away.
- Summoned Void Reavers will sprint towards their target and detonate if their health is low.
- Stacking increases the damage and health of the summons, instead of the reducing the summon cooldown and raising the total summons.
```

This buff aims to improve the base strength of the Newly Hatched Zoea.
One way it does this is by taking some of the rng away from what Void creature is summoned, this smooths out the average strength of the item.
Stacking improves the quality of the summons rather than the quantity, this is to avoid hitting the ally cap.
Void Reavers were given a suicide attack to make them more useful as they're incredibly weak compared to the Jailer and Devastator.

## Rework

```
- Corrupts "boss" items. (By default all vanilla yellow items that drop from bosses.)
- Cannot be inherited. (Engineer turrets, etc)
- Summons a single Void Devastator, instead of multiple random Void creatures.
- The Void Devastator can teleport to its owner if it's too far away.
- Stacking increases the Void Devastator's damage instead of the total summon count.
- The Void Devastator inherits their owner's items.
```

This item is now about having a single powerful minion, that inherits your items.
I'm personally against allowing item summons to inherit items, but I believe this item is special enough to allow it.

## Known Issues

- The teleport's visual effects might be out of sync for clients in some situations. (I'm guessing this exists, it might not.)
- Void Devastators in the rework will drop an item if its Umbral owner is killed first, this is a "feature" that happens in vanilla with Engineer Turrets.
- Void Reavers are clearly not designed to fight anything that can avoid floor.

## Change Log

**1.1.7**

```
- Fixed item inheritance breaking with Tesla Trooper installed.

Allies:
- Changed "Base Recall Distance" default configuration. (120 -> 130)

Void Reaver Ally:
- Changed "Recall Cooldown" default value. (15 -> 20)

Rework:
- Added "Base Damage" and "Base Health" configurations, controls the base health and damage of the Void Devastator.
- Changed "Stack Health Bonus" default configuration. (0 -> 1)
```

**1.1.6**

```
- Removed code that was made redundant in Patch 1.2.4.
```

**1.1.5**

```
General:
- Summons are unaffected by Swarms.
- Summons now ignore the ally limit.

Rework:
- Changed "Stack Damage Bonus" default configuration. (3 -> 2)
```

**1.1.4**

```
General:
- Void allies are no longer summoned in the Bazaar.

Allies:
- Changed "Recall Distance Scaler" default value. (3 -> 4)

Buff:
- Fixed leash distance not being updated.
```

**1.1.3**

```
Allies:
- Fixed their teleport sending them to the centre of the map if their owner is in an invalid spot.
- Teleport animation speed doubled.
- The distance at which the AI will use the recall ability now scales with the current difficulty.
- Added "AI Shared" configuration category.
- Added "Base Recall Distance" configuration, controls the minimum distance the AI will use its recall ability.
- Added "Max Recall Distance" configuration, caps the distance the AI will use its recall ability.
- Added "Recall Distance Scaler" configuration, scales the distance the AI will use its recall ability based on the run's difficulty.
```

**1.1.2**

```
Rework:
- Added a seperate configuration section for item inheritance called "Rework Inheritance".
- Added "Item Blacklist" configuration, allows you to prevent the Void Devastator from gaining specific items.
- Added "Tier Blacklist" configuration, allows you to prevent the Void Devastator from gaining items from a specific tier.
- Added "Golden Knurl", "Guardian's Eye" and "Titanic Great Sword" from Skell's GoldenCoastPlus to the "Corrupt List" configuration.
```

**1.1.1**

_Changed the corruptions list and made them configurable for compatability. Made the rework by default take any yellow item that is intended to drop from a boss, also reduced its stacking power to compensate for this._

```
Rework:
- Added "Corrupt List" configuration, allows you to set a list of items that the Newly Hatched Zoea can corrupt.
- Fixed item displays requiring the rework to be disabled.
- Changed "Stack Damage Bonus" default value. (4 -> 3)

Buff:
- Added "Corrupt List" configuration, allows you to set a list of items that the Newly Hatched Zoea can corrupt.

Void Reaver Ally:
- Changed "Recall Cooldown" default value. (25 -> 15)
- Changed "Base Movement Speed" default value. (6 -> 8)
```

**1.1.0**

```
Configs:
- Recategorise all configurations, be sure to reset yours.
- Changed "Stack Damage Bonus" default value. (10 -> 4) (Was actually meant to be 5).

Other:
- Added a choice between buffing the item's original functionality or using this rework.
- Now uses a unique language token for the pickup and detailed descriptions, to prevent other mods from changing it.
- Added a few more deployable checks to help prevent too many allies from spawning.
```

**1.0.0**

````
- Public release.
````

## Credits

* **Conq** - Starting a thread to buff the Newly Hatched Zoea.
* **Atti, Jino and Kai** - Contributing to Conq's thread.

## Contact

You can find me as kking117#0370 on discord.