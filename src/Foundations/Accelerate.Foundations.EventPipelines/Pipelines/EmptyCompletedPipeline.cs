using Accelerate.Foundations.EventPipelines.Pipelines;

namespace Accelerate.Foundations.EventPipelines.Pipelines
{
    public class EmptyCreatedCompletedPipeline<T> : DataCreateCompletedEventPipeline<T>
    {
        public EmptyCreatedCompletedPipeline()
        {
        }
    }
    public class EmptyUpdatedCompletedPipeline<T> : DataUpdateCompletedEventPipeline<T>
    {
        public EmptyUpdatedCompletedPipeline()
        {
        }
    }
    public class EmptyDeletedCompletedPipeline<T> : DataDeleteCompletedEventPipeline<T>
    {
        public EmptyDeletedCompletedPipeline()
        {
        } 
    }
}
