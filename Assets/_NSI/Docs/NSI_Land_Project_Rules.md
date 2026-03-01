# NSI Land — Project Rules & Working Agreement (Codex + Human)

> **Project:** NSI Land: Reclamation Protocol  
> **Engine:** Unity 6.3 LTS (6000.3.10f1) + URP  
> **Goal of this document:** prevent “AI drift,” protect working systems, and keep the build shippable at all times.

---

## 0) Non‑Negotiables (North Star)

1. **PC-first**: mouse/keyboard first; controller optional later.
2. **Freedom placement**: build anywhere valid; no overlap/clipping.
3. **Systems-first automation**: topology matters (power + water networks first).
4. **Visible restoration**: world reacts to performance.
5. **Ship in slices**: every milestone produces a playable build.

---

## 1) “Do Not Break” Contract (Anti-regression Rules)

## Versioning Policy

v1 = Minimal playable core (Loop-only locomotion, basic build, power/water)
v1.1 = Add responsiveness improvements (Start/Stop)
v1.2 = Add pivots + polish
v2 = Traversal expansion
v3 = Combat/Action expansion

Codex must not implement features from higher versions unless explicitly requested.

### 1.1 No silent refactors
- **No refactor** unless:
  - feature is finished, and
  - there’s a failing test / reproducible bug that the refactor fixes, and
  - a rollback plan exists.

### 1.2 Never replace whole files unless requested
- AI must **edit surgically**:
  - “replace entire file” is allowed only if explicitly requested or if file is new.
  - otherwise provide: *exact blocks to insert/replace* + file path.

### 1.3 Diff-first workflow
- Every change request must produce:
  - **What changes**
  - **Why it changes**
  - **Where it changes**
  - **How to validate**

### 1.4 Rollback required
- Before risky edits:
  - commit to git or copy the file to `*_bak_YYYYMMDD_HHMM.cs`.
- If a step breaks anything: rollback immediately.

### 1.5 No “mystery toggles”
- Any new Inspector field must state:
  - default value
  - which mode uses it
  - how it’s set (manual vs via script)

---

## 2) Unity Project Conventions

### 2.1 Folder structure (keep it stable)
```
Assets/
  _NSI/
    Core/
    Player/
    Camera/
    Animation/
    BuildSystem/
    Systems/
      Power/
      Water/
    UI/
    World/
    Data/
    Audio/
    VFX/
    Prefabs/
    Scenes/
    ThirdParty/
```

### 2.2 Scene policy
- Scenes live in `Assets/_NSI/Scenes/`
- Keep 3 permanent scenes:
  - `00_Bootstrap.unity` (loads systems, persistent singletons)
  - `01_TestIsland_Foundation.unity` (flat test world)
  - `02_Sandbox_Playground.unity` (experimental)

### 2.3 Prefab policy
- Runtime objects must be prefabs under `Assets/_NSI/Prefabs/`
- No “scene-only” gameplay objects unless explicitly marked with `// SCENE-ONLY`.
- Every buildable must be a prefab + ScriptableObject definition.

---

## 3) Packages & Baseline Requirements

### 3.1 Required Unity packages
- **Input System**
- **Cinemachine**
- **Animation Rigging** (recommended for head/torso aiming & IK stability)
- **URP** + Post-processing (bloom/glow for futuristic UI & holograms)

### 3.2 Assets you already use / plan to use
- **KAWAII ANIMATIONS — Cool Action** (large animation library)
- **Gridbox Prototype Materials** (1m grid/prototype scale)
- **Starter Assets (Third Person Controller)** as baseline reference (or custom controller built on CharacterController)

### 3.3 Optional “time saver” assets (pick later, don’t overbuy)
- Save system: simple JSON first; later consider a robust save asset.
- Debug UI (in-game): simple IMGUI or UI Toolkit debug overlay first.
- Spline tool: Unity Splines package for cables/pipes (or lightweight custom line renderer).

---

## 4) Input & Camera Rules (must feel like Satisfactory)

### 4.1 Input actions
- `Move` (WASD), `Look` (Mouse Delta), `Jump`, `Sprint`, `Interact`
- `BuildModeToggle`, `BuildMenu`, `RotateHologramLeft/Right`, `Nudge`, `Cancel`, `Confirm`

