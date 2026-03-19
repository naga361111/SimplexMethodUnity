# SimplexMethodUnity

A Unity-based interactive visualizer for the **Simplex Method** — a step-by-step linear programming solver rendered in real-time using Unity's UI system.

---

## Overview

This project implements the core Simplex algorithm (단체법) inside Unity, allowing users to observe each iteration of the optimization process directly in the game view. The tableau is loaded from a ScriptableObject, rendered as a 2D grid of TextMeshPro cells, and progresses one pivot step at a time via keyboard input.

---

## Features

- **ScriptableObject-based data input** — define the initial tableau as a 2D integer array via the Unity Inspector
- **Runtime tableau rendering** — automatically spawns a TextMeshPro grid aligned to screen space
- **Step-by-step execution** — press `Space` to advance one full pivot iteration
- **Ratio Test** — identifies the pivot column (most negative entry in the objective row) and pivot row (minimum ratio test)
- **Pivot Row Normalization** — divides the pivot row by the pivot element
- **Gaussian Elimination** — updates the objective row using the normalized pivot row
- **Live cell updates** — all changes are reflected immediately in the UI

---

## Project Structure

```
Assets/
├── Scripts/
│   ├── Data2DObject.cs        # ScriptableObject: stores the initial tableau as int[][]
│   ├── DataSpawner.cs         # MonoBehaviour: loads data, renders grid UI, exposes RuntimeTable
│   └── ProcessFlowChart.cs    # MonoBehaviour: runs Simplex iterations on key press
├── ScriptableObjects/
│   └── 2DData.asset           # Example tableau asset
└── Scenes/                    # Unity scene(s)
```

---

## How It Works

1. **Data Setup** — Create a `Data2DObject` ScriptableObject asset and fill in the tableau values (objective row first, then constraint rows).
2. **Scene Setup** — Attach `DataSpawner` to a UI Canvas child object and assign the `Data2DObject` asset. Attach `ProcessFlowChart` to any GameObject with a `PlayerInput` component.
3. **Runtime** — On Play, the tableau is rendered as a grid. Press `Space` (Jump action) to execute one pivot step.
4. **Each pivot step performs:**
   - Find all negative entries in the objective row → select the pivot column with the largest absolute value
   - Perform the Ratio Test across constraint rows → select the pivot row
   - Normalize the pivot row by dividing by the pivot element
   - Apply Gaussian elimination to zero out the pivot column in the objective row

---

## Requirements

| Requirement | Version |
|---|---|
| Unity | 2022.3 LTS or later |
| Input System (Package) | 1.7+ |
| TextMeshPro (Package) | Included via UPM |
| Render Pipeline | Universal Render Pipeline (URP) |

---

## Getting Started

```bash
git clone https://github.com/naga361111/SimplexMethodUnity.git
```

1. Open the project in Unity Hub.
2. Open the scene under `Assets/Scenes/`.
3. Select the `DataSpawner` GameObject and assign a `Data2DObject` asset.
4. Press **Play** and hit `Space` to step through the Simplex iterations.

---

## Script Reference

### `Data2DObject` (ScriptableObject)
| Field | Type | Description |
|---|---|---|
| `Columns` | `RowData[]` | Array of rows; each row holds an `int[]` of column values |

### `DataSpawner` (MonoBehaviour)
| Member | Description |
|---|---|
| `RuntimeTable` | Live `float[][]` tableau used during execution |
| `UpdateCellValue(row, col, value)` | Updates a cell value and refreshes the UI text |

### `ProcessFlowChart` (MonoBehaviour)
| Method | Description |
|---|---|
| `OnJump(InputValue)` | Triggered on Space press; starts one pivot iteration |
| `CheckNegativeEntry()` | Scans objective row for negative coefficients |
| `DivideLastRowValue()` | Runs Ratio Test and selects the pivot row |
| `NormalizePivotRow(row, col)` | Divides the pivot row by the pivot element |
| `EliminateObjectiveRow(row, col)` | Applies Gaussian elimination to the objective row |

---

## License

This project is open source. Feel free to use it for educational purposes.
