using Accelerate.Foundations.Mediator.Commands;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Mediator.Mappings
{
    public class EntityMapping<T> : Profile
    {
        public EntityMapping()
        {
            //CreateMap<T, EntityDto>().ReverseMap();
            CreateMap<T, CreateEntityCommand<T>>().ReverseMap();
            CreateMap<T, UpdateEntityCommand<T>>().ReverseMap();
            CreateMap<T, DeleteEntityCommand<T>>().ReverseMap();
        }
    }
}
