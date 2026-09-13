# Delivery Plan — *Where Are My Keys?!* Vertical Slice

Everything needed to get from three unimported asset packs to a playable,
finished-looking slice on the dungeon demo map.

## How to read this

**Owner tags**

- `[DEV]` — adult in Unity or in code
- `[KID]` — a child can own this end to end
- `[BOTH]` — sat together at the same machine
- `[PLAY]` — everyone, playing

**Size**: `S` an evening or less · `M` a couple of evenings · `L` a week of
evenings

**Milestones**

| | Milestone | What "done" means |
|---|---|---|
| **M1** | Grey-box loop | You can walk, hit a skeleton, open a chest, get a key, open the door and win. Ugly but complete. |
| **M2** | Feature complete | All three enemy types, candles, checkpoints, hearts, UI, all systems in. Placeholder content. |
| **M3** | Content complete | Real writing, real placement, real audio. |
| **M4** | Beautiful corner | Lit, polished, juiced, built and shareable. |

The order matters more than the dates. M1 first is deliberate — get something
playable in front of the kids as early as possible, because that is what keeps
everyone interested. Pretty comes later, and none of the early work is wasted.

---

## E0 — Foundation & setup  `M1`

> Blocks everything. Do not start anything else until this is green.

### E0.1 Project created and configured
- `[DEV]` `S` Create the Unity project on **Unity 6.3 LTS (6000.3.x)**, latest patch release; record the exact version in the repo README
- `[DEV]` `S` Initialise git, add a Unity `.gitignore`, set up Git LFS for assets
- `[DEV]` `S` Work through `docs/project-setup-checklist.md` §1 — colour space, input handling, skin weights, transparency sort axis, URP install
- `[DEV]` `S` Import all three asset packs at latest versions
- `[DEV]` `S` Import the **URP 3D** lighting patches: `Patch - URP Lit - Unity 6` (character), `Patch - URP Lit` (monster). The dungeon pack needs none — it already ships 3D lit.
- `[DEV]` `S` Import the character pack's `Patch - Input System`
- `[DEV]` `S` Create layers and configure Physics 2D collision matrix
- `[DEV]` `S` Run the §4 smoke test — demo scene renders, character moves, skeleton moves, no sprite glitching

### E0.2 Project structure
- `[DEV]` `S` Folder convention: `_Game/Scripts`, `_Game/Prefabs`, `_Game/Scenes`, `_Game/Audio`, `_Game/Data` — kept separate from `Cainos/` so pack updates never clobber our work
- `[DEV]` `S` Duplicate the dungeon demo scene as `_Game/Scenes/Dungeon.unity` and work only in the copy
- `[DEV]` `S` Confirm asset pack updates can be reimported without losing our changes

**Done when:** a copy of the demo scene opens, looks right, and a character
preset walks around in it with a gamepad.

---

## E1 — Player & traversal  `M1`

### E1.1 Wizard prefab
- `[BOTH]` `S` Create `PF Wizard` as a variant of a character preset
- `[DEV]` `S` Check the pack's `Prefab/Weapon` folder for a staff or wand; if there isn't one, a reskinned existing weapon or a simple custom sprite will do
- `[DEV]` `S` Attach the staff to the Weapon Slot, on the `Player Weapon` layer
- `[DEV]` `S` Set `Attack Action` to **Cast** and `Attack Action Melee` to the staff swing
- `[DEV]` `S` Tune the controller: walk/run speed, jump height, coyote time (`Jump Tolerance`), fall gravity multiplier
- `[DEV]` `S` Enable dash and dodge; disable swim and anything else unused
- `[DEV]` `S` Set Z scale to `0.1` to avoid the documented sprite-interleaving glitch

### E1.2 Camera
- `[DEV]` `S` Cinemachine follow camera with soft damping and a dead zone
- `[DEV]` `S` Confine the camera to the map bounds so it never shows past the level edges

### E1.3 Traversal in the map
- `[DEV]` `M` Place `Ladder` components on the demo map's ladders — trigger collider, `Direction`, `Climb Pos`
- `[DEV]` `M` Place `Platform` components on one-way platforms — `Used By Effector`, `Platform Effector 2D`, collider mask excluding player
- `[PLAY]` `S` Walk the entire map and confirm everywhere reachable is reachable and nothing traps the player

**Done when:** the hero can get to every part of the demo map and it feels good
to move.

---

## E2 — Interaction system  `M1`

> The biggest piece of original engineering in the project. Build it properly
> once and everything downstream gets cheap.

