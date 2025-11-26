**1.4.0**

```
- Updated DLLs, Dependencies and some code to stay up to date with Alloyed Collective.

Delicate Watch (Broken):
- Added "Play Proc VFX" configuration, controls if the proc VFX is used. (Default: true)
```

**1.3.0**

```
- Updated DLLS and Dependencies for Seekers of the Storm.
- All item changes are disabled by default and must be enabled via the configs.

Delicate Watch (Broken):
- Damage increase is now consistent with how other items work.
```

**1.2.4**

```
Pluripotent Larva (Consumed):
- Fixed on hit code blocking later ServerDamageDealt hooks.
```

**1.2.3**

```
Pluripotent Larva (Consumed):
- Redone code so it's less prone to break with mods that change Safer Spacers or Needle Ticks.
- Added "Collapse Chance", "Collapse Damage", "Collapse Total Damage" and "Block Cooldown" configurations.

Dio's Best Friend (Consumed):
- Redone code so it's less prone to break with mods that change Tougher Times.
- Added "Block Chance" configuration.

Empty Bottle:
- Changed "Passive Regen" default value. (0.6 -> 1.0)
```

**1.2.2**

```
Delicate Watch (Broken):
- Fixed it applying Symbiotic Scorpion's debuff instead of dealing bonus damage.

Misc:
- Added Delicate Watch (Broken) and Empty Bottle's descriptions to all languages as a fall back.
```

**1.2.1**

```
Pluripotent Larva (Consumed):
- Updated the item corruption to reflect the changes made in Patch 1.2.3.

Misc:
- Empty Bottle's modified description is now English only, so it overwrites WolfoQualityOfLife's description.
- Delicate Watch (Broken)'s modified description is now English only, for the above reason.
```

**1.2.0**

```
Configs:
- Basically renamed all the categories so they reference the actual item instead of its unconsumed version.

Delicate Watch:
- Added a buff indicator to track hits.
- Hits on the same frame are now counted, previously it didn't, this was intentional but I've changed it for simplicity.
- This also means you can't proc the watch multiple times in a single frame, meaning you won't hear a loud breaking sound from hitting 10 enemies at once.
- No longer counts hits that deal 0 damage, before it only ignored hits that were blocked/rejected.
- Renamed "Hits To Proc" configuration to "Hit To Proc".
- Renamed "Hits To Proc Super" configuration to "Proc to Double Proc".
- Default value for the above changed. (144 -> 12)
- "Stack Slow On Proc" configuration now doesn't count the first item in the stack.
- Changed "Base Slow On Proc" default value. (0.75 -> 1)

Power Elixir:
- Fixed "Regen Buff Duration" not actually changing the buff's duration.
- Changed "Regen Buff Duration" default value. (1.25 -> 2.5) (It was intended to heal the last 25% Elixir would miss, the old value was actually too low.)
```

**1.1.0**

```
- Changed "Damage On Proc" to change the damage bonus it gives.
- Changed "Passive Regen" default value. (1 -> 0.5)
```

**1.0.0**

```
- Public release.
```