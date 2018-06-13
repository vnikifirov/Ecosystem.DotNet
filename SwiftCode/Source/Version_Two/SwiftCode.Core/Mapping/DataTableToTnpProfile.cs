
namespace SwiftCode.Core.Mapping
{
    using System.Data;
    using AutoMapper;
    using SwiftCode.Core.Persistence.Entities;

    public sealed class DataTableToTnpProfile : Profile
    {
        public DataTableToTnpProfile() =>
        // DataSource to Domain
            CreateMap<DataRow, TnpEntity>()
                    .ForMember(v => v.VKEY, opt => opt.MapFrom(row => row["VKEY"]))
                    .ForMember(v => v.FULLNAME, opt => opt.MapFrom(row => row["FULLNAME"]))
                    .ForMember(v => v.SHORTNAME, opt => opt.MapFrom(row => row["SHORTNAME"]))
                    .ForMember(v => v.TNP, opt => opt.MapFrom(row => row["TNP"]));
    }
}
