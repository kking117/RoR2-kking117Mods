**1.22.4**

<details><summary>Notes</summary>Death Mark Rework has been changed completely and the fix to expired pings has been moved to a seperate mod. Most changes made are balancing related with a focus on being more consistent with the game, doing all this as the idea of the mod being complete is within sight, but still a while away.</details>

```
- Knockback Fin:
- Fixed certain flying enemies not going on cooldown once they have stable velocity, such as Flying Pests and Alloy Vultures.
- Made velocity based damage more consistent and the damage cap much more realistic to achieve.

- Deathmark Rework:
- Scrapped since the ping system can be quite frustrating at times.
- Changed to be closer to how Deathmark works but less binary and can be tweaked to work better with modded setups.
- Is now classed as a buff instead of a rework.

- Ignition Tank Rework:
- Added a configuration to make the proc requirement chance based like it was originally. (Default: 0, Originally: 10)

- Leeching Seed Rework:
- Added a configuration to make the healing from DoTs scale with the tick rate of said DoT. (It was doing this already by default)
- Changed "Base DoT Healing" default configuration. (4 -> 1)
- Changed "Stack DoT Healing" default configuration. (4 -> 1)
- ^ A tick rate of 4 is used as the baseline for DoT scaling so the healing amount is unchanged with these values.
- Changed "Leech Base Damage" default configuration. (0.5 -> 2.5) (Unchanged technically)
- Replaced "Leech Stack Duration" with "Leech Stack Damage" configuration.
- ^ This is done to be consistent with how other items increase their DoT damage.
- Healing from the Leech debuff is now scaled from the attacker's damage stat rather than with time.
- Changed "Leech Life Steal" default configuration. (0.1 -> 1)
- Removed "Leech Minimum Life Steal", now always in effect at a value of 1.

- Unstable Transmitter Rework:
- Changed "Cap Cooldown" default configuration. (2 -> 1) (I thought this would be annoying to hear constantly but it's not.)
- Changed "Base Radius" default configuration. (15 -> 16)

- Symbiotic Scorpion Rework:
- Changed "Slayer DoT Base Damage" default configuration. (1 -> 2)
- ^ Originally at 1 for balance reasons, but decided to set it to 2 so it's consistent with actual Slayer damage.
- Changed "Venom Base Damage" default configuration. (8 -> 6)
- Changed "Venom Stack Damage" default configuration. (8 -> 6)
- Removed Monster specific interaction, is now AI Blacklisted by default.

- Titanic Knurl Rework:
- Changed "Stack Cooldown" default configuration. (0.15 -> 0.25)
```

**1.22.3**

```
- Defense Nucleus Shared:
- Summon projectiles will only impact against the world now.

- Defense Nucleus Rework:
- Configuration "Summon Count" is now capped at 8 instead of 6 because I said so.
```

**1.22.2**

```
- Added compatability with Moffein-AssistManager, the following changes are supported:
- Bison Steak Rework
- Topaz Brooch
- Hunters Harpoon
- Infusion
- Defense Nucleus Buff (Disabled by default)

- Bison Steak:
- Item description now states that the health amount increases with level to avoid confusion.

- Knockback Fin:
- Renamed "Distance Damage" configuration to "Velocity Damage" to properly reflect what it does.
- Removed "Add Fall Damage" configuration, replaced with "Credit Fall Damage", simply credits fall damage the target takes while launched instead of adding it to the impact.
- This means "Credit Fall Damage" will work even while Impact damage is disabled.
- Will no longer proc when hitting yourself.

- Topaz Brooch:
- Changed "Base Flat Barrier" default configuration. (15 -> 8)
- Changed "Stack Flat Barrier" default configuration. (15 -> 0)
- Changed "Base Percent Barrier" default configuration. (0.005 -> 0.02)
- Changed "Stack Percent Barrier" default configuration. (0.005 -> 0.02)

- Leeching Seed:
- Changed "Proc Healing" and "Base Healing" default configurations. (0.75 -> 1)
```

**1.22.1**

```
- Knockback Fin:
- specified in the config description of "Base Radius" that it disables the Impact damage when set to 0.
- Fixed the knockback only working once when the Impact damage was disabled.

- Unstable Transmitter Rework:
- Allies now get 1s of invincibility upon teleporting.
- Added configurations "Teleport Radius" and "Telefrag Radius" to control the range of ally teleports.
- Added configuration "Ally Owns Damage", allows the explosion to be owned by the ally that teleported instead of the item holder.
- Added configuration "Proc Coefficient", controls the Proc Coefficient of the blast.
- Added configuration "Proc Bands", controls if the blast can proc bands or not.
```

