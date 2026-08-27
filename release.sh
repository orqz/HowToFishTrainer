#!/usr/bin/env bash
# Builds the distributable zips.
#
#   HowToFishTrainer-<ver>-ALL-IN-ONE.zip  BepInEx + the mod. Extract into the game
#                                          folder and you're done.
#   HowToFishTrainer-<ver>.zip             just the plugin, for people who already
#                                          have BepInEx (and for Vortex).
set -euo pipefail
cd "$(dirname "$0")"

NAME="HowToFishTrainer"
VERSION="$(grep -oP 'Version\s*=\s*"\Kv?[0-9.]+' src/config.cs | head -1)"
BUILD="build-tmp"
CACHE=".cache"

rm -rf release "$BUILD" && mkdir -p release "$BUILD" "$CACHE"
trap 'rm -rf "$BUILD"' EXIT

echo "==> compiling $NAME $VERSION"
dotnet build src/${NAME}.csproj -c Release -p:NoDeploy=true -o "$BUILD/out" >/dev/null

# ---------------------------------------------------------------- plugin only
PLUGIN="$BUILD/plugin"
mkdir -p "$PLUGIN/BepInEx/plugins"
cp "$BUILD/out/${NAME}.dll" "$PLUGIN/BepInEx/plugins/"
cp README.md LICENSE "$PLUGIN/"

# ---------------------------------------------------------------- all in one
if [[ ! -f "$CACHE/bepinex.zip" ]]; then
  echo "==> fetching BepInEx 5 (cached after this)"
  URL="$(curl -fsSL https://api.github.com/repos/BepInEx/BepInEx/releases \
        | grep -o '"browser_download_url": *"[^"]*BepInEx_win_x64_5\.[^"]*\.zip"' \
        | head -n1 | cut -d'"' -f4)"
  [[ -n "$URL" ]] || { echo "could not find a BepInEx 5 release" >&2; exit 1; }
  curl -fL --progress-bar -o "$CACHE/bepinex.zip" "$URL"
fi

FULL="$BUILD/full"
mkdir -p "$FULL"
python3 -c "
import zipfile, sys
zipfile.ZipFile('$CACHE/bepinex.zip').extractall('$FULL')
"
mv "$FULL/winhttp.dll" "$FULL/version.dll"
mkdir -p "$FULL/BepInEx/plugins"
cp "$BUILD/out/${NAME}.dll" "$FULL/BepInEx/plugins/"
cp README.md LICENSE "$FULL/"
cp installers/INSTALL-WINDOWS.bat installers/INSTALL-LINUX.sh "$FULL/"
chmod +x "$FULL/INSTALL-LINUX.sh"

# ---------------------------------------------------------------- zip them up
python3 - "$NAME" "$VERSION" "$BUILD" <<'PYEOF'
import sys, zipfile, pathlib
name, version, build = sys.argv[1], sys.argv[2], sys.argv[3]

def pack(src_dir, out_name):
    src = pathlib.Path(build) / src_dir
    out = pathlib.Path("release") / out_name
    with zipfile.ZipFile(out, "w", zipfile.ZIP_DEFLATED) as z:
        for f in sorted(src.rglob("*")):
            if f.is_file():
                z.write(f, f.relative_to(src))
    kb = out.stat().st_size // 1024
    print(f"  {out}  ({kb} KB)")
    return out

print("==> packaged")
pack("plugin", f"{name}-{version}.zip")
pack("full",   f"{name}-{version}-ALL-IN-ONE.zip")
PYEOF
