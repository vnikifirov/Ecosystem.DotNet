
namespace bank_identification_code.Mapping
{
    using System.Data;
    using AutoMapper;
    using bank_identification_code.Core.Models;

  public sealed class DataTableToBNKSEEKProfile : Profile
    {
        public DataTableToBNKSEEKProfile()
        {
            // DataSource to Domain
            CreateMap<DataRow, BNKSEEKEntity>()
                .ForMember( v => v.VKEY, opt => opt.MapFrom(row => row["VKEY"]))
                .ForMember( v => v.REAL, opt => opt.MapFrom(row => row["REAL"]))
                .ForMember( v => v.PZN, opt => opt.MapFrom(row => row["PZN"]))
                .ForMember( v => v.UER, opt => opt.MapFrom(row => row["UER"]))
                .ForMember( v => v.RGN, opt => opt.MapFrom(row => row["RGN"]))
                .ForMember( v => v.IND, opt => opt.MapFrom(row => row["IND"]))
                .ForMember( v => v.TNP, opt => opt.MapFrom(row => row["TNP"]))
                .ForMember( v => v.NNP, opt => opt.MapFrom(row => row["NNP"]))
                .ForMember( v => v.ADR, opt => opt.MapFrom(row => row["ADR"]))
                .ForMember( v => v.RKC, opt => opt.MapFrom(row => row["RKC"]))
                .ForMember( v => v.NAMEP, opt => opt.MapFrom(row => row["NAMEP"]))
                .ForMember( v => v.NAMEN, opt => opt.MapFrom(row => row["NAMEN"]))
                .ForMember( v => v.NEWNUM, opt => opt.MapFrom(row => row["NEWNUM"]))
                .ForMember( v => v.NEWKS, opt => opt.MapFrom(row => row["NEWKS"]))
                .ForMember( v => v.PERMFO, opt => opt.MapFrom(row => row["PERMFO"]))
                .ForMember( v => v.SROK, opt => opt.MapFrom(row => row["SROK"]))
                .ForMember( v => v.AT1, opt => opt.MapFrom(row => row["AT1"]))
                .ForMember( v => v.AT2, opt => opt.MapFrom(row => row["AT2"]))
                .ForMember( v => v.TELEF, opt => opt.MapFrom(row => row["TELEF"]))
                .ForMember( v => v.REGN, opt => opt.MapFrom(row => row["REGN"]))
                .ForMember( v => v.OKPO, opt => opt.MapFrom(row => row["OKPO"]))
                .ForMember( v => v.DT_IZM, opt => opt.MapFrom(row => row["DT_IZM"]))
                .ForMember( v => v.CKS, opt => opt.MapFrom(row => row["CKS"]))
                .ForMember( v => v.KSNP, opt => opt.MapFrom(row => row["KSNP"]))
                .ForMember( v => v.DATE_IN, opt => opt.MapFrom(row => row["DATE_IN"]))
                .ForMember( v => v.DATE_CH, opt => opt.MapFrom(row => row["DATE_CH"]))
                .ForMember( v => v.VKEYDEL, opt => opt.MapFrom(row => row["VKEYDEL"]))
                .ForMember( v => v.DT_IZMR, opt => opt.MapFrom(row => row["DT_IZMR"]));
        }
    }
}