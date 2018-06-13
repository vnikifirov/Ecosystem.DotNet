namespace bank_identification_code.Mapping
{
    using System.Data;
    using AutoMapper;
    using bank_identification_code.Core.Models;

    public class DataTableToPZNProfile : Profile
    {
        public DataTableToPZNProfile()
        {
            // DataSource to Domain
            CreateMap<DataRow, PZNEntity>()
                .ForMember(v => v.VKEY, opt => opt.MapFrom(row => row["VKEY"]))
                .ForMember(v => v.CB_DATE, opt => opt.MapFrom(row => row["CB_DATE"]))
                .ForMember(v => v.CE_DATE, opt => opt.MapFrom(row => row["CE_DATE"]))
                .ForMember(v => v.IMY, opt => opt.MapFrom(row => row["IMY"]))
                .ForMember(v => v.NAME, opt => opt.MapFrom(row => row["NAME"]))
                .ForMember(v => v.PZN, opt => opt.MapFrom(row => row["PZN"]));
        }
    }
}