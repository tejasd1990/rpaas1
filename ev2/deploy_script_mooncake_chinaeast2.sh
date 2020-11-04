#!/usr/bin/env bash

#make your script exit when a command fails.
set -o errexit
set -o pipefail
set -o nounset
set -o xtrace
set +x

azCloudName="AzureChinaCloud"
password="$(cat /mnt/secrets/ClustersSPSecret)"
username="b0e2a254-ceba-4d29-af0d-02bd2ee840fc"
tenantId="a55a4d5b-9241-49b1-b4ff-befa8db00269"
subscriptionId="de932dc7-3baf-44a0-8ec3-b7440ecd2527"
acrname="mooncakerpsaasacr"
CDPXImageNameKeyToGrep="ame_build_image_name"
deploymentEnvironment="Mooncake"
CPDXContainerRepositoryOverride="containerreplicationmcchinanorth1.azurecr.cn"

source deploy_script.sh