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
    
    public partial class GPMS_DEVICE_TYPE
    {
        public GPMS_DEVICE_TYPE()
        {
            this.GPMS_CLIENT = new HashSet<GPMS_CLIENT>();
        }
    
        public int ID { get; set; }
        public string DEVICE_SYSTEM { get; set; }
        public string PNS_STRING { get; set; }
    
        public virtual ICollection<GPMS_CLIENT> GPMS_CLIENT { get; set; }
    }
}