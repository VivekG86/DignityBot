// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio CoreBot v4.11.1

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;

using DignityCoreBot;
using AdaptiveCards;
using Newtonsoft.Json.Linq;
using Microsoft.Bot.Builder.Dialogs.Choices;

namespace DignityCoreBot.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        //private readonly FlightBookingRecognizer _luisRecognizer;
        protected readonly ILogger Logger;

        // Dependency injection uses this constructor to instantiate MainDialog
        public MainDialog(UserSignInDialog bookingDialog, ILogger<MainDialog> logger)
            : base(nameof(MainDialog))
        {
            Logger = logger;

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(bookingDialog);
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                IntroStepAsync,
                UserSignInStepAsync
                //FinalStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {            
            var choices = new[] { "View Appointments", "Cancel Appointments" };
            // Create card
            var card = new AdaptiveCard(new AdaptiveSchemaVersion(1, 0))
            {                
                // Use LINQ to turn the choices into submit actions
                Body = { new AdaptiveTextBlock{Text = "What would you like me to help you with?",
                    Size = AdaptiveTextSize.Default } },
                Actions = choices.Select(choice => new AdaptiveSubmitAction
                {
                    Title = choice,
                    Data = choice,  // This will be a string
                }).ToList<AdaptiveAction>(),
            };

            // Prompt
            return await stepContext.PromptAsync(
                nameof(TextPrompt),
                new PromptOptions
                {
                    Prompt = (Activity)MessageFactory.Attachment(new Attachment
                    {
                        ContentType = AdaptiveCard.ContentType,
                        // Convert the AdaptiveCard to a JObject
                        Content = JObject.FromObject(card),
                    }),
                    Choices = ChoiceFactory.ToChoices(choices),
                    // Don't render the choices outside the card
                    Style = ListStyle.None,
                },
                cancellationToken);
        }

        private async Task<DialogTurnResult> UserSignInStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // LUIS is not configured, we just run the BookingDialog path with an empty BookingDetailsInstance.
            return await stepContext.BeginDialogAsync(nameof(UserSignInDialog), new UserDetails(), cancellationToken);
        }


        //private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        //{
        //    // If the child dialog ("BookingDialog") was cancelled, the user failed to confirm or if the intent wasn't BookFlight
        //    // the Result here will be null.
        //    if (stepContext.Result is BookingDetails result)
        //    {
        //        // Now we have all the booking details call the booking service.

        //        // If the call to the booking service was successful tell the user.

        //        var timeProperty = new TimexProperty(result.TravelDate);
        //        var travelDateMsg = timeProperty.ToNaturalLanguage(DateTime.Now);
        //        var messageText = $"I have you reserved to {result.Destination} from {result.Origin} on {travelDateMsg}";
        //        var message = MessageFactory.Text(messageText, messageText, InputHints.IgnoringInput);
        //        await stepContext.Context.SendActivityAsync(message, cancellationToken);
        //    }

        //    // Restart the main dialog with a different message the second time around
        //    var promptMessage = "What else can I do for you?";
        //    return await stepContext.ReplaceDialogAsync(InitialDialogId, promptMessage, cancellationToken);
        //}
    }
}
