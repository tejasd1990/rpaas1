// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ------------------------------------------------------------
namespace Provider.Helpers
{
    using System;

    /// <summary>
    /// Helper class for URI engine.
    /// </summary>
    public static class UriEngine
    {
        /// <summary>
        /// Gets the resource group scope resource request URI.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="subscriptionId">The subscription Id.</param>
        /// <param name="resourceGroupName">The resource group name.</param>
        /// <param name="resourceProviderNamespace">The resource provider namespace.</param>
        /// <param name="resourceType">The resource type.</param>
        /// <param name="resourceName">The resource name.</param>
        /// <param name="apiVersion">The API version.</param>
        public static Uri GetResourceGroupResourceRequestUri(
            string endpoint,
            string subscriptionId,
            string resourceGroupName,
            string resourceProviderNamespace,
            string resourceType,
            string resourceName,
            string apiVersion)
        {
            if (string.IsNullOrWhiteSpace(resourceName))
            {
                return new Uri($"{endpoint}/subscriptions/{subscriptionId}/resourcegroups/{resourceGroupName}/providers/{resourceProviderNamespace}/{resourceType}?api-version={apiVersion}");
            }

            return new Uri($"{endpoint}/subscriptions/{subscriptionId}/resourcegroups/{resourceGroupName}/providers/{resourceProviderNamespace}/{resourceType}/{resourceName}?api-version={apiVersion}");
        }

        /// <summary>
        /// Gets the resource type scope resources request URI with optional $filter.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="subscriptionId">The subscription Id.</param>
        /// <param name="resourceProviderNamespace">The resource provider namespace.</param>
        /// <param name="resourceType">The resource type.</param>
        /// <param name="apiVersion">The API version.</param>
        /// <param name="filter">The OData filter query.</param>
        public static Uri GetResourceTypeResourceRequestUri(
            string endpoint,
            string subscriptionId,
            string resourceProviderNamespace,
            string resourceType,
            string apiVersion,
            string filter = null)
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                return new Uri($"{endpoint}/subscriptions/{subscriptionId}/providers/{resourceProviderNamespace}/{resourceType}?api-version={apiVersion}");
            }

            return new Uri($"{endpoint}/subscriptions/{subscriptionId}/providers/{resourceProviderNamespace}/{resourceType}?api-version={apiVersion}&$filter={filter}");
        }
    }
}