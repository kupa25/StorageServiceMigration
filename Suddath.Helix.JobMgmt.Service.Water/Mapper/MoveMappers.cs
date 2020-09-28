﻿using System;
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

        public static List<CreateJobNoteRequest> ToNotesModel(this List<Notes> notes)
        {
            var obj = notes == null ? null : Mapper.Map<List<CreateJobNoteRequest>>(notes);

            return obj;
        }

        public static Models.ServiceDto ToModel(this Move entity)
        {
            return entity == null ? null : Mapper.Map<Models.MoveServiceDto>(entity);
        }

        internal static string ToNotesCategory(Notes src)
        {
            var result = string.Empty;

            switch (src.TABLE_NAME)
            {
                case "MOVE_JOBCOST":
                    result = "JC";
                    break;

                case "CLAIMS_NOTES":
                    result = "IC";
                    break;

                case "MOVES":
                    result = "serviceboard";
                    break;

                case "MOVE_AGENTS6":
                    result = "ST";
                    break;

                case "MOVE_AGENTS1":
                    result = "OA";
                    break;

                case "MOVE_AGENTS2":
                    result = "DA";
                    break;

                default:
                    result = "serviceboard";
                    break;
            }

            return result;
        }

        internal static TransfereeDto ToTransferee(Move src)
        {
            var origin = src.OriginShipper;
            var destination = src.DestinationShipper;

            var emailList = new List<EmailDto>();

            if (!string.IsNullOrEmpty(origin.Email1))
            {
                emailList.Add(new EmailDto { Value = origin.Email1 });
            }
            if (!string.IsNullOrEmpty(origin.Email2))
            {
                emailList.Add(new EmailDto { Value = origin.Email2 });
            }
            if (!string.IsNullOrEmpty(origin.Email3))
            {
                emailList.Add(new EmailDto { Value = origin.Email3 });
            }

            if (!string.IsNullOrEmpty(destination.Email1))
            {
                emailList.Add(new EmailDto { Value = destination.Email1 });
            }
            if (!string.IsNullOrEmpty(destination.Email2))
            {
                emailList.Add(new EmailDto { Value = destination.Email2 });
            }
            if (!string.IsNullOrEmpty(destination.Email3))
            {
                emailList.Add(new EmailDto { Value = destination.Email3 });
            }

            var phoneList = new List<PhoneDto>();

            if (!string.IsNullOrEmpty(origin.PHONE1))
            {
                phoneList.Add(new PhoneDto { NationalNumber = origin.PHONE1 });
            }
            if (!string.IsNullOrEmpty(origin.PHONE2))
            {
                phoneList.Add(new PhoneDto { NationalNumber = origin.PHONE2 });
            }
            if (!string.IsNullOrEmpty(destination.PHONE1))
            {
                phoneList.Add(new PhoneDto { NationalNumber = destination.PHONE1 });
            }
            if (!string.IsNullOrEmpty(destination.PHONE2))
            {
                phoneList.Add(new PhoneDto { NationalNumber = destination.PHONE2 });
            }

            var dto = new TransfereeDto
            {
                FirstName = origin.FirstName,
                LastName = origin.LastName,
                IsVip = origin.IsVip.Equals("YES", StringComparison.CurrentCultureIgnoreCase),
                Emails = emailList,
                OriginPhones = phoneList
            };

            return dto;
        }

        internal static JobInfoDto ToJobInfo(Move move)
        {
            var origin = move.OriginShipper;
            var destination = move.DestinationShipper;

            if (origin == null)
            {
                throw new Exception($"origin is null on the moves table. Group_CODE probably doesn't match");
            }
            if (destination == null)
            {
                throw new Exception($"destination is null on the moves table. Group_CODE probably doesn't match");
            }

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
    }
}