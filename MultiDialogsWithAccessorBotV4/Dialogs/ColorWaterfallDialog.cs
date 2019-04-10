using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

using SimplifiedWaterfallDialogBotV4.BotAccessor;

namespace Bot_Builder_Simplified_Echo_Bot_V4
{
    public class ColorWaterfallDialog : WaterfallDialog
    {
        public static string DialogId { get; } = "colorDialog";
        public static ColorWaterfallDialog BotInstance { get; } = new ColorWaterfallDialog(DialogId, null);

        public ColorWaterfallDialog(string dialogId, IEnumerable<WaterfallStep> steps)
            : base(dialogId, steps)
        {
            AddStep(FirstStepAsync);
            AddStep(NameStepAsync);
            AddStep(NameConfirmStepAsync);
            AddStep(OneNameConfirmStepAsync);
            AddStep(TwoNameConfirmStepAsync);
            AddStep(ThreeNameConfirmStepAsync);

        }

        private static async Task<DialogTurnResult> FirstStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
            // Running a prompt here means the next WaterfallStep will be run when the users response is received.
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Welcome to the Password reset center.  We're going to ask you a few questions to help you reset your password."), cancellationToken);
            return await stepContext.NextAsync("Data from First Step", cancellationToken);
        }

        private static async Task<DialogTurnResult> NameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string stringFromFirstStep = (string)stepContext.Result;
            return await stepContext.PromptAsync("colorName", new PromptOptions { Prompt = MessageFactory.Text("What is your company email address?") }, cancellationToken);
        }

        private async Task<DialogTurnResult> NameConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //WITHOUT SAVING STATE WITH ACCESSOR TO 'THEUSERSTATE'
            // We can send messages to the user at any point in the WaterfallStep.
            //await stepContext.Context.SendActivityAsync(MessageFactory.Text($"COLOR WATERFALL STEP 3: I like the color {stepContext.Result} too!"), cancellationToken);
            //END-WITHOUT SAVING STATE WITH ACCESSOR TO 'THEUSERSTATE'

            //WITH SAVING STATE WITH ACCESSOR TO 'THEUSERSTATE'
            var botState = await (stepContext.Context.TurnState["DialogBotConversationStateAndUserStateAccessor"] as DialogBotConversationStateAndUserStateAccessor).TheUserProfile.GetAsync(stepContext.Context);
            botState.PassResetEmail = stepContext.Result.ToString();
            //END-WITH SAVING STATE WITH ACCESSOR TO 'THEUSERSTATE'

            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Thank you {botState.ITName} "), cancellationToken);
            return await stepContext.PromptAsync("passResetBirthDate", new PromptOptions { Prompt = MessageFactory.Text("What is your birthdate? (Format: MM/DD/YYYY)") }, cancellationToken);
        }

        private async Task<DialogTurnResult> OneNameConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //WITHOUT SAVING STATE WITH ACCESSOR TO 'THEUSERSTATE'
            // We can send messages to the user at any point in the WaterfallStep.
            //await stepContext.Context.SendActivityAsync(MessageFactory.Text($"COLOR WATERFALL STEP 3: I like the color {stepContext.Result} too!"), cancellationToken);
            //END-WITHOUT SAVING STATE WITH ACCESSOR TO 'THEUSERSTATE'

            //WITH SAVING STATE WITH ACCESSOR TO 'THEUSERSTATE'
            var botState = await (stepContext.Context.TurnState["DialogBotConversationStateAndUserStateAccessor"] as DialogBotConversationStateAndUserStateAccessor).TheUserProfile.GetAsync(stepContext.Context);
            botState.PassResetBirthDate = stepContext.Result.ToString();
            //END-WITH SAVING STATE WITH ACCESSOR TO 'THEUSERSTATE'

            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Thank you for confirming. "), cancellationToken);

            return await stepContext.PromptAsync("passResetPhoneNumber", new PromptOptions { Prompt = MessageFactory.Text("What is your listed mobile number where we will text you your temporary password. (ie. 555-555-5555) ") }, cancellationToken);
        }
        
        private async Task<DialogTurnResult> TwoNameConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //WITHOUT SAVING STATE WITH ACCESSOR TO 'THEUSERSTATE'
            // We can send messages to the user at any point in the WaterfallStep.
            //await stepContext.Context.SendActivityAsync(MessageFactory.Text($"COLOR WATERFALL STEP 3: I like the color {stepContext.Result} too!"), cancellationToken);
            //END-WITHOUT SAVING STATE WITH ACCESSOR TO 'THEUSERSTATE'

            //WITH SAVING STATE WITH ACCESSOR TO 'THEUSERSTATE'
            var botState = await (stepContext.Context.TurnState["DialogBotConversationStateAndUserStateAccessor"] as DialogBotConversationStateAndUserStateAccessor).TheUserProfile.GetAsync(stepContext.Context);
            botState.PassResetMobileNumber = stepContext.Result.ToString();
            //END-WITH SAVING STATE WITH ACCESSOR TO 'THEUSERSTATE'

            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Thank you for confirming. "), cancellationToken);
            return await stepContext.PromptAsync("passResetOTPDevice", new PromptOptions { Prompt = MessageFactory.Text("Take a look at your OTP Device.  What is the currently listed code? (ie. ### ###)") }, cancellationToken);

        }

        private async Task<DialogTurnResult> ThreeNameConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //WITHOUT SAVING STATE WITH ACCESSOR TO 'THEUSERSTATE'
            // We can send messages to the user at any point in the WaterfallStep.
            //await stepContext.Context.SendActivityAsync(MessageFactory.Text($"COLOR WATERFALL STEP 3: I like the color {stepContext.Result} too!"), cancellationToken);
            //END-WITHOUT SAVING STATE WITH ACCESSOR TO 'THEUSERSTATE'

            //WITH SAVING STATE WITH ACCESSOR TO 'THEUSERSTATE'
            var botState = await (stepContext.Context.TurnState["DialogBotConversationStateAndUserStateAccessor"] as DialogBotConversationStateAndUserStateAccessor).TheUserProfile.GetAsync(stepContext.Context);
            botState.PassResetOTPDevice = stepContext.Result.ToString();
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Thank you. Your temporary password has been sent to your mobile device. Please immediately log in as it will expire in 4 minutes."), cancellationToken);
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"We have also sent a notification of your password reset to your work email, your mobile device, and your listed personal email."), cancellationToken);
            //END-WITH SAVING STATE WITH ACCESSOR TO 'THEUSERSTATE'

            await Task.Delay(4000);
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}