### 4.2 Camera modes
- **TPS**: free look with mouse; character faces move direction (baseline).
- **FPS**: camera is attached to head/helmet anchor; movement is camera-yaw relative.

### 4.3 Camera collision (no phasing through buildings)
- FPS camera must use:
  - near clip tuning,
  - soft collision (sphere cast) pushing camera away from geometry,
  - optionally a helmet interior mesh to prevent seeing “inside body”.

---

## 5) Animation System Rules

### 5.1 “Movement first” order
- Implement locomotion first (idle/walk/run/jump/fall).
- Then add: turns, pivots, crouch, ladders, wall-run/climb, status effects.

### 5.2 Parameter conventions
- Locomotion uses:
  - `MoveX` (-1..1), `MoveY` (-1..1)
  - `IsSprinting` (bool)
  - keep compatibility params if needed (`Grounded`, `Jump`, `FreeFall`, etc.)

### 5.3 Blend tree structure (do not nest movement trees)
- **Single 2D blend tree** for 8-direction locomotion.
- Motions should be **AnimationClips**, not nested blend trees/states.

### 5.4 Head/torso follow camera (human spiral)
Use one of:
- **Animation Rigging** (Multi-Aim constraint on chest/neck/head)
- or Animator IK / bone offsets:
  - apply offsets on top of the animated pose
  - clamp yaw/pitch
  - smoothing & deadzone to prevent jitter

### 5.5 “Rare idle” policy
- “Personality idles” only after prolonged inactivity:
  - first trigger after 10–15 minutes idle,
  - then no more often than every 10–15 minutes,
  - never while player is interacting/building,
  - can be triggered by comms/voice lines.

---

## 6) Build System Rules (Build Gun / Hologram)

### 6.1 Hologram placement feedback
- Colors for validity states:
  - valid
  - invalid (hard clearance)
  - soft clearance / warning (optional)
- Must be configurable.

### 6.2 Build flow
- Enter build mode → choose buildable → hologram preview → rotate → validate → place.
- Cancel returns to previous tool.

### 6.3 Grid & snapping
- World has an **invisible global grid**.
- Snapping rules:
  - default snap to “foundation grid”
  - optional finer 1m snap for precision

### 6.4 Rotation & nudge (precision mode)
- Rotate left/right
- Optional “hologram lock + nudge”:
  - lock hologram position
  - nudge with arrow keys
  - useful for precision and avoiding camera clipping

---

## 7) Systems v1 (keep it scoped)

### 7.1 Power system
- generator output → network graph → consumers
- battery storage (optional early)
- UI: production / consumption / surplus

Network Rules:

- Power and Water systems must use graph-based node structure.
- Each buildable provides connection nodes.
- Networks are recalculated only when topology changes.
- No per-frame network recalculation.

### 7.2 Water system
- extractor → purifier → clean water
- UI: clean water rate
- feeds restoration meter

### 7.3 Restoration meter
- single “Island Health” 0–100%
- driven by clean water + power surplus

Restoration System:

- Must be owned by a RestorationManager singleton in Bootstrap scene.
- Driven by aggregated data from Power + Water systems.
- No direct writes from buildables.

---

## 8) QA / Validation Checklist (run every milestone)

### 8.1 Movement
- WASD works
- Sprint works
- Camera free look works
- No jitter when spamming A/D in FPS
- Backpedal in FPS does not rotate the body unexpectedly

### 8.2 Animation
- MoveX/MoveY live update
- All locomotion clips loop correctly
- No nested movement blend trees
- No foot “hop” on diagonals

### 8.3 Build
- Ghost preview follows cursor
- Valid/invalid feedback correct
- Rotation works
- Snapping works
- Cannot place overlapping

Placement Validation Rules:

- All buildables must be on a dedicated "Buildable" layer.
- Validation must use Physics.OverlapBox or OverlapCollider with layer mask.
- Ignore self and hologram layer.
- No trigger-only validation.

### 8.4 Save/load (when implemented)
- Save → quit → reload restores:
  - player position
  - placed buildings
  - networks (power/water links)
  - progression/unlocks

---

## 9) Codex Work Rules (how Codex must behave)

