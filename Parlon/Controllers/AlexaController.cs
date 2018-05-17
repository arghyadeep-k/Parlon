using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Parlon.Controllers
{
    public class AlexaController : ApiController
    {        
        [HttpPost, Route("api/parlon")]
        public dynamic _Parlon(AlexaRequest request)
        {
            AlexaResponse response = null;

            switch (request.Request.Type)
            {
                case "LaunchRequest":
                    response = LaunchRequestHandler(request);
                    break;
                case "IntentRequest":
                    response = IntentRequestHandler(request);
                    break;
                case "SessionEndedRequest":
                    response = SessionEndedRequestHandler(request);
                    break;
            }
            return response;
        }

        private AlexaResponse LaunchRequestHandler(AlexaRequest request)
        {
            var response = new AlexaResponse("Hello, this is Parlon.");
            response.Response.Card.Title = "Parlon";
            response.Response.Card.Content = "Hello, this is Parlon.";
            response.Response.ShouldEndSession = true;
            return response;
        }

        private AlexaResponse IntentRequestHandler(AlexaRequest request)
        {
            AlexaResponse response = null;

            switch (request.Request.Intent.Name.ToString())
            {
                case "Parlon":
                case "parlon":
                    response = Process(request.Request.Intent.Slots);
                    break;
                case "AMAZON.CancelIntent":
                case "AMAZON.StopIntent":
                    response = CancelOrStopIntentHandler(request);
                    break;
                case "AMAZON.HelpIntent":
                    response = HelpIntent(request);
                    break;
            }

            return response;
        }

        private AlexaResponse HelpIntent(AlexaRequest request)
        {
            var response = new AlexaResponse("To use Parlon, you can say, Alexa, ask Parlon what is the steam drum temperature of unit 1?", true);            
            return response;
        }

        private AlexaResponse CancelOrStopIntentHandler(AlexaRequest request)
        {
            return new AlexaResponse("Thanks for using Parlon.", true);
        }

        private AlexaResponse Process(AlexaRequest.RequestAttributes.SlotAttributes request)
        {
            string result = null;
            string unit, parameter;
            if (request.Unit.Value.ToString().Equals(null) || request.Num.Value.ToString().Equals(null) || request.Parameter.Value.ToString().Equals(null))
            {
                result = "You didn't seem to provide all the inputs.";
            }
            else
            {
                unit = request.Unit.Value.ToString() + "_" + request.Num.Value.ToString();
                parameter = request.Parameter.Value.ToString();

                var obj = new ProcessData();
                result = obj.Process(unit, parameter);

                if (result.Equals("") || result.Equals(null))
                    result = "Sorry, Parlon failed to fetch your data right now.";
                else
                    result = "The "+ parameter +" of "+ request.Unit.Value.ToString()+request.Num.Value.ToString() +" is " + result;
            }
            
            var response = new AlexaResponse(result, true);
            return response;
        }

        private AlexaResponse SessionEndedRequestHandler(AlexaRequest request)
        {
            return null;
        }
    }
    
}