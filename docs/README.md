# Asset Documentation — Index

Condensed working notes for the three Cainos asset packs this project is built
on. These are **summaries in our own words**, organised for how we will actually
use them, with links back to the canonical vendor pages. When something looks
wrong or out of date, the vendor page wins.

All three packs are published by Cainos. The full vendor documentation index
lives at <https://docs.cainos.net/llms.txt> — every page there is also available
as Markdown by appending `.md` to the URL, which makes it easy to re-pull.

| Pack | Our notes | Vendor docs |
|---|---|---|
| Pixel Art Platformer – Dungeon | [asset-pixel-art-platformer-dungeon.md](asset-pixel-art-platformer-dungeon.md) | <https://docs.cainos.net/pixel-art-platformer-dungeon> |
| Customizable Pixel Character | [asset-customizable-pixel-character.md](asset-customizable-pixel-character.md) | <https://docs.cainos.net/customizable-pixel-character> |
| Pixel Art Monster – Dungeon | [asset-pixel-art-monster-dungeon.md](asset-pixel-art-monster-dungeon.md) | <https://docs.cainos.net/pixel-art-monster-dungeon> |

Also in this folder:

- [project-setup-checklist.md](project-setup-checklist.md) — the combined,
  ordered setup steps across all three packs. **Read this before touching the
  project.** The packs each have their own setup requirements and several of
  them conflict or have to be done in a particular order.

## The three things that surprised us

1. **This is a side-view platformer, not top-down.** Exploration means connected
   rooms, ladders and one-way platforms, with backtracking — not walking around
   a map from above.

2. **Chests, doors, barrels and candles are art, not interactables.** The only
   scripted interactables that ship with the character pack are ladders and
   one-way platforms. Everything else — press-to-interact, opening a chest,
   unlocking a door, lighting a candle — is ours to build. This is the single
   biggest chunk of engineering in the slice.

3. **Monsters have animation and movement but no AI.** Each monster prefab
   exposes input fields (`Input Move`, `Input Attack`, etc.) that a keyboard
   script feeds in the demo. We strip that script out and feed the same fields
   from our own AI. That is a much smaller job than writing movement from
   scratch, and it is how we get fast / normal / brute variants cheaply.

## Licensing note

These packs are licensed under the standard Unity Asset Store EULA
(Single Entity, Extension Asset). Fine for a personal / family project and for
sharing builds. If this ever turned into something commercial, re-check the
licence terms before distributing.
