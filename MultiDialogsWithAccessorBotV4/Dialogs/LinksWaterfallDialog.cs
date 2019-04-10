using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using SimplifiedWaterfallDialogBotV4.BotAccessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using SimplifiedWaterfallDialogBotV4.BotAccessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bot_Builder_Simplified_Echo_Bot_V4
    {
        public class LinksWaterfallDialog : WaterfallDialog
        {
            public static string DialogId { get; } = "linksDialog";
            public static LinksWaterfallDialog BotInstance { get; } = new LinksWaterfallDialog(DialogId, null);
            public LinksWaterfallDialog(string dialogId, IEnumerable<WaterfallStep> steps)
                : base(dialogId, steps)
            {
                AddStep(ShowHeroCardsStepAsync);

                AddStep(FirstStepAsync);
                //AddStep(NameStepAsync);
                //AddStep(NameConfirmStepAsync);
                //AddStep(Hero1StepAsync);
                //AddStep(Hero1ConfirmStepAsync);
            }

            private static async Task<DialogTurnResult> FirstStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
            {
                // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
                // Running a prompt here means the next WaterfallStep will be run when the users response is received.
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"FOOD WATERFALL STEP 1: This is the first step.  You can put your code in each of these steps."), cancellationToken);
                return await stepContext.NextAsync("Data from First Step", cancellationToken);
            }

            private static async Task<DialogTurnResult> NameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
            {
                string stringFromFirstStep = (string)stepContext.Result;
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"FOOD WATERFALL STEP 2: You can pass objects/strings step-to-step like this: {stringFromFirstStep}"), cancellationToken);
                return await stepContext.PromptAsync("linksName", new PromptOptions { Prompt = MessageFactory.Text("What is your favorite food?") }, cancellationToken);
            }

            private async Task<DialogTurnResult> NameConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
            {
                //WITHOUT SAVING STATE WITH ACCESSOR TO 'THEUSERSTATE'
                // We can send messages to the user at any point in the WaterfallStep.
                //await stepContext.Context.SendActivityAsync(MessageFactory.Text($"FOOD WATERFALL STEP 3: I like {stepContext.Result} as well!"), cancellationToken);
                //END-WITHOUT SAVING STATE WITH ACCESSOR TO 'THEUSERSTATE'


                //WITH SAVING STATE WITH ACCESSOR TO 'THEUSERSTATE'
                var botState = await (stepContext.Context.TurnState["DialogBotConversationStateAndUserStateAccessor"] as DialogBotConversationStateAndUserStateAccessor).TheUserProfile.GetAsync(stepContext.Context);
                botState.Food = stepContext.Result.ToString();
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"FOOD WATERFALL STEP 3: I like {botState.Food} as well!"), cancellationToken);
                //END-WITH SAVING STATE WITH ACCESSOR TO 'THEUSERSTATE'

                return await stepContext.NextAsync(null, cancellationToken);


            }

            private async Task<DialogTurnResult> Hero1StepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
            {
                //WITHOUT SAVING STATE WITH ACCESSOR TO 'THEUSERSTATE'
                // We can send messages to the user at any point in the WaterfallStep.
                //await stepContext.Context.SendActivityAsync(MessageFactory.Text($"FOOD WATERFALL STEP 3: I like {stepContext.Result} as well!"), cancellationToken);
                //END-WITHOUT SAVING STATE WITH ACCESSOR TO 'THEUSERSTATE'


                //WITH SAVING STATE WITH ACCESSOR TO 'THEUSERSTATE'
                var botState = await (stepContext.Context.TurnState["DialogBotConversationStateAndUserStateAccessor"] as DialogBotConversationStateAndUserStateAccessor).TheUserProfile.GetAsync(stepContext.Context);
                //botState.Food = stepContext.Result.ToString(); //null from previous step
                //           await stepContext.Context.SendActivityAsync(MessageFactory.Text($"FOOD WATERFALL STEP 3: I like {botState.Food} as well!"), cancellationToken);
                //END-WITH SAVING STATE WITH ACCESSOR TO 'THEUSERSTATE'

                //return await stepContext.EndDialogAsync(null, cancellationToken);
                //return await stepContext.PromptAsync("confirmHero1", new ConfirmPrompt { Prompt = MessageFactory.Text($"{botState.Food} do you want to see Hero card 1?") }, cancellationToken);

                return await stepContext.PromptAsync("confirmHero1Links",
                    new PromptOptions
                    {
                        Prompt = MessageFactory.Text($"{botState.Food} do you want to see Hero card 1?"),
                        Choices = new[]
                        {
                        new Choice { Value = "Yes" },
                        new Choice { Value = "No" }
                        },
                    }, cancellationToken);
            }

            private async Task<DialogTurnResult> Hero1ConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
            {
                var chosenDialogResponse = (stepContext.Result as FoundChoice)?.Value;

                if (chosenDialogResponse == "Yes")
                {
                    //YES

                    var reply = stepContext.Context.Activity.CreateReply();
                //reply.Attachments = new List<Attachment>();
                //reply.Attachments.Add(GetHeroCard1().ToAttachment());

                reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                //reply.Attachments.Add(CreateAdaptiveCardAttachment());
                //reply.Attachments.Add(GetHeroCard().ToAttachment());
                reply.Attachments.Add(GetHeroCard1().ToAttachment());
                reply.Attachments.Add(GetHeroCard2().ToAttachment());
                reply.Attachments.Add(GetHeroCard3().ToAttachment());
                reply.Attachments.Add(GetHeroCard4().ToAttachment());
                reply.Attachments.Add(GetHeroCard5().ToAttachment());



                // Send the card(s) to the user as an attachment to the activity
                await stepContext.Context.SendActivityAsync(reply, cancellationToken);

                    //return await stepContext.NextAsync(null, cancellationToken);
                    return await stepContext.EndDialogAsync(null, cancellationToken);

                }
                else if (chosenDialogResponse == "No")
                {
                    //NO

                    var botState = await (stepContext.Context.TurnState["DialogBotConversationStateAndUserStateAccessor"] as DialogBotConversationStateAndUserStateAccessor).TheUserProfile.GetAsync(stepContext.Context);

                    await stepContext.Context.SendActivityAsync(MessageFactory.Text($"{botState.Food} you did not want to watch Hero card 1."), cancellationToken);

                    //return await stepContext.NextAsync(null, cancellationToken);
                    return await stepContext.EndDialogAsync(null, cancellationToken);


                }

                else
                {
                    return await stepContext.EndDialogAsync(null, cancellationToken);

                }


            }

        private async Task<DialogTurnResult> ShowHeroCardsStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Below you'll find the most important IT links to keep handy:"), cancellationToken);

                var reply = stepContext.Context.Activity.CreateReply();
                //reply.Attachments = new List<Attachment>();
                //reply.Attachments.Add(GetHeroCard1().ToAttachment());

                reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                //reply.Attachments.Add(CreateAdaptiveCardAttachment());
                //reply.Attachments.Add(GetHeroCard().ToAttachment());
                reply.Attachments.Add(GetHeroCard1().ToAttachment());
                reply.Attachments.Add(GetHeroCard2().ToAttachment());
                reply.Attachments.Add(GetHeroCard3().ToAttachment());
                reply.Attachments.Add(GetHeroCard4().ToAttachment());
                reply.Attachments.Add(GetHeroCard5().ToAttachment());



                // Send the card(s) to the user as an attachment to the activity
                await stepContext.Context.SendActivityAsync(reply, cancellationToken);

                await Task.Delay(5000);

                //return await stepContext.NextAsync(null, cancellationToken);
                return await stepContext.EndDialogAsync(null, cancellationToken);

        }


        //METHODS FOR GRABBING SPECIFIC CARDS
        private static HeroCard GetHeroCard1()
            {
                var heroCard = new HeroCard
                {
                    Title = "Software Download Center",
                    Subtitle = "One-stop shop to download company software",
                    Text = "Employees can come here to download software. Check back frequently for updates and new software.",
                    Images = new List<CardImage> { new CardImage("https://img-prod-cms-rt-microsoft-com.akamaized.net/cms/api/am/imageFileData/RE2FXO5?ver=2a18") },
                    Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Get Started", value: "https://www.microsoft.com/en-us/download") },
                };

                return heroCard;
            }

            private static HeroCard GetHeroCard2()
            {
                var heroCard = new HeroCard
                {
                    Title = "Password Reset Instructions",
                    Subtitle = "Step-by-step guide to reset your Windows password",
                    Text = "Here is our company guide to resetting your Windows machine.",
                    Images = new List<CardImage> { new CardImage("https://c.s-microsoft.com/en-us/CMSImages/Microsoft-logo_rgb_wht.jpg?version=256EB2C3-1E17-2967-746F-3F32234EF429") },
                    Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Get Started", value: "https://support.microsoft.com/en-us/help/14048/windows-7-reset-your-windows-password") },
                };

                return heroCard;
            }

        //METHODS FOR GRABBING SPECIFIC CARDS
        private static HeroCard GetHeroCard3()
        {
            var heroCard = new HeroCard
            {
                Title = "Company IT Policies",
                Subtitle = "A comprehensive look at all our IT policies",
                Text = "From password strength to remote working, here is a list and information about all our IT policies.",
                Images = new List<CardImage> { new CardImage("https://its.ucsc.edu/services/images/get-help.png") },
                Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Get Started", value: "https://its.ucsc.edu/policies/index.html") },
            };

            return heroCard;
        }

        private static HeroCard GetHeroCard4()
        {
            var heroCard = new HeroCard
            {
                Title = "Remote Desktop Connection",
                Subtitle = "Remote desktop instructions for remote workers",
                Text = "From security issues to required permissions, this guide will walk users through creating secure remote desktop connections.",      
                Images = new List<CardImage> { new CardImage("https://c.s-microsoft.com/en-us/CMSImages/Microsoft-logo_rgb_wht.jpg?version=256EB2C3-1E17-2967-746F-3F32234EF429") },
                Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Get Started", value: "https://support.microsoft.com/en-us/help/17463/windows-7-connect-to-another-computer-remote-desktop-connection") },
            };

            return heroCard;
        }

        private static HeroCard GetHeroCard5()
        {
            var heroCard = new HeroCard
            {
                Title = "Microsoft Bot Framework",
                Subtitle = "Building Intelligent Bots with the BotFramework",
                Text =  "Build bots using text/sms to Skype, Slack, Office 365 mail, and other popular services.",
                Images = new List<CardImage> { new CardImage("https://sec.ch9.ms/ch9/7ff5/e07cfef0-aa3b-40bb-9baa-7c9ef8ff7ff5/buildreactionbotframework_960.jpg") },
                Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Get Started", value: "https://visualstudiomagazine.com/Home.aspx") },
            };

            return heroCard;
        }

    }
}
