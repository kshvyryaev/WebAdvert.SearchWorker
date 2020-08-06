using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.SNSEvents;
using Nest;
using Newtonsoft.Json;
using WebAdvert.Models.Messages;

[assembly:LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
namespace WebAdvert.SearchWorker
{
    public class SearchWorker
    {
        private readonly IElasticClient _client;

        public SearchWorker() : this(ElasticSearchHelper.GetInstance(ConfigurationHelper.GetInstance()))
        {
        }

        public SearchWorker(IElasticClient client)
        {
            _client = client;
        }

        public async Task Run(SNSEvent snsEvent, ILambdaContext context)
        {
            foreach (var record in snsEvent.Records)
            {
                context.Logger.LogLine(record.Sns.Message);

                var message = JsonConvert.DeserializeObject<AdvertConfirmedMessage>(record.Sns.Message);
                var advertType = MappingHelper.Map(message);

                await _client.IndexDocumentAsync(advertType);
            }
        }
    }
}
