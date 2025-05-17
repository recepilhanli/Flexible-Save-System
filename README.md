# Save System

## Overview

This project is a flexible and extensible save system designed for Unity games. It supports different data types (JSON/Binary), offers slot-based save management, and allows dynamic addition and removal of save listeners.

## Features

- **Slot-Based Saving:** Supports multiple save slots.
- **JSON and Binary Support:** Saves can be stored in JSON or Binary (GZip compressed) format.
- **Dynamic Listener Management:** Easily save different data types using save listeners (SaveListener).
- **Easy Integration:** Simple save and load operations through SaveManager.
- **Unity Compatible:** Platform-independent save directory using `Application.persistentDataPath`.

## Installation

1. Add the project files to your Unity project.
2. Install necessary NuGet packages (e.g., `Newtonsoft.Json`).
3. Create your own data types and listeners using the `ISave<T>` interface.

## Usage

### Creating a Save Listener

```csharp
var listener = new SaveListener<MyDataType>("myDataFile", myDataObject);
```

### Saving

```csharp
SaveManager.SaveGame();
```

### Loading

```csharp
SaveManager.LoadGame(slotIndex);
```

### Changing Save Slot

```csharp
SaveManager.ChangeSaveSlot(newSlotIndex);
```

## File Structure

- `SaveManager.cs`: Central manager for save and load operations.
- `SaveManager.Json.cs`: JSON format save/load operations.
- `SaveManager.Binary.cs`: Binary format save/load operations.
- `SavePool.cs`: Pool of save listeners and bulk data management.
- `SaveListener.cs`: Save listener that attaches to data objects.
- `ISave.cs`: Interfaces for objects to be saved.
- `Example.cs`: Example usage of the save system.

## SavePool Initialization

The `SavePool` is initialized in the `SaveManager` class. For example, in the `LoadAllSavePools` method, a `SavePool` for a specific data type is created and added to the data pools dictionary:

```csharp
private static void LoadAllSavePools()
{
    SavePool<StandMixer.MixerStandSaveData> mixerStandPool = new SavePool<StandMixer.MixerStandSaveData>();
    _dataPools.Add(nameof(StandMixer), mixerStandPool);
}
```

You can load all save pools by calling `SaveManager.LoadAllSavePools()`.

## Switching to JSON

To switch to JSON format, you can use the Unity Scripting Symbols. Add `SAVE_SYSTEM_USE_JSON` to your project's scripting symbols in Unity. This will enable JSON serialization for saving and loading data.

## License

You are free to use and modify this project as you wish.

4. Add your custom `SavePool` to the `SaveManager` by calling `SaveManager.LoadAllSavePools()`. 