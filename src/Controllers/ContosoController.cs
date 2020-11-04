// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ------------------------------------------------------------

namespace Provider.Controllers
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json.Serialization;
    using Provider.Helpers;
    using Provider.Models;

    /// <summary>
    /// The contoso controller.
    /// </summary>
    public class ContosoController : Controller
    {
        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        private IConfiguration Configuration { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContosoController"/> class.
        /// </summary>
        /// <param name="configuration">Application configuration</param>
        public ContosoController(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        /// <summary>
        /// Method that gets called if subscribed for ResourceCreationValidate trigger.
        /// </summary>
        /// <param name="subscriptionId">The subscription Id.</param>
        /// <param name="resourceGroup">The resource group.</param>
        /// <param name="providerNamespace">The provider namespace.</param>
        /// <param name="resourceType">The resource type.</param>
        /// <param name="resourceName">The resource name.</param>
        [HttpPost]
        [ActionName("OnResourceCreationValidate")]
        public async Task<IActionResult> OnResourceCreationValidate(string subscriptionId, string resourceGroup, string providerNamespace, string resourceType, string resourceName)
        {
            /*** Implement this method if you want to opt for "ResourceCreationValidate" extension for your resource type. ***/
            Logger.LogMessage("OnResourceCreationValidate called");

            return await this
                .ValidateResourceCreation(
                    subscriptionId: subscriptionId,
                    resourceGroup: resourceGroup,
                    providerNamespace: providerNamespace,
                    resourceType: resourceType,
                    tenantId: this.Request.Headers["x-ms-client-tenant-id"])
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Method that gets called if subscribed for ResourceCreationBegin trigger.
        /// </summary>
        /// <param name="subscriptionId">The subscription Id.</param>
        /// <param name="resourceGroup">The resource group.</param>
        /// <param name="providerNamespace">The provider namespace.</param>
        /// <param name="resourceType">The resource type.</param>
        /// <param name="resourceName">The resource name.</param>
        [HttpPut]
        [ActionName("OnResourceCreationBegin")]
        public IActionResult OnResourceCreationBegin(string subscriptionId, string resourceGroup, string providerNamespace, string resourceType, string resourceName)
        {
            /*** Implement this method if you want to opt for "ResourceCreationBegin" extension for your resource type. ***/
            Logger.LogMessage("OnResourceCreationBegin called");

            // Required response format.
            return ContosoController.CreateResponse(
                statusCode: HttpStatusCode.OK,
                value: this.AppendResourceWithInternalMetadata());
        }

        /// <summary>
        /// Method that gets called if subscribed for ResourceCreationComplete trigger.
        /// </summary>
        /// <param name="subscriptionId">The subscription Id.</param>
        /// <param name="resourceGroup">The resource group.</param>
        /// <param name="providerNamespace">The provider namespace.</param>
        /// <param name="resourceType">The resource type.</param>
        /// <param name="resourceName">The resource name.</param>
        [HttpPost]
        [ActionName("OnResourceCreationCompleted")]
        public async Task<IActionResult> OnResourceCreationCompleted(string subscriptionId, string resourceGroup, string providerNamespace, string resourceType, string resourceName)
        {
            /*** Implement this method if you want to opt for "OnResourceCreationCompleted" extension for your resource type. ***/
            Logger.LogMessage("OnResourceCreationCompleted called");

            // Do post creation processing here ex: start billing
            await this
                .CompleteResourceCreation(
                subscriptionId: subscriptionId,
                resourceGroup: resourceGroup,
                providerNamespace: providerNamespace,
                resourceType: resourceType,
                resourceName: resourceName,
                tenantId: this.Request.Headers["x-ms-client-tenant-id"]).ConfigureAwait(false);

            // Required response format with empty body.
            return ContosoController.CreateResponse(statusCode: HttpStatusCode.OK);
        }

        /// <summary>
        /// Method that gets called if subscribed for ResourceDeletionValidate trigger.
        /// </summary>
        /// <param name="subscriptionId">The subscription Id.</param>
        /// <param name="resourceGroup">The resource group.</param>
        /// <param name="providerNamespace">The provider namespace.</param>
        /// <param name="resourceType">The resource type.</param>
        /// <param name="resourceName">The resource name.</param>
        [HttpPost]
        [ActionName("OnResourceDeletionValidate")]
        public IActionResult OnResourceDeletionValidate(string subscriptionId, string resourceGroup, string providerNamespace, string resourceType, string resourceName)
        {
            /*** Implement this method if you want to opt for "OnResourceDeletionValidate" extension for your resource type. ***/
            Logger.LogMessage("OnResourceDeletionValidate called");

            // Do pre deletion validation here
            // This is a sample implementation
            return this
                .ValidateResourceDeletion(
                    subscriptionId: subscriptionId,
                    resourceGroup: resourceGroup,
                    providerNamespace: providerNamespace,
                    resourceType: resourceType);
        }

        /// <summary>
        /// Method that gets called if subscribed for SubscriptionLifeCycleNotification trigger.
        /// </summary>
        /// <param name="subscriptionId">The subscription Id.</param>
        /// <param name="providerNamespace">The provider namespace.</param>
        /// <param name="resourceType">The resource type.</param>
        [HttpPut]
        [ActionName("OnSubscriptionLifeCycleNotification")]
        public async Task<IActionResult> OnSubscriptionLifeCycleNotification(string subscriptionId, string providerNamespace, string resourceType)
        {
            /*** Implement this method if you want to opt for "SubscriptionLifeCycleNotification" extension for your resource type. ***/
            Logger.LogMessage("OnSubscriptionLifeCycleNotification called");

            // Do subscription state change validation here
            // This is a sample implementation
            // We also use OData $filter in this sample.
            await this
                .ValidateSubscriptionLifeCycleNotification(
                    subscriptionId: subscriptionId,
                    providerNamespace: providerNamespace,
                    resourceType: resourceType,
                    tenantId: this.Request.Headers["x-ms-client-tenant-id"])
                .ConfigureAwait(false);

            // Required response format with empty body.
            return ContosoController.CreateResponse(statusCode: HttpStatusCode.OK);
        }

        #region Private methods

        /// <summary>
        /// Validates if resource can be created.
        /// </summary>
        /// <param name="subscriptionId">The subscription Id.</param>
        /// <param name="resourceGroup">The resource group.</param>
        /// <param name="providerNamespace">The provider namespace.</param>
        /// <param name="resourceType">The resource type.</param>
        /// <param name="tenantId">The tenant Id.</param>
        private async Task<IActionResult> ValidateResourceCreation(string subscriptionId, string resourceGroup, string providerNamespace, string resourceType, string tenantId)
        {
            var errorResponse = this.ValidateAndGetErrorResponse(
                subscriptionId: subscriptionId,
                resourceGroup: resourceGroup,
                providerNamespace: providerNamespace,
                resourceType: resourceType,
                responseMessage: "SampleErrorMessage - Please don't create this resource. This is dangerous.");

            if (errorResponse == null)
            {
                return await this
                    .ValidateMaximumResourceCounts(
                        subscriptionId: subscriptionId,
                        resourceGroup: resourceGroup,
                        providerNamespace: providerNamespace,
                        resourceType: resourceType,
                        tenantId: tenantId)
                    .ConfigureAwait(continueOnCapturedContext: false);
            }

            // Required response format in case of validation failure.
            return ContosoController.CreateResponse(
                statusCode: HttpStatusCode.OK,
                value: errorResponse);
        }

        [HttpGet]
        [ActionName("OnHello")]
        public IActionResult Hello()
        {
            // Required response format.
            return ContosoController.CreateResponse(
                statusCode: HttpStatusCode.OK,
                value: "hELLO");
        }

        /// <summary>
        /// Validates if the number of resources in the resource group is under the limit.
        /// </summary>
        /// <param name="subscriptionId">The subscription Id.</param>
        /// <param name="resourceGroup">The resource group.</param>
        /// <param name="providerNamespace">The provider namespace.</param>
        /// <param name="resourceType">The resource type.</param>
        /// <param name="tenantId">The tenant Id.</param>
        private async Task<IActionResult> ValidateMaximumResourceCounts(string subscriptionId, string resourceGroup, string providerNamespace, string resourceType, string tenantId)
        {
            ErrorResponse errorRespone = null;

            // Call into ARM to get all resources of this resource type within the resource group.
            var resourceCollectionUri = UriEngine.GetResourceGroupResourceRequestUri(
                endpoint: this.ArmEndpoint,
                subscriptionId: subscriptionId,
                resourceGroupName: resourceGroup,
                resourceProviderNamespace: providerNamespace,
                resourceType: resourceType,
                resourceName: null,
                apiVersion: this.ApiVersion);

            var responseFromARM = await this.CallRPSaaSForMetadata(resourceCollectionUri, tenantId, HttpMethod.Get).ConfigureAwait(false);

            Logger.LogMessage($"Response status from ARM: '{responseFromARM.StatusCode}'");

            if (responseFromARM.StatusCode == HttpStatusCode.OK)
            {
                var reader = new StreamReader(await responseFromARM.Content.ReadAsStreamAsync().ConfigureAwait(false));
                var responseContent = reader.ReadToEnd();
                var resourcesCollection = JsonConvert.DeserializeObject<ResponseWithContinuation<Resource[]>>(responseContent);

                if (resourcesCollection.Value != null)
                {
                    // Return error if we have more than max count resources of this type in the resource group.
                    if (resourcesCollection.Value.Count() >= this.MaxCount)
                    {
                        errorRespone = new ErrorResponse
                        {
                            Error = new Error
                            {
                                Code = "SampleValidationErrorCode",
                                Message = "SampleValidationErrorMessage - Maximum number of resources per resource group count hit.",
                            },
                            Status = "Failed",
                        };
                    }
                }
            }

            // Required response format in case of validation failure.
            return ContosoController.CreateResponse(
                statusCode: HttpStatusCode.OK,
                value: errorRespone);
        }

        /// <summary>
        /// Helper method to append internal metadata.
        /// </summary>
        private Resource AppendResourceWithInternalMetadata()
        {
            // Deserializing the request body
            var reader = new StreamReader(this.Request.Body);
            var input = reader.ReadToEnd();
            var resource = JsonConvert.DeserializeObject<Resource>(input);

            if (resource != null)
            {
                // Provision your resource here, if required
                // Create/update your private data (internal metadata). This will never be returned to end user.
                var internalMetadata = (JObject)resource.Properties["internalMetadata"];

                if (internalMetadata == null)
                {
                    internalMetadata = new JObject();
                    internalMetadata["description"] = "This is your private data.";
                    internalMetadata["createdTime"] = DateTime.Now;
                    internalMetadata["lastUpdatedTime"] = DateTime.Now;
                }
                else
                {
                    internalMetadata["lastUpdatedTime"] = DateTime.Now;
                }

                resource.Properties["internalMetadata"] = internalMetadata;
            }

            return resource;
        }

        /// <summary>
        /// Completes the resource creation.
        /// </summary>
        /// <param name="subscriptionId">The subscription Id.</param>
        /// <param name="resourceGroup">The resource group.</param>
        /// <param name="providerNamespace">The provider namespace.</param>
        /// <param name="resourceType">The resource type.</param>
        /// <param name="resourceName">The resource name.</param>
        /// <param name="tenantId">The tenant Id.</param>
        private async Task CompleteResourceCreation(string subscriptionId, string resourceGroup, string providerNamespace, string resourceType, string resourceName, string tenantId)
        {
            // Call into ARM to get all resources of this resource type within the resource group.
            var resourceRequestUri = UriEngine.GetResourceGroupResourceRequestUri(
                endpoint: this.ArmEndpoint,
                subscriptionId: subscriptionId,
                resourceGroupName: resourceGroup,
                resourceProviderNamespace: providerNamespace,
                resourceType: resourceType,
                resourceName: resourceName,
                apiVersion: this.ApiVersion);

            var getResponseFromARM = await this.CallRPSaaSForMetadata(resourceRequestUri, tenantId, HttpMethod.Get).ConfigureAwait(false);
            Logger.LogMessage($"Response status from ARM for GET resource '{resourceRequestUri}': '{getResponseFromARM.StatusCode}'");

            if (getResponseFromARM.StatusCode == HttpStatusCode.OK)
            {
                var reader = new StreamReader(await getResponseFromARM.Content.ReadAsStreamAsync().ConfigureAwait(false));
                var getContent = reader.ReadToEnd();
                var resource = JsonConvert.DeserializeObject<Resource>(getContent);

                if (resource != null)
                {
                    // Add some tags on this resource.
                    // In order to do a PUT, ensure that all the properties specified in the swagger
                    // for PUT are present in the payload.
                    var updatedResource = new Resource
                    {
                        Tags = resource.Tags,
                    };

                    if (updatedResource.Tags.ContainsKey("contosoPrivateTag"))
                    {
                        updatedResource.Tags["contosoPrivateTag"] = $"Updated in OnResourceCreationCompleted at '{DateTime.UtcNow}'";
                    }
                    else
                    {
                        updatedResource.Tags.TryAdd("contosoPrivateTag", $"Updated in OnResourceCreationCompleted at '{DateTime.UtcNow}'");
                    }

                    var putResponseFromARM = await this.CallRPSaaSForMetadata(resourceRequestUri, tenantId, HttpMethod.Patch, updatedResource).ConfigureAwait(false);

                    Logger.LogMessage($"Response code from ARM for PUT: '{putResponseFromARM.StatusCode}'");
                }
            }
        }

        /// <summary>
        /// Validates if resource can be deleted.
        /// </summary>
        /// <param name="subscriptionId">The subscription Id.</param>
        /// <param name="resourceGroup">The resource group.</param>
        /// <param name="providerNamespace">The provider namespace.</param>
        /// <param name="resourceType">The resource type.</param>
        private IActionResult ValidateResourceDeletion(string subscriptionId, string resourceGroup, string providerNamespace, string resourceType)
        {
            var errorResponse = this.ValidateAndGetErrorResponse(
                subscriptionId: subscriptionId,
                resourceGroup: resourceGroup,
                providerNamespace: providerNamespace,
                resourceType: resourceType,
                responseMessage: "SampleErrorMessage - Please don't delete this resource. This is important.");

            // Required response format in case of validation failure.
            return ContosoController.CreateResponse(
                statusCode: HttpStatusCode.OK,
                value: errorResponse);
        }

        /// <summary>
        /// Validates if subscription state change can occur.
        /// </summary>
        /// <param name="subscriptionId">The subscription Id.</param>
        /// <param name="providerNamespace">The provider namespace.</param>
        /// <param name="resourceType">The resource type.</param>
        /// <param name="tenantId">The tenant ID of the subscription.</param>
        private async Task ValidateSubscriptionLifeCycleNotification(string subscriptionId, string providerNamespace, string resourceType, string tenantId)
        {
            // Call into ARM to get all resources of this resource type which are in successful provisioning state.
            var resourceRequestUri = UriEngine.GetResourceTypeResourceRequestUri(
                endpoint: this.ArmEndpoint,
                subscriptionId: subscriptionId,
                resourceProviderNamespace: providerNamespace,
                resourceType: resourceType,
                apiVersion: this.ApiVersion,
                filter: "ProvisioningState eq 1 AND Properties.group eq 'Avengers'");

            var getResponseFromARM = await this.CallRPSaaSForMetadata(resourceRequestUri, tenantId, HttpMethod.Get).ConfigureAwait(false);
            Logger.LogMessage($"Response status from ARM for GET resources '{resourceRequestUri}': '{getResponseFromARM.StatusCode}'");

            if (getResponseFromARM.StatusCode == HttpStatusCode.OK)
            {
                // Do any validations.
                var reader = new StreamReader(await getResponseFromARM.Content.ReadAsStreamAsync().ConfigureAwait(false));
                var responseContent = reader.ReadToEnd();
                var resourcesCollection = JsonConvert.DeserializeObject<ResponseWithContinuation<Resource[]>>(responseContent);

                Logger.LogMessage($"Retrived '{resourcesCollection?.Value?.Length}' resources from ARM for GET resources '{resourceRequestUri}'.");
            }
        }

        /// <summary>
        /// Validates if resource is valid.
        /// </summary>
        /// <param name="subscriptionId">The subscription Id.</param>
        /// <param name="resourceGroup">The resource group.</param>
        /// <param name="providerNamespace">The provider namespace.</param>
        /// <param name="resourceType">The resource type.</param>
        /// <param name="responseMessage">The response message.</param>
        private ErrorResponse ValidateAndGetErrorResponse(string subscriptionId, string resourceGroup, string providerNamespace, string resourceType, string responseMessage)
        {
            ErrorResponse errorResponse = null;

            if (subscriptionId == null || resourceGroup == null || providerNamespace == null || resourceType == null || resourceType == null)
            {
                errorResponse = new ErrorResponse
                {
                    Error = new Error
                    {
                        Code = "SampleErrorCode",
                        Message = responseMessage,
                    },
                    Status = "Failed",
                };
            }

            return errorResponse;
        }

        /// <summary>
        /// Async method to call into RPaaS through ARM to get metadata from the storage layer.
        /// </summary>
        /// <param name="requestUri">The request URI.</param>
        /// <param name="tenantId">The tenant Id.</param>
        /// <param name="httpMethod">The HTTP method.</param>
        /// <param name="resource">The resource.</param>
        private async Task<HttpResponseMessage> CallRPSaaSForMetadata(Uri requestUri, string tenantId, HttpMethod httpMethod, Resource resource = null)
        {
            using (var httpClient = new HttpClient())
            {
                var authenticationResult = await TokensHelper.GetAccessToken(this.Configuration, tenantId).ConfigureAwait(false);
                var proxyRequest = new HttpRequestMessage(httpMethod, requestUri);

                if (authenticationResult?.AccessToken != null)
                {
                    Logger.LogMessage("Got access token successfully");
                    proxyRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authenticationResult.AccessToken);
                }

                if (resource != null)
                {
                    var json = JsonConvert.SerializeObject(resource, ObjectSerializationSettings);
                    proxyRequest.Content = new StringContent(json, Encoding.UTF8, "application/json");
                }

                Logger.LogMessage($"Calling ARM to '{httpMethod}' for '{requestUri}'");

                return await httpClient.SendAsync(proxyRequest).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Creates http responses.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="value">The value.</param>
        private static JsonResult CreateResponse(HttpStatusCode statusCode, object value = null)
        {
            var response = new JsonResult(value)
            {
                StatusCode = (int)statusCode,
            };

            return response;
        }

        /// <summary>
        /// The JSON object serialization settings.
        /// </summary>
        private static readonly JsonSerializerSettings ObjectSerializationSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
        };

        #endregion

        #region Config Values

        /// <summary>
        /// Gets the ARM endpoint.
        /// </summary>
        private string ArmEndpoint { get => this.Configuration["Microsoft.Contoso.ContosoApp.ArmEndpoint"]; }

        /// <summary>
        /// Gets the api version.
        /// </summary>
        private string ApiVersion { get => this.Configuration["Microsoft.Contoso.ContosoApp.ApiVersion"]; }

        /// <summary>
        /// Gets the maximum count of resources.
        /// </summary>
        private int MaxCount { get => Convert.ToInt32(this.Configuration["Microsoft.Contoso.ContosoApp.MaxCount"]); }

        #endregion
    }
}