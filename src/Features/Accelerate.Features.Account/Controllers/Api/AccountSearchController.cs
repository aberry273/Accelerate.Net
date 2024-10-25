﻿using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Common.Controllers;
using Accelerate.Foundations.Common.Models;
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
using Accelerate.Foundations.Account.Models;
using Accelerate.Foundations.Account.Services;

namespace Accelerate.Features.Content.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountSearchController : ControllerBase
    {
        UserManager<AccountUser> _userManager;
        IAccountUserSearchService _searchService;
        public AccountSearchController(
            IAccountUserSearchService searchService,
            UserManager<AccountUser> userManager)
        {
            _searchService = searchService;
        }
        [Route("Users")]
        [HttpPost]
        public async Task<IActionResult> SearchUsers([FromBody] RequestQuery query)
        {
            var result = await _searchService.SearchUsers(query);
            return Ok(result);
        }
    }
}