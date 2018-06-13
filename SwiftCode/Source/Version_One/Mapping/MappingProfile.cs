namespace bank_identification_code.Mapping
{
    using bank_identification_code.Core.Models;
    using AutoMapper;
    using bank_identification_code.Controllers.Resources;

    public sealed class MappingProfile : Profile
    {
        public MappingProfile()
        {

            // ? Created in order to update entities with AutoMapper and ignoring a nested collections
            // ? URL: https://visualstudiomagazine.com/blogs/tool-tracker/2013/11/updating--entities-with-automapper.aspx
            CreateMap<PZNEntity, PZNEntity>().ForAllMembers( opt => opt.Ignore() );
            CreateMap<REGEntity, REGEntity>().ForAllMembers( opt => opt.Ignore() );
            CreateMap<TNPEntity, TNPEntity>().ForAllMembers( opt => opt.Ignore() );
            CreateMap<UEREntity, UEREntity>().ForAllMembers( opt => opt.Ignore() );

            // ? Domain to API Resources
            CreateMap<BNKSEEKEntity, SaveBNKSEEKResource>()
                .ForMember( br => br.VKEY, opt => opt.Ignore())
                .ForMember( br => br.PZN, opt => opt.MapFrom(b => b.PZN))
                .ForMember( br => br.REGN, opt => opt.MapFrom(b => b.RGN))
                .ForMember( br => br.TNP, opt => opt.MapFrom(b => b.TNP))
                .ForMember( br => br.UER, opt => opt.MapFrom(b => b.UER));

            CreateMap<PZNResource, PZNEntity>()
                .ForMember( p => p.NAME, opt => opt.MapFrom( pr => pr.NAME ))
                .ForMember( p => p.IMY, opt => opt.MapFrom( pr => pr.IMY ))
                .ForAllMembers( others => others.Ignore() );

            // CreateMap<Filter, FilterResource>();
            CreateMap<REGEntity, REGResource>();
                    // .ForMember( r => r.VKEY, opt =>  opt.Ignore())

            CreateMap<TNPEntity, TNPResource>();
                // .ForMember( t => t.VKEY, opt =>  opt.Ignore())

            CreateMap<UEREntity, UERResource>();
                // .ForMember( u => u.VKEY, opt =>  opt.Ignore())

            // ? Becaouse a shape is different, i made a manually mapping
            CreateMap<BNKSEEKEntity, BNKSEEKResource>()
                .ForMember( b => b.DATEIN, opt => opt.MapFrom(br => br.DATE_IN))
                .ForMember( b => b.DATECH, opt => opt.MapFrom(br => br.DATE_CH))
                .ForMember( b => b.PZN, opt => opt.MapFrom(br => br.PZNEntity))
                .ForMember( b => b.UER, opt => opt.MapFrom(br => br.UEREntity))
                .ForMember( b => b.REG, opt => opt.MapFrom(br => br.REGEntity))
                .ForMember( b => b.TNP, opt => opt.MapFrom(br => br.TNPEntity));

            // ? API Resources to Domain
            CreateMap<FilterResource, Filter>();

            CreateMap<REGResource, REGEntity>()
                .ForMember( r => r.CENTER, opt => opt.MapFrom(rr => rr.CENTER))
                .ForMember( r => r.NAME, opt => opt.MapFrom(rr => rr.NAME))
                .ForMember( r => r.NAMET, opt => opt.MapFrom(rr => rr.NAMET))
                .ForAllOtherMembers( others => others.Ignore());

            CreateMap<TNPResource, TNPEntity>();
                // .ForMember( r => r.NAME, opt => opt.MapFrom(rr => rr.name))
                // .ForMember( r => r.NAMET, opt => opt.MapFrom(rr => rr.namet))
                // .ForAllOtherMembers( others => others.Ignore());

            CreateMap<UERResource, UEREntity>()
                .ForMember( u => u.VKEY, opt => opt.Ignore());
                // .ForMember( u => u.UER, opt => opt.Ignore());

            // ? Send only ids to a client in order to Upadte/Create BNKSEEK record
            CreateMap<SaveBNKSEEKResource, BNKSEEKEntity>()
                .ForMember( b => b.VKEY, opt => opt.Ignore())
                .ForMember( b => b.NEWNUM, opt => opt.Ignore())
                .ForMember( b => b.PZN, opt => opt.MapFrom(br => br.PZN))
                .ForMember( b => b.REGN, opt => opt.MapFrom(br => br.RGN))
                .ForMember( b => b.TNP, opt => opt.MapFrom(br => br.TNP))
                .ForMember( b => b.UER, opt => opt.MapFrom(br => br.UER));

            // ? Get BNKSEEK to display a full info
            CreateMap<BNKSEEKResource, BNKSEEKEntity>()
                .ForMember( b => b.VKEY, opt => opt.Ignore())
                .ForMember( b => b.REAL, opt => opt.Ignore())
                .ForMember( b => b.PZN, opt => opt.Ignore())
                .ForMember( b => b.PZNEntity, opt =>
                    opt.MapFrom( br => Mapper.Map<PZNResource, PZNEntity>(br.PZN)))
                .ForMember( b => b.UER, opt => opt.Ignore())
                .ForMember( b => b.UEREntity, opt =>
                    opt.MapFrom( br => Mapper.Map<UERResource, UEREntity>(br.UER)))
                .ForMember( b => b.RGN, opt => opt.Ignore())
                .ForMember( b => b.REGEntity, opt =>
                    opt.MapFrom( br => Mapper.Map<REGResource, REGEntity>(br.REG)))
                .ForMember( b => b.IND, opt => opt.MapFrom(br => br.IND))
                .ForMember( b => b.TNP, opt => opt.Ignore())
                .ForMember( b => b.TNPEntity, opt =>
                    opt.MapFrom( br => Mapper.Map<TNPResource, REGEntity>(br.TNP)))
                .ForMember( b => b.NNP, opt => opt.MapFrom(br => br.NNP))
                .ForMember( b => b.ADR, opt => opt.MapFrom(br => br.ADR))
                .ForMember( b => b.RKC, opt => opt.MapFrom(br => br.RKC))
                .ForMember( b => b.NAMEP, opt => opt.MapFrom(br => br.NAMEP))
                .ForMember( b => b.NEWNUM, opt => opt.MapFrom(br => br.NEWNUM))
                .ForMember( b => b.TELEF, opt => opt.MapFrom(br => br.TELEF))
                .ForMember( b => b.REGN, opt => opt.MapFrom(br => br.REGN))
                .ForMember( b => b.OKPO, opt => opt.MapFrom(br => br.OKPO))
                .ForMember( b => b.KSNP, opt => opt.MapFrom(br => br.KSNP))
                .ForMember( b => b.DATE_IN, opt => opt.MapFrom(br => br.DATEIN))
                .ForMember( b => b.DATE_CH, opt => opt.MapFrom(br => br.DATECH));
        }
    }
}