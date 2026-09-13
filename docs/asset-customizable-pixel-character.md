# Customizable Pixel Character (Cainos)

Our player character. Modular, skeletal-animated, with a controller that already
handles most of the traversal we need.

- Store: <https://assetstore.unity.com/packages/2d/characters/customizable-pixel-character-177945>
- Docs: <https://docs.cainos.net/customizable-pixel-character>

## Quick start

Drag a preset from `Cainos/Customizable Pixel Character/Prefab/Character Preset`
into the scene. That's the whole setup. Appearance is edited in the
`Customization` foldout of the `Pixel Character` component.

Every preset is a prefab variant of `Cainos/Customizable Pixel
Character/Prefab/PF Pixel Character`, so a change that should apply to all
characters goes on that base prefab only.

## Default controls

Keyboard and mouse: A/D move, Shift to move fast, Ctrl to dodge, Shift plus a
double-tap of A or D to dash, C to crouch, Z to crawl, Space to jump, left mouse
to attack, hold right mouse to look, V for melee, W/S to climb ladders, hold S
to drop through a platform.

Controller is supported too (left stick to move, south button to jump, right
trigger to attack, and so on) but **only after importing the `Patch - Input
System` package**, which also requires the Input System package to be installed.

## Scripts

Source: <https://docs.cainos.net/customizable-pixel-character/script-reference.md>

Three components sit on the character object:

| Component | Role |
|---|---|
| `Pixel Character` | Holds references to the parts of the character and all appearance customisation |
| `Pixel Character Controller` | All movement, attack and state logic |
| `Pixel Character Input Mouse and Keyboard` | Reads input and writes it into the controller |

Two more sit on the inner `Animator` object — `Animation Event Receiver` and
`Root Motion Receiver`. We normally leave these alone.

There is also a `Pixel Character Audio` script included but **not attached by
default**. The vendor describes it as a starting point rather than a finished
system, so if we want footsteps and swings we either wire that up or use the
events below.

### `Pixel Character` — what we'll touch

Appearance is driven almost entirely by swapping materials: hat, hair, eyes, eye
base, facewear, cloth, pants, socks, shoes, back and body each have their own
material slot. Three booleans matter when fitting hats: `Clip Hair` hides part
of the hair, `Hide Hair` hides all of it, and `Shoes in Front` controls whether
shoes draw over the pants.

Runtime-only properties include `Facing` (also settable in the editor to choose
the starting direction), `Alpha`, `Expression`, `Injured Front` / `Injured Back`
to play hit reactions, and `Is Dead`. Note that `Is Dead` here only changes the
*visuals* — to actually stop the character you must also set `Is Dead` on the
controller.

### `Pixel Character Controller` — what we'll tune

Movement exposes separate max speed and acceleration for walk, run, crouch,
crawl, air and swim, plus ground and air drag. Jump has speed, cooldown,
tolerance (a coyote-time window where you can still jump just after leaving the
ground), and separate gravity multipliers for rising and falling — lowering the
jump multiplier is what gives variable jump height from holding the button.
Dash, dodge, swim, ladder climb and ledge climb each have an enable toggle plus
their own speed and cooldown values.

Attack exposes a primary attack action and a melee attack action, an attack
speed multiplier, a cooldown, throw force and angular speed, and a projectile
speed and prefab for cast/archery style attacks.

**The Event foldout is the important one for us.** It exposes UnityEvents for
`On Footstep`, `On Jump`, `On Land`, `On Dodge Start` / `End`, `On Attack Start`
/ `On Attack Hit` / `On Attack End`, `On Bow Pull`, `On Bow Shoot` and
`On Throw`.

> `On Attack Hit` is explicitly described as the moment damage should be
> applied. That is the hook our whole combat system hangs off — we do not need
> to write animation timing code, we just subscribe to this event.

The Input foldout mirrors the controller's inputs — `Input Move`, `Input Run`,
`Input Dash`, `Input Dodge`, `Input Crouch`, `Input Crawl`, `Input Jump`,
`Input Attack`, `Input Melee`, `Input Look`, `Input Target`. To use our own
input (or to have an AI drive a character) we remove the bundled input script
and write these fields ourselves. The vendor points at the bundled script as a
worked example of exactly that.

## Interactable objects that already exist

Source: <https://docs.cainos.net/customizable-pixel-character/interactable-objects.md>

Only two, and both are traversal rather than interaction:

**Ladder** — needs a Box Collider with `Is Trigger` on to define the climbable
area, a `Ladder` script with `Direction` set to Left or Right, and a `Climb Pos`
child transform marking where the character snaps to while climbing.

