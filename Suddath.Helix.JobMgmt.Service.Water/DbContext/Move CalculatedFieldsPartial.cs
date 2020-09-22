using System;
using System.Linq;
using static Suddath.Helix.JobMgmt.Services.Water.Constants;

namespace Suddath.Helix.JobMgmt.Services.Water.DbContext
{
    public partial class Move
    {
        private Name _originShipper;

        public Name OriginShipper
        {
            get
            {
                if (_originShipper == null)
                {
                    _originShipper = Names.Where(s => s.ORIGIN_PICKUP?.Trim().ToUpper() == "ORIGIN PRIMARY").FirstOrDefault();
                }

                return _originShipper;
            }
        }

        private Name _destinationShipper;

        public Name DestinationShipper
        {
            get
            {
                if (_destinationShipper == null)
                {
                    _destinationShipper = Names.Where(s => s.ORIGIN_PICKUP?.Trim().ToUpper() == "DESTINATION PRIMARY").FirstOrDefault();
                }

                return _destinationShipper;
            }
        }

        public string FirstName
        {
            get
            {
                return OriginShipper.FirstName;
            }
        }

        public string Title
        {
            get
            {
                return OriginShipper.Title;
            }
        }

        public string LastName
        {
            get
            {
                return OriginShipper.LastName;
            }
        }

        public DateTime? BookDate
        {
            get
            {
                return DateEntered;
            }
        }

        private MoveAgent _originAgent;

        public MoveAgent OriginAgent
        {
            get
            {
                if (_originAgent == null)
                {
                    _originAgent = MoveAgents.Where(ma => ma.JobCategory == MoveAgentJobCategory.Origin).FirstOrDefault();
                }

                return _originAgent;
            }
        }

        private bool? _hasDestinationAgent;

        public bool HasDestinationAgent
        {
            get
            {
                if (_hasDestinationAgent == null)
                {
                    _hasDestinationAgent = MoveAgents.FirstOrDefault(ma => ma.JobCategory == MoveAgentJobCategory.Destination) == null ? false : true;
                }

                return _hasDestinationAgent.Value;
            }
        }

        private bool? _hasOriginAgent;

        public bool HasOriginAgent
        {
            get
            {
                if (_hasOriginAgent == null)
                {
                    _hasOriginAgent = MoveAgents.FirstOrDefault(ma => ma.JobCategory == MoveAgentJobCategory.Origin) == null ? false : true;
                }

                return _hasOriginAgent.Value;
            }
        }

        private MoveAgent _destinationAgent;

        public MoveAgent DestinationAgent
        {
            get
            {
                if (_destinationAgent == null)
                {
                    _destinationAgent = MoveAgents.Where(ma => ma.JobCategory == MoveAgentJobCategory.Destination).FirstOrDefault();
                }

                return _destinationAgent;
            }
        }

        private MoveAgent _storageAgent;

        public MoveAgent StorageAgent
        {
            get
            {
                if (_storageAgent == null)
                {
                    _storageAgent = MoveAgents.Where(ma => ma.JobCategory == MoveAgentJobCategory.Storage).FirstOrDefault();
                }

                return _storageAgent;
            }
        }

        private DateTime? _packDate;

        public DateTime? PackDate
        {
            get
            {
                if (_packDate == null)
                {
                    var mao = MoveAgents.Where(ma => ma.JobCategory == MoveAgentJobCategory.Origin).FirstOrDefault();
                    if (mao != null)
                    {
                        _packDate = mao.PackDate;
                    }
                }

                return _packDate;
            }
        }

        private DateTime? _departureDate;

        public DateTime? DepatureDate
        {
            get
            {
                if (_departureDate == null)
                {
                    var mao = MoveAgents.Where(ma => ma.JobCategory == MoveAgentJobCategory.OceanAir).FirstOrDefault();

                    if (mao != null)
                    {
                        if (mao.ACT_DEPARTURE_DATE.HasValue)
                        {
                            _departureDate = mao.ACT_DEPARTURE_DATE;
                        }
                        else
                        {
                            _departureDate = mao.EST_DEPARTURE_DATE;
                        }
                    }
                }

                return _departureDate;
            }
        }

