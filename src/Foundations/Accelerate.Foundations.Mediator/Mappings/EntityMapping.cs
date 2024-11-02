using Accelerate.Foundations.Database.Models;
using Accelerate.Foundations.Mediator.Commands;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Mediator.Mappings
{
    public class EntityMapping<T> : Profile where T : IBaseEntity
    {
        public EntityMapping()
        {
            CreateMap<T, T>().ReverseMap();
            CreateMap<T, CreateEntityCommand<T>>(MemberList.Destination).ForMember(d => d.Entity, opt => opt.MapFrom(src => src))
                .ForPath(dest => dest.Entity, opt => opt.MapFrom(src => src))
                .ReverseMap();
            CreateMap<T, UpdateEntityCommand<T>>(MemberList.Destination).ForMember(d => d.Entity, opt => opt.MapFrom(src => src))
                .ForPath(dest => dest.Entity, opt => opt.MapFrom(src => src))
                .ReverseMap();
            CreateMap<T, DeleteEntityCommand<T>>(MemberList.Destination).ForMember(d => d.Entity, opt => opt.MapFrom(src => src))
                .ForPath(dest => dest.Entity, opt => opt.MapFrom(src => src))
                .ReverseMap();
            
            //CreateMap<T, UpdateEntityCommand<T>>().ForMember(d => d.Entity, opt => opt.MapFrom(src => src));
            //CreateMap<T, DeleteEntityCommand<T>>().ForMember(d => d.Entity, opt => opt.MapFrom(src => src));

            //CreateMap<T, UpdateEntityCommand<T>>().ReverseMap();
            //CreateMap<T, DeleteEntityCommand<T>>().ReverseMap();
        }
    }
}
