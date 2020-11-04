// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ------------------------------------------------------------

namespace Provider.Models
{
    /// <summary>
    /// The error response.
    /// </summary>
    public class ErrorResponse
    {
        /// <summary>
        /// Gets or sets the status of the request.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the error object.
        /// </summary>
        public Error Error { get; set; }
    }
}
