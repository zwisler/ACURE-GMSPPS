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
    
    public partial class GPMS_PROVIDER
    {
        public GPMS_PROVIDER()
        {
            this.GPMS_PROVIDER_CLIENTS = new HashSet<GPMS_PROVIDER_CLIENTS>();
            this.GPMS_PROVIDER_SUPPLY = new HashSet<GPMS_PROVIDER_SUPPLY>();
            this.GPMS_MISSION = new HashSet<GPMS_MISSION>();
        }
    
        public int ID { get; set; }
        public Nullable<int> GPMS_PROVIDER_TYP_ID { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string URL { get; set; }
        public string LOGO_URL { get; set; }
        public string API_TOOKEN { get; set; }
        public string Suscribe_Password { get; set; }
    
        public virtual GPMS_PROVIDER_TYP GPMS_PROVIDER_TYP { get; set; }
        public virtual ICollection<GPMS_PROVIDER_CLIENTS> GPMS_PROVIDER_CLIENTS { get; set; }
        public virtual ICollection<GPMS_PROVIDER_SUPPLY> GPMS_PROVIDER_SUPPLY { get; set; }
        public virtual ICollection<GPMS_MISSION> GPMS_MISSION { get; set; }
    }
}
