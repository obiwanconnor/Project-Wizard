# Where Are My Keys?! — game scripts

Vertical-slice scripts for *Where Are My Keys?!*, the family dungeon
platformer. The Unity project now exists (`ProjectSettings/`,
`Packages/manifest.json`) and the **Pixel Art Platformer – Dungeon**
environment pack is imported under `Assets/Cainos/`. The **Customizable
Pixel Character** and **Pixel Art Monster – Dungeon** packs are not yet
imported — see `docs/project-setup-checklist.md` for the remaining steps.
The `Assets/_Game/` content below is meant to keep working unchanged as the
rest of the packs land.

See `docs/GDD.md` for the design this implements and `docs/DELIVERY-PLAN.md`
for how it maps onto epics/stories.

## What's real vs. what's a stub

Everything under `Assets/_Game/` is real, self-contained C# with no
dependency on the Cainos asset packs — it doesn't know they exist. It's
organised into an assembly definition (`WhereAreMyKeys.asmdef`) precisely so
that it *can't* accidentally reference Cainos types; the compiler enforces
the boundary described below.

`Assets/Rigs/PlayerRig.cs` and `Assets/Rigs/MonsterRig.cs` are the *only*
two files that are allowed to know about both sides — our game logic and the
real Cainos controllers. They're full of `// TODO` comments because I don't
have the Cainos source, only the vendor docs, which confirm field and event
*names* but not their real C# types or signatures. Fixing those two files
against the real `PixelCharacterController.cs` / `MonsterController.cs` is
the one Unity-side job standing between this repo and a running slice.

## Why the asmdef boundary works

Unity compiles unmarked scripts (no `.asmdef` covering them) into the
predefined `Assembly-CSharp`. Assembly definitions can't reference
`Assembly-CSharp` — it's compiled last. So:

- The Cainos packs ship with no `.asmdef` → they land in `Assembly-CSharp`.
- `Assets/_Game/WhereAreMyKeys.asmdef` covers everything under `_Game/` →
  that code physically cannot reference Cainos types, even by accident.
- `Assets/Rigs/` has **no asmdef of its own** on purpose, so it also lands
  in `Assembly-CSharp` — which *can* see both the Cainos scripts and
  `WhereAreMyKeys.asmdef` (predefined assemblies can reference asmdefs).

That's the whole trick: two small adapter files carry all the risk, and
the compiler won't let that risk leak anywhere else.

## Assumptions baked in (check these first)

Written down here so a wrong guess is a five-minute fix, not a scavenger
hunt through every script:

| Assumption | Why | Where it shows up |
|---|---|---|
| **Physics2D** (`Rigidbody2D`, `Collider2D`, `OnTriggerEnter2D`) | The three Cainos packs are 2D pixel-art packs | `Interaction/`, `Combat/Projectile.cs` |
| **`UnityEngine.Light` (3D light), not URP `Light2D`** | Matches the locked decision: "URP 3D lighting — the dungeon demo scene already ships lit this way" | `Interaction/Candle.cs` |
| **New Input System**, a `PlayerInput` with `Cast` / `Interact` actions | Locked decision: "New Input System — import the character pack's Patch - Input System" | `Rigs/PlayerRig.cs` |
| **uGUI + TextMeshPro** for the memory-text overlay | Most common pairing for a project this size; swap `TMPro.TMP_Text` for legacy `Text` and drop the `Unity.TextMeshPro` asmdef reference if the project isn't using TMP | `UI/MemoryLog.cs`, `WhereAreMyKeys.asmdef` |
| **Unity 6.3 LTS API** (`Rigidbody2D.linearVelocity`, not `.velocity`) | Locked decision on the Unity version | `Combat/Projectile.cs` |
| **Single scene**, singletons via `Instance` (no DI framework) | Matches "vertical slice / beautiful corner" scope — this is a one-map game | `KeyRing`, `CheckpointManager`, `GameFlowController`, `MemoryLog` |

If any of these turn out wrong, the fix is contained to the file(s) listed —
nothing else in `_Game/` assumes them.

## Layout

