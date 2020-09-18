﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

namespace Suddath.Helix.JobMgmt.Infrastructure.Domain
{
    public partial class ServiceOrder
    {
        public ServiceOrder()
        {
            ServiceOrderAirFreightItem = new HashSet<ServiceOrderAirFreightItem>();
            ServiceOrderAirFreightLeg = new HashSet<ServiceOrderAirFreightLeg>();
            ServiceOrderContact = new HashSet<ServiceOrderContact>();
            ServiceOrderDestinationAgentPartialDelivery = new HashSet<ServiceOrderDestinationAgentPartialDelivery>();
            ServiceOrderInsuranceClaim = new HashSet<ServiceOrderInsuranceClaim>();
            ServiceOrderOceanFreightContainer = new HashSet<ServiceOrderOceanFreightContainer>();
            ServiceOrderOceanFreightLCL = new HashSet<ServiceOrderOceanFreightLCL>();
            ServiceOrderOceanFreightLeg = new HashSet<ServiceOrderOceanFreightLeg>();
            ServiceOrderOceanFreightVehicle = new HashSet<ServiceOrderOceanFreightVehicle>();
            ServiceOrderRoadFreightLTL = new HashSet<ServiceOrderRoadFreightLTL>();
            ServiceOrderRoadFreightLeg = new HashSet<ServiceOrderRoadFreightLeg>();
            ServiceOrderStoragePartialDelivery = new HashSet<ServiceOrderStoragePartialDelivery>();
            ServiceOrderStorageRevenue = new HashSet<ServiceOrderStorageRevenue>();
            ServiceOrderThirdPartyCrate = new HashSet<ServiceOrderThirdPartyCrate>();
            ServiceOrderThirdPartyService = new HashSet<ServiceOrderThirdPartyService>();
        }

        public int Id { get; set; }
        public int JobId { get; set; }
        public int ServiceId { get; set; }
        public string ServiceOrderStatusIdentifier { get; set; }
        public int SuperServiceOrderId { get; set; }
        public int TransfereeId { get; set; }
        public int? VendorId { get; set; }
        public bool IsAuthorized { get; set; }
        public string QuoteReferenceNumber { get; set; }
        public bool? ISAllDocumentsReceived { get; set; }
        public DateTime DateCreated { get; set; }
        public string CreatedBy { get; set; }
        public DateTime DateModified { get; set; }
        public string ModifiedBy { get; set; }

        public virtual Job Job { get; set; }
        public virtual Service Service { get; set; }
        public virtual ServiceOrderStatus ServiceOrderStatusIdentifierNavigation { get; set; }
        public virtual SuperServiceOrder SuperServiceOrder { get; set; }
        public virtual Transferee Transferee { get; set; }
        public virtual Vendor Vendor { get; set; }
        public virtual ServiceOrderAirFreight ServiceOrderAirFreight { get; set; }
        public virtual ServiceOrderCustomClearance ServiceOrderCustomClearance { get; set; }
        public virtual ServiceOrderDestinationAgent ServiceOrderDestinationAgent { get; set; }
        public virtual ServiceOrderLineHaul ServiceOrderLineHaul { get; set; }
        public virtual ServiceOrderMoveInfo ServiceOrderMoveInfo { get; set; }
        public virtual ServiceOrderOceanFreight ServiceOrderOceanFreight { get; set; }
        public virtual ServiceOrderRoadFreight ServiceOrderRoadFreight { get; set; }
        public virtual ServiceOrderStorage ServiceOrderStorage { get; set; }
        public virtual ServiceOrderThirdParty ServiceOrderThirdParty { get; set; }
        public virtual ICollection<ServiceOrderAirFreightItem> ServiceOrderAirFreightItem { get; set; }
        public virtual ICollection<ServiceOrderAirFreightLeg> ServiceOrderAirFreightLeg { get; set; }
        public virtual ICollection<ServiceOrderContact> ServiceOrderContact { get; set; }
        public virtual ICollection<ServiceOrderDestinationAgentPartialDelivery> ServiceOrderDestinationAgentPartialDelivery { get; set; }
        public virtual ICollection<ServiceOrderInsuranceClaim> ServiceOrderInsuranceClaim { get; set; }
        public virtual ICollection<ServiceOrderOceanFreightContainer> ServiceOrderOceanFreightContainer { get; set; }
        public virtual ICollection<ServiceOrderOceanFreightLCL> ServiceOrderOceanFreightLCL { get; set; }
        public virtual ICollection<ServiceOrderOceanFreightLeg> ServiceOrderOceanFreightLeg { get; set; }
        public virtual ICollection<ServiceOrderOceanFreightVehicle> ServiceOrderOceanFreightVehicle { get; set; }
        public virtual ICollection<ServiceOrderRoadFreightLTL> ServiceOrderRoadFreightLTL { get; set; }
        public virtual ICollection<ServiceOrderRoadFreightLeg> ServiceOrderRoadFreightLeg { get; set; }
        public virtual ICollection<ServiceOrderStoragePartialDelivery> ServiceOrderStoragePartialDelivery { get; set; }
        public virtual ICollection<ServiceOrderStorageRevenue> ServiceOrderStorageRevenue { get; set; }
        public virtual ICollection<ServiceOrderThirdPartyCrate> ServiceOrderThirdPartyCrate { get; set; }
        public virtual ICollection<ServiceOrderThirdPartyService> ServiceOrderThirdPartyService { get; set; }
    }
}