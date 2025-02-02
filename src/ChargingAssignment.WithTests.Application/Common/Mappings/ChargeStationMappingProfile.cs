using AutoMapper;
using CharginAssignment.WithTests.Application.ChargeStationUseCases.CreateChargeStation;
using CharginAssignment.WithTests.Application.ChargeStationUseCases.GetChargeStationById;
using CharginAssignment.WithTests.Domain.Entities;

namespace CharginAssignment.WithTests.Application.Common.Mappings;

public class ChargeStationMappingProfile : Profile
{
    public ChargeStationMappingProfile()
    {
        CreateMap<CreateChargeStationCommand, ChargeStationEntity>()
            .AfterMap((x, chargeStationEntity) => chargeStationEntity.Id = Guid.NewGuid());

        CreateMap<ConnectorDto, ConnectorEntity>().ReverseMap();

        CreateMap<ChargeStationEntity, ChargeStationDto>();
    }
}