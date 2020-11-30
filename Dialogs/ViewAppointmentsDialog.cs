using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DignityCoreBot.Dialogs
{
    public class ViewAppointmentsDialog : CancelAndHelpDialog
    {
        public ViewAppointmentsDialog()
            : base(nameof(ViewAppointmentsDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new DateResolverDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                ViewAppointmentsStepAsync
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> ViewAppointmentsStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            var signInMessage = MessageFactory.Text("Would you like to sign-in into your dignity health account?", null, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = signInMessage }, cancellationToken);

        }
    }
}
