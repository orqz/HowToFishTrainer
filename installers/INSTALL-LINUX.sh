#!/usr/bin/env bash
# HowToFishTrainer installer for Linux / Steam Deck
set -uo pipefail
cd "$(dirname "$0")"

echo
echo "  HowToFishTrainer - installer"
echo "  ==========================="
echo

find_game() {
  # already in the game folder?
  [[ -f "How to Fish.exe" ]] && { echo "$PWD"; return; }

  local roots=(
    "$HOME/.local/share/Steam"
    "$HOME/.steam/steam"
    "$HOME/.var/app/com.valvesoftware.Steam/.local/share/Steam"
  )
  local vdf lib
  for r in "${roots[@]}"; do
    vdf="$r/steamapps/libraryfolders.vdf"
    [[ -f "$vdf" ]] || continue
    # every library path Steam knows about, plus the root itself
    while read -r lib; do
      [[ -f "$lib/steamapps/common/How to Fish/How to Fish/How to Fish.exe" ]] && {
        echo "$lib/steamapps/common/How to Fish/How to Fish"; return; }
    done < <( { echo "$r"; grep -oP '"path"\s+"\K[^"]+' "$vdf" 2>/dev/null; } )
  done
}

GAME="$(find_game)"

if [[ -z "$GAME" ]]; then
  echo "  Could not find How to Fish automatically."
  echo
  echo "  Copy this installer into your game folder - the one containing"
  echo "  'How to Fish.exe' - and run it there."
  echo
  echo "  In Steam: right-click How to Fish -> Manage -> Browse local files,"
  echo "  then open the 'How to Fish' folder inside."
  echo
  exit 1
fi

echo "  Found the game:"
echo "    $GAME"
echo

cp -r BepInEx "$GAME/"
cp -f winhttp.dll doorstop_config.ini .doorstop_version "$GAME/" 2>/dev/null

echo "  Installed."
echo
echo "  ONE MORE STEP - Linux only:"
echo
echo "    Steam -> right-click How to Fish -> Properties -> General"
echo "    -> Launch Options, and paste exactly this:"
echo
echo '      WINEDLLOVERRIDES="winhttp=n,b" %command%'
echo

if command -v wl-copy >/dev/null 2>&1; then
  printf 'WINEDLLOVERRIDES="winhttp=n,b" %%command%%' | wl-copy
  echo "    (copied to your clipboard - just paste it)"
  echo
elif command -v xclip >/dev/null 2>&1; then
  printf 'WINEDLLOVERRIDES="winhttp=n,b" %%command%%' | xclip -selection clipboard
  echo "    (copied to your clipboard - just paste it)"
  echo
fi

echo "  Then launch the game and press DELETE."
echo
