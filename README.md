# SkyPirates

SkyPirates is a Unity project for a tactical, hex-grid based game where you build and command sky ships.

## Project Goals
- Provide a ship building interface so players can design their own sky ships.
- Engage in hex-based tactical battles between ships.
- Allow editing and management of characters and ships between battles.

## Getting Started
1. Install **Unity 2020.3.14f1**. The project was created with this LTS version and should be opened with the same editor version for best compatibility.
2. Clone or download this repository.
3. Open the project folder in the Unity Hub and launch it with `2020.3.14f1`.

### Dependencies
The project relies on Unity packages listed in `Packages/manifest.json`. Notable dependencies include:
- `com.unity.inputsystem` for handling player input.
- `com.unity.textmeshpro` for UI text.
- `com.unity.nuget.newtonsoft-json` (Newtonsoft JSON) for save/load serialization.
Unity will automatically install these packages when the project is opened.

## Running the Game
After opening the project in the Unity editor, load one of the available scenes from the **Assets/Scenes** folder:
- **BattleScene** – the main tactical combat scene.
- **ShipBuildScreen** – scene used for designing ships.
- **CharaterScene** – an additional scene for character-related features.
Press **Play** in the Unity editor to run the selected scene.

## Building
To create a standalone build:
1. Open `File > Build Settings…` in Unity.
2. Add the scenes you want included in the build (e.g., `BattleScene`, `ShipBuildScreen`).
3. Choose your target platform and click **Build**.

