using AutoMapper;
using Suddath.Helix.JobMgmt.Infrastructure.Domain;
using Suddath.Helix.JobMgmt.Models;
using Suddath.Helix.JobMgmt.Models.ResponseModels.ApplicationPatch;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Suddath.Helix.JobMgmt.Infrastructure.Mapper
{
    public class VendorMapperProfile : Profile
    {
        public VendorMapperProfile()
        {
            CreateMap<Vendor, VendorDTO>()

                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(d => d.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(d => d.Website, opt => opt.MapFrom(src => src.Website))
                .ForMember(d => d.Accounting_SI_Code, opt => opt.MapFrom(src => src.Accounting_SI_Code))
                .ForMember(d => d.Accounting_MIL_Code, opt => opt.MapFrom(src => src.Accounting_MIL_Code))
                .ForMember(d => d.IsActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(d => d.VendorAddress, y => y.MapFrom(new VendorAddressDtoResolver()))
                ;

            CreateMap<VendorDTO, Vendor>()
                .ForMember(d => d.AccountingId, opt => opt.MapFrom(src => src.Accounting_SI_Code))
                .ForMember(d => d.ShortAddress, opt => opt.MapFrom(src => DtoTranslations.ToShortAddress(
                    src.VendorAddress.FirstOrDefault(a =>
                        a.VendorAddressType.Name.Equals("Main", StringComparison.InvariantCultureIgnoreCase)).City,
                    src.VendorAddress.FirstOrDefault(a =>
                            a.VendorAddressType.Name.Equals("Main", StringComparison.InvariantCultureIgnoreCase)).State
                        .Name,
                    src.VendorAddress.FirstOrDefault(a =>
                            a.VendorAddressType.Name.Equals("Main", StringComparison.InvariantCultureIgnoreCase))
                        .Country
                        .Name)))
                .ForMember(d => d.PrimaryAddress, y => y.MapFrom(new VendorDtoVendorAddressResolver()))
                ;

            // Note: VendorlegacyQueryDto will likely be deprecated
            CreateMap<VendorLegacyQueryDto, Vendor>()
                .ForMember(d => d.AccountingId, opt => opt.MapFrom(src => src.AccountingId))
                .ForMember(d => d.ShortAddress, opt => opt.MapFrom(src => src.ShortAddress))
                .ReverseMap()
                ;
        }

        public class VendorAddressDtoResolver : IValueResolver<Vendor, VendorDTO, List<VendorAddressDTO>>
        {
            public List<VendorAddressDTO> Resolve(Vendor source, VendorDTO destination, List<VendorAddressDTO> destMember, ResolutionContext context)
            {
                var list = new List<VendorAddressDTO>();

                if (source.PrimaryAddress != null)
                {
                    var vendorAddressDTO = new VendorAddressDTO()
                    {
                        Id = source.PrimaryAddress.Id,
                        VendorAddressType = new NameIdDto() { Id = 1, Name = "Main"},
                        Address1 = source.PrimaryAddress.Address1,
                        Address2 = source.PrimaryAddress.Address2,
                        Address3 = source.PrimaryAddress.Address3,
                        City = source.PrimaryAddress.City,
                        StateName = source.PrimaryAddress.State,
                        PostalCode = source.PrimaryAddress.PostalCode,
                        Longitude = source.PrimaryAddress.Longitude,
                        Latitude = source.PrimaryAddress.Latitude,
                        AdditionalAddressInfo = source.PrimaryAddress.AdditionalAddressInfo,
                        Type = "Main",
                        // TODO: Figure a way to look-up the Id value
                        State = new NameIdDto() { Id = 0, Name = source.PrimaryAddress.State},
                        Country = new NameIdDto() { Id = 0, Name = source.PrimaryAddress.Country }
                    };
                    list.Add(vendorAddressDTO);
                }

                return list;
            }
        }

        public class VendorDtoVendorAddressResolver : IValueResolver<VendorDTO, Vendor, Address>
        {
            public Address Resolve(VendorDTO source, Vendor destination, Address destMember, ResolutionContext context)
            {
                if (source.VendorAddress != null && source.VendorAddress.Any(x => x.Id == 1))
                {
                    var vendorAddress = source.VendorAddress.FirstOrDefault(x => x.Id == 1);

                    if (vendorAddress != null)
                    {
                        var address = new Address()
                        {
                            Id = vendorAddress.Id,
                            Address1 = vendorAddress.Address1,
                            Address2 = vendorAddress.Address2,
                            Address3 = vendorAddress.Address3,
                            City = vendorAddress.City,
                            State = vendorAddress.StateName,
                            PostalCode = vendorAddress.PostalCode,
                            Country = vendorAddress.Country?.Name,
                            Type = "Main",
                            AdditionalAddressInfo = vendorAddress.AdditionalAddressInfo,
                            Longitude = vendorAddress.Longitude,
                            Latitude = vendorAddress.Latitude,
                        };

                        return address;
                    }
                }

                return null;
            }
        }
    }
}