        private DateTime? _actualDepartureDate;

        public DateTime? ActualDepatureDate
        {
            get
            {
                if (_actualDepartureDate == null)
                {
                    var mao = MoveAgents.Where(ma => ma.JobCategory == MoveAgentJobCategory.OceanAir).FirstOrDefault();
                    if (mao != null)
                    {
                        _actualDepartureDate = mao.ACT_DEPARTURE_DATE;
                    }
                }

                return _actualDepartureDate;
            }
        }

        private DateTime? _estimatedDepartureDate;

        public DateTime? EstimatedDepatureDate
        {
            get
            {
                if (_estimatedDepartureDate == null)
                {
                    var mao = MoveAgents.Where(ma => ma.JobCategory == MoveAgentJobCategory.OceanAir).FirstOrDefault();
                    if (mao != null)
                    {
                        _estimatedDepartureDate = mao.EST_DEPARTURE_DATE;
                    }
                }

                return _estimatedDepartureDate;
            }
        }

        private DateTime? _loadDate;

        public DateTime? LoadDate
        {
            get
            {
                if (_loadDate == null)
                {
                    var mao = MoveAgents.Where(ma => ma.JobCategory == MoveAgentJobCategory.Origin).FirstOrDefault();
                    if (mao != null)
                    {
                        _loadDate = mao.PickupDate;
                    }
                }

                return _loadDate;
            }
        }

        public DateTime? HaulerLoadDate
        {
            get
            {
                return InlandMoveAgent?.PickupDate;
            }
        }

        private DateTime? _requiredDeliveryDate;

        public DateTime? RequiredDeliveryDate
        {
            get
            {
                if (_requiredDeliveryDate == null)
                {
                    var mad = MoveAgents.Where(ma => ma.JobCategory == MoveAgentJobCategory.Destination).FirstOrDefault();
                    if (mad != null)
                    {
                        _requiredDeliveryDate = mad.RequiredDeliveryDate;
                    }
                }

                return _requiredDeliveryDate;
            }
        }

        private DateTime? _serveyDate;

        public DateTime? SurveyDate
        {
            get
            {
                if (_serveyDate == null)
                {
                    var mad = MoveAgents.Where(ma => ma.JobCategory == MoveAgentJobCategory.Origin).FirstOrDefault();
                    if (mad != null)
                    {
                        if (mad.ACT_SURVEY_DATE.HasValue)
                        {
                            _serveyDate = mad.ACT_SURVEY_DATE;
                        }
                        else
                        {
                            _serveyDate = mad.EST_SURVEY_DATE;
                        }
                    }
                }

                return _serveyDate;
            }
        }

        private DateTime? _actualDeliveryDate;

        public DateTime? ActualDeliveryDate
        {
            get
            {
                if (_actualDeliveryDate == null)
                {
                    var mad = MoveAgents.Where(ma => ma.JobCategory == MoveAgentJobCategory.Destination).FirstOrDefault();
                    if (mad != null)
                    {
                        if (mad.ActualDeliveryDate.HasValue)
                        {
                            _actualDeliveryDate = mad.ActualDeliveryDate;
                        }
                        else
                        {
                            _actualDeliveryDate = mad.UNPACK_DATE1;
                        }
                    }
                }

                return _actualDeliveryDate;
            }
        }

        public DateTime? DeliveryOutOfStorageActualDate
        {
            get
            {
                return ActualDeliveryDate;
            }
        }

        private DateTime? _originSITinDate;

        public DateTime? OrignSITinDate
        {
            get
            {
                if (_originSITinDate == null)
                {
                    var mad = MoveAgents.Where(ma => ma.JobCategory == MoveAgentJobCategory.Origin).FirstOrDefault();
                    if (mad != null)
                    {
                        _originSITinDate = mad.SITinDate;
                    }
                }

                return _originSITinDate;
            }
        }

