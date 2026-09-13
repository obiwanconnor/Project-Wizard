# Game Design Document — *Where Are My Keys?!*

**Working title.** Naming the game is a kid decision, not an adult one. Leave
this placeholder in until they've argued about it properly.

| | |
|---|---|
| **Format** | 2D side-view platformer, single player |
| **Scope** | Vertical slice / beautiful corner |
| **Playable space** | The Cainos dungeon pack's demo map, dressed and extended |
| **Session length** | 10–20 minutes to complete |
| **Audience** | Us, and whoever the kids show it to |
| **Platform** | Windows / Mac standalone, keyboard and gamepad |

---

## 1. The pitch

A wizard has lost his keys. Again. They are somewhere in this dungeon, because
that is where he was last night, and he can remember almost none of it. He can
conjure fire from his fingertips. He cannot remember whether he checked his
coat pocket.

The joke is that this is a heroic fantasy adventure about the least heroic
possible problem. The dungeon takes itself entirely seriously. The wizard does
not.

And no, he cannot simply magic the door open. He knew that spell once.

## 2. What the player actually does

**Core loop:**

> Explore a room → light a candle → remember something → search the things the
> memory points at → fight whatever objects to being searched → find a key →
> reach the door

Repeat three times, then open the door and go home.

**Win condition.** Find all three keys and unlock the Big Door.

**Lose condition.** There isn't one. Running out of hearts sends the hero back
to the last candle he lit, slightly embarrassed. No game over screen, no lives,
no progress lost. This is deliberate — the point is exploring and laughing, and
a death screen would only interrupt that.

## 3. The three pillars

**Everything in the dungeon can be searched.** Chests, barrels, crates, bone
piles, doors, sarcophagi. Most of them contain nonsense. The nonsense is the
reward — the flavour text is where the comedy lives, and it's the part the kids
can write themselves.

**Candles are the backbone of the game.** Lighting a candle does three jobs at
once: it lights the room so you can see what's in it, it triggers a memory that
hints at where a key might be, and it becomes your checkpoint. One interaction,
three payoffs. This is the mechanic the whole design hangs off.

**Combat is simple and readable.** Three creature types with obviously different
speeds and weights. You should be able to tell what's coming at you from across
the room by how it moves.

## 4. The memory mechanic

Each candle, when lit, plays a short memory as text over a dimmed screen. The
memories are hints, but unreliable ones — the hero half-remembers.

Examples of the tone:

> *"I definitely had them when I came in. Or when I left. One of those."*

> *"There was a chest. A big one. Or was it a barrel? It was container-shaped."*

> *"I remember thinking: nobody would ever look in here. Which was clever of me
> at the time and is now the problem."*

Candles also serve the practical purpose of making the dungeon progressively
brighter as you explore it, which gives a nice sense of progress without a
progress bar.

Lighting them is the same spell he attacks with, which is a small thing that
makes the fiction hold together: the one bit of magic he *can* reliably do is
setting things on fire, and he uses it for absolutely everything.

**Design rule:** every key location is hinted by at least one candle memory, but
no memory names the location outright. The player should feel clever, not
instructed.

## 5. The keys

Three keys, each in a different kind of hiding place, so each one teaches a
different search behaviour:

| Key | Where | What it teaches |
|---|---|---|
| **Key 1** | In an obvious chest, guarded by a couple of zombies | Interact with things; fight a bit |
| **Key 2** | In one of several identical barrels, and the wrong ones contain jokes | Search everything, don't give up |
| **Key 3** | Requires reaching somewhere awkward — a high ledge, or dropping through a one-way platform into a hidden room | Use the traversal moves properly |

The Big Door shows three keyholes, filling in as keys are collected, so progress
is always visible.

## 6. Combat

**The wizard.** Two attacks, both already supported by the character
controller — we are not building a combat system, we are configuring one.

- **Spell** (primary attack, left mouse / right trigger). The controller has a
  `Cast` attack action with `Projectile Speed` and `Projectile Prefab` fields.
  We supply a magic bolt prefab and it works.
- **Staff whack** (melee, V / right shoulder). The controller's separate
  `Attack Action Melee` slot, and the fallback when the spell is recharging. It
  also fires automatically in states where the primary attack isn't available,
  such as on a ladder.

Dodge and dash are already in the controller — leave them enabled, they cost
nothing and make movement feel good. Three hearts.

**The spell has a cooldown, and no mana bar.** A resource meter would be the
single biggest thing back on a screen we just cleared. Instead the staff's
crystal glows when the spell is ready and dims while it recharges. The
information lives on the character, where the player is already looking.

The cooldown is what keeps ranged attack from trivialising the game: you get one
bolt, then you are a man with a stick until it comes back. That tension is the
whole combat design, and it's why the enemy mix matters — the Spider closes the
gap fast enough to punish a wasted cast, while the Orc's long wind-up gives you
exactly enough time to get a bolt off if you read it.

Damage is applied on the character controller's `On Attack Hit` event. We do not
write attack timing.

**The enemies.** Three archetypes, but each one a *different creature* rather
than a reskinned skeleton. Different silhouettes read far better across a dark
room than three versions of the same body:

| Archetype | Creature | Feel | Tuning |
|---|---|---|---|
| **Fast** | **Spider** | Twitchy, rushes in, dies fast | High run speed, low health, short attack cooldown, `Can Attack When Moving` on |
| **Normal** | **Zombie** | The baseline. Shambles over, swings | Middle of everything |
| **Brute** | **Orc** | Slow, heavy, telegraphed, hurts | Low speed, high health, long wind-up, `Can Attack When Moving` **off** so it commits to its swing |

Turning `Can Attack When Moving` off for the brute is what makes it fair — it
freezes during its attack, so a player who reads the wind-up can walk around it.

