// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio CoreBot v4.11.1

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DignityCoreBot.Dialogs
{
    public class UserSignInDialog : CancelAndHelpDialog
    {
        private const string UserSignInMsgText = "Could you please sign-in into your dignity health account?";
        

        public UserSignInDialog()
            : base(nameof(UserSignInDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new DateResolverDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                UserSignInStepAsync,
                //DestinationStepAsync,
                //OriginStepAsync,
                //TravelDateStepAsync,
                //ConfirmStepAsync,
                //FinalStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> UserSignInStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var choices = new[] { "Sign-in" };

            var card = new AdaptiveCard(new AdaptiveSchemaVersion(1, 0))
            {
                Body = { new AdaptiveTextBlock{Text = UserSignInMsgText,
                    Size = AdaptiveTextSize.Default } },
                Actions = choices.Select(choice => new AdaptiveSubmitAction
                {
                    Title = choice,
                    Data = choice,  // This will be a string
                }).ToList<AdaptiveAction>()


            };

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

            //return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);


        }

        //private async Task<DialogTurnResult> DestinationStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        //{
        //    var bookingDetails = (BookingDetails)stepContext.Options;

        //    if (bookingDetails.Destination == null)
        //    {
        //        var promptMessage = MessageFactory.Text(DestinationStepMsgText, DestinationStepMsgText, InputHints.ExpectingInput);
        //        return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        //    }

        //    return await stepContext.NextAsync(bookingDetails.Destination, cancellationToken);
        //}

        //private async Task<DialogTurnResult> OriginStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        //{
        //    var bookingDetails = (BookingDetails)stepContext.Options;

        //    bookingDetails.Destination = (string)stepContext.Result;

        //    if (bookingDetails.Origin == null)
        //    {
        //        var promptMessage = MessageFactory.Text(OriginStepMsgText, OriginStepMsgText, InputHints.ExpectingInput);
        //        return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        //    }

        //    return await stepContext.NextAsync(bookingDetails.Origin, cancellationToken);
        //}

        //private async Task<DialogTurnResult> TravelDateStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        //{
        //    var bookingDetails = (BookingDetails)stepContext.Options;

        //    bookingDetails.Origin = (string)stepContext.Result;

        //    if (bookingDetails.TravelDate == null || IsAmbiguous(bookingDetails.TravelDate))
        //    {
        //        return await stepContext.BeginDialogAsync(nameof(DateResolverDialog), bookingDetails.TravelDate, cancellationToken);
        //    }

        //    return await stepContext.NextAsync(bookingDetails.TravelDate, cancellationToken);
        //}

        //private async Task<DialogTurnResult> ConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        //{
        //    var bookingDetails = (BookingDetails)stepContext.Options;

        //    bookingDetails.TravelDate = (string)stepContext.Result;

        //    var messageText = $"Please confirm us, I have you traveling to: {bookingDetails.Destination} from: {bookingDetails.Origin} on: {bookingDetails.TravelDate}. Is this correct?";
        //    var promptMessage = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput);

        //    return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        //}

        //private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        //{
        //    if ((bool)stepContext.Result)
        //    {
        //        var bookingDetails = (BookingDetails)stepContext.Options;

        //        return await stepContext.EndDialogAsync(bookingDetails, cancellationToken);
        //    }

        //    return await stepContext.EndDialogAsync(null, cancellationToken);
        //}

        private static bool IsAmbiguous(string timex)
        {
            var timexProperty = new TimexProperty(timex);
            return !timexProperty.Types.Contains(Constants.TimexTypes.Definite);
        }

        public Attachment CreateAdaptiveCardAttachment(string cardName)
        {
            var cardResourcePath = GetType().Assembly.GetManifestResourceNames().First(name => name.EndsWith(cardName));

            using (var stream = GetType().Assembly.GetManifestResourceStream(cardResourcePath))
            {
                using (var reader = new StreamReader(stream))
                {
                    var adaptiveCard = reader.ReadToEnd();
                    return new Attachment()
                    {
                        ContentType = "application/vnd.microsoft.card.adaptive",
                        Content = JsonConvert.DeserializeObject(adaptiveCard),
                    };
                }
            }
        }
    }
}