### E2.1 Core interaction framework
- `[DEV]` `M` `Interactable` base class: trigger volume, `CanInteract`, `Interact()`, `OnInteracted` event
- `[DEV]` `S` `PlayerInteractor` on the player: tracks interactables in range, picks the nearest, calls `Interact()` on input
- `[DEV]` `S` Bind an Interact action in the Input System (E on keyboard, a face button on gamepad)
- `[DEV]` `S` Floating prompt that appears above the nearest interactable and disappears on exit
- `[DEV]` `S` One-shot vs repeatable interaction handling

### E2.2 Searchable containers
- `[DEV]` `M` `SearchableContainer : Interactable` — plays an open animation or sprite swap, marks itself searched, fires its contents
- `[DEV]` `S` `LootTable` scriptable object: key / heart / nothing, plus a flavour text line
- `[DEV]` `S` Searched state is visually obvious (open lid, different sprite) so the player never re-searches by accident
- `[DEV]` `S` Flavour text popup that reads the line and dismisses on any input

### E2.3 Content authoring path
- `[DEV]` `S` Make flavour text authorable from a plain data file (CSV or scriptable object list), not hardcoded
- `[BOTH]` `S` Show the kids how to add a new line without opening a script

**Done when:** an adult can place a new searchable barrel with custom contents
and a custom joke in under two minutes, and a child can add a joke without
help.

---

## E3 — Keys, doors & progression  `M1`

### E3.1 Keys
- `[DEV]` `S` `Key` pickup with an ID; collecting it updates game state
- `[DEV]` `S` `GameState` singleton tracking which keys are held
- `[DEV]` `S` Key collection moment: pause, jingle, the hero reacts

### E3.2 The Big Door
- `[DEV]` `M` `BigDoor : Interactable` — shows three keyholes, fills them in as keys arrive
- `[DEV]` `S` Refuse to open with a line of dialogue if keys are missing
- `[DEV]` `S` Open sequence when all three are in, transitioning to the win state

### E3.3 Placement
- `[BOTH]` `M` Decide and place the three key locations per the GDD: obvious chest, one-of-many barrels, awkward-to-reach spot
- `[PLAY]` `S` Confirm all three are findable without being told where they are

**Done when:** you can find three keys, open the door, and see a win screen.
**This is M1 complete — stop and play it.**

---

## E4 — Combat & enemies  `M2`

### E4.1 Damage framework
- `[DEV]` `M` `Health` component — current/max, `TakeDamage`, `Die`, invulnerability window after being hit
- `[DEV]` `S` `Hitbox` / `Hurtbox` on player and enemy layers
- `[DEV]` `S` Wire player melee damage to the character controller's `On Attack Hit` event — do not write attack timing by hand
- `[DEV]` `S` Wire enemy damage to the monster animation clips' existing `On Attack` event

### E4.1b The spell
- `[DEV]` `M` Magic bolt projectile prefab — travels, collides, damages, despawns on hit or after a distance
- `[DEV]` `S` Define the spell as data (projectile prefab, cooldown, damage, cast sound) rather than hardcoding the bolt — costs nothing now, makes a second spell a small job later
- `[DEV]` `S` Assign it to the controller's `Projectile Prefab` and tune `Projectile Speed`
- `[DEV]` `S` Spell cooldown timer, with melee always available as the fallback
- `[DEV]` `S` Staff crystal glows when ready, dims while recharging — no mana bar, no HUD
- `[DEV]` `S` Bolt can light an unlit candle from range
- `[PLAY]` `M` Tune the cooldown until ranged attack feels useful but not dominant — if you can clear a room without ever swinging the staff, it's too short
- `[DEV]` `S` Hit reaction: `Injured Front` / `Injured Back` animations, knockback, brief hit-stop
- `[DEV]` `S` Player death → respawn at last lit candle, hearts restored, enemies reset

### E4.2 Enemy AI

> **Architecture note.** Because the three enemies are now *different* monster
> prefabs rather than variants of one, the AI can't live on a shared base
> prefab. It becomes a `MonsterAI` component plus a `BehaviourProfile`
> scriptable object, droppable onto any monster in the pack. All ten monsters
> expose the same `Monster Controller` input fields, so this works universally —
> and adding a fourth enemy later costs one prefab and one profile asset.

