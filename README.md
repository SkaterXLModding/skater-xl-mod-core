# skater-xl-mod-core 
Unified codebase with core runtime logic and and debugging tools for both in-game and Unity Editor.

## Skater XL 
### SXLMod 
The core in-game runtime logic
- Main Mod Logic
- Debug Console  
- Customization Access (Needs update for v1.0)
### SXLUnityCore 
Gameplay representation of components used in Unity Editor. These classes are the full implementation of logic for gameplay.
- Grind Spline Generation
- Gameplay Components
- World Building Components

## Unity Editor
### SXLUnityEditorCore
Editor representation of components used in Gameplay. These classes are effectively minimized abstractions of the full logic from SXLUnityCore to allow for simple set up and removal of dependencies tied to paid add-on marketplace content such as DreamTech Splines.
- Grind Spline Generation
- Gameplay Components
- World Building Components