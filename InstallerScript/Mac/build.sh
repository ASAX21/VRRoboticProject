#!/bin/bash

NAME="EyeSim"

IDENTIFIER="UWA.CSSE.Eyesim.VR"

VERSION="0.8.1"

INSTALL_LOCATION="/"

PLIST_PATH="components.plist"

find Files/ -name '*.DS_Store' -type f -delete

/bin/chmod -R 755 Files

/usr/bin/xattr -rc Files

/usr/bin/pkgbuild \
    --root Files/ \
    --component-plist "$PLIST_PATH" \
    --install-location "$INSTALL_LOCATION" \
    --identifier "$IDENTIFIER" \
    --version "$VERSION" \
    "Output/$NAME-$VERSION.pkg"