**1.22.0**

```
- Antler Shield Rework:
- Properly marked as a rework in the config.

- Knockback Fin:
- Damage now scales from the target's velocity so it makes more sense with higher stacks against airborne targets.
- Now counts as a debuff only while launched, the cooldown afterwards is not a debuff.
- Added an experimental option that moves all the target's fall damage to the impact damage, added configuration "Add Fall Damage" to enable this behaviour.

- Added a rework for Unstable Transmitter.

- Squid Polyp:
- Fixed Squids not spawning from interacting with Chests, has been broken since Devotion Update.

- Aegis:
- Changed "Base Overheal" default configuration. (0.5 -> 1)
- Changed "Stack Overheal" default configuration. (0.5 -> 0)
- Changed "Base Max Barrier" default configuration. (0.25 -> 1)
- Changed "Stack Max Barrier" default configuration. (0.25 -> 1)
```

**1.21.1**

```
- Knockback Fin:
- Added "Flying Force Mult" configuration, a knockback multiplier against flying enemies.
- Removed "Small Force Mult", "Medium Force Mult" and "Large Force Mult" configurations as I misunderstood how the original effect was coded.
```

**1.21.0**

```
- Updated to work with Seekers of the Storm, still needs more thorough testing.
- Fully uses R2API ContentManagement instead of partially.

- Roll of Pennies Rework:
- Changed "Stack Armor" default configuration. (1 -> 0)
- Changed "Stack Armor Duration" default configuration. (1 -> 2)

- Added changes for Antler Shield.

- Added changes for Knockback Fin.

- Infusion Rework:
- Added checks to prevent gaining level up effects from spawning in with or losing stacks of the item.

- Happiest Mask Rework:
- Changed "Stack Damage" default configuration. (1.5 -> 2)
```

**1.20.4**

```
- Symbiotic Scorpion Rework:
- Fixed an error caused by DoTs inflicted by non-character bodies.
```

**1.20.3**

```
- Defense Nucleus:
- Item is not longer copied by Turrets.

- Defense Nucleus Rework:
- Item is not longer copied by Turrets.
```

**1.20.2**

```
- Newly Hatched Zoea Rework:
- Changed when in the code corruption changes are made to better prevent other mods ignoring changes made.

- Leeching Seed Rework:
- Changed the description to state "Damage over time" instead of "Status damage" for consistency.

- Defense Nucleus:
- Removed "Cooldown" configuration, pretty sure it wasn't being used in the first place.
- The cooldown for summoning Constructs is now 0.5s but goes up to 7s if it suspects you'll cap out.

- Defense Nucleus Rework:
- Changed "Shield Stack Duration" default configuration. (1 -> 2)

- Updated dependancies.
```

**1.20.1**

```
- Ignition Tank Rework:
- Hits are now tracked per enemy.

- Defense Nucleus:
- Improved in-game description slightly.

- Defense Nucleus Rework:
- Fixed the shield not deploying.
- Improved in-game description slightly.
```

**1.20.0**

