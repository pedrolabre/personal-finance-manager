#nullable enable
using AutoMapper;
using PersonalFinanceManager.Data.Entities;
using PersonalFinanceManager.Models.DTOs;
using System.Linq;

namespace PersonalFinanceManager.Core.Mapping;

public class PendenciaProfile : Profile
{
    public PendenciaProfile()
    {
        // Pendencia <-> PendenciaDto
        CreateMap<Pendencia, PendenciaDto>()
            .ForMember(dest => dest.NomeCartao, opt => opt.MapFrom(src => src.CartaoCredito != null ? src.CartaoCredito.Nome : null))
            .ForMember(dest => dest.QuantidadeParcelas, opt => opt.MapFrom(src => src.Parcelas.Count))
            .ForMember(dest => dest.ValorPago, opt => opt.MapFrom(src => src.Parcelas.Where(p => p.Status == Models.Enums.StatusParcela.Paga).Sum(p => p.Valor)));

        CreateMap<PendenciaDto, Pendencia>()
            .ForMember(dest => dest.Parcelas, opt => opt.Ignore())
            .ForMember(dest => dest.Acordos, opt => opt.Ignore())
            .ForMember(dest => dest.CartaoCredito, opt => opt.Ignore());
    }
}

public class CartaoCreditoProfile : Profile
{
    public CartaoCreditoProfile()
    {
        // CartaoCredito <-> CartaoCreditoDto
        CreateMap<CartaoCredito, CartaoCreditoDto>()
            .ForMember(dest => dest.TotalDividas, opt => opt.MapFrom(src => src.Pendencias.Sum(p => p.ValorTotal)))
            .ForMember(dest => dest.QuantidadeDividas, opt => opt.MapFrom(src => src.Pendencias.Count));

        CreateMap<CartaoCreditoDto, CartaoCredito>()
            .ForMember(dest => dest.Pendencias, opt => opt.Ignore());
    }
}

public class ParcelaProfile : Profile
{
    public ParcelaProfile()
    {
        // Parcela <-> ParcelaDto
        CreateMap<Parcela, ParcelaDto>();
        
        CreateMap<ParcelaDto, Parcela>()
            .ForMember(dest => dest.Pendencia, opt => opt.Ignore())
            .ForMember(dest => dest.Acordo, opt => opt.Ignore());
    }
}

public class AcordoProfile : Profile
{
    public AcordoProfile()
    {
        // Acordo <-> AcordoDto
        CreateMap<Acordo, AcordoDto>()
            .ForMember(dest => dest.NomePendencia, opt => opt.MapFrom(src => src.Pendencia != null ? src.Pendencia.Nome : string.Empty))
            .ForMember(dest => dest.Parcelas, opt => opt.MapFrom(src => src.Parcelas));

        CreateMap<AcordoDto, Acordo>()
            .ForMember(dest => dest.Pendencia, opt => opt.Ignore())
            .ForMember(dest => dest.Parcelas, opt => opt.Ignore());
    }
}

public class RecebimentoProfile : Profile
{
    public RecebimentoProfile()
    {
        // Recebimento <-> RecebimentoDto
        CreateMap<Recebimento, RecebimentoDto>();
        
        CreateMap<RecebimentoDto, Recebimento>();
    }
}