1. **Never delete lines “because unused”** without explicit instruction.
2. **When editing**: return the full function being edited + file path.
3. **No multi-file sweeping changes** without a checklist and rollback plan.
4. **If a change impacts input/camera/animator**:
   - provide a “how to verify” step list.
5. **No guessing** about Unity object names:
   - always reference by component type + how to locate in Inspector.

---

## 10) Milestone Definition (v1 “good build”)

A v1 build is “good” when:
- 8-direction locomotion feels solid in TPS + FPS
- FPS camera is stable, collision-safe, and doesn’t show inside the body
- Head/torso aim works without jitter
- Build gun + hologram placement + snapping works
- Power + water + single restoration meter works
- A playable loop exists: place → connect → health rises → save/load

---


## 11) Animation Plan & Milestones

Global rules (apply to every category)
Loop / Start / Stop / End rules

Start: play once when entering a sustained state (walking, climbing, status effect, speaking).

Loop: the steady-state clip while the state remains true.

Stop: play once when leaving a sustained state back to idle/locomotion.

End / End1 / End2: play once when terminating a mode that has a “finish” (status clears, ladder dismount, wall-climb end). If there are End1/End2, pick randomly.

Variants (01, 02, 03, and 1/2)

Use variants for variety without changing mechanics:

Pick randomly with a cooldown (don’t spam).

Or pick contextually:

01 = calm/neutral

02 = more expressive

03 = strongest/most exaggerated
(If you later want “mood/personality,” variants become an easy dial.)

“Distance” & “Turn” fields in your CSV

You have fields like Turn (90/180) and Distance (like ladder, climb up/down 1m/2m). Use those as wiring hints:

TurnLeft_90 and TurnLeft_180 are used for in-place turns (when input wants to rotate but speed is ~0).

ClimbDown_1m/2m are used when you detect ledge height and choose the correct animation.

1) Locomotion (225 clips) — your “core set”

Locomotion set is consistently structured across these “modes”:

Walk

Run

Sprint

Crouch

InjuredWalk

Each has:

8-direction movement: Fwd, Bwd, Left, Right, FL, FR, BL, BR

phase sets: Start, Loop, Stop

plus Pivot variants for direction changes

plus TurnLeft/TurnRight (90/180) for crouch

Locomotion sub-sets and how to wire them
Subset	Clips you have	When to use
Walk	@CA_Walk_*_(Start/Loop/Stop/Pivot) + diagonals	default low-speed movement
Run	@CA_Run_*_(Start/Loop/Stop/Pivot)	mid-speed movement
Sprint	@CA_Sprint_*_(Start/Loop/Stop/Pivot)	sprint mode
Crouch	@CA_Crouch_* + @CA_Crouch_Walk_*	crouch movement + turns
InjuredWalk	@CA_InjuredWalk_*	low health / damaged state
Left2 / Right2 (important detail from your catalog)

In your CSV:

Walk_Left2_Loop/Start/Stop/Pivot exists

Right2 appears as Pivot only (no Right2_Loop found in the catalog)

How to use Left2/Right2 idea anyway:

Treat Left2 as “wide strafe left” / “wide diagonal left” option.

For the right side, either:

stick to normal Right, or

later create a mirrored clip workflow (Unity can mirror some humanoid clips) if it looks good.

“Start/Loop/Stop/Pivot” recommended wiring

To avoid the “gets stuck after start” problem you had before:

Use a locomotion state machine approach:

If movement magnitude rises above threshold → play Start

When start normalizedTime ≥ ~0.85 → crossfade into Loop

If input direction changes sharply while moving → play the matching Pivot, then go back to loop

When magnitude falls below threshold → play Stop, then return Idle

You can still drive the directional choice via MoveX/MoveY, but the “Start/Stop/Pivot” should be controlled by a higher-level “LocomotionPhase.”

Small, practical v1 plan:

v1 uses Loop-only for all directions (fast to ship).

v1.1 adds Start/Stop.

v1.2 adds Pivots for responsiveness.

That’s how you avoid drowning.

2) Idle (40 clips) — personality layer

You have:

Idle01_Breathing (baseline)

a bunch of idles like look around, stretch, folded arms, happy/surprised etc.

