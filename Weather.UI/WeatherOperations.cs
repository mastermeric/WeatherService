using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Weather.Entities;

namespace Weather.UI
{
    public class WeatherOperations
    {
        public async void GetExistingRecords()
        {
            try
            {
                IEnumerable<WeatherRecord> result = null;
                using (HttpClient clnt = new HttpClient())
                {
                    clnt.BaseAddress = new Uri("http://localhost:50217/");
                    clnt.DefaultRequestHeaders.Accept.Clear();
                    clnt.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await clnt.GetAsync("api/WeatherApi/GetDailyRecords");
                    response.EnsureSuccessStatusCode();

                    if (response.IsSuccessStatusCode)
                    {
                        result = await response.Content.ReadAsAsync<IEnumerable<WeatherRecord>>();
                    }

                    if (result != null)
                    {
                        Console.WriteLine("\n");
                        Console.WriteLine("==========  Mevcut Kayitlar  ==========");
                        foreach (var item in result)
                        {
                            Console.WriteLine(item.wrLocation.ToUpper());
                            Console.WriteLine("Gunluk -> Max: " + item.wrDailyMaxValue + ", Min: " + item.wrDailyMinValue);
                            Console.WriteLine("Haftalık -> Max: " + item.wrMaxWeaklyValue +", Min: " + item.wrMinWeaklyValue + "\n");
                        }
                        Console.WriteLine("=======================================");
                    }
                    else
                    {
                        Console.WriteLine("Bekleyen bir kayit yok.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR at GetExistingRecords:" + ex.Message);
            }
        }
    }
}
