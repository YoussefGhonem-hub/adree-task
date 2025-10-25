using AutoMapper;
using InspectionVisits.Application.DTOs;
using InspectionVisits.Domain.Entities;

namespace InspectionVisits.Application.Mappings;
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<EntityToInspect, EntityToInspectDto>().ReverseMap();
        CreateMap<Inspector, InspectorDto>().ReverseMap();
        CreateMap<Violation, ViolationDto>().ReverseMap();
        CreateMap<InspectionVisit, InspectionVisitDto>().ReverseMap();

    }
}