Only ArmsFolded has Start/Loop/End (Idle04_ArmsFolded_Start/Loop/End)

Wiring approach

Split idle into 3 buckets:

Base Idle (always available)

Idle01_Breathing

Common micro-idles (light personality)

Look around, stretch, look down, pat head, cool pose, etc.

Trigger after player is idle for e.g. 30–90 seconds, random selection, long cooldown.

Rare / emotional idles (very rare)

happy/surprised, prevention, etc.

Trigger after 10–15 minutes AFK, with 10–15 minute cooldown

OR use as a “comms reaction” when the suit AI / command speaks.

ArmsFolded special case

Use Start→Loop while “idle long enough”

On movement resume: play End

3) Speak (9 clips) — comms / suit AI moments

You have:

StandingSpeak01/02/03_(Start/Loop/End)

Wiring

When a voice line starts:

pick one of 01/02/03 (random or based on tone)

play Start then Loop

When voice line ends:

play End

This gives you “character is alive” without forcing facial animation yet.

4) StatusEffect (40 clips) — clean pattern

You have for each status (Burning, Entangled, Frozen, Poisoned, Shocked etc.):

Variant 1 and 2

Start

Loop

End1, End2

Wiring

On status applied:

choose variant 1 or 2 (random)

play Start then Loop

While active: stay in Loop

On status removed:

choose randomly between End1 and End2

Rule: Status effects should be on an upper-body override layer (so you can still move while poisoned etc.), unless the status logically immobilizes (entangled/frozen can reduce locomotion or override fully).

5) Ladder (47 clips) — full “system set”

You have:

Climb Start/End (+ Top variants)

Up/Down with Start/Loop/Stop (LadderClimb2Up_*, LadderClimb2Down_*)

Some numbered single clips (LadderClimb1Up_1/_2, Down_1/_2)

Slide down sets (SlideDown1/2_Start/Loop/Stop/End)

Jump off ladder directions: JumpBack/Left/Right/Up with Set/Loop/End

Wiring (v1 ladder system)

States:

Ladder_Enter → ClimbIdle → ClimbUp / ClimbDown → Ladder_Exit

Use:

LadderClimb_Start when attaching from ground

LadderClimb_Start_Top when attaching from the top

Up/Down:

Start → Loop → Stop when input stops

Exit:

LadderClimb_End or _End_Top based on exit direction

Slide down:

If sprint + down (or “slide key”): SlideDown_Start → Loop → Stop → End

Jump off ladder:

Use JumpBack/Left/Right/Up sequences:

Start → Set_Loop while “aiming / holding direction” (optional)

End when you commit

Keep this as “ready to wire later” for v1 unless ladder is part of the first island.

6) WallClimbing (99 clips) — includes WallRun + WallSlide

Your WallClimbing category contains three systems:

A) Wall Climbing (8 directions)

Examples:

WallClimbing_Up_Loop/Start/Stop

WallClimbing_DownLeft_*, etc.

FromJump, FromRelease, Idle01/02, End1/End2

Jump sets off the wall (back/left/right/up) similar to ladder

Use:

Start when attaching

Loop while moving on wall

Stop when movement input stops

End when detaching / reaching top

B) Wall Run

Examples:

WallRun_FromStanding_Up_Start

WallRun_FromClimbing_*_(Start/Loop/Stop/End)

Use:

FromStanding_*_Start when player initiates wall run from ground

FromClimbing variants when transitioning from wall climb into run

Loop while running, Stop when input ends

C) Wall Slide

Examples:

WallSlide_FromJump, FromWallClimbing, Loop, Stop, End, Jump

Use:

Enter slide from fall/jump if colliding with wall

Loop while sliding

Stop when leaving wall or grounding

Jump when player kicks off

v1 note: You can keep wall systems “cataloged + animator-ready” but not implement mechanics until after placement/power/water.

7) ObjectGrab (18 clips) — push/pull interactions

You have:

ObjectGrab Start/Loop/End

Push Start/Loop/Stop

Pull Start/Loop/Stop

PushHard equivalents + knockback

Use:

When grabbing a physics object / handle:

ObjectGrab_Start → Loop

When pushing:

ObjectGrabToPushHard or Push_Start → Loop → Stop

