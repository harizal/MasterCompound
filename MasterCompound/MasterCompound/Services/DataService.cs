using MasterCompound.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MasterCompound.Services
{
    public static class DataService
    {
        public static async Task<BaseModel<List<Akta>>> GetAkta()
        {
            var result = new BaseModel<List<Akta>>();
            try
            {
                using HttpClient httpClient = new HttpClient();
                string url = "http://58.26.126.39/WebserviceDataMaster/servicecompound.asmx/GetAkta?key=string";

                HttpResponseMessage response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string soapResponse = await response.Content.ReadAsStringAsync();
                    result.Data = JsonConvert.DeserializeObject<List<Akta>>(soapResponse);
                }
                else
                {
                    result.Error = true;
                    result.ErrorMessage = $"Error {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                result.Error = true;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }
    }
}