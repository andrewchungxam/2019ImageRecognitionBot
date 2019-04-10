using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using SimplifiedWaterfallDialogBotV4.BotAccessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bot_Builder_Simplified_Echo_Bot_V4
{
    public class NameWaterfallDialog : WaterfallDialog
    {
        public static string DialogId { get; } = "nameDialog";
        public static NameWaterfallDialog BotInstance { get; } = new NameWaterfallDialog(DialogId, null);
        public NameWaterfallDialog(string dialogId, IEnumerable<WaterfallStep> steps)
            : base(dialogId, steps)
        {
            AddStep(FirstStepAsync);
            AddStep(NameStepAsync);
            AddStep(NameConfirmStepAsync);
        }

        private static async Task<DialogTurnResult> FirstStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
            // Running a prompt here means the next WaterfallStep will be run when the users response is received.
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"NAME WATERFALL STEP 1: This is the first step.  You can put your code in each of these steps."), cancellationToken);
            return await stepContext.NextAsync("Data from First Step", cancellationToken);
        }

        private static async Task<DialogTurnResult> NameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string stringFromFirstStep = (string)stepContext.Result;
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"NAME WATERFALL STEP 2: You can pass objects/strings step-to-step like this: {stringFromFirstStep}"), cancellationToken);
            return await stepContext.PromptAsync("name", new PromptOptions { Prompt = MessageFactory.Text("What is your name?") }, cancellationToken);
        }

        private async Task<DialogTurnResult> NameConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //WITHOUT SAVING STATE WITH ACCESSOR TO 'THEUSERSTATE'
            // We can send messages to the user at any point in the WaterfallStep.
            //await stepContext.Context.SendActivityAsync(MessageFactory.Text($"COLOR WATERFALL STEP 3: I like the color {stepContext.Result} too!"), cancellationToken);
            //END-WITHOUT SAVING STATE WITH ACCESSOR TO 'THEUSERSTATE'


            //WITH SAVING STATE WITH ACCESSOR TO 'THEUSERSTATE'
            var botState = await (stepContext.Context.TurnState["DialogBotConversationStateAndUserStateAccessor"] as DialogBotConversationStateAndUserStateAccessor).TheUserProfile.GetAsync(stepContext.Context);
            botState.Name = stepContext.Result.ToString();
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"NAME WATERFALL STEP 3: Thank you {botState.Name}! "), cancellationToken);
            //END-WITH SAVING STATE WITH ACCESSOR TO 'THEUSERSTATE'

            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}