```
- Added basic error handling for IL changes, be warned if something does break it'll be much less obvious now, but should be easier to debug.

- General Config:
- Added "Tweak Barrier Decay" configuration, adjusts base barrier decay to use Max Health+Shield instead of Max Barrier, this is primarily for Aegis changes.

- Bison Steak:
- Changed "Base HP" default configuration. (20 -> 10)
- Changed "Level HP" default configuration. (2 -> 3)
- Default values have been changed to scale the same as player health.
- Removed "Base Regen Duration" configuration.
- Removed "Stack Regen Duration" configuration.

- Added a rework for Bison Steak that's based on its Fresh Meat version.

- Roll of Pennies Rework:
- Changed "Base Gold" default configuration. (5 -> 3)
- Changed "Stack Armor" default configuration. (0 -> 1)
- Changed "Base Armor Duration" default configuration. (1 -> 2)

- Hunter's Harpoon:
- Buff doesn't have a fading effect, is now a full buff for the entire duration.
- Buff doesn't increase skill cooldown rate, instead it reduces skill cooldown by a flat amount of seconds on kill.
- Changed "Base Duration" default configuration. (1.5 -> 1)
- Changed "Stack Duration" default configuration. (0.75 -> 1)
- Changed "Movement Speed Bonus" default configuration. (0.25 -> 1.25)
- Changed "Cooldown Primary" default configuration. (true -> false)
- Removed "Cooldown Rate Bonus" configuration.
- Added "Cooldown Reduction" configuration. Default: (1)
- Added "Extend Duration" configuration. Default: (true)

- Lepton Daisy:
- Changed "Base Healing" default configuration. (0.15 -> 0.1)

- Old War Stealthkit:
- Stealth is now given as 5 stacks that expire one after another, similar to Hunter's Harpoon.
- Combat and Danger cancels are now only applied while you have 5 stacks of Stealth or more.
- Removed configuration "Cancel Duration" as a result of the above changes.
- Added configuration "Stealth Movement Speed", controls the movement speed bonus from being Stealthed. Default: (0.4)
- Added configuration "Stealth Armor", controls how much armor each stack of Stealth gives. Default: (20)

- Squid Polyp:
- Squid Turret base damage increased from 4 to 5, matching drone damage scaling.
- The knockback on Squid Turret shots now scales down with attack speed, should stop them from pushing everything away at higher stacks.
- Added "Stack Health" configuration, controls how much extra health Squid Turrets get per stack. Default: (2)
- Added "Base Duration" configuration, controls the duration of Squid Turrets at a single stack. Default: (25)
- Changed "Stack Duration" default configuration. (3 -> 5)
- Added "Max Turrets" configuration, controls how many Squid Turrets each player can have, newer turrets will replace old ones. Default: (8)
- Removed "Inactive Removal" configuration, the above configuration is designed to replace its functionality.
- Removed "Stack Armor" configuration.

- Aegis:
- No longer increases armor, instead increases maximum barrier similar to its RoRR's version.
- Added "Base Overheal" and "Stack Overheal" configurations, controls how much barrier to get from overhealing. Default: (0.5)
- Added "Base Max Barrier" and "Stack Max Barrier" configurations, controls the maximum barrier increase. Default: (0.25)

- Happiest Mask Rework:
- Changed "Base Damage" default configuration. (1 -> 2)
- Changed "Stack Damage" default configuration. (1 -> 1.5)
- Changed "Cooldown" default configuration. (31 -> 3)
- Cooldown now starts the moment a minion slot is available instead of after spawning a ghost.

- Added a rework for Symbiotic Scorpion.

- Defense Nucleus and Defense Nucleus Rework:
- Changed "Base Attack Speed" default configuration. (5 -> 3)
- Changed "Base Damage" default configuration. (5 -> 0)
- Changed "Stack Damage" default configuration. (0 -> 5)
- Changed "Shield Base Duration" default configuration. (3.5 -> 5)

- Titanic Knurl:
- Changed "Base HP" default configuration. (40 -> 30)
- Changed "Level HP" default configuration. (4 -> 9)
- Default values have been changed to scale the same as player health.

- Voidsent Flame:
- Has been given the "CannotCopy" item tag for simplicity.
```

**1.19.2**

```
- Death Mark Rework:
- The Death Mark is applied in an earlier hook to (hopefully) prevent the item breaking in some mod setups.
```

**1.19.1**

```
- Wax Quail:
- Fixed the default config values being incorrect.

- Happiest Mask Rework:
- Fixed the ghost not spawning.

- Defense Nucleus:
- Fixed the shared changes configurations still being defaulted to on.
```

**1.19.0**

<details><summary>Notes</summary>I'm unable to bug test multiplayer at this time since RoR2's dedicated server wasn't updated. Be sure to check your configs after this update. Also made some changes to Ignition Tank rework as I wanted to keep the item away from being luck based.</details>

