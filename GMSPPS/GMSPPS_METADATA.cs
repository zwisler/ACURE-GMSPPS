using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace GMSPPS
{
    //public partial class GPMS_PROVIDER
    //{
        
    //    public int ID { get; set; }
    //    public Nullable<int> GPMS_PROVIDER_TYP_ID { get; set; }
    //    public string UserName { get; set; }
    //    public string Name { get; set; }
    //    public string Email { get; set; }
    //    public string URL { get; set; }
    //    public string LOGO_URL { get; set; }
    //    public string API_TOOKEN { get; set; }
    //    public string Suscribe_Password { get; set; }

    //    public virtual GPMS_PROVIDER_TYP GPMS_PROVIDER_TYP { get; set; }
    //    public virtual ICollection<GPMS_PROVIDER_CLIENTS> GPMS_PROVIDER_CLIENTS { get; set; }
    //    public virtual ICollection<GPMS_PROVIDER_SUPPLY> GPMS_PROVIDER_SUPPLY { get; set; }
    //}
    public class GPMS_PROVIDERMetadata
    {
        [Required]
        
        [Display(Name = "Name of Provider ", Description = "test desc")]
        
        public int Name { get; set; }

        //[StringLength(100)]
        //public string Url { get; set; }

        [Required]             
        [Display(Name = "Url off Logo", Description = "test desc")]
        [DataType(DataType.ImageUrl)]
        public string LOGO_URL { get; set; }

        [Required]
        [DataType(DataType.Url)]
        [Display(Name = "Url off Info Page"  , Description = "test desc")]
        public string URL { get; set; }

        
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password for Suscription  ", Description = "test desc")]
        public string Suscribe_Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Suscribe_Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    [MetadataType(typeof(GPMS_PROVIDERMetadata))]
    public partial class GPMS_PROVIDER
    {
        public string ConfirmPassword { get; set; }
    }
}