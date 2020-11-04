// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ------------------------------------------------------------

namespace Provider.Models
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// The resource proxy definition object.
    /// </summary>
    public class Resource
    {
        /// <summary>
        /// Gets or sets the <c>API</c> version of the resource.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public string ApiVersion { get; set; }

        /// <summary>
        /// Gets or sets the id for the resource.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the subscription.
        /// </summary>
        [JsonIgnore]
        public string SubscriptionId { get; set; }

        /// <summary>
        /// Gets or sets the name of the resource group to which the resource belongs.
        /// </summary>
        [JsonIgnore]
        public string ResourceGroup { get; set; }

        /// <summary>
        /// Gets or sets the name of the resource definition.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type of the resource definition.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the <c>sku</c> of the resource definition.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public JToken Sku { get; set; }

        /// <summary>
        /// Gets or sets the kind of the resource definition.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public string Kind { get; set; }

        /// <summary>
        /// Gets or sets the managed by property.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public string ManagedBy { get; set; }

        /// <summary>
        /// Gets or sets the resource location.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the availability zones.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public string[] Zones { get; set; }

        /// <summary>
        /// Gets or sets the resource plan.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public JToken Plan { get; set; }

        /// <summary>
        /// Gets or sets the <c>etag</c>.
        /// </summary>
        [JsonProperty(Required = Required.Default, PropertyName = "etag")]
        public string ETag { get; set; }

        /// <summary>
        /// Gets or sets the tags.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public InsensitiveDictionary<string> Tags { get; set; }

        /// <summary>
        /// Gets or sets the scale.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public JToken Scale { get; set; }

        /// <summary>
        /// Gets or sets the resource identity.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public JToken Identity { get; set; }

        /// <summary>
        /// Gets or sets the properties.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        public JToken Properties { get; set; }
    }
}