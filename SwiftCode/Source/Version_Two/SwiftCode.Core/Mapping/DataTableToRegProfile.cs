namespace SwiftCode.Core.Mapping
{
    using AutoMapper;
    using System.Data;
    using SwiftCode.Core.Persistence.Entities;

    public sealed class DataTableToRegProfile : Profile
    {
        public DataTableToRegProfile() =>
            // DataSource to Domain
            CreateMap<DataRow, RegEntity>()
                .ForMember(v => v.VKEY, opt => opt.MapFrom(row => row["VKEY"]))
                .ForMember(v => v.CENTER, opt => opt.MapFrom(row => row["CENTER"]))
                .ForMember(v => v.NAME, opt => opt.MapFrom(row => row["NAME"]))
                .ForMember(v => v.NAMET, opt => opt.MapFrom(row => row["NAMET"]))
                .ForMember(v => v.RGN, opt => opt.MapFrom(row => row["RGN"]));
    }
}
