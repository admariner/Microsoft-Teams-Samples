﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//

using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder.TraceExtensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace ReceiveMessagesWithRSC
{
    /// <summary>
    /// A CloudAdapter with error handling capabilities.
    /// </summary>
    public class AdapterWithErrorHandler : CloudAdapter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AdapterWithErrorHandler"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="httpClientFactory">The HTTP client factory.</param>
        /// <param name="logger">The logger.</param>
        public AdapterWithErrorHandler(
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            ILogger<IBotFrameworkHttpAdapter> logger)
            : base(configuration, httpClientFactory, logger)
        {
            OnTurnError = async (turnContext, exception) =>
            {
                // Log any leaked exception from the application.
                logger.LogError(exception, $"[OnTurnError] unhandled error: {exception.Message}");

                // Uncomment the line below for local debugging.
                // await turnContext.SendActivityAsync($"Sorry, it looks like something went wrong. Exception Caught: {exception.Message}");

                // Send a trace activity, which will be displayed in the Bot Framework Emulator.
                await turnContext.TraceActivityAsync("OnTurnError Trace", exception.Message, "https://www.botframework.com/schemas/error", "TurnError");
            };
        }
    }
}
