namespace bank_identification_code.Mapping
{
    using System.Data;
    using AutoMapper;
    using bank_identification_code.Core.Models;
    public class DataTableToTNPProfile : Profile
    {
        public DataTableToTNPProfile()
        {
            // DataSource to Domain
            CreateMap<DataRow, TNPEntity>()
                .ForMember(v => v.VKEY, opt => opt.MapFrom(row => row["VKEY"]))
                .ForMember(v => v.FULLNAME, opt => opt.MapFrom(row => row["FULLNAME"]))
                .ForMember(v => v.SHORTNAME, opt => opt.MapFrom(row => row["SHORTNAME"]))
                .ForMember(v => v.TNP, opt => opt.MapFrom(row => row["TNP"]));
        }
    }
}