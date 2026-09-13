# Pixel Art Monster – Dungeon (Cainos)

Our enemies. Same rig philosophy and same script shape as the character pack,
which makes them cheap to wire up.

- Store: <https://assetstore.unity.com/packages/2d/characters/pixel-art-monster-dungeon-202071>
- Docs: <https://docs.cainos.net/pixel-art-monster-dungeon>
- Latest version at time of writing: 1.4.0, ~10 MB, built against Unity 2022.3,
  compatible with Built-in and URP (not HDRP).

## Quick start

Drag a prefab from `Cainos/Pixel Art Monster - Dungeon/Prefab` into the scene
and you can drive it yourself: A/D to move, W/S for flying monsters, Shift to
run, Space to jump, left mouse to attack, K to kill or revive, and `[` / `]` to
trigger hit reactions. Playing with the demo scene this way is the fastest route
to picking which monsters we want.

**A skeleton archer prefab (`PF Skeleton – Archer`) is named explicitly in the
vendor docs**, and its `On Attack` event is already wired to a `Projectile
Launcher` — making it the cheapest ranged enemy if we ever want one.

## The roster

Ten monsters, each with skin variations. All share the same animation set:
Idle, Walk, Run, Attack, Jump, Injured, Die. Animation is skinned mesh rather
than frame-by-frame.

| Monster | Variants | Notes |
|---|---|---|
| Slime | Green, Ice, Magma | |
| Zombie | ×4 | Pure skin variants — free visual variety |
| Skeleton | Warrior, Mage, Archer | Mage and Archer are ranged |
| Mimic | Wooden, Iron, Silver, Golden | Only monster supporting `Is Hiding` |
| Bat | Purple, Brown, Black | Flying — uses `Monster Flying Controller` |
| Spider | ×3 | |
| Ghost | Blue, White | Flying |
| Reaper | Blue, White | Boss-shaped |
| Orc | ×3 | |
| Knight | Normal, Rusted, Moss | |

Our slice uses **Spider** (fast), **Zombie** (normal), **Orc** (brute) and
**Mimic** (the gag). See the GDD for why.

## Prefab structure

For each kind of monster there is **one original prefab**, and every skin
variation is a **prefab variant** of it. So a behaviour change made on the
original propagates to all its skins.

> This is exactly what we need for fast / normal / brute skeletons. One base
> skeleton prefab carries the AI, and the three variants differ only in tuning
> values and skin.

## Scripts

Source: <https://docs.cainos.net/pixel-art-monster-dungeon/script-reference.md>

Three components per prefab:

| Component | Role |
|---|---|
| `Pixel Monster` | Appearance and animation |
| `Monster Controller` | Movement |
| `Monster Input Mouse and Keyboard` | Reads keyboard/mouse into the controller |

The vendor's own guidance is that for real enemies you **delete the input
script** and have your AI write the controller's input fields instead. (The
alternative — keeping only `Pixel Monster` and moving the monster entirely
yourself — is more work and loses the movement tuning, so we won't.)

### `Pixel Monster`

Holds renderer, animator and FX references, plus an optional `Die Fx Prefab`
spawned on death. Runtime properties include `Alpha`, `Facing` (also editable in
the editor for starting direction), `Is Grounded`, `Is Dead`, and `Is Hiding`
— which the docs note **only works for the Mimic**.

`Moving Blend` drives the locomotion animation: `0.0` idle, `0.5` walk, `1.0`
run. `Attack`, `Injured Front` and `Injured Back` play their respective
animations.

As with the player, setting `Is Dead` here only plays the death animation; to
stop the monster moving you also set `Is Dead` on the controller.

### `Monster Controller`

The tuning surface for our three skeleton variants. It exposes a default
movement type (walk or run), max speed and acceleration for walk, run and air,
braking acceleration for ground and air, and a jump block: jump speed, cooldown,
gravity multipliers for rise and fall, and a **jump delay** that plays a
prepare animation before actually leaving the ground (settable to 0 to remove).

Two flags shape how an enemy feels in a fight: `Can Attack in Air`, and
`Can Attack When Moving` — when the latter is off, the monster also cannot move
during its attack animation, which is what makes a slow brute readable and
dodgeable.

`Ground Check Size` defines the box under the monster used for ground
detection, and `Moving Blend Transition Speed` smooths the animation blend.

Inputs our AI writes: `Input Move` (x and y in −1..1), `Input Move Modifier`
(walk vs run), `Input Jump`, `Input Attack`, and `Is Dead`.

### `Monster Flying Controller`

Same idea for flying monsters, with a single speed/acceleration/brake set
instead of walk and run. Flying monsters have gravity scale forced to 0 while
alive; `Dead Gravity Scale` decides whether they drop to the floor when killed.

## Animation events

Source: <https://docs.cainos.net/pixel-art-monster-dungeon/setup-guide.md>

The monster animation clips already contain `On Footstep` and `On Attack`
events, and the `Animator` child carries an `Animation Event Receiver` that
surfaces them. The vendor's worked example is the skeleton archer, where
`On Attack` calls `Launch` on a `Projectile Launcher` component to fire an
arrow.

> Same pattern as the player's `On Attack Hit`. Enemy damage hooks into
> `On Attack`, so we never write attack timing by hand.

## Setup requirements

**Input handling.** Same legacy Input Manager issue as the character pack — set
`Active Input Handling` to `Both` in `Project Settings > Player` if inputs error
out. (Note this pack has no Input System patch of its own, but since we delete
the input scripts from enemies anyway, it only matters while playing with the
demo scene.)

**Skin weights.** `2 Bones` minimum, same as the character pack.

**Lighting patches.** `Patch - URP 2D Lit` or `Patch - URP Lit` from the asset
root, matching whichever lighting model the project uses. Unlike the character
pack, these are not version-split.

**Transparency sort axis.** `Custom Axis` with `(0,0,1)` — same setting as the
character pack, so doing it once covers both.

**Sorting glitch between monsters.** Identical to the character issue: two
monsters at the same Z interleave their sprites. Give them different Z
positions and set Z scale to something small like `0.1`, never `0.0`. With a
pack of skeletons crowding the player this is guaranteed to show up, so it needs
handling in the spawn logic, not just by hand-placing.

## Blender source files

Source: <https://docs.cainos.net/pixel-art-monster-dungeon/blender-source-files.md>

Only relevant if we ever want custom monster animations. Requires **Blender
5.0.1 or later**.

Clip names encode their own settings — `Walk 30L` means the clip is called
Walk, its target frame range is 0–30, and the trailing `L` means it loops. Some
clips contain keyframes *outside* the target range as offset loop frames for
secondary motion, so on import each clip must be trimmed to its stated range.

Jumps are split into `Jump Prepare`, `Air Up`, `Air Down` and `Land`, with some
monsters splitting the rise further into `Air Up Idle` and `Air Up Move`. There
is also a `Jump Full` clip combining the lot, but it has root motion on the
vertical axis and is intended for preview only — not for use in engine.

On import into Unity: enable `Bake Axis Conversion`, trim each clip to its
target frame range, and enable looping on any clip whose name ends in `L`.

> Out of scope for the vertical slice. Recorded here so we don't rediscover it
> later.
