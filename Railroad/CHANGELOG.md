**1.3.0**

This update has been planned for a while and is mostly near completion. The mod has undergone
large changes to how it functions but is working "good enough" to release as is. I've been
dragging my feet working on it and hope that releasing it will help me figure out
what's left to do and what could be improved or fixed and any major incompatibilities.
Ideally I want this version to be in a more refined state before Hallowed Concepts.

```
- Proper Save Support!
- Proper Save is now supported to retain memory of certain run achievements (mainly boss defeats) when loading a save.

- Glass Frog configs:
- Configure the amount of times to pet the frog, the portal it spawns and the currency required.

- Eclipse configs:
- Added "Allow Beads of Fealty", allows Beads of Fealty to appear during Eclipse runs.

- Planetarium:
- Spawns the Void Outro Portal somewhere inaccessible if disabled, so that the boss music finishes properly. (Top Tier Jank)

- Celestial Orb:
- Added configurations to change when this portal orb appears.

- General Changes:
- Removed randomness from how multiple portals are placed, any randomness left is likely from the director.
- Added tags for most config portal spawns, tags are mostly major boss defeats so you can have certain portals spawn or not spawn based on run progress.
- Separated a large set of configs into Eclipse and Non-Eclipse runs.

- Probably a few more small things I forgot about.
```

**1.2.1**

```
- Stages
- Fixed Planetarium portals using the Commencement Portal configuration.
```

**1.2.0**

Changes made will require you to review your configs.
Biggest of note is that multiple completion portals can be spawned now, spawning is a bit off but it's functional for now.

```
- Looping
- "Loop Artifacts" configuration now also accepts English names with the spaces removed, along with internal names.
^ Example: Artifact of Honor can be represented as "EliteOnly" (Internal Name) or "ArtifactofHonor" (English, space removed name).

- Stages
- "Completion Portal" configurations have been renamed to "Completion Portals" and will now accept a list of multiple portals.
- Added configurations for Solus Web that include: "Completion Portals", "Completion Reward" and "Allow Decompile".
```

**1.1.0**

No Solus Web and Solutional Haunt configs yet because I'm lazy.

```
- Updated DLLs for Alloyed Collective.
- Added five portals from Alloyed Collective to the available portal choices.
- Renamed configuration files so they're easier to find in Gale's config editor.

- Looping
- Removed "Min Stage Clears" configuration from "!LoopDefinition".
^ The game checks for loops slightly differently now so this shouldn't be needed anymore.
- Removed "Enable Honor" configuration in favour of the new "Loop Artifacts" configuration.

- Stages
- Added measures to prevent the extra portal being hard to reach in the Planetarium.
- Planetarium's item reward now uses Void Potentials containing a choice of Legendary items.
- Added "Time Flow" configurations for Void Fields and Planetarium, may refine these settings and extend them to similar stages in the future.
```

**1.0.4**

```
- Fixed Commencement item rewards clumping together.
```

**1.0.3**

```
- Fixed "Allow On Eclipse" configuration doing the opposite.
- Fixed "Completion Portal" for "Prime Meridian" configuration not respecting the "Allow On Eclipse" configuration.
```

**1.0.2**

```
- Same mistake twice, shame on me.
```

**1.0.1**

```
- Minor config mistake, I lose.
```

**1.0.0**

```
- Public release.
```