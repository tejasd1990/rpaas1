#!/usr/bin/env bash

#make your script exit when a command fails.
set -o errexit
set -o pipefail
set -o nounset
set -o xtrace
set +x

azCloudName="AzureUSGovernment"
password="$(cat /mnt/secrets/ClustersSPSecret)"
username="384aae5d-02a4-40a4-b831-743ea2d2f7b6"
tenantId="cab8a31a-1906-4287-a0d8-4eef66b95f6e"
subscriptionId="40c337a2-85a1-4a3d-a444-18dbc4e041e5"
acrname="fairfaxrpsaasacr"
CDPXImageNameKeyToGrep="ame_build_image_name"
deployementEnvironment="Fairfax"
CPDXContainerRepositoryOverride="containerreplicationffusgovvirginia1.azurecr.us"

source deploy_script.sh