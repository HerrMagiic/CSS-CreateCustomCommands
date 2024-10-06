# Changelogs


## v3.0.0

### Features
- **Config Option for Command Registration**: Added a new configuration option to disable automatic CSS command registration (e.g., `css_<command>`). This allows you to create custom commands like `!ip` without conflicting with commands like `ip` in vanilla CS2.
- **Client Command Execution**: Introduced support for executing client-side commands. Check out [`ClientCommands.md`](https://github.com/HerrMagiic/CSS-CreateCustomCommands/blob/main/Examples/ClientCommands.md) for more details.
- **Client Command from Server**: Added functionality to send commands from the server to clients. Refer to [`ClientCommands.md`](https://github.com/HerrMagiic/CSS-CreateCustomCommands/blob/main/Examples/ClientCommands.md) for implementation instructions.
- **Server Commands**: New server commands have been added. Example usages can be found in [`ServerCommands.md`](https://github.com/HerrMagiic/CSS-CreateCustomCommands/blob/main/Examples/ServerCommands.md).
- **Updated Example Files**: The example files have been updated with more information and clearer descriptions, making it easier to use the plugin.
- **Server Kick Example**: A server kick example has been added in [`ServerCommands.md`](https://github.com/HerrMagiic/CSS-CreateCustomCommands/blob/main/Examples/ServerCommands.md) to demonstrate kicking players.
- **Random Tag Example**: A new example showcasing random tag functionality has been added. See [`Tags.md`](https://github.com/HerrMagiic/CSS-CreateCustomCommands/blob/main/Examples/Tags.md) for details.
- **Color Tags for Prefixes**: Added the ability to apply color tags to command prefixes for more customization.

### Bug Fixes
- **Fixed Newline in Language Tag**: Resolved an issue where newlines were not working correctly in language tags.
- **Fixed Unicode characters not working**: Commands and messages now fully support Unicode, enabling broader character compatibility.

### Optimizations
- **Code Optimizations**: Made small optimizations to improve the code's performance.
- **Variable Refactoring**: Changed most variables to use `var` instead of specific types (like `int` or `string`) for cleaner and more flexible code.
- **Async Command File Reading**: Command files are now read asynchronously, improving performance and responsiveness.
