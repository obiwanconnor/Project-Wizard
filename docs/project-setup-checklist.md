# Project Setup Checklist

The three packs each have their own setup instructions, and several overlap or
have to happen in a particular order. This is the combined list. Work top to
bottom. Do not start dressing rooms or building gameplay until everything here
is ticked off.

## 0. Locked decisions

These are settled. Reversing any of them after rooms are lit means real rework,
so treat them as fixed for the slice.

### Unity 6.3 LTS (6000.3.x) — latest patch release

Install from the Unity Hub and stay on it. Do not drift onto 6.4/6.5/6.6.

Unity 6.3 LTS is supported until December 2027, so it comfortably outlasts this
project. Unity 6.0 LTS support ends in October 2026, which rules it out. The
newer Update releases (6.4 onward, running quarterly toward 6.7 LTS during 2026)
carry shorter support and more churn — the wrong trade for a project that gets
picked up and put down. Asset publishers also target the current LTS, so the
Cainos packs will be tested against it.

The dungeon pack's changelog mentions Unity 6.5 support; that's forward
compatibility, not a problem with 6.3.

### URP 3D lighting

The dungeon demo scene already ships lit and post-processed this way, and the
entire slice is built on that map — so this is zero rework and matches the look
in the store screenshots.

Helpfully, this also removes the version trap. The character pack only splits
its patches by Unity version on the **2D** Lit path (6.0–6.2 vs 6.3+). On the
3D path there is a single `Patch - URP Lit - Unity 6` covering all of 6.x, so
there is no wrong patch to pick.

> **Fallback, if ever needed:** if candle glow doesn't read well once the candle
> mechanic is in, 2D Lit is the alternative. That call has to be made in the
> first week, not the last — after E5 it means re-lighting every room.

### New Input System

Import the character pack's `Patch - Input System`. Gamepad support is worth a
lot for a game two kids will play on the sofa, and the alternative
(`Active Input Handling = Both` alone) leaves us on a legacy path we'd have to
migrate off later anyway.

The monster pack has no equivalent patch, which doesn't matter — we delete the
input scripts from enemies and drive them from our own AI.

## 1. Project settings

- [ ] `Project Settings > Player > Other Settings` → Colour Space = **Linear**
- [ ] `Project Settings > Player` → Active Input Handling = **Both** (needed
      while the demo scenes still use legacy input, even if we later move fully
      to the new Input System)
- [ ] `Project Settings > Quality` → Skin Weights = **2 Bones** or higher
- [ ] `Project Settings > Graphics` → Transparency Sort Mode = **Custom Axis**,
      Transparency Sort Axis = **(0, 0, 1)**
- [ ] Install **Universal Render Pipeline** via Package Manager
- [ ] Assign the URP asset that ships with the dungeon pack, at
      `Cainos/Pixel Art Platformer – Dungeon/Rendering` — do not create a new one
- [ ] Install **Input System** package via Package Manager
- [ ] Install **TextMeshPro Essentials** when prompted, then reopen the scene

## 2. Import the packs and their patches

Import all three packs at their latest versions first, then the patches.

- [ ] Pixel Art Platformer – Dungeon (v1.1.8 or later, for the Unity 6 fixes
      and improved tile collider shapes)
- [ ] Customizable Pixel Character
- [ ] Pixel Art Monster – Dungeon (v1.4.0 or later)

Then the patches, for URP **3D** lighting:

- [ ] Dungeon pack: **no lighting patch needed** — the demo scene is already 3D
      lit. Do *not* import `Patch - URP 2D Lit`.
- [ ] Character pack: `Patch - URP Lit - Unity 6`
- [ ] Monster pack: `Patch - URP Lit`
- [ ] Character pack: `Patch - Input System`

> If you see `Patch - URP 2D Lit` anywhere in a pack root, that is the wrong
> one for this project. Leave it alone.

## 3. Layers and physics

- [ ] Create layers: `Player`, `Player Weapon`, `Enemy`, `Enemy Weapon`,
      `Ground`, `Interactable`
- [ ] In Physics 2D settings, make `Player` and `Player Weapon` **not** collide
- [ ] Same for `Enemy` and `Enemy Weapon`
- [ ] Set the character controller's `Ground Check Layer Mask` to `Ground`
- [ ] Set the one-way platform `Platform Effector 2D` collider masks to exclude
      the player layer

## 4. Smoke test before writing any code

- [ ] Open the dungeon demo scene — it renders, is lit, and post-processing is on
- [ ] Drop a character preset in — it walks, jumps, attacks, climbs a ladder,
      drops through a platform
- [ ] Drop a skeleton prefab in — it moves and attacks under keyboard control
- [ ] Two characters standing close together do **not** glitch through each
      other's sprites (if they do, fix Z positions and set Z scale to `0.1`)

If any of these fail, stop and fix it here. Every one of them gets harder to
diagnose once our own code is in the scene.

## 5. Known-issue quick reference

| Symptom | Cause | Fix |
|---|---|---|
| Shaders pink / erroring after URP install | Shaders need reimport | Right-click the dungeon pack's `Shader` folder → Reimport |
| Scene very dark after importing 2D Lit patch | Documented quirk | Import the patch again, reload the scene |
| Inputs do nothing | Unity 6 disables legacy Input Manager | Active Input Handling → `Both`, or import the Input System patch |
| Character collides with its own weapon | Same physics layer | Separate `Player` / `Player Weapon` layers, disable collision between them |
| Two sprites interleaving when close | Same Z depth | Different Z positions, Z scale `0.1` (never `0.0`) |
| Character partly hidden by background | Background at same Z | Push background to a larger Z |
| UI text missing | TMP not imported | Import TMP Essentials, reopen scene |
| Animation looks stiff | Skin weights too low | Quality settings → Skin Weights ≥ 2 Bones |
