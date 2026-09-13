# Pixel Art Platformer – Dungeon (Cainos)

Environment / tileset pack. This is the world our game lives in, and its demo
scene is the map we are shipping the vertical slice on.

- Store: <https://assetstore.unity.com/packages/2d/environments/pixel-art-platformer-dungeon-237109>
- Docs: <https://docs.cainos.net/pixel-art-platformer-dungeon>
- Support: support@cainos.net

## What it gives us

- A dungeon tileset with collider shapes already built into the tiles.
- Props: chests, doors, candles, barrels, crates, banners, pillars, bones and
  similar dressing.
- A fully dressed demo scene with lighting and post-processing already set up.
- A bundled **Lucid Editor** tool (added in pack v1.1.7) for level editing.
- Its own URP Render Pipeline Asset and post-processing profile, at
  `Cainos/Pixel Art Platformer – Dungeon/Rendering`.

Props are visual only. None of them have interaction behaviour attached — see
the interaction epic in the delivery plan.

## Setup requirements

Source: <https://docs.cainos.net/pixel-art-platformer-dungeon/setup-guide.md>

**Colour space.** Set to `Linear` in `Project Settings > Player > Other
Settings`. The pack is authored for it.

**Render pipeline.** The demo scene needs URP. Install URP via Package Manager,
then assign the Render Pipeline Asset that ships with the pack rather than
making a new one.

**Shader errors.** If shaders break after installing URP, select the pack's
`Shader` folder in the Project window, right-click, and Reimport. This is a
known and expected step, not a sign anything is broken.

**2D vs 3D lighting — decide once, early.** The demo scene ships using URP *3D*
lighting. To switch to URP *2D* lighting, import the `Patch - URP 2D Lit`
package from the pack root and then reopen the demo scene. If the scene comes
back very dark after importing, import the patch a second time and reload — the
vendor documents this as normal.

> This choice has to be made before we dress or light any rooms, because
> redoing lights afterwards is real rework. It also has to match the choice we
> make for the character and monster packs, which have their own separate 2D/3D
> lit patches. See the setup checklist.

**Post-processing.** The demo scene already has it configured, and the vendor's
own before/after comparison shows it makes a large difference to how the art
reads. The profile is at `Cainos/Pixel Art Platformer – Dungeon/Rendering/Post
Processing Profile`. If we ever start a scene from scratch, we have to add post
-processing ourselves — but since we are building on the demo scene, we inherit
it.

## Version history worth knowing

Source: <https://docs.cainos.net/pixel-art-platformer-dungeon/changlog.md>

- **1.1.8** — Unity 6.5 support.
- **1.1.7** — Bundled Lucid Editor updated to 1.1.0.
- **1.1.4** — Fixes for minor Unity 6 issues.
- **1.1.0** — URP 2D lighting support added (this is where the 2D Lit patch
  came from).
- **1.0.3** — Tileset collider shapes improved, editor script improvements.

Practical takeaway: make sure we are on the latest version before we start, so
we get the improved collider shapes and the Unity 6 fixes.
