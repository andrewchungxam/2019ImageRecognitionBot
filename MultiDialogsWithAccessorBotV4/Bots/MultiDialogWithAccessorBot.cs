// Licensed under the MIT License.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using MultiDialogsWithAccessorBotV4.BotAccessor;
using SimplifiedWaterfallDialogBotV4;
using SimplifiedWaterfallDialogBotV4.BotAccessor;

namespace Bot_Builder_Simplified_Echo_Bot_V4
{
    public class MultiDialogWithAccessorBot : IBot
    {
        private readonly DialogSet _dialogSet;
        private readonly DialogBotConversationStateAndUserStateAccessor _dialogBotConversationStateAndUserStateAccessor;

        public DialogBotConversationStateAndUserStateAccessor DialogBotConversationStateAndUserStateAccessor { get; set; }
        
   
        //public static readonly string QnAMakerKey = "RoyaltyInfo2018";
        public static readonly string QnAMakerKey = "GitHubEnterpriseFAQ";

        private readonly BotServices _services;

        public MultiDialogWithAccessorBot(DialogBotConversationStateAndUserStateAccessor accessor, BotServices services)
        {
            _dialogBotConversationStateAndUserStateAccessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
            _dialogSet = new DialogSet(_dialogBotConversationStateAndUserStateAccessor.ConversationDialogState);
            _dialogSet.Add(RootWaterfallDialog.BotInstance);

            _dialogSet.Add(new TextPrompt("name"));
            _dialogSet.Add(new TextPrompt("colorName"));
            _dialogSet.Add(new TextPrompt("linksName"));
            _dialogSet.Add(new TextPrompt("foodName"));
            _dialogSet.Add(new TextPrompt("foodITEmail"));

            _dialogSet.Add(new AttachmentPrompt("promptITBarcode"));
            _dialogSet.Add(new TextPrompt("passResetBirthDate"));
            _dialogSet.Add(new TextPrompt("passResetPhoneNumber"));
            _dialogSet.Add(new TextPrompt("passResetOTPDevice"));

            _dialogSet.Add(new TextPrompt("thirdWaterName"));

            _dialogSet.Add(FoodWaterfallDialog.BotInstance);
            _dialogSet.Add(ColorWaterfallDialog.BotInstance);
            _dialogSet.Add(LinksWaterfallDialog.BotInstance);
            _dialogSet.Add(NameWaterfallDialog.BotInstance);

            _dialogSet.Add(new ChoicePrompt("dialogChoice"));
            _dialogSet.Add(ThirdWaterfallDialog.BotInstance);

            _dialogSet.Add(new ChoicePrompt("confirmHero1"));
            _dialogSet.Add(new ChoicePrompt("confirmHero1Links"));
            
            DialogBotConversationStateAndUserStateAccessor = accessor;

            _services = services ?? throw new System.ArgumentNullException(nameof(services));
            if (!_services.QnAServices.ContainsKey(QnAMakerKey))
            {
                throw new System.ArgumentException($"Invalid configuration. Please check your '.bot' file for a QnA service named '{QnAMakerKey}'.");
            }

        }

        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            var botState = await DialogBotConversationStateAndUserStateAccessor.TheUserProfile.GetAsync(turnContext, () => new UserProfile(), cancellationToken);

            //qna
            var myWelcomeUserState = await DialogBotConversationStateAndUserStateAccessor.WelcomeUserState.GetAsync(turnContext, () => new WelcomeUserState(), cancellationToken);
            //qna

            turnContext.TurnState.Add("DialogBotConversationStateAndUserStateAccessor", DialogBotConversationStateAndUserStateAccessor);

            if (turnContext == null)
            {
                throw new ArgumentNullException(nameof(turnContext));
            }

            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                //Welcome to the Company IT Bot.  
                if (myWelcomeUserState.DidBotWelcomeUser == false)
                {
                    myWelcomeUserState.DidBotWelcomeUser = true;
                    // Update user state flag to reflect bot handled first user interaction.
                    await _dialogBotConversationStateAndUserStateAccessor.WelcomeUserState.SetAsync(turnContext, myWelcomeUserState);
                    await _dialogBotConversationStateAndUserStateAccessor.UserState.SaveChangesAsync(turnContext);

                    await turnContext.SendActivityAsync($"Welcome to the Company IT Bot - the best, first stop for your IT needs.", cancellationToken: cancellationToken);

                }

                // Run the DialogSet - let the framework identify the current state of the dialog from
                // the dialog stack and figure out what (if any) is the active dialog.
                var dialogContext = await _dialogSet.CreateContextAsync(turnContext, cancellationToken);

                if (dialogContext != null)
                {
                    if (dialogContext.ActiveDialog != null)
                    {
                        if (dialogContext.ActiveDialog.Id == "thirdWaterName")
                        {
                            if (turnContext.Activity.Type == ActivityTypes.Message && turnContext.Activity.Text == "Back")
                            {
                                await dialogContext.CancelAllDialogsAsync();
                            }
                            else
                            {
                                var response = await _services.QnAServices[QnAMakerKey].GetAnswersAsync(turnContext);
                                if (response != null && response.Length > 0)
                                {
                                    await turnContext.SendActivityAsync(response[0].Answer, cancellationToken: cancellationToken);
                                }
                            }
                        }
                    }
                }

                
                //POP OFF ANY DIALOG IF THE "FLAG IS SWITCHED" 
                string didTypeNamestring = "";
                if (turnContext.TurnState.ContainsKey("didTypeName"))
                {
                    didTypeNamestring = turnContext.TurnState["didTypeName"] as string;
                }

                if (didTypeNamestring == "name")
                {

                    //OPTION 1:
                    await dialogContext.CancelAllDialogsAsync();

                    //OPTION 2: //TRY BELOW OPTIONS - WHY DOES IT MISBEHAVE?
                    //NOTE-CALLING THIS HITS THE CONTINUE IN THE BELOW IF STATEMENT
                    //await dialogContext.ReplaceDialogAsync(NameWaterfallDialog.DialogId, null, cancellationToken);

                    //OPTION 3:
                    //DOES NOT WORK WELL HERE - WHEN HAVE YOU SEEN IT WORK CORRECTLY IN PREVIOUS PROJECTS?
                    //await dialogContext.EndDialogAsync();
                }
                
                if (dialogContext.ActiveDialog == null)
                {
                    if (turnContext.TurnState.ContainsKey("didTypeName"))
                    {
                        didTypeNamestring = turnContext.TurnState["didTypeName"] as string;
                        if (didTypeNamestring == "name")
                        {
                            await dialogContext.BeginDialogAsync(NameWaterfallDialog.DialogId, null, cancellationToken);
                        }
                    }
                    else
                    {
                        await dialogContext.BeginDialogAsync(RootWaterfallDialog.DialogId, null, cancellationToken);
                    }
                }
                else
                {
                    await dialogContext.ContinueDialogAsync(cancellationToken);
                }

                await _dialogBotConversationStateAndUserStateAccessor.ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);
                await _dialogBotConversationStateAndUserStateAccessor.UserState.SaveChangesAsync(turnContext, false, cancellationToken);
            }
        }
    }
}