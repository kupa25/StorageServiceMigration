using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Suddath.Helix.Common.Extensions;
using Suddath.Helix.Common.Infrastructure.EventBus.Events;
using Suddath.Helix.JobMgmt.Models;
using Suddath.Helix.JobMgmt.Services.Interfaces;
using Suddath.Helix.JobMgmt.Services.Water.DbContext;
using Tandem.Utility;

namespace Suddath.Helix.JobMgmt.Services.Water.Mapper
{
    public static class MoveMappers
    {
        static MoveMappers()
        {
            Mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MoveMapperProfile>();
            }).CreateMapper();
        }

        internal static IMapper Mapper { get; }

        public static Job2Dto ToJobModel(this Move entity, int accountid, int vendorid, int? billToId, string billToLabel)
        {
            var obj = entity == null ? null : Mapper.Map<Job2Dto>(entity);

            obj.Job.Account.Id = accountid;
            obj.Job.Booker.Id = vendorid;
            obj.Job.BillToType = billToLabel;
            obj.Job.BillTo.Id = billToId.GetValueOrDefault();

            return obj;
        }

        public static Models.ServiceDto ToModel(this Move entity)
        {
            return entity == null ? null : Mapper.Map<Models.MoveServiceDto>(entity);
        }

        #region Converters

        public static string CreateWaterMoveId(Move m)
        {
            return $"{MoveIdPrefix.GMMS_SI.Name}{m.Id}";
        }

        public static string GetServiceName(Move m)
        {
            return "Moving Service";
        }

        public static string GetMobilityType(Move m)
        {
            if (m.TYPE_OF_MOVE.StartsWith("D/P"))
                return "Door to Port";
            if (m.TYPE_OF_MOVE.StartsWith("D/D"))
                return "Door to Door";
            if (m.TYPE_OF_MOVE.StartsWith("P/D"))
                return "Port to Door";
            if (m.TYPE_OF_MOVE.StartsWith("P/P"))
                return "Port to Port";

            return "Unknown";
        }

        internal static TransfereeDto ToTransferee(Move src)
        {
            var origin = src.OriginShipper;
            var dto = new TransfereeDto
            {
                FirstName = origin.FirstName,
                LastName = origin.LastName,
                IsVip = origin.IsVip.Equals("YES", StringComparison.CurrentCultureIgnoreCase),
                Emails = new List<EmailDto>
                {
                    new EmailDto { Value = origin.Email1 },
                    new EmailDto { Value = src.DestinationShipper.Email1 },
                },
                OriginPhones = new List<PhoneDto>
                {
                    new PhoneDto{ NationalNumber = origin.PHONE1}
                }
            };

            return dto;
        }

        internal static JobInfoDto ToJobInfo(Move move)
        {
            var origin = move.OriginShipper;
            var destination = move.DestinationShipper;

            var dto = new JobInfoDto
            {
                OriginAddressLabel = $"{origin.Street} {origin.City}, {origin.State} {origin.Zip}",
                OriginAddressAdditionalInfo = origin.Appartment,
                DestinationAddressAdditionalInfo = $"{destination.Street} {destination.City}, {destination.State} {destination.Zip}",
                DestinationAddressLabel = destination.Appartment,
                Addresses = new List<AddressDto>
                {
                    new AddressDto
                    {
                        Type = "Origin",
                        Address1 = origin.Street,
                        City = origin.City,
                        State = origin.State,
                        PostalCode = origin.Zip,
                        AdditionalAddressInfo = origin.Appartment
                    },
                    new AddressDto
                    {
                        Type = "Destination",
                        Address1 = destination.Street,
                        City = destination.City,
                        State = destination.State,
                        PostalCode = destination.Zip,
                        AdditionalAddressInfo = destination.Appartment
                    }
                }
            };

            return dto;
        }

        internal static string ToBranchName(string bRANCH_CODE)
        {
            var result = string.Empty;

            switch (bRANCH_CODE)
            {
                case "05-030-00":
                    result = "SENTRY_MOVE_MANAGEMENT";
                    break;

                case "05-020-00":
                    result = "SHIPPER_DIRECT";
                    break;

                case "05-016-00":
                    result = "SUDDATH_INTERNATIONAL";
                    break;

                case "05-026-00":
                    result = "UNIGROUP_UNITED_FORWARDING";
                    break;
            }

            return result;
        }

        internal static object ToJob(Move src)
        {
            return Mapper.Map<JobDto>(src);
        }

        public static string GetModeOfTransportName(Move m)
        {
            if (m.MODE_OF_TRANSPORT.ToUpper() == "FCL" ||
                m.MODE_OF_TRANSPORT.ToUpper() == "LCL")
                return "Ocean";

            return m.MODE_OF_TRANSPORT.ToTitleCase(TitleCase.All);
        }

        public static string GetAccountName(Move m)
        {
            if (m != null && m.Account != null)
                return m.Account.FirstName.ToTitleCase(TitleCase.All);

            return string.Empty;
        }

        public static string GetMoveStatus(Move m)
        {
            string status = string.Empty;
            if (string.IsNullOrEmpty(m.RegNumber))
            {
                return "Registration Not Complete";
            }

            if (m.SurveyDate.HasValue)
            {
                if (m.SurveyDate.Value > DateTime.Now.Date)
                {
                    status = $"Survey Scheduled For {m.SurveyDate.Value.ToShortDateString()}";
                }
                else
                {
                    status = $"Survey Completed On {m.SurveyDate.Value.ToShortDateString()}";
                }
            }
            if (m.OrignSITinDate.HasValue && m.OrignSITinDate.Value.Date <= DateTime.Now.Date)
            {
                status = $"In Storage At Origin Since {m.OrignSITinDate.Value.ToShortDateString()}";
            }

            if (m.OrignSIToutDate.HasValue && m.OrignSIToutDate.Value.Date <= DateTime.Now.Date)
            {
                status = $"Taken Out Of Storage At Origin On {m.OrignSIToutDate.Value.ToShortDateString()}";
            }

            if (m.CustomsInDate.HasValue && m.CustomsInDate.Value.Date <= DateTime.Now.Date)
            {
                status = $"Clearing Customs, In Since {m.CustomsInDate.Value.ToShortDateString()}";
            }

            if (m.CustomsOutDate.HasValue && m.CustomsOutDate.Value.Date <= DateTime.Now.Date)
            {
                status = $"Cleared Customs On {m.CustomsOutDate.Value.ToShortDateString()}";
            }

            if (m.PackDate.HasValue)
            {
                if (m.PackDate.Value.Date > DateTime.Now.Date)
                {
                    status = $"Packing Scheduled For {m.PackDate.Value.ToShortDateString()}";
                }
                else
                {
                    status = $"Packing Completed On {m.PackDate.Value.ToShortDateString()}";
                }
            }
            if (m.LoadDate.HasValue)
            {
                if (m.LoadDate.Value.Date > DateTime.Now.Date)
                {
                    status = $"Pickup Scheduled For {m.LoadDate.Value.ToShortDateString()}";
                }
                else
                {
                    status = $"In Transit, Picked Up On {m.LoadDate.Value.ToShortDateString()}";
                }
            }

            if (m.TYPE_OF_MOVE == "CANCELLED")
                return "Move Cancelled";

            if (m.DestSITinDate.HasValue && m.DestSITinDate.Value.Date <= DateTime.Now.Date)
            {
                status = $"In Storage At Destination Since {m.DestSITinDate.Value.ToShortDateString()}";
            }

            if (m.DestSIToutDate.HasValue && m.DestSIToutDate.Value.Date <= DateTime.Now.Date)
            {
                status = $"Taken Out Of Storage At Destination On {m.DestSIToutDate.Value.ToShortDateString()}";
            }

            if (m.ActualDeliveryDate.HasValue)
            {
                if (m.ActualDeliveryDate.Value.Date > DateTime.Now.Date)
                {
                    status = $"In Transit, Delivery Scheduled For {m.ActualDeliveryDate.Value.ToShortDateString()}";
                }
                else
                {
                    status = $"Delivered On {m.ActualDeliveryDate.Value.ToShortDateString()}";
                }
            }
            if (m.MODE_OF_TRANSPORT == "STORAGE")
            {
                if (!m.LoadDate.HasValue)
                {
                    if (!m.StorageSITinDate.HasValue)
                        status = $"In Storage;Storage";
                    else
                        status = $"In Storage since {m.StorageSITinDate.Value.ToShortDateString()}";
                }
                else
                {
                    if (m.LoadDate.Value.Date > DateTime.Now.Date)
                    {
                        status = $"Bound for Storage, Pickup Scheduled For {m.LoadDate.Value.ToShortDateString()}";
                    }
                    else
                    {
                        status = $"In Storage, Pickup Up On {m.LoadDate.Value.ToShortDateString()}";
                    }
                }
                if (m.ActualDeliveryDate.HasValue)
                {
                    status = $"Delivered From Storage On {m.ActualDeliveryDate.Value.ToShortDateString()}";
                }
            }

            return status;
        }

        public static ICollection<TrackerDto> CreateTrackingDetails(ICollection<MoveTracking> trackings)
        {
            var trackingDetails = new List<TrackerDto>();
            if (trackings != null)
            {
                foreach (var detail in trackings)
                {
                    trackingDetails.Add(new TrackerDto
                    {
                        Date = detail.EventDate?.ToShortDateString().ToTitleCase(TitleCase.All),
                        DetailTxt = detail.Description.ToTitleCase(TitleCase.All),
                        IconTxt = GetMoveDetailsStatusType(detail).ToTitleCase(TitleCase.All)
                    });
                }
            }
            return trackingDetails;
        }

        private static string GetMoveDetailsStatusType(MoveTracking detail)
        {
            string statType = string.Empty;
            string detailStr = detail.Description.ToLower();
            if (detailStr.Contains("register"))
                statType = "Register";
            else
            if (detailStr.Contains("survey"))
                statType = "Survey";
            else
            if (detailStr.Contains("storage"))
                statType = "Storage";
            else
            if (detailStr.Contains("inland"))
                statType = "Truck";
            else
            if (detailStr.Contains("customs"))
                statType = "Customs";
            else
            if (detailStr.Contains("ocean"))
                statType = "Ocean";
            else
            if (detailStr.Contains("pack"))
                statType = "Pack";
            else
            if (detailStr.Contains("pickup") ||
                detailStr.Contains("load"))
                statType = "Pickup";
            else
            if (detailStr.Contains("deliver"))
                statType = "Delivered";
            else
            if (detailStr.Contains("air"))
            {
                statType = "Air";

                if (detailStr.Contains("arrived"))
                {
                    statType += "-Arrived";
                }
                else if (detailStr.Contains("departed"))
                {
                    statType += "-Departed";
                }
            }

            return statType;
        }

        private static string GetNormalizedState(string country, string state)
        {
            if (country != null && country.IsUSA())
            {
                var fndState = CountrySubdivisionLookup.GetCountrySubdivisionData().FirstOrDefault(c => c.subdivisionName.ToLower() ==
                            state?.ToLower());
                return fndState == null ? state : fndState.subdivisionCode;
            }
            return state?.ToTitleCase(TitleCase.All);
        }

        private static string GetNormalizedCountry(string country)
        {
            if (country != null && country.IsUSA())
            {
                return "US";
            }
            return country?.ToTitleCase(TitleCase.All);
        }

        #endregion Converters
    }
}