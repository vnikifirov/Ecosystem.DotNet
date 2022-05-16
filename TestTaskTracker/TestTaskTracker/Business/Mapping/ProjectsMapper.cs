using AutoMapper;
using Business.Services.Domain.Requests;
using Business.Services.Domain.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Mapping
{
    public class ProejctsMapper : Profile
    {
        public ProejctsMapper()
        {
            // Domain to API Resources
            CreateMap<Context.Models.Project, ProjectResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Tasks, opt => opt.MapFrom(src => src.Tasks))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Start, opt => opt.MapFrom(src => src.Start))
                .ForMember(dest => dest.Completion, opt => opt.MapFrom(src => src.Completion))
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority));

            // API Resources to Domain
            CreateMap<CreateProjectRequest, Context.Models.Project>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Tasks, opt => opt.MapFrom(src => src.Tasks))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Start, opt => opt.MapFrom(src => src.Start))
                .ForMember(dest => dest.Completion, opt => opt.MapFrom(src => src.Completion))
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority));

            // If i made changes with tasks and add / removed them from project or assign task to project
            // then will i need me to do some manipulations with projects?
            CreateMap<SaveProjectRequest, Context.Models.Project>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Tasks, opt => opt.MapFrom(src => src.Tasks))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Start, opt => opt.MapFrom(src => src.Start))
                .ForMember(dest => dest.Completion, opt => opt.MapFrom(src => src.Completion))
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority));
        }
    }
}
