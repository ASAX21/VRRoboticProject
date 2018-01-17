#!/bin/bash

NAME="EyeSim-Aux"

IDENTIFIER="UWA.CSSE.Eyesim.VR.aux"

VERSION="0.7"

INSTALL_LOCATION="/tmp/eyesimaux"

find Aux/ -name '*.DS_Store' -type f -delete

/bin/chmod -R 755 Aux

/usr/bin/xattr -rc Aux

/usr/bin/pkgbuild \
    --root Aux/ \
    --install-location "$INSTALL_LOCATION" \
    --identifier "$IDENTIFIER" \
    --version "$VERSION" \
    "compiled/$NAME-$VERSION.pkg"