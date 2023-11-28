**5.1.0**

````
- Updated dependancies.

- Added "Banner Merging" configurations, allows banners that are too close to merge into a single banner, useful for clarity and getting more value out of banners.

- Removed "Increase Base Attack Speed" configuration.
````

**5.0.2**

````
- Added "Focus Banners" configuration, makes players spawn Warbanners when activating the Focus in Simulacrum.

- "Attack Speed Bonus" now behaves as normal.
- Added "Increase Base Attack Speed" configuration, to revert the above change.
- Changed "Regen Bonus" default value. (4.5 -> 3)
````

**5.0.1**

````
- Fixed buff icon not loading due to the latest patch.
````

**5.0.0**

_This update changes a lot that I really can't be bothered documenting._

````
- Updated for SotV.
- Added "Deep Void Signal Banners" configuration, makes players spawn Warbanners when activating a Deep Void Signal.

- The mod now always creates its own buff, configurations that enabled this behaviour have been removed.
- Attack Speed boost now increases base attack speed. (This means normal attack speed boosts will be multiplied further.)
- Added configuration for a flat Regen bonus.
- "Healing and Recharge" configurations have been changed and are now disabled by default.
- Healing configurations now only heal and cannot be configured as regen.
- "Damage Bonus" configuration now scales with level.
````

**4.1.1**

````
- Fixed a multiplayer issue that prevented banners being place by this mod and generally messed up the events that triggered this error.
````

**4.1.0**

````
- Added basic description changes to the War Banner item.
- Added NetworkServer check to placing additional War Banners. (Should hopefully fix duplicate banners being place by clients.)

- Renamed "Regen" configurations to say "Heal"
- Categorized configs.
````

**4.0.0**

````
- New configuration "Create New Buff" that moves all changes this mod makes to an entirely new buff (Useful for those that use Queen Beetle Plus or EliteVariety).
````

**3.1.0**

````
- Brought back "Attack Speed Bonus" and "Move Speed Bonus" configurations.
- New configuration "Regen is Regen" that makes the healing effect get applied to the target's regen stat instead.
- New configuration "Recharge Interval" to change the duration between recharge ticks.
- "Regen Interval" configuration now only applies to the regen/healing effect.

- Default configuration changes:
- "Moon Pillar Banners" (0.25 -> 0.5)
- "Void Cell Banners" (0.25 -> 0.3)
````

**3.0.0**

````
- Moved back to R2API.
- Uses R2API's stathook.
- Renamed "Recharge Max Health" and "Recharge Level Health" to say "Shield" instead of "Health".
- Removed "Attack Speed Bonus" and "Move Speed Bonus" configurations.
````

**2.0.1**

````
- Oops.
- Removed MMHOOK Standalone dependency. 
- Changed internal version number to what it should be.
````

**2.0.0**

````
- Added configurations for extra attack speed and movement speed.
- Added configurations to spawn War Banners from activating Moon Pillars and Void Cells.
- Configurations for placing banners from Mithrix Phases, Moon Pillars and Void Cells now allow setting the size multiplier of the placed banners instead of just enable/disabling the feature.
- Changed default configurations as I thought they were a bit OP.

- Switched to EngimaticThunder for Anniversery Update compatibility.
- No longer uses TILER2 for now. (This may change in the future.)
````

**1.0.0**

````
- Public release.
````