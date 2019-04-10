using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimplifiedWaterfallDialogBotV4.BotAccessor
{
    // Stores User Welcome state for the conversation.
    // Stored in Microsoft.Bot.Builder.ConversationState and backed by Microsoft.Bot.Builder.MemoryStorage
    public class WelcomeUserState
    {
        /// A bool property --> gets or sets whether the user has been welcomed in the conversation.
        public bool DidBotWelcomeUser { get; set; } = false;
        public bool DidSeeVideo { get; set; } = false;
    }
}