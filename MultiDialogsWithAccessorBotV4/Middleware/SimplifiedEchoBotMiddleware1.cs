// Licensed under the MIT License.
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace Bot_Builder_Simplified_Echo_Bot_V4
{
    public class SimplifiedEchoBotMiddleware1 : IMiddleware
    {
        public async Task OnTurnAsync(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken = default(CancellationToken))
        {
            //await turnContext.SendActivityAsync($"STEP 1: MIDDLEWARE - BEFORE ");

            await next(cancellationToken);

           // await turnContext.SendActivityAsync($"STEP 7: MIDDLEWARE - AFTER ");
        }
    }
}