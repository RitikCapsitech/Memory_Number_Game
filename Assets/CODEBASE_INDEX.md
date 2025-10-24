Project: MemoryNumberGame (Unity)

This file provides a short, navigable index of the core scripts in the project and their public APIs / responsibilities.

Core Scripts
------------

- Assets/Scripts/GameManager.cs
  - Public fields:
    - GridManager gridManager
    - TextMeshProUGUI levelText
    - GameObject NextLevelPanel
    - GameObject startPanel
    - GameObject pausePanel
  - Public methods:
    - void CheckLevelComplete()  -- checks if all `Cell.IsFilled` and triggers NextLevel
    - void RestartGame()        -- resets level to 1 and regenerates grid
    - void StartGame()          -- UI hook: hides start panel, resets and starts game
    - void PauseGame()          -- UI hook: pauses the game (Time.timeScale = 0)
    - void ResumeGame()         -- UI hook: resumes the game
    - void QuitGame()           -- UI hook: quits application (handles Editor vs Build)
    - void RetryLevel()         -- UI hook: regenerate current grid and re-run show sequence
    - void RestartNextLevel()   -- UI wrapper that advances to next level
  - Notes: Invokes GridManager.IncreaseGridSize() when progressing levels.

- Assets/Scripts/GridManager.cs
  - Public fields:
    - GameObject cellPrefab
    - int gridSize
    - float cellSpacing
    - float showDuration
    - NumberPoolManager numberPoolManager
  - Public methods:
    - void GenerateGrid()               -- builds the grid of `Cell` prefabs and assigns target numbers
    - void IncreaseGridSize()           -- increments gridSize and regenerates grid + sequence
    - void StartShowNumbersSequence()   -- new wrapper to start the show/hide/spawn coroutine
  - Notes: `GenerateGrid()` clears previous children, creates colliders and assigns random numbers. `ShowNumbersSequence()` shows numbers for `showDuration` seconds, hides them, then calls `NumberPoolManager.SpawnNumbers()`.

- Assets/Scripts/NumberPoolManager.cs
  - Public fields:
    - GameObject numberPrefab
    - int poolSize
    - float spacing
    - Vector3 poolStartPosition
  - Public methods:
    - void SpawnNumbers(List<int> numbers) -- clears active pool, constructs a set of pool numbers (always includes the level numbers), shuffles and instantiates number prefabs. 

- Assets/Scripts/Cell.cs
  - Public fields:
    - TextMeshPro numberText
    - SpriteRenderer spriteRenderer
    - Color correctColor, wrongColor
    - bool IsFilled
  - Public methods:
    - void SetTargetNumber(int number)
    - void ShowNumber()
    - void HideNumber()
    - bool TryPlaceNumber(int number)
    - int GetTargetNumber()
  - Notes: `TryPlaceNumber` marks the cell as filled on success and notifies `GameManager.CheckLevelComplete()`.

- Assets/Scripts/DraggableNumber.cs
  - Public fields:
    - TextMeshPro numberText
    - SpriteRenderer spriteRenderer
  - Public methods:
    - void SetNumber(int num)
    - int GetNumber()
  - Notes: Implements mouse drag logic using OnMouseDown/Drag/Up and uses Physics2D.OverlapPointAll to detect dropped `Cell`s.

Other scripts
-------------
The workspace contains many TextMeshPro example scripts under `Assets/TextMesh Pro/Examples & Extras/Scripts/`. They are third-party/example code and not modified by this work — they include many helper MonoBehaviours used by TMP.

Changes made in this session
---------------------------
- Added UI methods to `GameManager.cs`: StartGame, PauseGame, ResumeGame, QuitGame, RetryLevel, RestartNextLevel.
- Exposed `GridManager.StartShowNumbersSequence()` and made `GenerateGrid()` stop existing coroutines to avoid duplicates.
- Created this index file at `Assets/CODEBASE_INDEX.md`.

How to wire UI buttons
----------------------
- Hook Start button -> `GameManager.StartGame()`
- Hook Pause button -> `GameManager.PauseGame()`
- Hook Resume button -> `GameManager.ResumeGame()`
- Hook Quit button -> `GameManager.QuitGame()`
- Hook Retry button -> `GameManager.RetryLevel()`
- Hook Next Level button -> `GameManager.RestartNextLevel()`

Follow-ups / Suggested small improvements
----------------------------------------
- Add events or callbacks instead of FindObjectOfType (for performance and decoupling).
- Add serialized UnityEvents on level complete to let UI react without hard references.
- Add unit tests for core logic where possible (non-MonoBehaviour logic extracted into plain classes).

Completion summary
------------------
I updated `GameManager.cs` and `GridManager.cs`, and added `Assets/CODEBASE_INDEX.md` summarizing the core public APIs. No immediate file errors were reported by the workspace scanner.