- `[DEV]` `L` `MonsterAI` state machine — Idle, Patrol, Chase, Attack, Dead — writing `Input Move`, `Input Move Modifier`, `Input Jump`, `Input Attack` on `Monster Controller`
- `[DEV]` `S` `BehaviourProfile` scriptable object: speeds, health, detection radius, attack range, attack cooldown, aggression
- `[DEV]` `S` Delete `Monster Input Mouse and Keyboard` from all enemy prefabs
- `[DEV]` `S` Player detection radius and line of sight
- `[DEV]` `S` Ledge detection so patrolling enemies don't walk off into the void
- `[DEV]` `S` De-aggro and return to patrol when the player escapes
- `[DEV]` `S` Randomised Z position on spawn/placement to avoid the sprite-interleave glitch when enemies cluster

### E4.3 The three enemy types
- `[DEV]` `S` **Spider** (fast) — high run speed, low health, short attack cooldown, `Can Attack When Moving` on
- `[DEV]` `S` **Zombie** (normal) — baseline tuning; use the four skin variants across the map for free variety
- `[DEV]` `S` **Orc** (brute) — low speed, high health, long wind-up, `Can Attack When Moving` **off** so it commits to its swing
- `[BOTH]` `S` Confirm all three read as different creatures from across a dimly lit room
- `[PLAY]` `M` Tune until each type *feels* like its archetype

> Stretch only, once M2 is done: Skeleton Archer is the cheapest additional
> enemy, because its `On Attack` event is already wired to a `Projectile
> Launcher` in the pack. Bat and Ghost need `Monster Flying Controller` and a
> second AI path — a bigger job than it looks.

### E4.4 The Mimic
- `[DEV]` `M` Mimic disguised as a chest using `Pixel Monster`'s `Is Hiding`; reveals and attacks when searched
- `[BOTH]` `S` Place it somewhere that will get a genuine reaction

**Done when:** all four enemy types work, fights are fair, and dying is annoying
rather than punishing.

---

## E5 — Candles, lighting & checkpoints  `M2`

### E5.1 Candle mechanic
- `[DEV]` `M` `Candle : Interactable` — unlit by default, lights on interact, stays lit
- `[DEV]` `S` Light source enabled on lighting, with a flicker
- `[DEV]` `S` Flare and room-brightening moment when lit
- `[DEV]` `S` Lit state persists across player death

### E5.2 Memory hints
- `[DEV]` `M` Memory display: dim the screen, show the text, dismiss on input
- `[DEV]` `S` Memory text stored as data per candle, authorable without code
- `[DEV]` `S` Re-reading a lit candle's memory should be possible (kids will want to)

### E5.3 Checkpoints
- `[DEV]` `S` Lighting a candle sets it as the respawn point
- `[DEV]` `S` Respawn restores hearts and resets enemies, but never resets keys or searched containers

### E5.4 Lighting pass
- `[DEV]` `M` Start the dungeon darker than the demo scene so lighting candles is visibly worth doing
- `[PLAY]` `S` Confirm the map is navigable but uninviting when unlit, and warm when lit

**Done when:** the dungeon gets visibly better as you explore it, and you never
lose more than a room's worth of progress.

---

## E6 — Content & writing  `M3`

> Mostly kid-owned. Start collecting this early — it can be written long before
> the systems are ready.

- `[KID]` `M` Name the game
- `[KID]` `S` Name the hero and decide what the keys actually unlock
- `[KID]` `M` Build the hero in the character customiser — materials for hair, hat, cloth, pants, shoes; pick a weapon
- `[KID]` `L` Write ~20 chest and barrel flavour text lines
- `[KID]` `M` Write ~8 candle memories, each hinting at a key without naming it
- `[KID]` `S` Write the sign and plaque text
- `[BOTH]` `S` Write the intro and win screen text
- `[DEV]` `S` Load all written content into the game and check nothing overflows its text box

**Done when:** every line of text in the game was written by a child.

---

## E7 — UI & game flow  `M2`

> **Design constraint: the HUD is as close to nothing as possible.** No health
> bar, key counter, objective text, minimap or timer. If a piece of information
> can be shown in the world instead of on the screen, it is.

### E7.1 In-world status (replaces most of the HUD)
- `[DEV]` `M` Key silhouettes attached to the hero's belt, appearing as each key is collected
- `[DEV]` `S` Big Door keyholes fill in to match — the goal itself shows progress
- `[DEV]` `S` Interaction prompt: small floating icon over the nearest searchable object, no text or button legend