```
Assets/
  _Game/                         WhereAreMyKeys.asmdef — pure game logic
    Interaction/                 Interactable base + Candle, Chest, Sign, HeartPickup, BigDoor
    Data/                        ScriptableObjects: FlavourTable, CandleMemory, SpellDefinition, EnemyBehaviourProfile
    Combat/                      Health, SpellCaster (+ pure SpellCooldownState), Projectile
    AI/                          MonsterAI (+ pure MonsterStateMachine): Idle → Patrol → Chase → Attack → Dead
    GameState/                   KeyRing, CheckpointManager, GameFlowController
    UI/                          HeartsDisplay, KeyBeltDisplay, MemoryLog
  _Game.Editor/                  WhereAreMyKeys.Editor.asmdef — FlavourCsvImporter (Where Are My Keys ▸ Import Flavour Text From CSV...)
  _Game.Tests/                   WhereAreMyKeys.Tests.asmdef — EditMode tests, no scene required
  Rigs/                          NO asmdef, deliberately. PlayerRig.cs, MonsterRig.cs — the Cainos adapter boundary
                                  (stubs, pending the character/monster packs). ChestRig.cs — same boundary, but
                                  live: the environment pack is already imported, so it wires our Chest.OnOpened
                                  straight to the real Cainos Chest.Open(). WhereAreMyKeys.inputactions — our own
                                  Cast/Interact actions; assign this (or the character pack's own action asset,
                                  if it turns out to already have a Cast-equivalent) to PlayerInput, since the
                                  project's default InputSystem_Actions template has no Cast action.
docs/                            GDD, delivery plan, and the vendor-doc notes from earlier planning
```

## Design choices worth knowing before you extend this

- **Pure logic is split from `MonoBehaviour`s wherever it mattered for
  testing.** `SpellCooldownState`, `KeyRingState`, and `MonsterStateMachine`
  are plain C# classes with no Unity lifecycle — the `MonoBehaviour`
  wrappers (`SpellCaster`, `KeyRing`, `MonsterAI`) just tick them and wire
  Unity-side concerns (events, Inspector fields). That's what makes the
  EditMode tests run without a scene.
- **`Interactable` doesn't do its own trigger detection.**
  `PlayerInteractor` owns a single overlap check, tracks the nearest
  interactable, and shows one reused `InteractionPromptView` — rather than
  every chest/candle/sign running its own trigger and spawning its own
  prompt. Cheaper, and there's one place to change if "nearest" needs to
  become "look direction" later.
- **The spell is data**, per the GDD's own "headroom worth protecting"
  section — `SpellDefinition` is a ScriptableObject, so a second spell is a
  new asset, not new code.
- **`MonsterAI` never touches the Cainos controller.** It computes a
  `MonsterIntent` (move direction, wants-attack, facing) every frame;
  `MonsterRig` is the only thing that reads it and pushes it into the real
  controller fields.

## Finishing the integration

1. Create the Unity 6.3 LTS project (URP, 3D lighting) per
   `docs/project-setup-checklist.md`, import the three Cainos packs and
   their Unity 6 / Input System patches.
2. Copy `Assets/_Game/`, `Assets/_Game.Editor/`, `Assets/_Game.Tests/` and
   `Assets/Rigs/` into the project's `Assets/` folder. Let Unity generate
   `.meta` files on import — none are checked in here.
3. Open `Rigs/PlayerRig.cs` and `Rigs/MonsterRig.cs`. Replace
   `controllerPlaceholder` with the real Cainos types and work through the
   `// TODO` comments — each one names the documented field/event it's
   standing in for.
4. Create the `ScriptableObject` assets (`SpellDefinition`, one
   `EnemyBehaviourProfile` per archetype, `CandleMemory` per candle,
   `FlavourTable` per searchable object) via the `Where Are My Keys` menu.
5. Wire prefabs in the Inspector: `Interactable` subclasses, `Health`,
   `SpellCaster`, `PlayerInteractor`, `MonsterAI`, the two Rigs, and the UI
   displays.
6. Run the EditMode tests (`Window ▸ General ▸ Test Runner ▸ EditMode`) —
   they don't need the Cainos packs installed at all, so they'll pass
   before step 3 is even done.

## Tests

`Assets/_Game.Tests/` covers the pure logic: spell cooldown gating, key
counting, and the monster state machine's transitions (notice radius, attack
range, the "dead overrides everything" rule). None of it needs a scene, a
prefab, or the Cainos packs — that's deliberate, so these can be green from
the very first commit.
