
namespace SwiftCode.Core.Mapping
{
    using AutoMapper;
    using SwiftCode.Core.Models.Common;
    using SwiftCode.Core.Models.Request;
    using SwiftCode.Core.Persistence.Entities;

    public sealed class MappingProfile : Profile
    {
        public MappingProfile()
        {

            // ? Created in order to update entities with AutoMapper and ignoring a nested collections
            // ? URL: https://visualstudiomagazine.com/blogs/tool-tracker/2013/11/updating--entities-with-automapper.aspx
            CreateMap<PznEntity, PznEntity>().ForAllMembers(opt => opt.Ignore());
            CreateMap<RegEntity, UerEntity>().ForAllMembers(opt => opt.Ignore());
            CreateMap<TnpEntity, TnpEntity>().ForAllMembers(opt => opt.Ignore());
            CreateMap<UerEntity, UerEntity>().ForAllMembers(opt => opt.Ignore());

            // ? Domain to API Resources
            CreateMap<BnkseekEntity, SaveBnkseekDTO>()
                .ForMember(br => br.VKEY, opt => opt.Ignore())
                .ForMember(br => br.PZN, opt => opt.MapFrom(b => b.PZN))
                .ForMember(br => br.REGN, opt => opt.MapFrom(b => b.RGN))
                .ForMember(br => br.TNP, opt => opt.MapFrom(b => b.TNP))
                .ForMember(br => br.UER, opt => opt.MapFrom(b => b.UER));

            CreateMap<PznDTO, PznEntity>()
                .ForMember(p => p.NAME, opt => opt.MapFrom(pr => pr.NAME))
                .ForMember(p => p.IMY, opt => opt.MapFrom(pr => pr.IMY))
                .ForAllMembers(others => others.Ignore());

            CreateMap<RegEntity, RegDTO>();
            // .ForMember( r => r.VKEY, opt =>  opt.Ignore())

            CreateMap<TnpEntity, TnpDTO>();
            // .ForMember( t => t.VKEY, opt =>  opt.Ignore())

            CreateMap<UerEntity, UerDTO>();
            // .ForMember( u => u.VKEY, opt =>  opt.Ignore())

            // ? Becaouse a shape is different, i made a manually mapping
            CreateMap<BnkseekEntity, BnkseekDTO>()
                .ForMember(b => b.DATEIN, opt => opt.MapFrom(br => br.DATE_IN))
                .ForMember(b => b.DATECH, opt => opt.MapFrom(br => br.DATE_CH))
                .ForMember(b => b.PZN, opt => opt.MapFrom(br => br.PznEntity))
                .ForMember(b => b.UER, opt => opt.MapFrom(br => br.UerEntity))
                .ForMember(b => b.REG, opt => opt.MapFrom(br => br.RegEntity))
                .ForMember(b => b.TNP, opt => opt.MapFrom(br => br.TnpEntity));

            //CreateMap(typeof(QueryResult<>), typeof(QueryResultResource<>));

            // ? API Resources to Domain
            // CreateMap<Object, QueryOject>();

            CreateMap<RegDTO, RegEntity>()
                .ForMember(r => r.CENTER, opt => opt.MapFrom(rr => rr.CENTER))
                .ForMember(r => r.NAME, opt => opt.MapFrom(rr => rr.NAME))
                .ForMember(r => r.NAMET, opt => opt.MapFrom(rr => rr.NAMET))
                .ForAllOtherMembers(others => others.Ignore());

            CreateMap<TnpDTO, TnpEntity>();
            // .ForMember( r => r.NAME, opt => opt.MapFrom(rr => rr.name))
            // .ForMember( r => r.NAMET, opt => opt.MapFrom(rr => rr.namet))
            // .ForAllOtherMembers( others => others.Ignore());

            CreateMap<UerDTO, UerEntity>()
                .ForMember(u => u.VKEY, opt => opt.Ignore());
            // .ForMember( u => u.UER, opt => opt.Ignore());

            // ? Send only ids to a client in order to Upadte BNKSEEK record
            CreateMap<SaveBnkseekDTO, BnkseekEntity>()
                .ForMember(b => b.VKEY, opt => opt.Ignore())
                .ForMember(b => b.NEWNUM, opt => opt.Ignore())
                .ForMember(b => b.PZN, opt => opt.MapFrom(br => br.PZN))
                .ForMember(b => b.REGN, opt => opt.MapFrom(br => br.RGN))
                .ForMember(b => b.TNP, opt => opt.MapFrom(br => br.TNP))
                .ForMember(b => b.UER, opt => opt.MapFrom(br => br.UER));

            // ? Send only ids to a client in order to Create BNKSEEK record
            CreateMap<CreateBnkseekDTO, BnkseekEntity>()
                .ForMember(b => b.VKEY, opt => opt.Ignore())
                .ForMember(b => b.NEWNUM, opt => opt.Ignore())
                .ForMember(b => b.PZN, opt => opt.MapFrom(br => br.PZN))
                .ForMember(b => b.REGN, opt => opt.MapFrom(br => br.RGN))
                .ForMember(b => b.TNP, opt => opt.MapFrom(br => br.TNP))
                .ForMember(b => b.UER, opt => opt.MapFrom(br => br.UER));

            // ? Get BNKSEEK to display a full info
            CreateMap<BnkseekDTO, BnkseekEntity>()
                .ForMember(b => b.VKEY, opt => opt.Ignore())
                .ForMember(b => b.REAL, opt => opt.Ignore())
                .ForMember(b => b.PZN, opt => opt.Ignore())
                .ForMember(b => b.PznEntity, opt =>
                   opt.MapFrom(br => Mapper.Map<PznDTO, PznEntity>(br.PZN)))
                .ForMember(b => b.UER, opt => opt.Ignore())
                .ForMember(b => b.UerEntity, opt =>
                   opt.MapFrom(br => Mapper.Map<UerDTO, UerEntity>(br.UER)))
                .ForMember(b => b.RGN, opt => opt.Ignore())
                .ForMember(b => b.RegEntity, opt =>
                   opt.MapFrom(br => Mapper.Map<RegDTO, RegEntity>(br.REG)))
                .ForMember(b => b.IND, opt => opt.MapFrom(br => br.IND))
                .ForMember(b => b.TNP, opt => opt.Ignore())
                .ForMember(b => b.TnpEntity, opt =>
                   opt.MapFrom(br => Mapper.Map<TnpDTO, TnpEntity>(br.TNP)))
                .ForMember(b => b.NNP, opt => opt.MapFrom(br => br.NNP))
                .ForMember(b => b.ADR, opt => opt.MapFrom(br => br.ADR))
                .ForMember(b => b.RKC, opt => opt.MapFrom(br => br.RKC))
                .ForMember(b => b.NAMEP, opt => opt.MapFrom(br => br.NAMEP))
                .ForMember(b => b.NEWNUM, opt => opt.MapFrom(br => br.NEWNUM))
                .ForMember(b => b.TELEF, opt => opt.MapFrom(br => br.TELEF))
                .ForMember(b => b.REGN, opt => opt.MapFrom(br => br.REGN))
                .ForMember(b => b.OKPO, opt => opt.MapFrom(br => br.OKPO))
                .ForMember(b => b.KSNP, opt => opt.MapFrom(br => br.KSNP))
                .ForMember(b => b.DATE_IN, opt => opt.MapFrom(br => br.DATEIN))
                .ForMember(b => b.DATE_CH, opt => opt.MapFrom(br => br.DATECH));
        }
    }
}
