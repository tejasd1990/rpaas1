// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ------------------------------------------------------------

namespace Provider
{
    using System;
    using Microsoft.AspNetCore.Hosting;

    /// <summary>
    /// The entry point class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main function.
        /// </summary>
        /// <param name="args">Main args.</param>
        public static void Main(string[] args)
        {
            try
            {
                new WebHostBuilder()
                    .WithArgs(args)
                    .Build()
                    .Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Role startup failed.");
                throw;
            }
        }
    }
}
