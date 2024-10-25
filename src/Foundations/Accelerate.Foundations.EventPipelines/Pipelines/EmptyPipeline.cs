using Accelerate.Foundations.EventPipelines.Pipelines;

namespace Accelerate.Foundations.EventPipelines.Pipelines
{
    public class EmptyCreatedPipeline<T> : DataCreateEventPipeline<T>
    {
        public EmptyCreatedPipeline()
        {
        }
    }
    public class EmptyUpdatedPipeline<T> : DataUpdateEventPipeline<T>
    {
        public EmptyUpdatedPipeline()
        {
        }
    }
    public class EmptyDeletedPipeline<T> : DataDeleteEventPipeline<T>
    {
        public EmptyDeletedPipeline()
        {
        } 
    }
}