When pulling:

Pull_Start → Loop → Stop

“PushHard_Knockback” is for impact / fail / collision reaction

This is very “future interaction system,” but it’s ready.

8) Action (112 clips) — reaction + mobility moves

Action contains:

Damage reactions (Damage1/2/3, BackDamage, knockdown)

Evades (Evade1/2/3_*)

Dashes (Dash_* including diagonals + Left2)

Sliding (Sliding_Start/Loop/End_*)

SuddenStop directions (SuddenStop_*)

Jump Start/Loop (separate from locomotion set)

ShowItem (1 and 2) with Start/Loop/End

PickUp, PushButton, Chest_Open

BattleStance Start/Loop/End

Fly Start/Hover/End

Wiring strategy

This category becomes modular “ability blocks”:

A “HitReact” layer (damage/knockdown)

A “TraversalMoves” set (dash/evade/slide)

An “Interact” set (pickup/chest/button/showitem)

A “Combat stance” set (battle stance)

A “Flight” set (if you ever enable it)

v1: only wire HitReact + basic interact (pickup/button). Keep the rest ready.

Integrate into your v1 Roadmap (not too in-depth)

Here’s how to slot these animation categories into the roadmap you already have — in small encasing steps.

Roadmap Insert: Animation milestones
Anim Milestone A — “Locomotion Loop Set” (must-have)

Import character + humanoid configure

Create Animator Controller:

Locomotion 2D blend tree (Loop-only) for Walk

Drive MoveX/MoveY
✅ Result: 8-direction movement with correct loops

Anim Milestone B — “Locomotion Start/Stop”

Add Start/Stop logic for Walk

Add Run and Sprint loops (can still be loop-only initially)
✅ Result: responsive starts/stops (no snapping)

Anim Milestone C — “Pivots & Left2”

Add Pivot transitions when direction changes fast

Use Left2 as a “wider left strafe/diag” option (and decide how to mirror/solve right2 later)
✅ Result: spam A/D feels crisp, not robotic

Anim Milestone D — “Crouch & Injured locomotion”

Add crouch states (Crouch_Start/Loop/End)

Add InjuredWalk mode gated by health
✅ Result: movement has readable “statefulness”

Anim Milestone E — “Idle personality system”

Base idle always

Micro-idles after 30–90 sec

Rare idles after 10–15 min AFK

Special Start/Loop/End for ArmsFolded
✅ Result: character feels alive

Anim Milestone F — “Speak layer”

Speak01/02/03 Start/Loop/End triggered by comms
✅ Result: voice + body acting

Anim Milestone G — “Status effect layer”

Status Start/Loop + random End1/End2
✅ Result: readable debuffs with almost no code cost

Anim Milestone H — “Interaction pack”

Pickup / PushButton / ShowItem (Start/Loop/End)
✅ Result: your later build tool interactions look grounded

Anim Milestone I — “Traversal expansions (post-v1 mechanics)”

Ladder, WallClimb/Run/Slide, Dash/Evade/Slide, Fly
✅ Result: everything is already wired and ready when mechanics arrive

What you should do in Unity to make this “ready to use” without wiring mechanics yet

Goal: you don’t implement ladder/wallrun now, but you prepare animator layers + states so future work is plug-and-play.

Minimal Animator Layer plan (clean and scalable)

Base Layer: Locomotion

Walk/Run/Sprint/Crouch/Injured locomotion

Upper Body Layer: Speak + Status

Masked to spine/chest/head/arms as needed

Full Body Override Layer: Actions

Hit reacts, knockdowns, slides, dashes (when used)

Traversal Layer: Ladder/Wall

Only enabled during traversal state

Animator State Rules:

- No AnyState transitions for locomotion.
- No nested sub-state machines for locomotion.
- Locomotion tree must remain flat.
- Traversal must be isolated from Base locomotion layer.
- Upper body overrides must use Avatar Masks.

## Appendix: “Satisfactory-like” feature references (design targets)
- Build Gun hologram placement and validity feedback.
- Rotation and precision placement behaviors (rotate/nudge/lock).
- Grid/foundation snapping modes.
- Zoop-like multi-placement for foundations (later; not required for first v1 slice).
