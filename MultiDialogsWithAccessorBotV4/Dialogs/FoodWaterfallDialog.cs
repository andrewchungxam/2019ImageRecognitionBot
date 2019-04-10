using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;

using SimplifiedWaterfallDialogBotV4.BotAccessor;

namespace Bot_Builder_Simplified_Echo_Bot_V4
{
    public class FoodWaterfallDialog : WaterfallDialog
    {
        public static string DialogId { get; } = "foodDialog";
        public static FoodWaterfallDialog BotInstance { get; } = new FoodWaterfallDialog(DialogId, null);
        public FoodWaterfallDialog(string dialogId, IEnumerable<WaterfallStep> steps)
            : base(dialogId, steps)
        {
            AddStep(FirstStepAsync);
            AddStep(NameStepAsync);
            AddStep(NameConfirmStepAsync);
            AddStep(ITEmailConfirmStepAsync);
            AddStep(ITBarcodeConfirmStepAsync);
            AddStep(Hero1StepAsync);
            AddStep(Hero1ConfirmStepAsync);
        }

        private static async Task<DialogTurnResult> FirstStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
            // Running a prompt here means the next WaterfallStep will be run when the users response is received.
            //await stepContext.Context.SendActivityAsync(MessageFactory.Text($"FOOD WATERFALL STEP 1: This is the first step.  You can put your code in each of these steps."), cancellationToken);
            return await stepContext.NextAsync("Data from First Step", cancellationToken);
        }

        private static async Task<DialogTurnResult> NameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string stringFromFirstStep = (string)stepContext.Result;
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Welcome to new user registration."), cancellationToken);

