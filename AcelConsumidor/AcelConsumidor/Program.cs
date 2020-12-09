using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace AcelConsumidor
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceBusClient = 
                new SubscriptionClient("<ServiceBusConnectionString>", "pagamentofeito", "PagamentoFeitoServicoB");

            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };

            serviceBusClient.RegisterMessageHandler(ProcessMessageAsync, messageHandlerOptions);

            while (true)
            {

            }
        }

        private static Task ProcessMessageAsync(Message message, CancellationToken arg2)
        {
            var pagamentoFeito = message.Body.ParseJson<PagamentoFeito>();

            Console.WriteLine(pagamentoFeito.ToString());

            return Task.CompletedTask;
        }

        private static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs arg)
        {
            throw new NotImplementedException();
        }

        
    }

    internal class PagamentoFeito
    {
        public string NumeroCartao { get; set; }
        public decimal Valor { get; set; }

        public override string ToString()
        {
            return $"Numero Cartao{NumeroCartao}, Valor{Valor}";

        }
    }

    public static class Utils
    {
        private static readonly UTF8Encoding Utf8NoBom = new UTF8Encoding(false);

        private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.None,
            Converters = new JsonConverter[] { new StringEnumConverter() }
        };

        /// <summary>
        /// Parses a Utf8 byte json to a specific object.
        /// </summary>
        /// <typeparam name="T">type of object to be parsed.</typeparam>
        /// <param name="json">The json bytes.</param>
        /// <returns>the object parsed from json.</returns>
        public static T ParseJson<T>(this byte[] json)
        {
            if (json == null || json.Length == 0) return default;
            var result = JsonConvert.DeserializeObject<T>(Utf8NoBom.GetString(json), JsonSettings);
            return result;
        }

    }
}
