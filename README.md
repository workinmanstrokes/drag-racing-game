# Drag Racing Game

> One strip, one car, one perfect launch — beat your best time.

A Windows PC drag-racing game built in Unity (C#) with the Unity Input System.
This repo is the next update for the existing Unity drag-racing project
(Scofather/DragRacingGame): it turns the unverified skeleton into the smallest
drivable proof — one straight strip, one car, a Christmas-tree launch,
accelerate/shift driving and finish-line timing — then adds a target-time
rival, a persisted best time, and one tuning variable.

No car roster, shop or multiplayer until this loop works.

## What it is

- One straight drag strip with a Christmas-tree launch sequence.
- One car: hold to accelerate, shift up manually, watch RPM and speed.
- Finish line with elapsed time, reaction time and false-start (red-light) handling.
- Instant restart after a run — no menu reload.
- A target time to beat, with a win/lose result and a persisted best time.
- One tuning variable: launch RPM, adjustable before a run.

## Unity version

- **Unity 2022.3 LTS** (recommended; confirm against the original
  Scofather/DragRacingGame project — the original repo ships no README, so
  its exact version is still unverified).
- **Scene to open:** `Assets/Scenes/DragStrip.unity`
- **Packages required:** Input System (Project Settings > Player >
  Active Input Handler: Input System Package (New)).

## First run (no setup)

1. Open the project in Unity 2022.3 LTS.
2. Open `Assets/Scenes/DragStrip.unity`.
3. Press Play. The strip, start/finish lines, car body and HUD are built at
   runtime (`Assets/Scripts/DragStripBuilder.cs`,
   `Assets/Scripts/DragRacingHud.cs`), so there is nothing to place by hand.
4. Wait for the Christmas tree, launch on green, hold Space to
   accelerate, press E to shift up, cross the finish line.
5. Press R at any time for an instant restart — including after a false
   start or a finished run.

Optional, one-time editor step: select the DragStrip object and assign
`Assets/InputActions/DragRacing.inputactions` to the DragRacingInput
component's **Actions** field to drive from the canonical asset. If it is
left unassigned, DragRacingInput builds the identical frozen bindings in
code and logs a warning, so a fresh checkout is drivable either way.

## Controls (frozen Input System map)

Defined in `Assets/InputActions/DragRacing.inputactions`, wired to
`RaceManager` by `Assets/Scripts/DragRacingInput.cs`.

| Action | Binding |
| ------ | ------- |
| Launch / Accelerate (hold) | Space (keyboard), Right Trigger (gamepad) |
| Shift Up | E (keyboard), Right Bumper / RB (gamepad) |
| Restart (instant) | R (keyboard), Y / Triangle (gamepad) |

Do not change these bindings without updating this README and the
`.inputactions` asset together.

## How to build (Windows PC)

1. File > Build Settings, add `Assets/Scenes/DragStrip.unity`.
2. Target Platform: **PC, Mac & Linux Standalone**, Target: **Windows**,
   Architecture: **x86_64**.
3. Build to `Builds/Windows/DragRacingGame.exe` and run it.

## Tuning: launch RPM

One tuning variable, `TuningSetup.launchRpm` (1,500–7,500 RPM, default
3,500). It sets the RPM the car leaves the line with — a higher launch RPM
launches harder. Change it on the DragStrip object's TuningSetup component
before a run; the HUD shows it, and the best time records the tuning that
set it (`BestTimeStore`).

Test its effect on elapsed time — fill this in from real runs:

| Launch RPM | Elapsed time | Reaction time | Win / Lose |
| ---------- | ------------ | ------------- | ---------- |
| 1,500 | | | |
| 3,500 | | | |
| 7,500 | | | |

## Works

- Straight-strip scene with start line, Christmas tree and finish line,
  built visibly at runtime (placeholder primitive art).
- Car accelerate / shift-up / RPM / speed HUD
  (`Assets/Scripts/CarController.cs`, `Assets/Scripts/DragRacingHud.cs`).
- Christmas-tree sequence, elapsed-time and reaction-time timing, false-start
  detection, win/lose vs target time, instant restart
  (`Assets/Scripts/RaceManager.cs`).
- Frozen Input System map actually wired to the game — Space / E / R drive
  the run (`Assets/Scripts/DragRacingInput.cs`).
- Best elapsed time persisted on-device and shown on the strip and results
  screen, with the tuning that set it
  (`Assets/Scripts/BestTimeStore.cs`).
- Launch-RPM tuning applied before a run (`Assets/Scripts/TuningSetup.cs`).
- No crash on Play when start/finish markers are unassigned: the car, start
  and finish fall back to the car on the same object, the origin, and
  start + strip length.

## Does not work yet

- The original project's exact Unity version and main scene are unverified
  (no README shipped with it) — confirm before a release build.
- No 30-second gameplay video captured yet, and no verified Windows PC build
  produced from this update yet — both still need Unity on a Windows PC.
- No ghost rival — the repeat-run opponent is a fixed target time only
  (default 12.5 s, `RaceManager.targetTimeSeconds`).
- Placeholder art only: the strip and car are runtime primitives, not
  finished models.
- No car roster, shop or multiplayer (deliberately out of scope until the
  core loop works).

## Milestones

1. Verify the existing Unity project: Unity version, main scene, one car
   driving down a straight strip.
2. Playable loop: Christmas-tree launch, accelerate/shift, finish-line
   timing, false start, instant restart.
3. Frozen Input System map + this README.
4. Repeatability: target time, win/lose, persisted best-time display.
5. One tuning variable (launch RPM), test its effect on elapsed time.
6. 30-second gameplay video + verified Windows PC build, README updated
   with the proof.

## Tester update (Game Test Lab, VEX 1.2/10)

VEX scored the shipped build 1.2/10: "a Unity project skeleton, not a
playable drag racing game." This update answers that directly: the Play
crash from unassigned references is fixed, the frozen Input System map is
wired to the game, a HUD and results screen show the run, and the strip
and car are visible — so the smallest drivable proof can now be played,
not just read. The video and Windows build are still outstanding and are
listed above, not claimed.
