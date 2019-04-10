// Licensed under the MIT License.
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace Bot_Builder_Simplified_Echo_Bot_V4
{
    public class SimplifiedEchoBotMiddleware2 : IMiddleware
    {
        public async Task OnTurnAsync(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken = default(CancellationToken))
        {

            //await turnContext.SendActivityAsync($"STEP 2: MIDDLEWARE - BEFORE ");

            //UNCOMMENT CODE WILL ADD A SKIP AVENUE THAT WILL STOP THE PIPELINE (IE. MIDDLEWARE 3 + BOT WILL NOT BE TRIGGERED)
            //if (turnContext.Activity.Type == ActivityTypes.Message && turnContext.Activity.Text != "skip")
            //{
                await next(cancellationToken);
            //}
            
            //await turnContext.SendActivityAsync($"STEP 6: MIDDLEWARE - AFTER ");
        }
    }
}