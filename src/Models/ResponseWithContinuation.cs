// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ------------------------------------------------------------

namespace Provider.Models
{
    using System.Collections;
    using Newtonsoft.Json;

    /// <summary>
    /// Response with next link signifying continuation.
    /// </summary>
    /// <typeparam name="T">Type of response.</typeparam>
    public class ResponseWithContinuation<T>
        where T : IEnumerable
    {
        /// <summary>
        /// Gets or sets the value of response.
        /// </summary>
        [JsonProperty]
        public T Value
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the next link to query to get the remaining results.
        /// </summary>
        [JsonProperty]
        public string NextLink
        {
            get;
            set;
        }
    }
}