            //await stepContext.Context.SendActivityAsync(MessageFactory.Text($"FOOD WATERFALL STEP 2: You can pass objects/strings step-to-step like this: {stringFromFirstStep}"), cancellationToken);
            return await stepContext.PromptAsync("foodName", new PromptOptions { Prompt = MessageFactory.Text("What is your name?") }, cancellationToken);
        }

        private async Task<DialogTurnResult> NameConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //WITHOUT SAVING STATE WITH ACCESSOR TO 'THEUSERSTATE'
            // We can send messages to the user at any point in the WaterfallStep.
            //await stepContext.Context.SendActivityAsync(MessageFactory.Text($"FOOD WATERFALL STEP 3: I like {stepContext.Result} as well!"), cancellationToken);
            //END-WITHOUT SAVING STATE WITH ACCESSOR TO 'THEUSERSTATE'


            //WITH SAVING STATE WITH ACCESSOR TO 'THEUSERSTATE'
            var botState = await (stepContext.Context.TurnState["DialogBotConversationStateAndUserStateAccessor"] as DialogBotConversationStateAndUserStateAccessor).TheUserProfile.GetAsync(stepContext.Context);
            botState.ITName = stepContext.Result.ToString();
            return await stepContext.PromptAsync("foodITEmail", new PromptOptions { Prompt = MessageFactory.Text("What is your email address?") }, cancellationToken);            
            //END-WITH SAVING STATE WITH ACCESSOR TO 'THEUSERSTATE'
            //return await stepContext.NextAsync(null, cancellationToken);    
        }


        private async Task<DialogTurnResult> ITEmailConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            //WITH SAVING STATE WITH ACCESSOR TO 'THEUSERSTATE'
            var botState = await (stepContext.Context.TurnState["DialogBotConversationStateAndUserStateAccessor"] as DialogBotConversationStateAndUserStateAccessor).TheUserProfile.GetAsync(stepContext.Context);
            botState.ITEmail = stepContext.Result.ToString();

            return await stepContext.PromptAsync("promptITBarcode", new PromptOptions { Prompt = MessageFactory.Text("Upload an image of your computer's barcode tag so we can scan it.") }, cancellationToken);
        }

        private async Task<DialogTurnResult> ITBarcodeConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            //WITH SAVING STATE WITH ACCESSOR TO 'THEUSERSTATE'
            var botState = await (stepContext.Context.TurnState["DialogBotConversationStateAndUserStateAccessor"] as DialogBotConversationStateAndUserStateAccessor).TheUserProfile.GetAsync(stepContext.Context);
            //botState.ITEmail = stepContext.Result.ToString();
            var resultImageUpload = stepContext.Result;

            var theActivity = stepContext.Context.Activity;
            var theReply = theActivity.CreateReply();

            List<string> returnedMessage = HandleIncomingAttachmentAsync(stepContext.Context.Activity, theReply).GetAwaiter().GetResult();

            var newStringBuilder = new StringBuilder();

            int count = 0;
            foreach (var individualReturnedMessage in returnedMessage)
            {
                ++count;
                newStringBuilder.Append($"{individualReturnedMessage}");
            }

            botState.ITBarcode = newStringBuilder.ToString();
            
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Thank you for registering {botState.ITName}. "), cancellationToken);
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Our records indicate your closest IT helpdesk is in Building 1 (First floor, Southeast corner).  It is staffed from 9am-5pm Monday to Friday by Jim Morrow and Tim Bow."), cancellationToken);
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"You'll receive confirmation of your registration to your email address: {botState.ITEmail}."), cancellationToken);
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Your computer is registered as {botState.ITBarcode}.  We'll have that on file for your future inquiries."), cancellationToken);

            return await stepContext.EndDialogAsync(null, cancellationToken);
        }


        private async Task<DialogTurnResult> Hero1StepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //WITHOUT SAVING STATE WITH ACCESSOR TO 'THEUSERSTATE'
            // We can send messages to the user at any point in the WaterfallStep.
            //await stepContext.Context.SendActivityAsync(MessageFactory.Text($"FOOD WATERFALL STEP 3: I like {stepContext.Result} as well!"), cancellationToken);
            //END-WITHOUT SAVING STATE WITH ACCESSOR TO 'THEUSERSTATE'


            //WITH SAVING STATE WITH ACCESSOR TO 'THEUSERSTATE'
            var botState = await (stepContext.Context.TurnState["DialogBotConversationStateAndUserStateAccessor"] as DialogBotConversationStateAndUserStateAccessor).TheUserProfile.GetAsync(stepContext.Context);
            //END-WITH SAVING STATE WITH ACCESSOR TO 'THEUSERSTATE'

            return await stepContext.PromptAsync("confirmHero1",             
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
                reply.Attachments = new List<Attachment>();
                reply.Attachments.Add(GetHeroCard1().ToAttachment());
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
                return await stepContext.EndDialogAsync(null, cancellationToken);
            }

            else
            {
                return await stepContext.EndDialogAsync(null, cancellationToken);
            }
        }

        //METHODS FOR GRABBING SPECIFIC CARDS
        private static HeroCard GetHeroCard1()
        {
            var heroCard = new HeroCard
            {
                Title = "BotFramework Hero Card",
                Subtitle = "Microsoft Bot Framework",
                Text = "Build and connect intelligent bots to interact with your users naturally wherever they are," +
                       " from text/sms to Skype, Slack, Office 365 mail and other popular services.",
                Images = new List<CardImage> { new CardImage("https://sec.ch9.ms/ch9/7ff5/e07cfef0-aa3b-40bb-9baa-7c9ef8ff7ff5/buildreactionbotframework_960.jpg") },
                Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Get Started", value: "https://docs.microsoft.com/bot-framework") },
            };

            return heroCard;
        }

        private static async Task<List<string>> HandleIncomingAttachmentAsync(IMessageActivity activity, IMessageActivity reply)
        {
            List<string> listOfString = new List<string>();

            foreach (var file in activity.Attachments)
            {
                // Determine where the file is hosted.
                var remoteFileUrl = file.ContentUrl;

                // Save the attachment to the system temp directory.
                var localFileName = Path.Combine(Path.GetTempPath(), file.Name);

                // Download the actual attachment
                using (var webClient = new WebClient())
                {
                    webClient.DownloadFile(remoteFileUrl, localFileName);

                }

                var serviceUrl = reply.ServiceUrl;
                var connector = new ConnectorClient(new Uri(serviceUrl));
                var imageAttachment = file; //this is the localfile
                byte[] sampleByteArray = await GetImageByteStreamDirectly(connector, imageAttachment);

                var bcss = new BarcodeScannerService();
                var returnedString = bcss.DecodeBarcode(sampleByteArray);

                listOfString.Add(returnedString);
            }

            return listOfString;
        }

        private static async Task<byte[]> GetImageByteStreamDirectly(ConnectorClient connector, Attachment imageAttachment)
        {
            using (var httpClient = new HttpClient())
            {
                // The Skype attachment URLs are secured by JwtToken,
                // you should set the JwtToken of your bot as the authorization header for the GET request your bot initiates to fetch the image.
                // https://github.com/Microsoft/BotBuilder/issues/662
                var uri = new Uri(imageAttachment.ContentUrl);
                //if (uri.Host.EndsWith("skype.com") && uri.Scheme == "https")
                //{
                //httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetTokenAsync(connector));
                //httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/octet-stream"));
                //}

                return await httpClient.GetByteArrayAsync(uri);
            }
        }
    }
}
