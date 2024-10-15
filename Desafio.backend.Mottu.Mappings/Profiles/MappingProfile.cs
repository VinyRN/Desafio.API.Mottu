using AutoMapper;
using Desafio.backend.Mottu.Dominio.Dto;
using Desafio.backend.Mottu.Dominio.Dto.Request;
using Desafio.backend.Mottu.Dominio.Dto.Response;
using Desafio.backend.Mottu.Dominio.Entidades;

namespace Desafio.backend.Mottu.Mappings.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<EntregadorRequestDTO, Entregador>();
            CreateMap<Entregador, EntregadorResponseDTO>();
        }
    }
}
