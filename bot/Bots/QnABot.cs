// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.5.0

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace Microsoft.BotBuilderSamples.Bots
{
    public class QnABot<T> : ActivityHandler where T : Microsoft.Bot.Builder.Dialogs.Dialog
    {
        protected readonly BotState ConversationState;
        protected readonly Microsoft.Bot.Builder.Dialogs.Dialog Dialog;
        protected readonly BotState UserState;

        public QnABot(ConversationState conversationState, UserState userState, T dialog)
        {
            ConversationState = conversationState;
            UserState = userState;
            Dialog = dialog;
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            await base.OnTurnAsync(turnContext, cancellationToken);

            // Save any state changes that might have occurred during the turn.
            await ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);
            await UserState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            if (turnContext.Activity.Text.ToLower().Contains("versione"))
            {
                if (System.Reflection.Assembly.GetExecutingAssembly() != null)
                {
                    System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
                    if (FileVersionInfo.GetVersionInfo(asm.Location) != null)
                    {
                        FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);

                        System.IO.FileInfo fileInfo = new System.IO.FileInfo(asm.Location);
                        DateTime lastModified = fileInfo.LastWriteTime;

                        string bldLevel = "Versione " + string.Format("{0}.{1}.{2}", fvi.ProductMajorPart, fvi.ProductMinorPart, fvi.ProductBuildPart);
                        bldLevel += ", data " + lastModified.Year.ToString() + "/" + lastModified.Month.ToString() + "/" + lastModified.Day.ToString();
                        await turnContext.SendActivityAsync(MessageFactory.Text(bldLevel), cancellationToken);
                    }
                    else
                        await turnContext.SendActivityAsync(MessageFactory.Text($"Perdonami, ora non riesco a risponderti."), cancellationToken);
                }
                else
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Perdonami, ora non riesco a risponderti."), cancellationToken);
            }
            else
                await Dialog.RunAsync(turnContext, ConversationState.CreateProperty<DialogState>(nameof(DialogState)), cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    var userName = turnContext.Activity.From.Name;
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Benvenuto."), cancellationToken); // (MessageFactory.Text($"Benvenuto, {userName }"), cancellationToken);
                }
            }
        }
    }
}
