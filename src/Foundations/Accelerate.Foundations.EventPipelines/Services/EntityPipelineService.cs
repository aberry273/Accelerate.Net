using Accelerate.Foundations.Database.Models;
using Accelerate.Foundations.Database.Services;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MassTransit.DependencyInjection;
using MassTransit;
using Accelerate.Foundations.EventPipelines.EventBus;
using Accelerate.Foundations.EventPipelines.Models.Contracts;

namespace Accelerate.Foundations.EventPipelines.Services
{
    public class EntityPipelineService<T, B> : EntityService<T>, IEntityPipelineService<T, B> where T : BaseEntity where B : IDataBus<T>
    {
        readonly Bind<B, IPublishEndpoint> _publishEndpoint;

        public EntityPipelineService(
            Bind<B, IPublishEndpoint> publishEndpoint,
            BaseContext<T> dbContext, 
            ILogger<EntityService<T>> logger) : base(dbContext, logger)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task RunCreatePipeline(T obj)
        {
            await _publishEndpoint.Value.Publish(new CreateDataContract<T>()
            {
                Data = obj,
            });
        }
        public async Task RunUpdatePipeline(T obj)
        {
            await _publishEndpoint.Value.Publish(new CreateDataContract<T>()
            {
                Data = obj,
            });
        }
        public async Task RunDeletePipeline(T obj)
        {
            await _publishEndpoint.Value.Publish(new CreateDataContract<T>()
            {
                Data = obj,
            });
        }
        public async Task ProcessMultipleCreatePipelinesAsync(IEnumerable<T> entities)
        {
            await Task.WhenAll(entities.Select(entity =>
            {
                try { return RunCreatePipeline(entity); }
                catch (Exception e) { return Task.FromException(e); }
            }));
        }
        public async Task ProcessMultipleUpdatePipelinesAsync(IEnumerable<T> entities)
        {
            await Task.WhenAll(entities.Select(entity =>
            {
                try { return RunUpdatePipeline(entity); }
                catch (Exception e) { return Task.FromException(e); }
            }));
        }
        public async Task ProcessMultipleDeletePipelinesAsync(IEnumerable<T> entities)
        {
            await Task.WhenAll(entities.Select(entity =>
            {
                try { return RunDeletePipeline(entity); }
                catch (Exception e) { return Task.FromException(e); }
            }));
        }

        public async Task<int> CopyRange(IEnumerable<T> entities)
        {
            var result = await base.CopyRange(entities);

            await ProcessMultipleCreatePipelinesAsync(entities);

            return result;
        }

        public async Task<int> DeleteRange(IEnumerable<T> entities)
        {
            var result = await base.DeleteRange(entities);

            await ProcessMultipleDeletePipelinesAsync(entities);

            return result;
        }
        public async Task<int> Delete(T entity)
        {
            var result = await base.Delete(entity);
            
            await RunDeletePipeline(entity);

            return result;
        }
        public async Task<int> Update(T entity)
        {
            var result = await base.Update(entity);

            await RunUpdatePipeline(entity);

            return result;
        }
        public async Task<int> UpdateRange(IEnumerable<T> entities)
        {
            var result = await base.UpdateRange(entities);

            await ProcessMultipleUpdatePipelinesAsync(entities);

            return result;
        }
        public async Task<int> Create(T entity)
        {
            var result = await base.Create(entity);

            await RunCreatePipeline(entity);

            return result;
        }
        public async Task<Guid?> CreateWithGuid(T entity)
        {
            var result = await base.CreateWithGuid(entity);

            await RunCreatePipeline(entity);

            return result;
        }
        public async Task<int> AddRange(IEnumerable<T> entities)
        {
            var result = await base.AddRange(entities);

            await ProcessMultipleCreatePipelinesAsync(entities);

            return result;
        }
        public async Task AddAsync(T entity)
        {
            await base.AddAsync(entity);

            await RunCreatePipeline(entity);
        }
    }
}
