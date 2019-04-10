using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using SimplifiedWaterfallDialogBotV4.BotAccessor;

namespace Bot_Builder_Simplified_Echo_Bot_V4
{
    public class ThirdWaterfallDialog : WaterfallDialog
    {
        public static string DialogId { get; } = "thirdWaterfallDialog";
        public static WaterfallDialog BotInstance { get; } = new ThirdWaterfallDialog(DialogId, null);

        public ThirdWaterfallDialog(string dialogId, IEnumerable<WaterfallStep> steps)
            : base(dialogId, steps)
        {
            AddStep(FirstStepAsync);
            AddStep(NameStepAsync);
            AddStep(NameConfirmStepAsync);
        }

        private static VideoCard GetVideoCard()
        {
            var videoCard = new VideoCard
            {
                Title = "What is GitHub?",
                //Subtitle = "by GitHub",
                Text = "Ever wondered how GitHub works? Let's see how Timmy and his team use GitHub.",
                Image = new ThumbnailUrl
                {
                    Url = "https://upload.wikimedia.org/wikipedia/commons/e/eb/Ei-sc-github.svg",
                },
                Media = new List<MediaUrl>
                {
                    new MediaUrl()
                    {
                        Url = "https://www.youtube.com/watch?v=w3jLJU7DT5E",
                    },
                },
                Buttons = new List<CardAction>
                {
                    new CardAction()
                    {
                        Title = "Learn More",
                        Type = ActionTypes.OpenUrl,
                        Value = "https://enterprise.github.com/faq",
                    },
                },
            };

            return videoCard;
        }
        private static async Task<DialogTurnResult> FirstStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //COMMENT OUT ON BEHALF OF TEAMS - VIDEO CARDS ARE NOT SUPPORTED ON TEAMS
            //var welcomeUserState = await (stepContext.Context.TurnState["DialogBotConversationStateAndUserStateAccessor"] as DialogBotConversationStateAndUserStateAccessor).WelcomeUserState.GetAsync(stepContext.Context);
            //if (welcomeUserState.DidSeeVideo == false)
            //{
            //    welcomeUserState.DidSeeVideo = true;

            //    // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
            //    // Running a prompt here means the next WaterfallStep will be run when the users response is received.
            //    //await stepContext.Context.SendActivityAsync(MessageFactory.Text($"THIRD WATERFALL STEP 1: This is the first step.  You can put your code in each of these steps."), cancellationToken);

            //    var reply = stepContext.Context.Activity.CreateReply();
            //    reply.Attachments = new List<Attachment>();
            //    reply.Attachments.Add(GetVideoCard().ToAttachment());
            //    // Send the card(s) to the user as an attachment to the activity
            //    await stepContext.Context.SendActivityAsync(reply, cancellationToken);

            //    await Task.Delay(3000);
            //}

            return await stepContext.NextAsync("Data from First Step", cancellationToken);

        }

        private static async Task<DialogTurnResult> NameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //! DO NOT CHANGE THE NAME OF THIS DIALOG -- thirdWaterName -- iBOT CHECKS AND ALLOWS QNA ANSWERS THROUGH VIA THIS DIALOG
            return await stepContext.PromptAsync("thirdWaterName", new PromptOptions { Prompt = MessageFactory.Text("What questions can I answer about GitHub?") }, cancellationToken);
        }

        private async Task<DialogTurnResult> NameConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.ReplaceDialogAsync(ThirdWaterfallDialog.DialogId, false, cancellationToken);
        }
    }
}