        private DateTime? _orignSIToutDate;

        public DateTime? OrignSIToutDate
        {
            get
            {
                if (_orignSIToutDate == null)
                {
                    var mad = MoveAgents.Where(ma => ma.JobCategory == MoveAgentJobCategory.Origin).FirstOrDefault();
                    if (mad != null)
                    {
                        _orignSIToutDate = mad.SIToutDate;
                    }
                }

                return _orignSIToutDate;
            }
        }

        private DateTime? _storageSITinDate;

        public DateTime? StorageSITinDate
        {
            get
            {
                if (_storageSITinDate == null)
                {
                    var mad = MoveAgents.Where(ma => ma.JobCategory == MoveAgentJobCategory.Storage).FirstOrDefault();
                    if (mad != null)
                    {
                        _storageSITinDate = mad.SITinDate;
                    }
                }

                return _storageSITinDate;
            }
        }

        private DateTime? _storageSIToutDate;

        public DateTime? StorageSIToutDate
        {
            get
            {
                if (_storageSIToutDate == null)
                {
                    var mad = MoveAgents.Where(ma => ma.JobCategory == MoveAgentJobCategory.Storage).FirstOrDefault();
                    if (mad != null)
                    {
                        _storageSIToutDate = mad.SIToutDate;
                    }
                }

                return _storageSIToutDate;
            }
        }

        private DateTime? _destSITinDate;

        public DateTime? DestSITinDate
        {
            get
            {
                if (_destSITinDate == null)
                {
                    var mad = MoveAgents.Where(ma => ma.JobCategory == MoveAgentJobCategory.Destination).FirstOrDefault();
                    if (mad != null)
                    {
                        _destSITinDate = mad.SITinDate;
                    }
                }

                return _destSITinDate;
            }
        }

        private DateTime? _destSIToutDate;

        public DateTime? DestSIToutDate
        {
            get
            {
                if (_destSIToutDate == null)
                {
                    var mad = MoveAgents.Where(ma => ma.JobCategory == MoveAgentJobCategory.Destination).FirstOrDefault();
                    if (mad != null)
                    {
                        _destSIToutDate = mad.SIToutDate;
                    }
                }

                return _destSIToutDate;
            }
        }

        private DateTime? _customsInDate;

        public DateTime? CustomsInDate
        {
            get
            {
                if (_customsInDate == null)
                {
                    var mad = MoveAgents.Where(ma => ma.JobCategory == MoveAgentJobCategory.PortAgent).FirstOrDefault();
                    if (mad != null)
                    {
                        _customsInDate = mad.CustomsInDate;
                    }
                }

                return _customsInDate;
            }
        }

        private DateTime? _customsOutDate;

        public DateTime? CustomsOutDate
        {
            get
            {
                if (_customsOutDate == null)
                {
                    var mad = MoveAgents.Where(ma => ma.JobCategory == MoveAgentJobCategory.PortAgent).FirstOrDefault();
                    if (mad != null)
                    {
                        _customsOutDate = mad.CustomsOutDate;
                    }
                }

                return _customsOutDate;
            }
        }

        private MoveAgent _inLandMoveAgent;

        public MoveAgent InlandMoveAgent
        {
            get
            {
                if (_inLandMoveAgent == null)
                {
                    _inLandMoveAgent = MoveAgents.Where(ma => ma.JobCategory == MoveAgentJobCategory.Inland).FirstOrDefault();
                }

                return _inLandMoveAgent;
            }
        }

        public DateTime? InlandEtaStart
        {
            get
            {
                return InlandMoveAgent?.EstArrivalDate1;
            }
        }

        public DateTime? InlandEtaEnd
        {
            get
            {
                return InlandMoveAgent?.EstArrivalDate2;
            }
        }

        public DateTime? InlandEstPickupDate1
        {
            get { return InlandMoveAgent?.EST_PU_DATE1; }
        }

        public DateTime? InlandEstPickupDate2
        {
            get { return InlandMoveAgent?.EST_PU_DATE2; }
        }
    }
}