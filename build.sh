#!/usr/bin/env bash
# Builds the plugin and copies it into the game's BepInEx/plugins folder.
set -euo pipefail

GAME_DIR="${GAME_DIR:-$HOME/.local/share/Steam/steamapps/common/How to Fish/How to Fish}"
cd "$(dirname "$0")"

if [[ ! -d "$GAME_DIR/BepInEx/core" ]]; then
  echo "error: BepInEx is not installed yet. Run ./install-bepinex.sh first." >&2
  exit 1
fi

dotnet build src/HowToFishTrainer.csproj -c Release -p:GameDir="$GAME_DIR" "$@"

echo
echo "Plugin is at: $GAME_DIR/BepInEx/scripts/HowToFishTrainer.dll"
echo "Game already running? Just press F6 to reload."
