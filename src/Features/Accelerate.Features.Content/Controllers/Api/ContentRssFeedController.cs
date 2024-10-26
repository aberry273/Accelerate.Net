using Accelerate.Foundations.Content.EventBus;
using Accelerate.Features.Content.Services;
using Accelerate.Foundations.Users.Models.Entities;
using Accelerate.Foundations.Common.Controllers;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Content.Services;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.EventPipelines.Models.Contracts;
using Accelerate.Foundations.Integrations.Contracts;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Accelerate.Foundations.Websockets.Hubs;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Core.TermVectors;
using Elastic.Clients.Elasticsearch.QueryDsl;
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
using Accelerate.Foundations.Users.Models;
using Accelerate.Foundations.Users.Services;
using Microsoft.Extensions.Primitives;

namespace Accelerate.Features.Content.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContentRssFeedController : ControllerBase
    {
        IRssReaderService _rssReaderService;
        public ContentRssFeedController(
            IRssReaderService rssReaderService)
        {
            _rssReaderService = rssReaderService;
        }
        [Route("Read")]
        [HttpPost]
        public async Task<IActionResult> Read([FromBody] string url)
        {
           // var url = "https://khalidabuhakmeh.com/feed.xml";
            var docs =  _rssReaderService.Read(url);
            return Ok(docs);
        }
    }
}
/*
 * 
    {
      "attributeExtensions": {},
      "authors": [],
      "baseUri": null,
      "categories": [
        {
          "attributeExtensions": {},
          "elementExtensions": [],
          "label": null,
          "name": "Communism (Theory and Philosophy)",
          "scheme": "http://www.nytimes.com/namespaces/keywords/des"
        },
        {
          "attributeExtensions": {},
          "elementExtensions": [],
          "label": null,
          "name": "France",
          "scheme": "http://www.nytimes.com/namespaces/keywords/nyt_geo"
        },
        {
          "attributeExtensions": {},
          "elementExtensions": [],
          "label": null,
          "name": "Festivals",
          "scheme": "http://www.nytimes.com/namespaces/keywords/des"
        },
        {
          "attributeExtensions": {},
          "elementExtensions": [],
          "label": null,
          "name": "French Communist Party",
          "scheme": "http://www.nytimes.com/namespaces/keywords/nyt_org"
        },
        {
          "attributeExtensions": {},
          "elementExtensions": [],
          "label": null,
          "name": "Music",
          "scheme": "http://www.nytimes.com/namespaces/keywords/des"
        },
        {
          "attributeExtensions": {},
          "elementExtensions": [],
          "label": null,
          "name": "Davis, Angela Y",
          "scheme": "http://www.nytimes.com/namespaces/keywords/nyt_per"
        },
        {
          "attributeExtensions": {},
          "elementExtensions": [],
          "label": null,
          "name": "Macron, Emmanuel (1977- )",
          "scheme": "http://www.nytimes.com/namespaces/keywords/nyt_per"
        }
      ],
      "content": null,
      "contributors": [],
      "copyright": null,
      "elementExtensions": [
        {
          "outerName": "creator",
          "outerNamespace": "http://purl.org/dc/elements/1.1/"
        },
        {
          "outerName": "content",
          "outerNamespace": "http://search.yahoo.com/mrss/"
        },
        {
          "outerName": "credit",
          "outerNamespace": "http://search.yahoo.com/mrss/"
        },
        {
          "outerName": "description",
          "outerNamespace": "http://search.yahoo.com/mrss/"
        }
      ],
      "id": "https://www.nytimes.com/2024/09/22/world/europe/france-humanity-festival-communism.html",
      "lastUpdatedTime": "0001-01-01T00:00:00+00:00",
      "links": [
        {
          "attributeExtensions": {},
          "baseUri": null,
          "elementExtensions": [],
          "length": 0,
          "mediaType": null,
          "relationshipType": "alternate",
          "title": null,
          "uri": "https://www.nytimes.com/2024/09/22/world/europe/france-humanity-festival-communism.html"
        },
        {
          "attributeExtensions": {},
          "baseUri": null,
          "elementExtensions": [],
          "length": 0,
          "mediaType": null,
          "relationshipType": "standout",
          "title": null,
          "uri": "https://www.nytimes.com/2024/09/22/world/europe/france-humanity-festival-communism.html"
        }
      ],
      "publishDate": "2024-09-22T04:01:16+00:00",
      "sourceFeed": null,
      "summary": {
        "text": "The Fête de l’Humanité, a blend of Burning Man, Woodstock and a political convention, attracts the masses with bands, lectures and food, but here K.F.C. is C.F.K.: Communist Fried Kitchen.",
        "type": "text",
        "attributeExtensions": {}
      },
      "title": {
        "text": "A French Fair as Workers’ Paradise, Feting Cuisine, Music and Communism",
        "type": "text",
        "attributeExtensions": {}
      }
    },
*/