//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GMSPPS
{
    using System;
    using System.Collections.Generic;
    
    public partial class GPMS_CLIENT
    {
        public GPMS_CLIENT()
        {
            this.GPMS_CLIENT_SUSCRIPT = new HashSet<GPMS_CLIENT_SUSCRIPT>();
            this.GPMS_PROVIDER_CLIENTS = new HashSet<GPMS_PROVIDER_CLIENTS>();
            this.GPMS_CLIENT_STATE = new HashSet<GPMS_CLIENT_STATE>();
            this.GPMS_CLIENT_STATE1 = new HashSet<GPMS_CLIENT_STATE>();
            this.GPMS_MISSION_CLIENT_ACC = new HashSet<GPMS_MISSION_CLIENT_ACC>();
        }
    
        public int ID { get; set; }
        public Nullable<int> GPMS_DEVICE_TYPE_ID { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string TOOKEN { get; set; }
        public string GoogleID { get; set; }
        public string RegistrationID { get; set; }
        public string LiveID { get; set; }
    
        public virtual GPMS_DEVICE_TYPE GPMS_DEVICE_TYPE { get; set; }
        public virtual ICollection<GPMS_CLIENT_SUSCRIPT> GPMS_CLIENT_SUSCRIPT { get; set; }
        public virtual ICollection<GPMS_PROVIDER_CLIENTS> GPMS_PROVIDER_CLIENTS { get; set; }
        public virtual ICollection<GPMS_CLIENT_STATE> GPMS_CLIENT_STATE { get; set; }
        public virtual ICollection<GPMS_CLIENT_STATE> GPMS_CLIENT_STATE1 { get; set; }
        public virtual ICollection<GPMS_MISSION_CLIENT_ACC> GPMS_MISSION_CLIENT_ACC { get; set; }
    }
}
