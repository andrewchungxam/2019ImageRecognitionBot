// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Integration;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Configuration;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MultiDialogsWithAccessorBotV4.BotAccessor;
using SimplifiedWaterfallDialogBotV4;
using SimplifiedWaterfallDialogBotV4.BotAccessor;

namespace Bot_Builder_Simplified_Echo_Bot_V4
{
    public class Startup
    {
        private bool _isProduction = false;

        public Startup(IHostingEnvironment env)
        {
            _isProduction = env.IsProduction();
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var secretKey = Configuration.GetSection("botFileSecret")?.Value;
            var botFilePath = Configuration.GetSection("botFilePath")?.Value;

            // Loads .bot configuration file and adds a singleton that your Bot can access through dependency injection.
            var botConfig = BotConfiguration.Load(botFilePath ?? @".\BotConfiguration.bot", secretKey);
            
            //#QNA 
            // Initialize Bot Connected Services clients.
            var connectedServices = InitBotServices(botConfig);
            services.AddSingleton(sp => connectedServices);

            services.AddBot<MultiDialogWithAccessorBot>(options =>
            {
            //MOVED ABOVE
            //var secretKey = Configuration.GetSection("botFileSecret")?.Value;
            //var botFilePath = Configuration.GetSection("botFilePath")?.Value;

            //// Loads .bot configuration file and adds a singleton that your Bot can access through dependency injection.
            //var botConfig = BotConfiguration.Load(botFilePath ?? @".\BotConfiguration.bot", secretKey);



            ////#QNA 
            //// Initialize Bot Connected Services clients.
            //var connectedServices = InitBotServices(botConfig);
            //services.AddSingleton(sp => connectedServices);
            //#QNA 
            //MOVED ABOVE

                services.AddSingleton(sp => botConfig ?? throw new InvalidOperationException($"The .bot config file could not be loaded. ({botConfig})"));

            // Retrieve current endpoint.
            var environment = _isProduction ? "production" : "development";
            var service = botConfig.Services.Where(s => s.Type == "endpoint" && s.Name == environment).FirstOrDefault();
            if (!(service is EndpointService endpointService))
            {
                throw new InvalidOperationException($"The .bot file does not contain an endpoint with name '{environment}'.");
            }
            options.CredentialProvider = new SimpleCredentialProvider(endpointService.AppId, endpointService.AppPassword);

            IStorage dataStore = new MemoryStorage();

            var conversationState = new ConversationState(dataStore);
            options.State.Add(conversationState);

            // Create and add user state.
            var userState = new UserState(dataStore);
            options.State.Add(userState);

            //SETUP ACCORDING TO MULTI-LINGUAL BOT FROM OFFICIAL SAMPLE 
            //TO SEE MORE OF HOW IT'S USED TAKE A LOOK AT THE OFFICIAL SAMPLES HERE: 
            //https://github.com/Microsoft/BotBuilder-Samples/tree/master/samples/csharp_dotnetcore 
            // Look for the sample Multilingual 

            var middleware3WithParameters = new SimplifiedEchoBotMiddleware3(userState.CreateProperty<string>("LanguagePreference"));

            options.Middleware.Add(new SimplifiedEchoBotMiddleware1());
            options.Middleware.Add(new SimplifiedEchoBotMiddleware2());
            options.Middleware.Add(middleware3WithParameters);
            });

            services.AddSingleton(sp =>
            {
                // We need to grab the conversationState we added on the options in the previous step.
                var options = sp.GetRequiredService<IOptions<BotFrameworkOptions>>().Value;
                if (options == null)
                {
                    throw new InvalidOperationException("BotFrameworkOptions must be configured prior to setting up the State Accessors");
                }

                var conversationState = options.State.OfType<ConversationState>().FirstOrDefault();
                if (conversationState == null)
                {
                    throw new InvalidOperationException("ConversationState must be defined and added before adding conversation-scoped state accessors.");
                }

                var userState = options.State.OfType<UserState>().FirstOrDefault();
                if (userState == null)
                {
                    throw new InvalidOperationException("UserState must be defined and added before adding user-scoped state accessors.");
                }

                // The dialogs will need a state store accessor. Creating it here once (on-demand) allows the dependency injection
                // to hand it to our IBot class that is create per-request.
                var accessors = new DialogBotConversationStateAndUserStateAccessor(conversationState, userState)
                {
                    ConversationDialogState = conversationState.CreateProperty<DialogState>("DialogState"),
                    TheUserProfile = userState.CreateProperty<UserProfile>("UserProfile"),
                    WelcomeUserState = userState.CreateProperty<WelcomeUserState>(DialogBotConversationStateAndUserStateAccessor.WelcomeUserName),
                    LanguagePreference = userState.CreateProperty<string>("LanguagePreference"),
                    
                };
                return accessors;
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseBotFramework();
        }



        //#QNA 
        /// <summary>
        /// Initialize the bot's references to external services.
        /// For example, the QnA Maker instance is created here. This external service is configured
        /// using the <see cref="BotConfiguration"/> class (based on the contents of your ".bot" file).
        /// </summary>
        /// <param name="config">The <see cref="BotConfiguration"/> object based on your ".bot" file.</param>
        /// <returns>A <see cref="BotConfiguration"/> representing client objects to access external services the bot uses.</returns>
        /// <seealso cref="BotConfiguration"/>
        /// <seealso cref="QnAMaker"/>
        private static BotServices InitBotServices(BotConfiguration config)
        {
            var qnaServices = new Dictionary<string, QnAMaker>();

            foreach (var service in config.Services)
            {
                switch (service.Type)
                {
                    case ServiceTypes.QnA:
                        {
                            // Create a QnA Maker that is initialized and suitable for passing
                            // into the IBot-derived class (QnABot).
                            var qna = (QnAMakerService)service;
                            if (qna == null)
                            {
                                throw new InvalidOperationException("The QnA service is not configured correctly in your '.bot' file.");
                            }

                            if (string.IsNullOrWhiteSpace(qna.KbId))
                            {
                                throw new InvalidOperationException("The QnA KnowledgeBaseId ('kbId') is required to run this sample. Please update your '.bot' file.");
                            }

                            if (string.IsNullOrWhiteSpace(qna.EndpointKey))
                            {
                                throw new InvalidOperationException("The QnA EndpointKey ('endpointKey') is required to run this sample. Please update your '.bot' file.");
                            }

                            if (string.IsNullOrWhiteSpace(qna.Hostname))
                            {
                                throw new InvalidOperationException("The QnA Host ('hostname') is required to run this sample. Please update your '.bot' file.");
                            }

                            var qnaEndpoint = new QnAMakerEndpoint()
                            {
                                KnowledgeBaseId = qna.KbId,
                                EndpointKey = qna.EndpointKey,
                                Host = qna.Hostname,
                            };

                            var qnaMaker = new QnAMaker(qnaEndpoint);
                            qnaServices.Add(qna.Name, qnaMaker);

                            break;
                        }
                }
            }

            var connectedServices = new BotServices(qnaServices);
            return connectedServices;
        }
        //#QNA 
    }
}
