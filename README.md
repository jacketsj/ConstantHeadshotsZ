# Constant Headshots Z
Constant Headshots Z is top-down zombie survival shooter with simplistic visuals. It was written in C# with XNA 4.0, and can be compiled with Visual Studio 2010. The solution can build a windows version of the game, a windows level editor, (hypothetically) an Xbox 360 version of the game (stability not certain, since it has not been debugged on Xbox for quite some time), as well as the namespace "LevelBuilder", which allows custom levels to be saved and loaded.  
The level editor is a windows forms application. It (currently) lacks any sort of grid or alignment system, so it is somewhat unwieldly. It allows for custom textures for most things, which are then packaged into the custom level file.  
Custom levels (*.chz files) are xml-based, and can contain textures (in bitmap form), as well as the data for the level layout.  
The game itself allows for one player to play with either a keyboard and mouse or an Xbox 360 controller, or two players (stability unsure, has not been tested for many builds) with two controllers.
Keyboard Controls (hardcoded): WASD to move, mouse to aim, LMB to use equipped weapon, F to toggle rotation camera, scroll to zoom, F11 for fullscreen.

Constant Headshots Z was an early high school project. As unfortunate consequences, it lacks proper physics, uses a large amount of hardcoded things that should be loaded from a configuration file, is in desperate need of refactoring, lacks documentation, and has a rather large amount of commented out code (since it was originally made without version control).
