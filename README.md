# Echo

A Unity puzzle platformer built around recording player input and replaying it through physical clones called **echoes**. Each attempt adds another actor to the puzzle, allowing the player to coordinate with previous runs, activate pressure plates, and reach the exit.

This portfolio project uses explicit dependency injection, focused gameplay services, and reusable movement logic. Gameplay code lives in [`Assets/_Project/Scripts`](Assets/_Project/Scripts).

## Technology stack

| Technology | Version | Role |
| --- | --- | --- |
| Unity | 6000.3.24f1 | Editor and runtime |
| Universal Render Pipeline | 17.3.0 | Rendering pipeline |
| VContainer | 1.19.0 | Dependency registration, injection, scopes, and entry points |
| Input System | 1.20.0 | Input actions and generated C# wrapper |
| Addressables | 2.10.3 | Asynchronous gameplay scene loading |
| DOTween | Bundled in `Assets/Plugins/Demigiant` | Rewind sequences, pressure plate feedback, and timer animation |
| Unity UI / TextMesh Pro | uGUI 2.0.0 | Menus, countdown, and pressure plate labels |

Additional packages include **Cinemachine 3.1.7**, **ProBuilder 6.1.4**, and **MCP for Unity** for camera, level authoring, and editor tooling. Package dependencies are declared in [`Packages/manifest.json`](Packages/manifest.json); DOTween is included as an asset plugin.

## Architecture

### Dependency injection and lifetimes

VContainer scopes act as composition roots:

- **`ProjectLifetimeScope`** persists across scene changes and registers scene navigation and JSON persistence. `ProjectEntryPoint` restores the saved level index at startup.
- **`MainMenuLifetimeScope`** registers the menu controller.
- **`GameLifetimeScope`** registers per-level services, input actions, scene components, the echo prefab, and the configured collection of rewindable objects. `GameEntryPoint` connects level completion to saving progress.

Plain C# services use constructor injection. Scene components receive dependencies through VContainer `[Inject]` methods. `IStartable` entry points handle startup wiring, while disposable services and entry points release their event subscriptions when their scope ends.

The project separates orchestration from Unity callbacks where practical, while retaining Unity types and physics APIs in services that need them. It uses feature folders rather than separate domain/application/infrastructure assemblies.

### Shared movement simulation

`PlayerController` reads input and creates an `InputFrame` on each `FixedUpdate`. `EchoController` consumes previously recorded frames at the same physics cadence. Both delegate movement to the plain C# `CharacterMotor` using the same `CharacterMovementSettings`.

The motor handles Rigidbody movement, acceleration, air control, rotation, ground checks, coyote time, jump buffering, and additional falling gravity. Sharing this implementation keeps player and echo movement rules aligned without duplicating controller logic.

### Input recording and echo playback

`RecordService` subscribes to `PlayerController.InputCaptured` and stores movement vectors and jump commands in a `List<InputFrame>`. Recording duration is converted to a frame budget using `Time.fixedDeltaTime`. The countdown is exposed through `RemainingSecondsChanged`, so the timer UI updates when the displayed second changes.

`EchoFactory` instantiates the echo prefab through VContainer, applies the player's movement settings, assigns the recorded frames, and registers the actor with `EchoService`. `EchoController.PlaybackCompletion` exposes playback completion as a `Task` for level orchestration.

Playback re-simulates input through physics rather than restoring recorded transform snapshots. It therefore depends on the current world state and is not a deterministic physics replay system.

### Rewind and level orchestration

`LevelManager` coordinates the recording loop with `Task` and `TaskCompletionSource`:

1. Position the player at the level spawn point and begin a timed recording.
2. After a non-final recording, rewind the player, existing echoes, and configured puzzle objects.
3. Spawn an echo from the completed recording and restart playback alongside the next player attempt.
4. After the final recording, wait for existing echo playbacks to finish before raising the loss event.

`WinTrigger` raises the win event when an actor tagged `Player` or `Echo` enters the exit.

`RewindService` coordinates DOTween sequences, temporarily disables actor controllers and physics simulation, synchronizes transforms, and resumes playback after the animation. Objects opt into reset behavior through `IRewindable`; for example, `Box` resets its position, rotation, and velocity. This is an animated reset to a starting state rather than continuous time reversal.

