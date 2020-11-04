#!/bin/bash

echo "Going into root path"
PWD=`pwd`
pushd $PWD
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
cd $DIR
cd ..
ROOT=$(pwd)

echo "Copy  metadata file into ev2"
cp $ROOT/src/provider-image-meta.json $ROOT/ev2/

echo "Package EV2 artifacts"
cd $ROOT/ev2
tar -cvf deploy.tar .

# Save exit code from gcc
EX=$?

# Check exit code and exit with it if it is non-zero so that build will fail
if [ "$EX" -ne "0" ]; then
    popd
    echo Failed to build RPSAAS.
fi

# Restore working directory
popd 

# Exit with explicit 0 exit code so build will not fail
exit $EX