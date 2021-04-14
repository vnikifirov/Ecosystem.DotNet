namespace bank_identification_code.Mapping
{
    using System.Data;
    using AutoMapper;
    using bank_identification_code.Core.Models;

    public class DataTableToREGProfile : Profile
    {
        public DataTableToREGProfile()
        {
            // DataSource to Domain
            CreateMap<DataRow, REGEntity>()
                .ForMember(v => v.VKEY, opt => opt.MapFrom(row => row["VKEY"]))
                .ForMember(v => v.CENTER, opt => opt.MapFrom(row => row["CENTER"]))
                .ForMember(v => v.NAME, opt => opt.MapFrom(row => row["NAME"]))
                .ForMember(v => v.NAMET, opt => opt.MapFrom(row => row["NAMET"]))
                .ForMember(v => v.RGN, opt => opt.MapFrom(row => row["RGN"]));
        }
    }
}