```
- Split up configs into more categories, "General", "Items" and "Artifacts".
- All item changes default to disabled now instead of only reworks.

- Ignition Tank Rework:
- The Explosion is no longer chance based and will always explode every X times status damage is dealt.
- Removed "Explosion Chance" and "Explosion Scale DoT Tickrate" configurations as a result.
- Removed "Explosion Bonus Radius From Target" configuration.
- Added "Explode Ticks" configuration as a result. Default: (10)
- Changed "Explosion Base Damage" default configuration. (2.5 -> 3)
- Changed "Explosion Roll Crits" default configuration. (true -> false)

- Leeching Seed Rework:
- Changed "Leech Life Steal" default configuration. (0.02 -> 0.1)
- Changed "Leech Minimum Life Steal" default configuration. (0.5 -> 1)
- Life steal from the Leech debuff now scales down with time.

- War Horn:
- Changed "Stack Duration" default configuration. (2 -> 3)

- Ben's Raincoat:
- Added a brief grace period after consuming a debuff block which prevents further debuffs without consuming stacks.
- Added configuration "Debuff Grace Time" for the above change. Default Value: (0.25)
- Changed "Stack Block" default configuration. (2 -> 1)
- For reference it would take roughly 27 Raincoats for 100% uptime with the new default configurations.

- Defense Nucleus:
- Changed "Base Attack Speed" default configuration. (6 -> 5)
- Changed "Base Damage" default configuration. (6 -> 5)

- Defense Nucleus Rework:
- Changed "Base Attack Speed" default configuration. (6 -> 5)
- Changed "Base Damage" default configuration. (6 -> 5)

- Planula Rework:
- Changed "Base Damage" default configuration. (1 -> 0.8)
- Changed "Stack Damage" default configuration. (1 -> 0.6)
- Added some visual feedback when it burns enemies.

- Voidsent Flame:
- Changed "Base Radius" default configuration. (10 -> 12)
- Changed "Stack Radius" default configuration. (2 -> 2.4)

- Newly Hatched Zoea Rework:
- Added configurations for controlling its item corruption.

- Added a rework for Death Mark.

- Added a rework for Roll of Pennies.

- Removed the rework to Infusion and added a Happiest Mask rework that's somewhat similar.
```

**1.18.0**

<details><summary>Notes</summary>I greatly suggest checking the configs in regards to the Ignition Tank rework, there's a few umentioned options that might be worth changing. Also no full bug testing has been done in regards to the recent RoR2 Update, wanted to churn this out before RoRR.</details>

```
- Leeching Seed Rework:
- The Healing from DoTs is now scaled by the DoT's tick rate, faster ticks heal less and slower ticks heal more. (For context most DoTs tick 4 times a second so they will give 1/4th of the intended amount.)
- Added "Base DoT Healing" configuration, controls how much healing each DoT tick gives at single stack. Default Value: (4)
- Added "Stack DoT Healing" configuration, controls how much healing each DoT tick gives for each additional stack. Default Value: (4)
- Changed "Leech Chance" default configuration. (25 -> 20)
- Damage over time ticks from the Leech debuff no longer triggers its life steal effect, so it doesn't double dip with Leeching Seed.
- Healing from the Leech debuff and Leeching Seed are now rolled into a singular instance of healing, for visual clarity and because they're both related to the same item.

- Chronobauble:
- Fixed it applying the slow on hits with a proc coefficient of 0 or less.

- Added a rework for Ignition Tank.
```

**1.17.0**

```
- Added changes for Chronobauble.
```

**1.16.1**

```
- Infusion Rework:
- Blood Clones now no longer run the effects of infusion to prevent infinite loops under certain conditions.

- Newly Hatched Zoea Rework:
- Missiles are now loaded upon activation so that it doesn't fire forever if you recharge missiles faster than you fire them.
- Removed configuration "Restock When Finished".
```

**1.16.0**

```
- Infusion:
- Changed how levels are calculated so "Soft Cap" isn't skewed by "Stack Level".

- Leeching Seed Rework:
- "Leech Minimum Life Steal" no longer scales with level.
- Changed "Leech Minimum Life Steal" default configuration. (0.2 -> 0.5)

- Lost Seer's Lenses:
- Added "Deal Total" configuration, makes it use the total damage instead of the base damage.

- Titanic Knurl Rework:
- Fixed the effect doubling up.

- Planula Rework:
- Changed "Burn Radius" default configuration. (13 -> 15)

- Added a rework to the Newly Hatched Zoea.
```

**1.15.2**

<details><summary>Notes</summary>The way the mod handles its configs has changed greatly, so be ready to reconfigure them. Also default configurations may have changed that is not listed in this update.</details>

