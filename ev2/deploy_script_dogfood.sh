#!/usr/bin/env bash

#make your script exit when a command fails.
set -o errexit
set -o pipefail
set -o nounset
set -o xtrace
set +x

azCloudName="AzureCloud"
password="$(cat /mnt/secrets/ClustersSPSecret)"
username="9f2cb846-2a0a-4ae6-a09d-bc3c5b5a2bad"
tenantId="72f988bf-86f1-41af-91ab-2d7cd011db47"
subscriptionId="6f53185c-ea09-4fc3-9075-318dec805303"
acrname="dogfoodacr"
CDPXImageNameKeyToGrep="unique_image_name"
deploymentEnvironment="Dogfood"

source deploy_script.sh