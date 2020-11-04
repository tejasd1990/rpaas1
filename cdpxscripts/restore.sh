#!/bin/bash

echo "Going into root path"
PWD=`pwd`
pushd $PWD
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
cd $DIR
cd ..
ROOT=$(pwd)

echo "Installing .net core sdk 2.2"
wget -q https://packages.microsoft.com/config/ubuntu/16.04/packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb

sudo apt-get install apt-transport-https
sudo apt-get update
sudo apt-get install dotnet-sdk-2.2 -y

echo "CD into src"
cd $ROOT/src

echo "Restore provider"
dotnet restore

# Save exit code from gcc
EX=$?

# Check exit code and exit with it if it is non-zero so that build will fail
if [ "$EX" -ne "0" ]; then
    popd
    echo Failed to restore RPSAAS.
fi

# Restore working directory
popd 

# Exit with explicit 0 exit code so build will not fail
exit $EX