```
- Bison Steak:
- Changed pickup text to be consistent with other pickup texts.

- Topaz Brooch:
- Changed "Base Flat Barrier" default configuration. (14 -> 15)
- Changed "Stack Flat Barrier" default configuration. (14 -> 15)

- Infusion:
- Brought back the old infusion changes as per a user's request, the new infusion changes are now listed as a rework.

- Infusion Rework:
- Added "Leash Distance" configuration, gives the clone a custom minion leash distance.
- Clones are automatically killed if their owner no longer exists.

- Leeching Seed:
- Changed "Proc Healing" default configuration. (0.5 -> 0.75)
- Changed "Base Healing" default configuration. (0.5 -> 0.75)

- Leeching Seed Rework:
- Changed "Leech Minimum Life Steal" default configuration. (0.1 -> 0.02)

- Squid Polyp:
- Changed "Inactive Removal" default configuration. (20 -> 30)
- Squid Turrets are now killed from inactive removal instead of having their healing disabled.

- Ben's Raincoat:
- Removed "Improve Cooldown" configuration, is now always in effect when changes are enabled.
```

**1.15.1**

```
- Infusion:
- Fixed changes breaking Cautious Slug.

- Wax Quail:
- Actually uses the default configurations from the previous update.

- Ben's Raincoat:
- Changed "Stack Block" default configuration. (1 -> 2)

- Fixed a few item descriptions that mention base damage to be consistent with vanilla.
```

**1.15.0**

<details><summary>Notes</summary>Completely changed Lost Seer's Lenses effect since the bands mechanic was too complicated for a common item. Also completely changed Infusion since the previous version didn't address the scaling issue at all and there's a few other mods that already improve it.</details>

```
- Infusion:
- Effect has been reworked completely.
- Collecting enough blood from slain enemies creates a clone of yourself.
- Further collection of blood increases the clone's level.

- Old War Stealthkit:
- Removes damage over time effects on activation.
- Changed "Cancel Duration" default configuration. (1.0 -> 0.5)

- Wax Quail:
- Changed "Capped Horizontal Boost" default configuration. (150 -> 240)
- Changed "Capped Air Speed Bonus" default configuration. (1.5 -> 2.8)

- Lost Seer's Lenses:
- Effect has been changed completely.
- Works the same as the vanilla version but instead of an instant kill it deals a very high amount of base damage and works against bosses.

- Titanic Knurl Rework:
- Removed the unmentioned 50% cooldown rate bonus while at low health.
- Changed "Base Damage" default configuration. (7.0 -> 8.0)
- Changed "Stack Damage" default configuration. (4.0 -> 6.0)
- Changed "Attack Distance" default configuration. (50.0 -> 60.0)

- Added a buff and rework to Planula.
```

**1.14.4**

```
- Added changes for Artifact of Spite that are disabled by default.

- Defense Nucleus Buff:
- No longer procs itself when it kills, removed the "Minion Can Proc" configuration as a result.

- Lepton Daisy:
- Fixed various bugs involving heal amounts.
```

**1.14.3**

```
- Updated R2API dependencies.
- Likely broke 10 other things.

- Lost Seer's Lenses:
- Fixed breaking everything if RiskyMod wasn't installed.
- Changed "Proc Coefficient" default configuration. (0 -> 0.1)
```

**1.14.2**

```
- Hunter's Harpoon:
- Added configurations to enable the cooldown effect for Primary, Secondary, Utility and Special skills.

- Lepton Daisy:
- Changed "Base Healing" default configuration. (0.1 -> 0.15)

- Lost Seer's Lenses:
- Rework effect so that it chains to nearby enemies instead of acting like a blast.
- Now ignores hits that have the "Rings" proc mask.
- Fixed crowbar changes from RiskyMod triggering the damage threshold.
- Changed "Base Radius" default configuration. (20 -> 25)
- Changed "Base Damage" default configuration. (0.15 -> 0.1)
- Changed "Stack Damage" default configuration. (0.15 -> 0.1)

- Titanic Knurl Rework:
- Added "Proc Bands" configuration, controls if the stone fist is allowed to proc bands.

- Defense Nucleus Shared:
- Removed "Death Explosion Radius" and "Death Explosion Damage" configurations.
- Alpha Construct Ally no longer create explosions on death.
```

**1.14.1**

```
- Lost Seer's Lenses:
- Removed the automatic crit effect.
- Now triggers on a damage threshold like Runald's and Kjaro's.
- Added "Trigger Threshold" configuration, controls the amount of damage needed to trigger the effect.
```

**1.14.0**

