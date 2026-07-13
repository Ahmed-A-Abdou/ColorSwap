# ColorSwap

A graph-coloring puzzle game built in Unity 6. Players drag and swap colored tokens across a network of connected nodes — every line between two nodes must end up connecting two *different* colors to solve the puzzle.

## Gameplay

[![ColorSwap Gameplay](https://img.youtube.com/vi/_fB-zVBE8gI/0.jpg)](https://www.youtube.com/watch?v=_fB-zVBE8gI)

## How It Works

Each level is a graph of nodes connected by edges. The win condition is a proper graph coloring — no two directly connected nodes can share a color. Players drag a colored token onto an adjacent connected node to swap their colors. Swaps are only valid between nodes that share a connection, so the puzzle boils down to finding the right sequence of swaps to resolve all color conflicts.

## Architecture

### Data-Driven Levels
Levels are `LevelData` ScriptableObjects holding a list of nodes (viewport-normalized position + starting color) and a list of index-pair connections. New levels are authored entirely as assets — zero code changes required. Node positions convert from normalized `[0,1]` viewport space to world space via `Camera.main.ViewportToWorldPoint`, keeping layouts resolution-independent.

### Decoupled Systems via Static Events
`InputHandler`, `LevelManager`, and `UIManager` communicate entirely through static C# events wired in `OnEnable`/`OnDisable`. No system holds a direct reference to another — input detection, swap validation, and UI presentation stay fully decoupled through a lightweight pub/sub layer.

### Interaction & Feedback
Drag detection uses `Physics2D.OverlapPoint` against oversized colliders for forgiving touch input. A two-layer node prefab (static base socket + draggable colored child) creates the visual effect of a token sliding between fixed positions. Invalid swaps and drag cancellations both resolve through a DoTween `Ease.OutBack` return animation. Tween logic is centralized in a static `TweenHelper` wrapper.

## Tech Stack

- **Engine:** Unity 6 (2D)
- **Language:** C#
- **Animation:** DoTween
- **Architecture:** ScriptableObject-driven data, static C# event pub/sub, separated Input/Logic/UI layers
- **Target Platform:** Mobile (Android, portrait mode, any resolution)

## Project Structure

```
Assets/
├── Scripts/
│   ├── Core/          # LevelManager, Node, WinCondition
│   ├── Data/          # LevelData, NodeData, NodeConnection ScriptableObjects
│   ├── Input/         # InputHandler
│   └── UI/            # UIManager
├── Prefabs/           # Node prefab, LineRenderer prefab
└── ScriptableObjects/ # Level1, Level2 assets
```

## Author

**Ahmed Alaa** — Unity Game Developer  
[Portfolio](https://ahmed-a-abdou.github.io) · [GitHub](https://github.com/Ahmed-A-Abdou) · [YouTube](https://www.youtube.com/@AbuAlaa007)
