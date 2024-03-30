using Accelerate.Foundations.EventPipelines.Pipelines;

namespace Accelerate.Foundations.EventPipelines.Pipelines
{
    public class DefaultObjectCreatedCompletedPipeline : DataCreateCompletedEventPipeline<object>
    {
        public DefaultObjectCreatedCompletedPipeline()
        {
        }
    }
    public class DefaultObjectUpdatedCompletedPipeline : DataUpdateCompletedEventPipeline<object>
    {
        public DefaultObjectUpdatedCompletedPipeline()
        {
        }
    }
    public class DefaultObjectDeletedCompletedPipeline : DataDeleteCompletedEventPipeline<object>
    {
        public DefaultObjectDeletedCompletedPipeline()
        {
        } 
    }
}
