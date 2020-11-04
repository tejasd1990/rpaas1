// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ------------------------------------------------------------

namespace Provider.Models
{
    using System;

    /// <summary>
    /// The attribute to preserve the letter case for dictionary keys.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false)]
    public sealed class JsonPreserveCaseDictionaryAttribute : Attribute
    {
    }
}
