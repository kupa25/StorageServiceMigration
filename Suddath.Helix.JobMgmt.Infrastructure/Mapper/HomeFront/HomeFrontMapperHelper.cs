using AutoMapper;
using Suddath.Helix.Common.Infrastructure.EventBus.Events;
using Suddath.Helix.Common.Infrastructure.EventBus.Events.HomeFront;
using Suddath.Helix.Common.Infrastructure.EventBus.Events.HomeFront.FromHomeFront.TaskOrderAssigned;
using Suddath.Helix.JobMgmt.Infrastructure.Constants;
using Suddath.Helix.JobMgmt.Infrastructure.Domain;
using Suddath.Helix.JobMgmt.Models;
using Suddath.Helix.JobMgmt.Models.ResponseModels.TaskOrder;
using System.Collections.Generic;
using System.Linq;

namespace Suddath.Helix.JobMgmt.Infrastructure.Mapper.HomeFront
{
    public static class ModelTranslations
    {
        static ModelTranslations()
        {
            Mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<TaskOrderMapperProfile>();
            }).CreateMapper();
        }

        internal static IMapper Mapper { get; }

        public static ICollection<EmailDto> ToEmailDto(ContactInfoDto src)
        {
            var results = new List<EmailDto>();
            if (!string.IsNullOrWhiteSpace(src.Email))
            {
                results.Add(
                    new EmailDto()
                    {
                        EmailType = EmailType.PRIMARY,
                        Value = src.Email
                    });
            };
            if (!string.IsNullOrWhiteSpace(src.AlternateEmail))
            {
                results.Add(
                    new EmailDto()
                    {
                        EmailType = EmailType.ALTERNATE,
                        Value = src.AlternateEmail
                    });
            };
            return results;
        }

        public static string ToContactInfoDto(IEnumerable<EmailDto> src)
        {
            return src.FirstOrDefault().Value;
        }

        public static ContactInfoDto ToContactInfoDto(TransfereeDto transferee)
        {
            return new ContactInfoDto
            {
                Email = transferee.Emails.FirstOrDefault()?.Value,
                ContactNumber = ToContactNumberDto(transferee.OriginPhones)
            };
        }

        public static ContactInfoDto ToContactInfoDto(Transferee transferee)
        {
            return new ContactInfoDto
            {
                Email = transferee.Email.FirstOrDefault()?.Value,
                ContactNumber = ToContactNumberDto(transferee.TransfereePhone)
            };
        }

        public static ICollection<PhoneDto> ToPhoneDto(IEnumerable<ContactNumberDto> src)
        {
            return Mapper.Map<ICollection<PhoneDto>>(src);
        }

        public static Phone ToPhone(ContactNumberDto src)
        {
            return Mapper.Map<Phone>(src);
        }

        public static ICollection<ContactNumberDto> ToContactNumberDto(ICollection<TransfereePhone> src)
        {
            return Mapper.Map<ICollection<ContactNumberDto>>(src);
        }

        public static ICollection<ContactNumberDto> ToContactNumberDto(IEnumerable<PhoneDto> src)
        {
            return Mapper.Map<ICollection<ContactNumberDto>>(src);
        }

        public static JobInfoDto ToAriveAddresses(this HfAddressDto origin, HfAddressDto destination)
        {
            var originAddress = Mapper.Map<AddressDto>(origin);
            if (originAddress != null)
            {
                originAddress.Type = AddressType.ORIGIN;
            }

            var destinationAddress = Mapper.Map<AddressDto>(destination);
            if (destinationAddress != null)
            {
                destinationAddress.Type = AddressType.DESTINATION;
            }

            var addresses = new List<AddressDto> { originAddress, destinationAddress };

            return new JobInfoDto
            {
                Addresses = addresses,
                OriginAddressAdditionalInfo = originAddress.AdditionalAddressInfo,
                OriginAddressLabel = ToFullAddress(origin),
                DestinationAddressLabel = ToFullAddress(destination),
                DestinationAddressAdditionalInfo = destinationAddress.AdditionalAddressInfo,
            };
        }

        private static bool AddressMatch(AddressDto address, string addressLabel)
        {
            return addressLabel.Contains(address.Address1)
                && addressLabel.Contains(address.Address2)
                && addressLabel.Contains(address.City)
                && addressLabel.Contains(address.State)
                && addressLabel.Contains(address.PostalCode);
        }

        public static TaskOrderAddressInfoResponse ToAddresseInfoResponse(this JobInfoDto jobInfo)
        {
            var originAddress = jobInfo.Addresses.FirstOrDefault(a => AddressMatch(a, jobInfo.OriginAddressLabel));
            var destinationAddress = jobInfo.Addresses.FirstOrDefault(a => AddressMatch(a, jobInfo.DestinationAddressLabel));

            return new TaskOrderAddressInfoResponse
            {
                OriginAddress = Mapper.Map<DutyStationAddressDto>(originAddress),
                DestinationAddress = Mapper.Map<DutyStationAddressDto>(destinationAddress)
            };
        }

        public static TaskOrderAddressInfoResponse ToTaskOrderAddressInfoResponse(this Job job)
        {
            return new TaskOrderAddressInfoResponse
            {
                OriginAddress = Mapper.Map<HfAddressDto>(job.OriginAddress),
                DestinationAddress = Mapper.Map<HfAddressDto>(job.DestinationAddress)
            };
        }

        public static AddressDto ToGoogleAddressFormattedList(this DutyStationAddressDto homeFrontAddress)
        {
            return new AddressDto
            {
                Address1 = homeFrontAddress.AddressLine1.Split(' ')[0],
                Address2 = homeFrontAddress.AddressLine1.Substring(homeFrontAddress.AddressLine1.IndexOf(' ') + 1),
                Address3 = homeFrontAddress.AddressLine3,
                Display = ToFullAddress(homeFrontAddress),
            };
        }

        public static AddressDto ToGoogleAddressFormattedList(this HfAddressDto homeFrontAddress)
        {
            return new AddressDto
            {
                Address1 = homeFrontAddress.AddressLine1.Split(' ')[0],
                Address2 = homeFrontAddress.AddressLine1.Substring(homeFrontAddress.AddressLine1.IndexOf(' ') + 1),
                Address3 = homeFrontAddress.AddressLine3,
                Display = ToFullAddress(homeFrontAddress),
            };
        }

        public static string ToFullAddress(this HfAddressDto homeFrontAddress)
        {
            return string.Concat(
                    homeFrontAddress.AddressLine1,
                    " ",
                    string.Concat(homeFrontAddress.City, ", "),
                    homeFrontAddress.StateOrProvince,
                    " ",
                    homeFrontAddress.PostalCode,
                    ", ",
                    homeFrontAddress.CountryCode
                    );
        }

        public static string GetFirstNameFromFullName(string fullName)
        {
            return fullName.Split(' ')[0];
        }

        public static string GetLastNameFromFullName(string fullName)
        {
            var endOfFirstName = fullName.Split(' ')[0].Length;
            var lastName = fullName.Substring(endOfFirstName + 1, fullName.Length - endOfFirstName - 1);

            return lastName;
        }
    }
}