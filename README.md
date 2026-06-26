<div align="center">

# 🚜 Multiplayer Farms (C# Core)

[![Nexus Mods](https://img.shields.io/badge/Nexus%20Mods-Multiplayer%20Farms-orange?style=for-the-badge)](https://www.nexusmods.com/stardewvalley/mods/26873)
[![Stardew Valley](https://img.shields.io/badge/Stardew%20Valley-1.6+-blue?style=for-the-badge)](https://www.stardewvalley.net/)
[![Ko-fi](https://img.shields.io/badge/Ko--fi-Support%20me-ff5f5f?style=for-the-badge&logo=ko-fi&logoColor=white)](https://ko-fi.com/lyzzcl)

</div>

## 📖 About This Repository

This repository contains **only the C# logic** (`MPF_Code`) for the Multiplayer Farms mod. The map assets and Content Patcher configurations are handled in the main package.

The C# core is responsible for patching base game behavior to natively support multiple independent farm maps in multiplayer, ensuring that custom farms act as "first-class citizens" in the valley.

## ✨ Features & Mechanics

Using Harmony patching and SMAPI APIs, this module handles:

*   **⚡ Custom Farm Lightning:** Ensures lightning rods charge correctly during storms on all additional farms, simulating the same visual flashes and sound effects like vanilla.
*   **📮 Mailbox Routing:** Redirects the player's mail directly to their cabin mailbox, regardless of which farm they reside on.
*   **🏡 Seamless Warping:** Modifies the **Return Scepter** and **Farm Totems** to teleport players directly to the front door of their respective cabins instead of defaulting to the main farm.
*   **🎆 Festival Exits:** Fixes vanilla festival exits so farmhands are sent back to their own cabins after an event ends.
*   **🔨 Robin's Carpenter Menu:** Allows farmhands to freely place and upgrade cabins on any of the new MPF farms.
*   **✏️ Editable Farm Names:** Lets a joining farmhand rename the MPF farm of the empty cabin they pick straight from the "Farm Name" field on the character creation screen.
*   **💎 Mini-Obelisks:** Adds support for placing Mini-Obelisks on the custom farms (limited to 2 per farm, mimicking vanilla behavior).
*   **⚙️ GMCM Integration:** Provides a rich, user-friendly in-game settings menu via Generic Mod Config Menu to toggle features, rename farms, and set warp coordinates.
*   **🔄 Live Farm Name Refresh:** Renaming a farm (from GMCM or the character creation screen) updates its in-world display name instantly for every player — no need to exit and re-enter the world.
*   **🔌 Content Patcher Tokens:** Exposes a suite of custom tokens so the companion CP mod can dynamically react to the user's settings.

## 🛠️ Architecture

The codebase is organized cleanly to separate concerns:
*   `Patches/`: Patches that safely intercept and override vanilla behavior with safe fallbacks.
*   `Config/`: GMCM menu registration, data binding, and Content Patcher token exports.
*   `Farms/`: The central `FarmRegistry` and world setup logic (spawning starter cabins, buildings, fences, etc. on Day 1).

## 🚀 Building from Source

1. Clone the repository.
2. Ensure you have the [.NET 6 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) installed.
3. Make sure Stardew Valley is installed and your `GameModsPath` is correctly set in `MPF_Code.csproj` (or let the Pathoschild ModBuildConfig handle it).
4. Build the project using your IDE or via CLI:
   ```bash
   dotnet build
   ```

## 🔗 Links

*   **Download the full mod:** [Nexus Mods - Multiple Farms For Multiplayer](https://www.nexusmods.com/stardewvalley/mods/26873)
*   **Support the project:** Donations are welcome on [Ko-fi](https://ko-fi.com/lyzzcl) — entirely optional, but hugely appreciated. 💛

## 📜 License

This project is licensed under the **PolyForm Noncommercial License 1.0.0**.

*   **Original code:** Licensed under PolyForm Noncommercial 1.0.0.
*   **Commercial use:** Requires explicit written permission from **LyzzCL**.
*   **Vendored API contracts:** Files under `Apis/` keep their original licenses.
*   **Donations:** Optional and do not grant commercial rights over this project.
