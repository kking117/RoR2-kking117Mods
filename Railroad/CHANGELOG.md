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