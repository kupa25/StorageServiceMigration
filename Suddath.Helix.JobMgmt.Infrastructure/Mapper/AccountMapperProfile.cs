using AutoMapper;
using Suddath.Helix.Common.Infrastructure.EventBus.Events.AccountManagement;
using Suddath.Helix.JobMgmt.Infrastructure.Domain;
using Suddath.Helix.JobMgmt.Models;

namespace Suddath.Helix.JobMgmt.Infrastructure.Mapper
{
    public class AccountMapperProfile : Profile
    {
        public AccountMapperProfile()
        {
            CreateMap<AccountEntityDTO, AccountEntity>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.AccountEntityId))
                .ForMember(d => d.ShortAddress, opt => opt.MapFrom(src => DtoTranslations.ToShortAddress(src.Address.City, src.Address.State, src.Address.Country)));

            CreateMap<AccountEntityCreatedIntegrationEvent, AccountEntity>()
                .IgnoreAllPropertiesWithAnInaccessibleSetter()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.AccountEntityId))
                .ForMember(d => d.ShortAddress, opt => opt.MapFrom(src => DtoTranslations.ToShortAddress(src.Address.City, src.Address.State, src.Address.Country)))
                ;

            CreateMap<AccountEntityUpdatedIntegrationEvent, AccountEntity>()
                .IgnoreAllPropertiesWithAnInaccessibleSetter()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.AccountEntityId))
                .ForMember(d => d.ShortAddress, opt => opt.MapFrom(src => DtoTranslations.ToShortAddress(src.Address.City, src.Address.State, src.Address.Country)))
                ;
        }
    }
}