```
- Removed Red Whip Changes.

- Voidsent Flame:
- Added "Base Damage" and "Stack Damage" configurations, controls the damage of the Voidsent Flame explosion.
- Added "Proc Coefficient" configuration, controls the Proc Coefficient of the Voidsent Flame explosion.

- Leeching Seed Rework:
- Leech's minimum healing is now enforced after being multiplied by the proc coefficient.
- Changed "DoT Heal" default configuration. (1 -> 2)
- Changed "Leech Life Steal" default configuration. (0.01 -> 0.02)
- Added "Leech Minimum Life Steal" configuration, controls the minimum amount of healing Leech gives.

- Titanic Knurl Rework:
- Changed "Stack Damage" default configuration. (3.5 -> 4)

- Added a rework for Lost Seer's Lenses.
```

**1.13.4**

```
- Leeching Seed Rework:
- Changed "DoT Heal" default configuration. (1.5 -> 1)
- Changed "Leech Life Steal" default configuration. (0.05 -> 0.01)
- Leech Debuff heals at minimum 20% of the attacker's level. (2 healing at level 10 for example)
- Leech Debuff healing is now affected by Proc Coefficient, and is applied AFTER the minimum healing.

- Lepton Daisy:
- Changed "Base Healing" default configuration. (0.15 -> 0.1)
- Changed "Stack Healing" default configuration. (0.15 -> 0.1)
- Caps at 200% of the target's maximum health instead of 100%.
- Added "Capped Healing" configuration, controls the hyperbolic stacking cap of the healing.

- Wax Quail:
- Changed "Base Horizontal Boost" default configuration. (15 -> 12)
- Changed "Stack Horizontal Boost" default configuration. (5 -> 6)
- Changed "Base Air Speed Bonus" default configuration. (0.14 -> 0.12)
- Changed "Base Vertical Boost" default configuration. (0.3 -> 0.2)
- All effects given now have a limit, stacking is Hyperbolic as a result.
- Added "Capped Horizontal Boost", "Capped Vertical Boost" and "Capped Air Speed Bonus" configurations, they control the stacking limits.
- Aerial speed bonus is no longer multiplied by the sprinting speed multiplier while not sprinting. (Yeah don't ask.)

- Defense Nucleus Buff:
- Changed "Cooldown" default configuration. (1.5 - > 1)
```

**1.13.3**

```
- Titanic Knurl Rework:
- Added "Proc Coefficient" configuration, controls the proc coefficient of the Stone Fist.
- Added "Attack Distance" configuration, controls the maximum targeting distance of the Stone Fist.
- Added "Target Mode" configuration, controls which targeting mode the Stone Fist uses.
- Changed the "weak" targeting to account for the armor stat.
```

**1.13.2**

```
Old War Stealthkit:
- No longer activates if the user is still "Stealthed".
- "danger" and "combat" cancels are no longer tied to the "Stealthed" buff.
- Activation forces the user out of "danger" and "combat" for 1 second.
- Added "Cancel Duration" configuration, controls how long the "danger" and "combat" cancel lasts for.
- Added "Buff Duration" configuration, controls how long the "Stealthed" buff is given for.
- Added "Base Cooldown" configuration, controls the base cooldown between activations.
- Added "Stack Cooldown" configuration, controls the cooldown reduction from additional item stacks.
```

**1.13.1**

```
Leeching Seed:
- Fixed "Normal Heal" configuration not being used.
```

**1.13.0**

_Would appreciate feedback on the Wax Quail changes, including default configurations. Felt Lepton Daisy was bit too weak for how situational it is and also might look into Simulacrum specific changes._

```
Bison Steak:
- Changed "Base HP" default configuration. (25.0 -> 20.0)
- Changed "Level HP" default configuration. (2.5 -> 2.0)

Leeching Seed:
- Healing is received in a single instance instead of two.
- Changed "Normal Heal" default configuration. (1.0 -> 0.5)

Lepton Daisy:
- Changed "Base Healing" default configuration. (0.1 -> 0.15)
- Changed "Stack Healing" default configuration. (0.1 -> 0.15)

Wax Quail:
- Added changes to Wax Quail.
```

**1.12.4**

_This update includes balance changes for the Defense Nucleus Buff. The intent is to be less spammy, reducing the difference between this and the Vanilla version._

