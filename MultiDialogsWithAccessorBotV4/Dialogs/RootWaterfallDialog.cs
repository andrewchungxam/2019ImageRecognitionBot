using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;

namespace Bot_Builder_Simplified_Echo_Bot_V4
{
    public class RootWaterfallDialog : WaterfallDialog
    {
        public static string DialogId { get; } = "rootDialog";
        public static RootWaterfallDialog BotInstance { get; } = new RootWaterfallDialog(DialogId, null);
        
        // YOU CAN DEFINE AS ARRAY AND THEN USE WHEN CALLING DIALOG-- BUT THIS ADDS SOME USAGE COMPLEXITY
        // ADDING 'ADD STEPS' IN CONSTRUCTOR LIMITS USAGE COMPLEXITY WHEN CALLING BOT
        //public WaterfallStep[] RootDialogWaterfallSteps { get; } = new WaterfallStep[]
        //{
        //    FirstStepAsync,
        //    NameStepAsync
        //};

        public RootWaterfallDialog(string dialogId, IEnumerable<WaterfallStep> steps)
            : base (dialogId, steps)
        {
            AddStep(FirstStepAsync);
            AddStep(PromptDialogChoiceStepAsync);
            AddStep(LaunchDialogStepAsync);
            AddStep(LoopDialogStepAsync);
        }

        private static async Task<DialogTurnResult> FirstStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
            // Running a prompt here means the next WaterfallStep will be run when the users response is received.
            //await stepContext.Context.SendActivityAsync(MessageFactory.Text($"ROOT WATERFALL STEP 1: This is the first step.  You can put your code in each of these steps."), cancellationToken);
            return await stepContext.NextAsync("Data from First Step", cancellationToken);
        }

        private static async Task<DialogTurnResult> PromptDialogChoiceStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string stringFromFirstStep = (string)stepContext.Result;

            return await stepContext.PromptAsync("dialogChoice", 
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("How can I help?"),
                    Choices = new[]
                    {
                        new Choice { Value = "New Employee" },
                        new Choice { Value = "Password Reset" },
                        new Choice { Value = "IT Links"},
                        new Choice { Value = "GitHub FAQ" },
                    },
                }, cancellationToken);
        }

        private async Task<DialogTurnResult> LaunchDialogStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var chosenDialogResponse = (stepContext.Result as FoundChoice)?.Value;

            if (chosenDialogResponse == "New Employee")
            {
                return await stepContext.BeginDialogAsync(FoodWaterfallDialog.DialogId);
            }

            if (chosenDialogResponse == "Password Reset")
            {
                return await stepContext.BeginDialogAsync(ColorWaterfallDialog.DialogId);
            }

            if (chosenDialogResponse == "IT Links")
            {
                return await stepContext.BeginDialogAsync(LinksWaterfallDialog.DialogId);
            }
            if (chosenDialogResponse == "GitHub FAQ")
            {
                return await stepContext.BeginDialogAsync(ThirdWaterfallDialog.DialogId);
            }

            return await stepContext.NextAsync();
        }

        private async Task<DialogTurnResult> LoopDialogStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await Task.Delay(3000);
            return await stepContext.ReplaceDialogAsync(RootWaterfallDialog.DialogId);
        }
    }
}