**Platform** — a one-way platform. Needs a collider with `Used By Effector`
enabled, a `Platform Effector 2D` whose `Collider Mask` excludes the player
layer, and a `Platform` script. Holding down while standing on it drops the
character through.

Everything else — chests, doors, candles, levers, pickups — does not exist and
is ours to write.

## Customisation

Source: <https://docs.cainos.net/customizable-pixel-character/customization.md>

Gender is a combination of `Body Material` and `Eye Base Material`, plus a hair
material that suits. Skin tone is the `Skin Tint` parameter on the body
material. Hairstyle and hair colour are two separate textures on the hair
material — `Main Texture` for the style, `Ramp Texture` for the colour. Not
every style/colour combination ships as a ready-made material, but duplicating
an existing hair material and swapping those two textures creates any
combination we want.

Weapons are prefabs in `Cainos/Customizable Pixel Character/Prefab/Weapon`,
dragged into the character's `Weapon Slot`.

> This is the part of the project most suited to the kids. Building their own
> hero from materials is drag-and-drop, immediately visible, and impossible to
> break anything important with.

## Making new animations

Source: <https://docs.cainos.net/customizable-pixel-character/making-your-own-animations.md>

Short version, in case we want a custom move later:

1. Open a character prefab in Prefab Editing mode, select the `Animator` child,
   and create a clip in the Animation window. Only key objects under `Animator`;
   the bones live under `Rig`.
2. Unity's Animation Rigging package provides a `Bone Renderer` script that
   makes bones visible and selectable in the scene, which makes this far less
   painful.
3. Add the clip to the Animator Controller as its own layer with an empty entry
   state, a trigger parameter, and a transition back to empty when the clip
   finishes.
4. Fire the trigger from code.

Two gotchas worth remembering. The weapon in `Weapon Slot` only follows the
`Rig Weapon` bone at runtime, so for animating you need to temporarily parent it
under that bone — which is why the vendor recommends animating on a *duplicate*
of the prefab rather than the one used in game. And because all characters share
the same rig, any animation made on one character works on all of them.

## Setup requirements

Source: <https://docs.cainos.net/customizable-pixel-character/setup-guide.md>

**Input handling in Unity 6.** The pack defaults to the legacy Input Manager,
which Unity 6 no longer enables by default — so inputs will simply not respond
out of the box. Two fixes: set `Active Input Handling` to `Both` in
`Project Settings > Player`, or import `Patch - Input System` to move to the new
Input System. Setting it to `Input Manager (Old)` does **not** work if the new
Input System package is present.

**Layer collisions.** The character and its weapon must be on different layers
that do not collide with each other, or the character collides with its own
sword. The vendor's convention is `Player` and `Player Weapon`. Importing the
pack as a complete project brings these settings along; importing into an
existing project means setting them up by hand.

**Transparency sort axis.** Set `Transparency Sort Mode` to `Custom Axis` and
`Transparency Sort Axis` to `(0,0,1)` in Graphics settings.

**Skin weights.** `Project Settings > Quality` → `Skin Weights` at `2 Bones` or
higher, otherwise animation quality degrades.

**Colour space.** Linear, same as the dungeon pack.

**Lighting patches — version specific.** By default the character shaders are
unlit and work in any pipeline. For lighting we import a patch, and *which*
patch depends on both the lighting model and the Unity version:

| Lighting | Unity version | Patch |
|---|---|---|
| URP 2D | Before Unity 6 | `Patch - URP 2D Lit` |
| URP 2D | Unity 6.0 – 6.2 | `Patch - URP 2D Lit - Unity 6.0 to 6.2` |
| URP 2D | Unity 6.3 or newer | `Patch - URP 2D Lit - Unity 6.3 or newer` |
| URP 3D | Before Unity 6 | `Patch - URP Lit` |
| URP 3D | Unity 6.x | `Patch - URP Lit - Unity 6` |

Character lighting only supports URP.

**TextMeshPro.** The demo UI uses TMP. On first open you should get a prompt to
import TMP Essentials; complete it and reopen the scene. If the prompt doesn't
appear, selecting any TMP object in the Hierarchy triggers it.

## Known rendering gotchas

**Two characters standing close together glitch.** They occupy the same Z space
and their sprites interleave. Fix by giving them different Z positions, and set
the character's Z *scale* to something small like `0.1` — but never `0.0` — so
each takes up less Z depth. This will absolutely bite us when a skeleton walks
into the player, so it goes into the enemy prefab setup from the start.

**Parts of the character disappearing behind a background.** Same root cause —
the character has real 3D depth, so a background at the same Z occludes it. Push
the background to a larger Z value.
