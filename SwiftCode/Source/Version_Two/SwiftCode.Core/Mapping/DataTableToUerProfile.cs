namespace SwiftCode.Core.Mapping
{
    using System.Data;
    using AutoMapper;
    using SwiftCode.Core.Persistence.Entities;

    public sealed class DataTableToUerProfile : Profile
    {
        public DataTableToUerProfile() =>
            // DataSource to Domain
            CreateMap<DataRow, UerEntity>()
                .ForMember(v => v.VKEY, opt => opt.MapFrom(row => row["VKEY"]))
                .ForMember(v => v.UER, opt => opt.MapFrom(row => row["UER"]))
                .ForMember(v => v.UERNAME, opt => opt.MapFrom(row => row["UERNAME"]));
    }
}