Zombie takes the baseline slot rather than Skeleton for a practical reason: the
pack's four zombies are pure skin variants, so we get visual variety for free.
The three skeletons are Warrior, Mage and Archer — functionally different
enemies, and two of them are ranged, which the slice doesn't support.

**The rest of the pack, kept in the back pocket.** Slime, Bat, Ghost, Reaper,
Knight and the skeletons are all there if the slice wants one more thing. Worth
knowing before reaching for them: Bat and Ghost fly, which means
`Monster Flying Controller` and a second AI path; Skeleton Archer and Mage are
ranged, which means projectiles. Skeleton Archer is the cheapest of the lot,
because the pack already wires its `On Attack` animation event to a
`Projectile Launcher`.

**AI states.** Idle → Patrol → Chase → Attack → Dead. Enemies notice the player
within a radius, chase until in attack range, swing, and return to patrol if the
player escapes. No pathfinding, no ledge awareness beyond "don't walk off".

**The Mimic.** One chest in the dungeon is a mimic. It sits there looking
exactly like every other chest until searched. The monster pack supports this
directly — `Pixel Monster` has an `Is Hiding` property that the docs note works
only for the Mimic. This is the single best gag available to us and it costs
almost nothing to build.

## 7. Interactables to build

None of these exist in the packs. All are ours.

| Object | Behaviour |
|---|---|
| **Chest** | Opens, plays flavour text, may contain a key, a heart, or junk |
| **Barrel / crate / bone pile** | Same system, different art and animation |
| **Candle** | Lights, becomes a light source, triggers a memory, sets checkpoint |
| **Big Door** | Shows keys collected; opens when all three are in |
| **Heart pickup** | Restores one heart |
| **Sign / plaque** | Pure flavour text — cheap way for kids to add jokes |

All of these share one `Interactable` base: a trigger volume, a floating prompt
when the player is in range, and an `Interact()` call. Everything else is a
subclass or a data asset. Build the base properly once and adding a new
searchable thing becomes a two-minute job — which matters, because the kids will
want to add dozens.

## 8. Look and feel

The demo scene already looks good. The job is to not ruin it, and to make the
lighting do dramatic work as candles come on.

- Dungeon pack visuals unchanged, URP with the bundled post-processing profile
- Camera follows the player with a soft lag and a small dead zone
- Candle-lighting gets a moment: a brief flare, the room brightening
- The hero is built in the character customiser by the kids — this is theirs

**HUD — as close to nothing as possible.** The screen stays clear. No health
bar, no key counter, no objective text, no minimap, no timer.

The two things the player needs to know are told in the world instead:

- **Keys** hang visibly on the hero's belt as he collects them. Three keys, three
  silhouettes on his hip. The Big Door's three keyholes fill in to match, so
  progress is readable from both the hero and the goal.
- **Health** is three hearts that are *invisible by default*. They fade in when
  you take or regain a heart, and while enemies are engaged, then fade back out
  a couple of seconds after a fight ends. Damage also reddens the screen edges,
  so danger is felt before it's counted.

The only other screen element is the interaction prompt, which is a small icon
floating over the nearest searchable thing — no words, no button legend.

> Hearts fade in rather than vanish entirely because a child needs to be able to
> check how close to death they are. Pure vignette health reads as atmosphere,
> not information. This is the minimum that stays fair.

**Audio.** Footsteps, spell cast, bolt impact, staff swing, hits, chest creak,
candle whoosh, key jingle, door unlock, plus a quiet dungeon ambience. Freesound
or similar. The character controller already fires `On Footstep`, so footsteps
are a wiring job, not a coding job.

## 9. Content inventory for the slice

- 1 dungeon map (the demo scene, dressed)
- 1 wizard character, 1 staff, 1 magic bolt projectile prefab
- 3 enemy types (spider, zombie, orc) + 1 mimic
- 3 keys, 1 big door
- ~8 candles with written memories
- ~20 searchable objects with flavour text
- 1 title screen, 1 win screen
- ~10 sound effects, 1 ambience track

## 10. Explicitly out of scope

Writing these down so they stop being argued about mid-build:

- Saving and loading
- Multiple levels
- Inventory beyond the three keys
- Character progression, XP, levelling
- Magic beyond the one bolt spell
- Boss fight
- Dialogue trees or NPCs
- Custom animation

Any of these can come after the slice is finished and playable. None of them
before.

**Headroom worth protecting.** The wizard framing leaves an obvious door open,
and the design should stay shaped so that walking through it later is cheap
rather than a rebuild:

- A second or third spell is, mechanically, a different projectile prefab and a
  different cooldown. The one thing to watch is that the controller has a single
  `Attack Action` slot, so more than one spell means either swapping the
  assigned `Projectile Prefab` at runtime or adding a thin spell-selection
  layer. Build the spell as data (projectile, cooldown, damage, cast sound) from
  the start and that stays a small job.
- The memory mechanic is the perfect delivery vehicle. Candles currently hand
  out hints about the keys. A later version could have them hand back *spells* —
  he lights a candle, remembers how to do something he used to know, and the
  player gets a new verb. Progression, story and lighting all on one
  interaction.

Neither belongs in the slice. Both are much cheaper if we don't hardcode the
bolt.

## 11. Open questions for the kids

Deliberately left undecided. These are the design decisions they should own:

1. What is the game actually called?
2. What does the hero look like? (character customiser — fully theirs)
3. What is the hero's name?
4. What are the three keys *to*? His front door? A cupboard? Something sillier?
5. What nonsense is in the chests? (Write 20 of these. This is the best job.)
6. What do the candle memories say?
7. Should there be a pet? A talking sword? Something that follows him around
   and complains?
