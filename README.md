# Wizard Chess VR

Meta Quest 2 VR chess game (Unity 6000.3.23f1, URP, OpenXR, XR Interaction Toolkit, Meta Voice SDK).

## First-time setup (each member)

1. Install Unity **6000.3.23f1** with Android Build Support.
2. Install Git LFS and enable it:
   ```
   brew install git-lfs   # Windows: included in Git for Windows
   git lfs install
   ```
3. Register UnityYAMLMerge for this clone:
   ```
   # macOS
   git config merge.unityyamlmerge.name "Unity SmartMerge"
   git config merge.unityyamlmerge.driver "'/Applications/Unity/Hub/Editor/6000.3.23f1/Unity.app/Contents/Tools/UnityYAMLMerge' merge -p %O %B %A %A"
   # Windows: driver path is C:/Program Files/Unity/Hub/Editor/6000.3.23f1/Editor/Data/Tools/UnityYAMLMerge.exe
   ```
4. Open the project, then File > Build Profiles > Meta Quest > Switch Platform.

## Folder rules

- All our work goes under `Assets/_Project/`. Do not edit `Samples/`, `XR/`, `XRI/`, `CompositionLayers/` or `ThirdParty/`.
- Scripts are split into assemblies (`WizardChess.<Module>`). `WizardChess.Chess` has no UnityEngine references.
- Work in your own scene under `Assets/_Project/Scenes/Sandbox/`. Announce before editing shared scenes.

## Conventions

- Namespaces `WizardChess.<Module>`, PascalCase types/methods, `_camelCase` private fields, comments in English.
- Asset prefixes: `SM_` mesh, `M_` material, `T_` texture, `PF_` prefab, `SO_` ScriptableObject, `ANIM_`/`AC_`, `SH_`/`VFX_`, `SFX_`/`MUS_`/`VO_`.
- Squares: `e4` in data/logs, index `rank * 8 + file` (`0 = a1`), board origin at a1 centre, +X = file, +Z = rank.
- Branches `feature/<module>-<topic>`, PR to `main` with one review. Commits `<type>(<module>): <summary>`.
- Never commit Wit.ai tokens to a public repository.