### E7.2 Fade-in hearts
- `[DEV]` `S` Three hearts, hidden by default
- `[DEV]` `S` Fade in on damage, on healing, and while enemies are engaged
- `[DEV]` `S` Fade out ~2s after combat ends
- `[DEV]` `S` Screen-edge redness scaling with damage taken, so danger is felt before it's counted
- `[PLAY]` `S` Confirm a child can always tell how close to death they are — if not, lengthen the fade-out rather than making the hearts permanent

### E7.3 Screens
- `[DEV]` `M` Title screen — title, Play, Quit
- `[DEV]` `S` Win screen — outro text, time taken, chests searched, Play Again
- `[DEV]` `S` Pause menu with resume and quit
- `[DEV]` `S` Scene flow: Title → Dungeon → Win → Title
- `[KID]` `S` Choose fonts and colours within what TMP supports

**Done when:** the game can be started, played, won and restarted without
touching the editor.

---

## E8 — Audio  `M3`

- `[DEV]` `S` Audio mixer with Master / SFX / Music groups
- `[BOTH]` `M` Source or record sounds: footsteps, spell cast, bolt impact, staff swing, hit, chest creak, candle whoosh, key jingle, door unlock, monster noises
- `[DEV]` `S` Wire footsteps to the controller's existing `On Footstep` event
- `[DEV]` `S` Wire combat sounds to the attack events, including a small chime when the spell comes off cooldown
- `[DEV]` `S` Wire interaction sounds to `OnInteracted`
- `[DEV]` `S` Dungeon ambience loop
- `[KID]` `S` Record silly hero voice lines for key pickups — optional, and the single highest fun-per-hour item in this plan

**Done when:** playing with sound on is clearly better than playing with it off.

---

## E9 — Level dressing & beautiful corner pass  `M4`

- `[BOTH]` `M` Dress the map: place searchable containers, candles, signs across all rooms
- `[BOTH]` `S` Make sure each room has a reason to be entered
- `[DEV]` `M` Lighting polish — candle placement, colour temperature, shadow reads
- `[DEV]` `S` Post-processing tuning on the bundled profile
- `[DEV]` `M` Juice pass: hit-stop, screen shake on the brute's swing, particle on key pickup, door-opening flourish
- `[DEV]` `S` Transition polish — fades between scenes, no hard cuts
- `[PLAY]` `S` Take screenshots. If they look like a real game, this epic is done.

---

## E10 — Playtest & polish  `M4`

- `[PLAY]` `M` Full playthrough with each kid separately, watching without helping
- `[DEV]` `S` Write down every place they got stuck or confused — do not fix anything during the session
- `[DEV]` `M` Fix the top five confusions
- `[PLAY]` `S` Playtest with someone outside the family
- `[DEV]` `M` Difficulty tuning pass based on what you watched
- `[DEV]` `S` Fix the bug list
- `[PLAY]` `S` Time a full playthrough and confirm it lands in the 10–20 minute target

> The hardest and most valuable rule here: when watching a kid play, **say
> nothing**. Every hint you give is a bug you won't fix.

---

## E11 — Build & ship  `M4`

- `[DEV]` `S` Windows standalone build
- `[DEV]` `S` Mac build if needed
- `[DEV]` `S` Test the build on a machine that has never had Unity on it
- `[DEV]` `S` Icon and window title
- `[BOTH]` `S` Decide how to share it — a zip to grandparents, or an itch.io page
- `[KID]` `S` Write the description and choose the screenshots
- `[DEV]` `S` Tag the release in git

---

## Critical path

Everything else can move around. This sequence cannot:

```
E0 Setup
  → E1 Player
    → E2 Interaction system        ← the long pole
      → E3 Keys & door             ← M1: playable loop
        → E4 Combat  +  E5 Candles ← M2: feature complete
          → E6 Content + E8 Audio  ← M3: content complete
            → E9 Dressing
              → E10 Playtest
                → E11 Ship         ← M4: beautiful corner
```

## Risks worth watching

**The interaction system sprawls.** It's the piece with no vendor support and
the most tempting scope. Time-box it: if E2 isn't done when the budget runs
out, ship the version that works and stop adding container types.

**The lighting decision gets revisited.** URP 3D is locked. Changing to 2D after
E5 means redoing every light. If candle glow doesn't read well, that argument
happens in week one or not at all.

**Content generation outruns the systems.** The kids will write forty chest
jokes before the chest system exists. That's fine and good — just make sure E2.3
lands early enough that the writing has somewhere to go.

**Enthusiasm outruns the slice.** Someone will want a boss, a second level, or a
pet dragon. The GDD's out-of-scope list exists for exactly this conversation.
Write the idea on a "next game" list rather than saying no.
