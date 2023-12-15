using MasterCompound.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace MasterCompound.Services
{
    public static class DataService
    {
        private static readonly string _Url = "http://58.26.126.39/WebserviceDataMaster/servicecompound.asmx/";
        private static readonly string _Key = "string";

        private static async Task<BaseModel<T>> GetAsync<T>(string endPoint)
        {
            var result = new BaseModel<T>();
            try
            {
                using HttpClient httpClient = new HttpClient();
                string url = $"{_Url}{endPoint}?key={_Key}";

                HttpResponseMessage response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string soapResponse = await response.Content.ReadAsStringAsync();
                    result.Data = JsonConvert.DeserializeObject<T>(soapResponse);
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

        public static async Task<BaseModel<List<JenisKenderaan>>> GetJenisKenderaan()
        {
            return await GetAsync<List<JenisKenderaan>>("GetJenKen");
        }
        public static async Task<BaseModel<List<Kenderaan>>> GetKenderaan()
        {
            return await GetAsync<List<Kenderaan>>("GetKenderaan");
        }

        public static async Task<BaseModel<List<Warna>>> GetWarna()
        {
            return await GetAsync<List<Warna>>("GetWarna");
        }

        public static async Task<BaseModel<List<Kesalahan>>> GetKesalahan()
        {
            return await GetAsync<List<Kesalahan>>("GetKesalahan");
        }

        public static async Task<BaseModel<List<Kawasan>>> GetKawasan()
        {
            return await GetAsync<List<Kawasan>>("GetKawasan");
        }

        public static async Task<BaseModel<List<Akta>>> GetAkta()
        {
            return await GetAsync<List<Akta>>("GetAkta");
        }

        public static async Task<BaseModel<List<KodHantar>>> GetKodHantar()
        {
            return await GetAsync<List<KodHantar>>("GetKodHantar");
        }

        public static async Task<BaseModel<List<Zon>>> GetZon()
        {
            return await GetAsync<List<Zon>>("GetZon");
        }

        public static async Task<BaseModel<List<TempatJadi>>> GetTempatJadi()
        {
            return await GetAsync<List<TempatJadi>>("GetTempatJadi");
        }

        public static async Task<BaseModel<List<KodSita>>> GetKodSita()
        {
            return await GetAsync<List<KodSita>>("GetKodSita");
        }
    }
}