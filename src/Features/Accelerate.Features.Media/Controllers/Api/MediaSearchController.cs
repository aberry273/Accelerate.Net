using Accelerate.Features.Content.EventBus;
using Accelerate.Features.Media.Services;
using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Common.Controllers;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Accelerate.Foundations.Media.Models.Data;
using Accelerate.Foundations.Websockets.Hubs;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Core.TermVectors;
using Elastic.Clients.Elasticsearch.QueryDsl;
using MailKit.Search;
using MassTransit;
using MassTransit.DependencyInjection;
using MassTransit.Transports;
using MessagePack;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Twilio.Rest.Proxy.V1.Service.Session.Participant;
using static Accelerate.Foundations.Database.Constants.Exceptions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Accelerate.Features.Media.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediaSearchController : ControllerBase
    {
        UserManager<AccountUser> _userManager;
        IMediaViewService _contentService;
        readonly Bind<IMediaBlobEventBus, IPublishEndpoint> _publishEndpoint;
        IElasticService<MediaBlobDocument> _searchBlobService;
        public MediaSearchController(
            IMediaViewService contentService, 
            Bind<IMediaBlobEventBus, IPublishEndpoint> publishEndpoint,
            IElasticService<MediaBlobDocument> searchBlobService,
            UserManager<AccountUser> userManager)
        {
            _publishEndpoint = publishEndpoint;
            _userManager = userManager;
            _contentService = contentService;
            _searchBlobService = searchBlobService;
        }
        [Route("Blobs")]
        [HttpPost]
        public async Task<IActionResult> SearchBlobs([FromBody] RequestQuery query)
        {
            query.Filters = _contentService.GetActualFilterKeys(query.Filters);
            var searchQuery = this.BuildSearchQuery(query);
            var results = await _searchBlobService.Search(searchQuery, query.Page, query.ItemsPerPage);
            var docs = GetDocumentResults(results);
            return Ok(docs);
        }
        private List<MediaBlobDocument> GetDocumentResults(SearchResponse<MediaBlobDocument> results)
        {
            if (!results.IsValidResponse && !results.IsSuccess())
            {
                return new List<MediaBlobDocument>();
            }
            return results.Documents.ToList();
        }
        private QueryDescriptor<MediaBlobDocument> BuildSearchQuery(RequestQuery Query)
        {

            if (Query == null) Query = new RequestQuery();

            //Query.Filters.Add(PublicPosts());
            Query.Filters.Add(_searchBlobService.Filter(Constants.Fields.Status, ElasticCondition.Must, "Public"));
            Query.Filters.Add(_searchBlobService.Filter(Constants.Fields.Status, ElasticCondition.Must, "Public"));

            return _searchBlobService.CreateQuery(Query);
        }
        [Route("Index")]
        [HttpDelete]
        public async Task<IActionResult> DeleteIndex()
        {
            var docs = await _searchBlobService.DeleteIndex();
            return Ok(docs);
        }
    }
}