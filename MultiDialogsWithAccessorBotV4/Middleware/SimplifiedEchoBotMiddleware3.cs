// Licensed under the MIT License.
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using SimplifiedWaterfallDialogBotV4.BotAccessor;

namespace Bot_Builder_Simplified_Echo_Bot_V4
{
    public class SimplifiedEchoBotMiddleware3 : IMiddleware
    {
        private readonly IStatePropertyAccessor<string> _languageStateProperty;
        public SimplifiedEchoBotMiddleware3(IStatePropertyAccessor<string> languageStateProperty)
        {
            _languageStateProperty = languageStateProperty ?? throw new ArgumentNullException(nameof(languageStateProperty));
        }
        public async Task OnTurnAsync(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken = default(CancellationToken))
        {

            if (turnContext.Activity.Type == ActivityTypes.Message && turnContext.Activity.Text == "name")
            {
                var didTypeNameString = "name";

                // Update user state flag to reflect bot was given a specific prompt
                turnContext.TurnState.Add("didTypeName", didTypeNameString);
                await next(cancellationToken);

                //
                //BELOW SETUP ACCORDING TO MULTI-LINGUAL BOT FROM OFFICIAL SAMPLE 
                //THIS WILL NOT WORK IN OUR SCENARIO -- BUT YOU CAN SEE AN APPLICATION OF THIS TECHNIQUE IN THE MULTI-LINUAL BOT
                //IN THE OFFICIAL SAMPLES
                //TO SEE MORE OF HOW IT'S USED TAKE A LOOK AT THE OFFICIAL SAMPLES HERE: 
                //https://github.com/Microsoft/BotBuilder-Samples/tree/master/samples/csharp_dotnetcore 
                // Look for the sample Multilingual 
                //
                //didBotWelcomeUser = await _languageStateProperty.GetAsync(turnContext, () => string.Empty, cancellationToken);
                //await _languageStateProperty.SetAsync(turnContext, didBotWelcomeUser);
            }
            else
            {
                await next(cancellationToken);
            }
        }
    }
}