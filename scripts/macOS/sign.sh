#! /bin/bash

if [ "$#" -ne 1 ]; then
    echo "Usage: $0 <filename>"
    exit 1
fi

SCRIPT_FOLDER=$(dirname "${BASH_SOURCE[0]}")

APP_NAME="$1"
ENTITLEMENTS="$SCRIPT_FOLDER/etvctl.entitlements"
SIGNING_IDENTITY="C3BBCFB2D6851FF0DCA6CAC06A3EF1ECE71F9FFF"

codesign --force --verbose --timestamp --options=runtime --entitlements "$ENTITLEMENTS" --sign "$SIGNING_IDENTITY" --deep "$APP_NAME"
