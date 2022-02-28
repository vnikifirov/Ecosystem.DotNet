using System;
using System.Collections.Generic;
using System.Text;

using AutoMapper;
using Business.Services.Domain.Requests;
using Business.Services.Domain.Responses;

namespace Business.Mapping
{
    public class TasksMapper : Profile
    {
        public TasksMapper()
        {
            // Domain to API Resources
            CreateMap<Context.Models.Task, TaskResponse>()
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Id_Project, opt => opt.MapFrom(src => src.Id_Project))
                .ForMember(dest => dest.Project, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority));

            // API Resources to Domain
            CreateMap<CreateTaskRequest, Context.Models.Task>()
                .ForMember(source => source.Id, opt => opt.Ignore())
                .ForMember(source => source.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(source => source.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(source => source.Id_Project, opt => opt.MapFrom(src => src.Id_Project))
                .ForMember(source => source.Project, opt => opt.Ignore())
                .ForMember(source => source.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(source => source.Priority, opt => opt.MapFrom(src => src.Priority));

            CreateMap<SaveTaskRequest, Context.Models.Task>()
                .ForMember(source => source.Id, opt => opt.Ignore())
                .ForMember(source => source.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(source => source.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(source => source.Id_Project, opt => opt.MapFrom(src => src.Id_Project))
                .ForMember(source => source.Project, opt => opt.Ignore())
                .ForMember(source => source.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(source => source.Priority, opt => opt.MapFrom(src => src.Priority));

            // API Resources to API Resources
            CreateMap<TaskResponse, SaveTaskRequest> ()
                .ForMember(source => source.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(source => source.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(source => source.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(source => source.Id_Project, opt => opt.MapFrom(src => src.Id_Project))
                .ForMember(source => source.Project, opt => opt.Ignore())
                .ForMember(source => source.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(source => source.Priority, opt => opt.MapFrom(src => src.Priority));
        }
    }
}
