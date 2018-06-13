namespace bank_identification_code.Mapping
{
    using System.Data;
    using AutoMapper;
    using bank_identification_code.Core.Models;
    public class DataTableToUERProfile : Profile
    {
        public DataTableToUERProfile()
        {
            // DataSource to Domain
            CreateMap<DataRow, UEREntity>()
                .ForMember(v => v.VKEY, opt => opt.MapFrom(row => row["VKEY"]))
                .ForMember(v => v.UER, opt => opt.MapFrom(row => row["UER"]))
                .ForMember(v => v.UERNAME, opt => opt.MapFrom(row => row["UERNAME"]));
        }
    }
}