### Event-driven interactions and UI

- **`IPlateInteractable`** lets players, echoes, and boxes contribute mass to a pressure plate through a shared contract.
- **`PressurePlate`** tracks occupants and collider contact counts, updates its load display, and emits activation/deactivation events when the mass threshold is crossed.
- **`Door`** reacts to plate events to change its rotation and collision state.
- **`GameEndService`** exposes win/loss events consumed by menus and progression saving.
- **`GameStateService`** manages gameplay/pause state and `Time.timeScale`, with state changes exposed to the UI.
- **`Anim`** groups reusable DOTween effects for rewind transitions, pressure plates, and timer feedback.

### Level configuration and scene loading

`LevelConfig` is a ScriptableObject containing an Addressables scene reference, a level ID, the recording duration (`time`), and the recording-cycle budget (`echoMax`). The final recording does not spawn an echo, so a cycle budget of N creates at most N - 1 echoes.

`SceneChanger` discovers configurations with `Resources.LoadAll<LevelConfig>("Config/Levels")` and loads gameplay scenes through `Addressables.LoadSceneAsync`. The main menu is loaded through build index 0. Navigation currently indexes the loaded configuration array directly; the `id` field is not used to sort it.

### Persistence

`ISaveService` abstracts progress storage. `JsonSaveService` serializes `SaveData.levelId` with Unity's `JsonUtility` to `Application.persistentDataPath/save.json`. Winning saves the next level index; application startup restores that index for menu navigation. Recordings and world state remain in memory and are not part of the save file.

## Project structure

```text
Assets/
|-- _Project/
|   |-- Art/, Materials/, Shaders/
|   |-- Prefabs/
|   |-- Scenes/
|   `-- Scripts/
|       |-- Animations/          # Shared DOTween effects
|       |-- EchoSystem/          # Echo creation, registry, and playback
|       |-- InputSystems/        # Input actions and generated wrapper
|       |-- InteractionObjects/  # Plates, doors, and rewindable boxes
|       |-- Level/               # Configuration, game cycle, state, and outcomes
|       |-- PlayerSystem/        # Player adapter and shared character motor
|       |-- RecordSystem/        # Physics-frame input recording
|       |-- RewindSystem/        # Reset contracts and animated orchestration
|       |-- UI/                  # Menu and timer components
|       |-- VContainer/          # Composition roots and entry points
|       `-- _Global/             # Scene navigation and save services
|-- AddressableAssetsData/       # Addressables configuration and groups
|-- Plugins/Demigiant/           # DOTween plugin
`-- Resources/Config/Levels/     # LevelConfig assets
```

## Running the project

1. Install **Unity 6000.3.24f1** through Unity Hub and add this repository as a project.
2. Open the project and allow Unity to import assets and resolve packages. Git must be available for the Git-based package dependencies.
3. Open [`Assets/_Project/Scenes/MainMenu.unity`](Assets/_Project/Scenes/MainMenu.unity) and enter Play Mode.
4. Start a new game from the main menu.

For a player build, keep `MainMenu` at build index 0 and ensure gameplay scenes are included in the Addressables configuration. Generate Addressables content through **Window > Asset Management > Addressables > Groups > Build > New Build > Default Build Script** before building the player.

| Action | Keyboard |
| --- | --- |
| Move left / right | A / D |
| Jump | Space or W |
| Pause / resume | Escape |

Bindings are defined in [`PlayerInputSystem.inputactions`](Assets/_Project/Scripts/InputSystems/PlayerInputSystem.inputactions). Edit this asset and regenerate its wrapper instead of manually modifying the generated C# file.

## Authoring levels

Use `Assets/_Project/Scenes/Templates/TemplateLevel.unity` as a starting point. Create a `LevelConfig` through **Create > ScriptableObjects > Level**, store it under `Assets/Resources/Config/Levels`, and assign its Addressable scene reference and recording settings.

Wire the scene's `GameLifetimeScope` references, assign the configuration and spawn point to `LevelManager`, and add resettable puzzle components to the scope's rewind list. New resettable objects implement `IRewindable`; objects that should activate pressure plates implement `IPlateInteractable`. Verify configuration array order when adding levels because progression currently uses array indices.
