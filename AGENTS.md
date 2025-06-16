# Guidelines for Automated Contributions

This repository is a Unity project targeting **Unity 2020.3.14f1**. The project
uses [Git LFS](https://git-lfs.github.com/) to track most Unity assets and has
continuous integration configured through GitHub Actions.

## Coding style
- C# scripts live under `Assets/Scripts`. Indent using four spaces.
- Public methods and classes use **PascalCase**. Private fields use `camelCase`.
- Include summary XML comments where possible. Tests contain explanatory
  comments to help new contributors.

## Tests
- Unit tests reside in `Assets/Tests` and are split into *EditMode* and
  *PlayMode* assemblies. Each has an `.asmdef` file referencing `GameCore`.
- When adding new tests, ensure the corresponding `.asmdef` references remain
  valid and that the `.meta` files are committed.
- Tests use NUnit's `[Test]` attribute. See `RangeCalculatorTests.cs` for
  examples.
- The GitHub workflow `unity-ci.yml` builds the project and runs play mode
  tests on pull requests.

## Assets
- The `.gitattributes` file configures Git LFS for Unity asset types such as
  `*.prefab`, `*.unity`, `*.png`, `*.exr` and `*.fbx`.
- Commit the `.meta` file whenever an asset is added or moved.

## Commit messages
- Keep messages short and descriptive (e.g. "Extract merge layout logic").
- Include a brief summary of the change; additional details can be placed in the
  body if needed.

## Recommendations
- Run the Unity Test Runner before submitting changes. The CI workflow mirrors
  this process.
- Avoid large, unnecessary scene or prefab changes to keep diffs readable.
- If you modify assembly definitions or introduce new assemblies, update the
  corresponding `.asmdef` files so tests continue to compile.
