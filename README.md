<div align="center">

<img src="docs/banner.jpg" width="100%">

![Version](https://img.shields.io/badge/v1.0-2b3040?style=flat-square)
![BepInEx](https://img.shields.io/badge/BepInEx%205.4-2b3040?style=flat-square&logo=data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCAyNCAyNCI+PGNpcmNsZSBjeD0iMTIiIGN5PSIxMiIgcj0iOSIgZmlsbD0ibm9uZSIgc3Ryb2tlPSIjOTk2NmZmIiBzdHJva2Utd2lkdGg9IjIuNSIvPjwvc3ZnPg==)
![Windows](https://img.shields.io/badge/Windows-2b3040?style=flat-square&logo=data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCAyNCAyNCI+PHBhdGggZmlsbD0iIzViYzBhOCIgZD0iTTMgNS42IDEwLjIgNC42djcuMEgzek0xMS4zIDQuNCAyMSAzLjF2OC41aC05Ljd6TTMgMTIuN2g3LjJ2Ny4wTDMgMTguN3pNMTEuMyAxMi43SDIxdjguNmwtOS43LTEuM3oiLz48L3N2Zz4=)
![Linux](https://img.shields.io/badge/Linux-2b3040?style=flat-square&logo=linux&logoColor=5bc0a8)
![Steam Deck](https://img.shields.io/badge/Steam%20Deck-2b3040?style=flat-square&logo=steamdeck&logoColor=5bc0a8)
![License](https://img.shields.io/badge/MIT%20License-2b3040?style=flat-square)

[**Download on Nexus Mods**](https://www.nexusmods.com/howtofish/mods/54)

</div>

---

Press **Delete** in game.

## Features

**Money** — add or remove any amount, quick +1K / +10K / +100K, wipe to zero

**Player** — god mode, no recoil, one shot kills, infinite jumps, third person, unlock boat / grill / all skins

**Spawn** — every item in the game, searchable. dead and drip variants

**World** — teleport to any island, kill or reset creatures, kill boss, game speed, and a roulette wheel that always lands on your colour (bet green, 35x every time)

Status pills in the corner show what's on.

## Screenshots

<p align="center">
<img src="docs/screenshots/money.jpg" width="49%">
<img src="docs/screenshots/player.jpg" width="49%">
<img src="docs/screenshots/spawn.jpg" width="49%">
<img src="docs/screenshots/casino.jpg" width="49%">
</p>

## Install

1. Grab **ALL-IN-ONE** from [Releases](../../releases)
2. Extract it anywhere
3. Run **INSTALL-WINDOWS.bat**, or **INSTALL-LINUX.sh** on Linux/Deck
4. Launch, press Delete

BepInEx is bundled and the installer finds your game on its own.

Or just extract the zip into your game folder yourself. Same thing.

### Linux / Steam Deck

One extra step, set your launch options to:

```
WINEDLLOVERRIDES="winhttp=n,b" %command%
```

The installer puts that on your clipboard for you.

## Troubleshooting

| Problem | Fix |
| --- | --- |
| Delete does nothing | check `BepInEx/LogOutput.log` exists. no log = BepInEx didn't load |
| No log at all, on Linux | launch options are missing or mistyped |
| Menu opens, buttons do nothing | you're not the host |
| Installer can't find the game | put it in your game folder and run it there |
| Money went negative | it's a 32 bit int, wraps past 2.1 billion. use Remove ALL money |
| Antivirus / Nexus flags `winhttp.dll` | expected, not a virus — see below |

## Notes

Host only. As a guest most of it silently does nothing.

This game has co-op, so spawning and money hit everyone in the lobby, not just you.

Skins are written to your local save. Nothing touches Steam, and Wipe skins undoes it.

`winhttp.dll` gets flagged by some scanners. It works by pretending to be that system DLL so the game loads BepInEx instead — same trick every BepInEx mod uses, and it's a false positive. Use the plugin-only zip if you'd rather not have it and already run BepInEx.

If this is useful to you, a star on the repo is appreciated.

## License

MIT. Bundles [BepInEx](https://github.com/BepInEx/BepInEx) (LGPL-2.1), uses [HarmonyX](https://github.com/BepInEx/HarmonyX).