```
Infusion:
- No longer triggers a level up when gaining levels from samples.

Lepton Daisy:
- Changed "Base Healing" default configuration. (0.08 -> 0.1)
- Changed "Stack Healing" default configuration. (0.08 -> 0.1)

Squid Polyp:
- Damage and Proc Coefficient updated to reflect Patch 1.2.4's values.

Voidsent Flame:
- Fixed for Patch 1.2.4.

Defense Nucleus Shared:
- Removed the attack speed cap for Defense Nucleus' stacking bonus.

Defense Nucleus Buff:
- Added "Cooldown" configuration, controls the cooldown for spawning constructs from kills.
- Added "Base Damage" configuration, changes the base damage of summoned constructs.
- Added "Stack Damage" configuration, changes the stack damage of summoned constructs.
- Changed "Cooldown"  default configuration. (0.25 -> 1.5)
- Changed "Base Health" default configuration. (0 -> 10)
- Changed "Stack Health" default configuration. (5 -> 10)
- Changed "Base Attack Speed" default configuration. (10 -> 6)
- Changed "Stack Attack Speed" default configuration. (10 -> 0)

Defense Nucleus Rework:
- Added "Base Damage" configuration, changes the base damage of summoned constructs.
- Added "Stack Damage" configuration, changes the stack damage of summoned constructs.
- Changed "Base Attack Speed" default configuration. (10 -> 6)
- Changed "Stack Attack Speed" default configuration. (10 -> 0)
```

**1.12.3**

_Benthic Bloom rework has been removed due to being made into a standalone item in hex3's [item mod](https://thunderstore.io/package/hex3/Hex3Mod/), special thanks to Conq for the original item concept._

```
Benthic Bloom Rework:
- Removed, see update comment.

Red Whip:
- Added changes to Red Whip.

Other:
- Added "General" configuration category.
- Added "Out of Combat Time" configuration, used as an internal reference for the mod.
- Added "Out of Danger Time" configuration, used as an internal reference for the mod.
```

**1.12.2**

```
Titanic Knurl Rework:
- Improved pickup text.

Defense Nucleus Rework:
- Changed "Summon Count" default configuration. (4 -> 3)
- Changed "Shield Base Duration" default configuration. (3s -> 3.5s)
- Changed "Shield Stack Duration" default configuration. (0.75s -> 1s)

Defense Nucleus Shared:
- More checks in place to prevent summoning over the limit.

Infusions:
- Tracker updates more accurately.
```

**1.12.1**

```
Laser Scope:
- Fixed description showing the critical strike chance being 100 times larger than what it actually was.
```

**1.12.0**

```
Lepton Daisy:
- Added changes for Lepton Daisy.

Laser Scope:
- Added changes for Laser Scope.

Hunter's Harpoon:
- Slightly improved description text.

Voidsent Flame:
- Added changes for Voidsent Flame.
```

**1.11.0**

```
Benthic Bloom:
- Added a rework for Benthic Bloom.
```

**1.10.3**

```
Infusion:
- Updated the tracker icon to better match vanilla buff icons.

Leeching Seed Rework:
- Changed "Leech Stack Duration" configuration. (2.5 -> 0)

Old War Stealthkit:
- Updated description.

Titanic Knurl Rework:
- Now prefers enemies that are roughly within its kill range rather than the weakest.
```

**1.10.2**

```
Infusion:
- Added "Tracker" configuration, adds a cosmetic buff to help track samples.

Ben's Raincoat:
- Added changes for Ben's Raincoat.
```

**1.10.1**

```
- Quick fix for the latest patch.
```

**1.10.0**

```
Hunter's Harpoon:
- Added changes for Hunter's Harpoon.

Squid Polyp:
- Went back and made it use a new SkillState to apply the Tar debuff. (I feel this is be better for performance and compatability.)
- Fixed a few random bugs from the previous update's changes.
```

**1.9.0**

_I've made some big changes to how the mod handles the Squid Polyps, please bring any errors or compatibility issues to my attention._

```
Aegis:
- Added changes for Aegis.

War Horn:
- Added changes for War Horn.

Infusion:
- Updated the pickup test to be less confusing.

Squid Polyp:
- Now takes advantage of IL instead of checking and modifying Squid Polyps that spawn in.
- No longer creates a whole new SkillState to modify its primary skill.
- Now changes the damageType on RoR2.Orbs.SquidOrb.Begin instead.
```

**1.8.0**

```
Old War Stealthkit:
- Added changes for Old War Stealthkit.
```

**1.7.3**

