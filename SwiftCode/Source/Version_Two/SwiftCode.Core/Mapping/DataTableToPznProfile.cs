
namespace SwiftCode.Core.Mapping
{
    using AutoMapper;
    using System.Data;
    using SwiftCode.Core.Persistence.Entities;

    public sealed class DataTableToPznProfile : Profile
    {
        public DataTableToPznProfile() =>
            // DataSource to Domain
            CreateMap<DataRow, PznEntity>()
                .ForMember(v => v.VKEY, opt => opt.MapFrom(row => row["VKEY"]))
                .ForMember(v => v.CB_DATE, opt => opt.MapFrom(row => row["CB_DATE"]))
                .ForMember(v => v.CE_DATE, opt => opt.MapFrom(row => row["CE_DATE"]))
                .ForMember(v => v.IMY, opt => opt.MapFrom(row => row["IMY"]))
                .ForMember(v => v.NAME, opt => opt.MapFrom(row => row["NAME"]))
                .ForMember(v => v.PZN, opt => opt.MapFrom(row => row["PZN"]));
    }
}
