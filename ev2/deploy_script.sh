#!/usr/bin/env bash

echo "Fetching image name for contosorp"
imgname=$(cat contosorp-image-meta.json | grep -w "$CDPXImageNameKeyToGrep\"" | awk '{split($0,a,"\""); print a[4]}')

if [ "$deploymentEnvironment" = "Mooncake" ] || [ "$deployementEnvironment" = "Fairfax" ] ;
then
imgname="$CPDXContainerRepositoryOverride/${imgname#*/}"
fi

imgtag=$(cat contosorp-image-meta.json | grep -w "unique_tag\"" | awk '{split($0,a,"\""); print a[4]}')
echo "ContosoRP Image name = ${imgname}"

echo "Login to azure using az cli [Cloud = ${azCloudName}]"
az cloud set --name $azCloudName
az login --service-principal -u $username -p $password --tenant $tenantId
az account set -s $subscriptionId

az acr import -n $acrname --force --source $imgname -t contosorp:latest -t "contosorp:${imgtag}" -u $username -p $password
