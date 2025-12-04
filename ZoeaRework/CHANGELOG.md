**1.2.0**

Updated for Alloyed Collective, please contact me if there's any problems.

```
- Updated DLLs, API requirements and code to function correctly with Alloyed Collective.
- Small config changes that I can't recall.
- Void allies won't spawn in the Computational Exchange, similar to the Bazaar.
- Probably broke something in the process.
```

**1.1.8**

```
- Added additional checks to help avoid a null reference for OnInventoryChanged.
```

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