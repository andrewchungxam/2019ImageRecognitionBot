using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimplifiedWaterfallDialogBotV4.BotAccessor
{
    /// In Startup.cs, this class is created as a Singleton and passed into the Bot's constructor.
    public class DialogBotConversationStateAndUserStateAccessor
    {
        // Contains the ConversationState and the IStatePropertyAccessor<DialogState> ConversationDialogState
        public DialogBotConversationStateAndUserStateAccessor(ConversationState conversationState, UserState userState)
        {
            ConversationState = conversationState ?? throw new ArgumentNullException(nameof(conversationState));
            UserState = userState ?? throw new ArgumentNullException(nameof(userState));
        }

        /// <summary>
        /// Gets the <see cref="IStatePropertyAccessor{T}"/> name used for the <see cref="BotBuilderSamples.WelcomeUserState"/> accessor.
        /// </summary>
        /// <remarks>Accessors require a unique name.</remarks>
        /// <value>The accessor name for the WelcomeUser state.</value>
        public static string WelcomeUserName { get; } = $"{nameof(DialogBotConversationStateAndUserStateAccessor)}.WelcomeUserState";
        // ConversationDialogState is a type of DialogState. Under the covers, Dialog State is a serialized dialog stack.
        public IStatePropertyAccessor<DialogState> ConversationDialogState { get; set; }
        /// Gets or sets the IStatePropertyAccessor{T} for UserProfile
        public IStatePropertyAccessor<UserProfile> TheUserProfile { get; set; }
        public IStatePropertyAccessor<WelcomeUserState> WelcomeUserState { get; set; }
        public IStatePropertyAccessor<string> LanguagePreference { get; set; }

        // Gets the "ConversationState" for the conversation.
        public ConversationState ConversationState { get; }
        public UserState UserState { get; }
    }
}

//THIS IS THE DEFINITION OF THE DIALOG STATE --> NOTICE HOW IT HAS A LIST OF A DIALOGINSTANCE
//public class DialogState
//{
//    public DialogState();
//    public DialogState(List<DialogInstance> stack);

//    public List<DialogInstance> DialogStack { get; }
//}