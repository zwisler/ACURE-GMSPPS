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
    
    public partial class GPMS_PROVIDER_CLIENTS
    {
        public GPMS_PROVIDER_CLIENTS()
        {
            this.GPMS_PROVIDER_SUSCRIPT = new HashSet<GPMS_PROVIDER_SUSCRIPT>();
        }
    
        public int ID { get; set; }
        public Nullable<int> GPMS_PROVIDER_ID { get; set; }
        public Nullable<int> GPMS_CLIENT_ID { get; set; }
    
        public virtual GPMS_CLIENT GPMS_CLIENT { get; set; }
        public virtual GPMS_PROVIDER GPMS_PROVIDER { get; set; }
        public virtual ICollection<GPMS_PROVIDER_SUSCRIPT> GPMS_PROVIDER_SUSCRIPT { get; set; }
    }
}