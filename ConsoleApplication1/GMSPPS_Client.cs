using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using GMSPPStoGMS.Model;


namespace GMSPPStoGMS
{
    class GMSPPS_Client
    {
        private string MISSION_URL;
        private string TOOKEN;


        public GMSPPS_Client(string backendEndpoint, string token)
        {
            TOOKEN = token;
            MISSION_URL = backendEndpoint + "/api/GPMS_Mission";
            
        }
        public async Task RegisterAsync1(GPMS_MissionModel Mission)
        {
            var statusCode = await SendMissioAsync(Mission);

        }

        public async Task<String> SendMissioAsync( GPMS_MissionModel Mission)
        {
            using (var httpClient = new HttpClient())
            {
                string MissionId;
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("GMSPPS_Tooken", TOOKEN);
                string json = JsonConvert.SerializeObject(Mission);
                var response = await httpClient.PostAsync(MISSION_URL, new StringContent(json, Encoding.UTF8, "application/json"));
                if (response.IsSuccessStatusCode)
                {
                    MissionId = await response.Content.ReadAsStringAsync();

                }
                else
                {
                    throw new System.Net.WebException(response.StatusCode.ToString());
                }

                return MissionId;
            }
        }
    }
}
