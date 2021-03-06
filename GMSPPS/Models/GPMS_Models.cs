﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;




namespace GMSPPS.Models
{
    
    public class DeviceRegistration
    {
        public string Platform { get; set; }
        public string Key { get; set; }
        public int ProviderID { get; set; }
        public string Handle { get; set; }
        public string[] Tags { get; set; }
    }
    public class GPMS_MissionModel
    {
        public string DefaultUrl = "http://gmspps.azurewebsites.net/";
        public string CostumMissionID { get; set; }
        public string URL { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public Position[] Positions { get; set; }
    }
    public class MissionViewModel
    {
        public int ID { get; set; }
        public string ProviderLogo  { get; set; }
        public string ProviderName { get; set; }
        public string CostumMissionID { get; set; }       
        public string Title { get; set; }
        public string Text { get; set; }
        public double LAT { get; set; }
        public double LON { get; set; }
    }
    public class Position
    {
        public double LAT { get; set; }
        public double LON { get; set; }         
    }
    public class MissionState
    {
        public int ID { get; set; }
        public int State { get; set; }
    }

    public enum ProviderType
    {
        emergency = 1,
        business = 2,
        fun = 3
    }

    public interface IProvider
    {
        string Name { get; set; }
        ProviderType Type { get; set; }
        string Url { get; set; }

    }
    public class GPMS_ProviderTagsModel
    {
        [Display(Name = "ID of Provider ")]
        public int ProviderID { get; set; }

        [Display(Name = "ID of Tag ")]
        public int ID { get; set; }

        [Display(Name = "Name of Tag ")]
        public string Name { get; set; }

    }
    public class GPMS_PROVIDERSimpleModel 
    {
        [Display(Name = "ID of Provider ")]
        public int ID { get; set; }

        [Required]
        [Display(Name = "Name of Provider ", Description = "test desc")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Info Uri of Provider ")]
        public string Url { get; set; }

        [Required]
        [Display(Name = "Icon of Provider ")]
        public string Icon { get; set; }
    }

        public class GPMS_PROVIDERBindingModel : IProvider
    {
        [Display(Name = "ID of Provider ")]
        public int ID { get; set; }

        [Required]
        [Display(Name = "Name of Provider ", Description = "test desc")]       
        public string Name { get; set; }

        [Required]
        [Display(Name = "Info Uri of Provider ")]
        public string Url { get; set; }

        [Required]
        [Display(Name = "Icon of Provider ")]
        public string Icon { get; set; }

        [Required]
        public int Typeint { get; set; } // this is the EF model property
        //[Required]
        //[JsonConverter(typeof(ProviderType))]
        //public ProviderType Type { get; set; }
       
        [JsonConverter(typeof(StringEnumConverter))]
        public ProviderType Type // this is an additional custom property
        {
            get { return (ProviderType)Typeint; }
            set { Typeint = (int)value; }
        }
       
        
        
    }
   
}