_This update is mainly an attempt to resolve Alpha Construct Ally spawning beyond the deployable limit. I also suddenly wanted to try my hand at adding item displays, so Alpha Constructs are now flagged as Mechanical, have fun!_

```
Bison Steak:
- Added "Stack Buff Duration" configuration, controls how long the Regen buff lasts from additional stacks.
- Also renamed a few of the other configurations, go check your config.

Defense Nucleus:
- Now has a brief cooldown for the on kill effect.
- The summoned constructs will now replace the nearest construct within 6m before trying to replace the oldest.
- Changed "Base Attack Speed" default value. (5 -> 10)

Defense Nucleus Rework:
- Changed "Base Attack Speed" default value. (5 -> 10)

Alpha Construct Ally:
- Added "Is Mechanical" configuration, makes it have the Mechanical flag, allowing it to get boosted by Spare Drone Parts and Captain's passive.
- Added "Enable Modded Displays" configuration, enables extra item displays for Spare Drone Parts.
```

**1.7.2**

_This update changes how the Infusion changes work. The item on its own is now weaker, but has better synergy with items that scale with level._

```
Infusion:
- Directly increases level instead of using level stats.
- Default config values have been changed to account for this.
- Changed "Max Stacks" default value. (200 -> 100)
- Changed "Kill Stack" default value. (2 -> 1)
- Changed "Champion Stack" default value. (8 -> 5)
- Changed "Elite Bonus" defaul value. (3 -> 2)
```

**1.7.1**

```
Defense Nucleus:
- Improved logic for the death checks.

Defense Nucleus Rework:
- Added "Summon Count" configuration, controls how many constructs are summoned.
```

**1.7.0**

```
Defense Nucleus:
- Increased the default health and fire rate config values.
- Turrets and other minions now proc their owner's Defense Nucleus instead of their own.

Defense Nucleus Rework:
- Increased the default health and fire rate config values.

Leeching Seed:
- Added a choice between buffing or reworking this item.
```

**1.6.0**

```
Defense Nucleus:
- Added a choice between buffing or reworking this item.
```

**1.5.2**

```
Squid Polyp:
- Fixed them not applying Tar when configured.
```

**1.5.1** 

```
Infusion:
- Fixed changes causing almost everything to break.
```

**1.5.0** 

_This update hasn't been thoroughly tested just as an FYI._

```
Updated for SotV.

Titanic Knurl Rework:
- Default configuration for "Base Damage" changed. (500% -> 700%)
- Default configuration for "Stack Damage" changed. (300% -> 350%)
- Default configuration for "Stack Cooldown" changed. (20% -> 15%)
```

**1.4.1**

```
- Fixed incompatibility with ZetAspects and GoldenCoastPlus.
```

**1.4.0**

```
Titanic Knurl:
- Added a choice between buffing or reworking this item.

Squid Polyp:
- Removed configuration "Attack to Damage", found that this could be a bit game breaking under certain situations.
```

**1.3.0**

_Most of this update changed how the code is structured, so there isn't many changes. A few bugs/issues may have been fixed but I can't say for sure._

```
Bison Steak:
- Added "Buff Duration" configuration for Bison Steak. Enables the Fresh Meat's health regen boost on kill for this item.
- "Level HP" configuration now works as a flat number instead of being a multiplier based on "Base HP".
- Default configuration for "Level HP" changed due to the above (0.1 -> 2.5).

Squid Polyp:
- "Armor" configuration for Squid Poly is now how much armor each stack gives to the Squid Polyp.
- Default configuration for "Armor" changed due to the above (50 -> 10).

Infusion:
- "Champion Stack" for Infusion default configuration is higher (6 -> 8).
- Removed configuration "Normal Stat Gain" for Infusion. Felt completely unnecessary, item functions as if this was enabled.
```

**1.2.0**

```
Bison Steak:
- Health boost from levels reduced (0.2 -> 0.1) as it scaled a bit too quickly.

Infusion:
- Added configuration "Gives To Owner". This controls if orbs from minion kills go to the owner instead.
- Added configuration "Inherit From Owner". This controls if minions with infusions inherit their owner's collected samples.

Squid Polyp:
- Added balance changes for Squid Polyps.

Topaz Brooch:
- Added balance changes for Topaz Brooch.
```

**1.1.0**

```
Infusion:
- Added changes for Infusion.
```

**1.0.0**

```
Public release.
```