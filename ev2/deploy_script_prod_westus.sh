#!/usr/bin/env bash

#make your script exit when a command fails.
set -o errexit
set -o pipefail
set -o nounset
set -o xtrace
set +x

azCloudName="AzureCloud"
password="$(cat /mnt/secrets/ClustersSPSecret)"
username="effaa2fc-ceee-4f8f-bf24-0016e36b1365"
tenantId="33e01921-4d64-4f8c-a055-5bdaffd5e33d"
subscriptionId="d370dde1-4f84-414c-818f-919c61590320"
acrname="prodrpsaasacr"
CDPXImageNameKeyToGrep="ame_unique_image_name"
deploymentEnvironment="Production"

source deploy_script.sh