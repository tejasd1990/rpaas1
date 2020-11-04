// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ------------------------------------------------------------
namespace Provider.Helpers
{
    using System;

    /// <summary>
    /// Helper class for logging messages.
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// Helper method to log message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void LogMessage(string message)
        {
            // This is a sample implementation
            // Logging: this log will be stored to "DefaultTable" in your geneva namespace.
            var logMessage = @"{""Tablename"" : ""DefaultTable"", ""logMessage"": """ + message + " at " + DateTime.Now + @"""}";
            Console.WriteLine(logMessage);
        }
    }
}