
using Accelerate.Foundations.Users.Models;
using Accelerate.Foundations.Users.Models.Data;
using Accelerate.Foundations.Users.Models.Entities;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.IndexManagement;
using Elastic.Clients.Elasticsearch.Mapping;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Elastic.Transport;
using Microsoft.Extensions.Options;
using System.Threading;
using Accelerate.Foundations.Users.Models;

namespace Accelerate.Foundations.Users.Services
{
    //Overwrite the core service for custom filtering
    public class UsersUserSearchService : ElasticService<UsersUserDocument>, IUsersUserSearchService
    {
        public UsersUserSearchService(IOptions<ElasticConfiguration> options, IOptions<UsersConfiguration> config) : base(options)
        {
            this._indexName = config.Value.UserIndexName;
            //this._mapping = CreateContentPostMapping();
        }
        private TypeMapping CreateContentPostMapping()
        {
            var properties = new Properties();
            var propertyName = new PropertyName(Constants.Fields.Username);
            var KeywordName = new PropertyName(Constants.Fields.Keyword);
            var keywordProperty = new KeywordProperty();
            var subproperties = new Properties();
            subproperties.Add(KeywordName, keywordProperty);
            
            var textProperty = new TextProperty()
            {
                Fields = subproperties
            };
            properties.Add(propertyName, textProperty);
            
            /*
            properties.Add(p => p
                .Text(t => t.CompanyName)
                .Object(o => o.EmployeeInfo, objConfig => objConfig
                    .Properties(p => p
                        .Text(t => t.EmployeeInfo.EmployeeName)
                        .Object(n => n.EmployeeInfo.JobRoles, objConfig => objConfig
                            .Properties(p => p
                                .Text(t => t.EmployeeInfo.JobRoles.First().RoleName)
                            )
                        )
                    )
                )
            );
            */

            var mapping = new TypeMapping()
            {
                Properties = properties
            };
            return mapping;
        }
        public override async Task<SearchResponse<UsersUserDocument>> Find(RequestQuery<UsersUserDocument> query)
        {
            //Create if not existing
            await CreateIndex();
            //Search
            int take = query.ItemsPerPage > 0 ? query.ItemsPerPage : 10;
            int skip = take * query.Page;

            return await Search(
                CreateQuery(query),
                skip,
                take);
        }

        public async Task<UsersSearchResults> SearchUsers(RequestQuery Query, List<string> userIds)
        {
            var elasticQuery = BuildIdSearchQuery(userIds);
            return await SearchUsers(Query, elasticQuery);
        }
        public async Task<UsersSearchResults> SearchUsers(RequestQuery Query)
        {
            var elasticQuery = BuildSearchUserQuery(Query);
            return await SearchUsers(Query, elasticQuery);
        }

        public override Task<SearchResponse<UsersUserDocument>> GetAggregates(RequestQuery<UsersUserDocument> query, string sortByField = Foundations.Integrations.Elastic.Constants.Fields.CreatedOn, SortOrder sortOrder = SortOrder.Asc)
        {
            throw new NotImplementedException();
        }

        private QueryDescriptor<UsersUserDocument> CreateQuery(RequestQuery<UsersUserDocument> request)
        {
            var descriptor =  new QueryDescriptor<UsersUserDocument>();
            descriptor.MatchAll();
            if (!string.IsNullOrEmpty(request?.Query?.Username))
                descriptor.Term(x => x.Username, request?.Query?.Username);
            return descriptor;
        }
        public async Task<UsersSearchResults> SearchUsers(RequestQuery Query, QueryDescriptor<UsersUserDocument> elasticQuery)
        {
            //TODO: remove
            Query.ItemsPerPage = 100;

            int take = Query.ItemsPerPage > 0 ? Query.ItemsPerPage : this.ItemsPerPage;
            if (take > MaxQueryable) take = MaxQueryable;
            int skip = take * Query.Page;

            var model = new UsersSearchResults();
            var results = await Search(elasticQuery, skip, take, Foundations.Integrations.Elastic.Constants.Fields.CreatedOn, Elastic.Clients.Elasticsearch.SortOrder.Desc);
            if (!results.IsValidResponse && !results.IsSuccess() || results.Documents == null)
            {
                return model;
            }
            model.Users = results.Documents.ToList();
            //roles
            //
            return model;
        }

        public QueryDescriptor<UsersUserDocument> BuildSearchUserQuery(RequestQuery Query)
        {

            if (Query == null) Query = new RequestQuery();

            //Query.Filters.Add(PublicPosts());
            //Query.Filters.Add(Filter(Constants.Fields.Status, ElasticCondition.Must, "Public"));
            Query.Filters.Add(Filter(Constants.Fields.Domain, ElasticCondition.Filter, "Public"));

            //Filter any post where the poster is replying to themselves from the results
            var filter = Filter(Constants.Fields.Username, ElasticCondition.Filter, QueryOperator.Contains, Query.Text, false);
            Query.Filters.Add(filter);

            return CreateQuery(Query);
        }
        public QueryDescriptor<UsersUserDocument> BuildSearchUserQuery(List<Guid> userIds)
        {

            var Query = new RequestQuery();

            //Query.Filters.Add(PublicPosts());
            //Query.Filters.Add(Filter(Constants.Fields.Status, ElasticCondition.Must, "Public"));
            Query.Filters.Add(Filter(Constants.Fields.Domain, ElasticCondition.Filter, "Public"));

            //Filter any post where the poster is replying to themselves from the results
            var filter = Filter(Constants.Fields.Username, ElasticCondition.Filter, QueryOperator.Contains, Query.Text, false);
            Query.Filters.Add(filter);

            return CreateQuery(Query);
        }
    }
}
