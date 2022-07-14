using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AutoLot.Model.Entities;
using Microsoft.Extensions.Options;


namespace AutoLot.Services.ApiWrapper
{
    public class ApiServiceWrapper : IApiServiceWrapper
    {
        private readonly HttpClient _client;

        private readonly ApiServiceSettings _settings;


        public ApiServiceWrapper(HttpClient client, ApiServiceSettings settings)
        {
            _settings = settings;
            _client = client;
            _client.BaseAddress = new Uri(_settings.Uri);      
        }


        public async Task<Car> AddCarAsync(Car entity)
        {
            var response = await PostAsJson($"{_settings.Uri}{_settings.CarBaseUri}", JsonSerializer.Serialize(entity));

            if (response == null)
            {
                throw new Exception("Unable to communicate with service");
            }

            return await response.Content.ReadFromJsonAsync<Car>();
        }

        public async Task DeleteCarAsync(int id, Car entity)
        {
            var response = await DeleteAsJson($"{_settings.Uri}{_settings.CarBaseUri}/{id}", JsonSerializer.Serialize(entity));
            response.EnsureSuccessStatusCode();
        }

        public async Task<Car> GetCarAsync(int id)
        {
            var response = await _client.GetAsync($"{_settings.Uri}{_settings.CarBaseUri}/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Car>();
        }

        public async Task<IList<Car>> GetCarsAsync()
        {
            var response = await _client.GetAsync($"{_settings.Uri}{_settings.CarBaseUri}");
            response.EnsureSuccessStatusCode(); // throws
            var result = await response.Content.ReadFromJsonAsync<IList<Car>>();
            return result;
        }

        public async Task<IList<Car>> GetCarsByMakeAsync(int id)
        {
            var response = await _client.GetAsync($"{_settings.Uri}{_settings.CarBaseUri}/bymake/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IList<Car>>();
        }

        public async Task<IList<Make>> GetMakesAsync()
        {
            var response = await _client.GetAsync($"{_settings.Uri}{_settings.MakeBaseUri}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IList<Make>>();
        }

        public async Task<Car> UpdateCarAsync(int id, Car entity)
        {
            var response = await PutAsJson($"{_settings.Uri}{_settings.CarBaseUri}/{id}", JsonSerializer.Serialize(entity));
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<Car>();
        }

        internal async Task<HttpResponseMessage> PostAsJson(string uri, string json)
        {
            return await _client.PostAsync(uri, new StringContent(json, Encoding.UTF8, "application/json"));
        }

        internal async Task<HttpResponseMessage> PutAsJson(string uri, string json)
        {
            return await _client.PutAsync(uri, new StringContent(json, Encoding.UTF8, "application/json"));
        }

        // HttpClient has no method for delete action with passing body
        internal async Task<HttpResponseMessage> DeleteAsJson(string uri, string json)
        {
            HttpRequestMessage request = new()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json"),
                Method = HttpMethod.Delete,
                RequestUri = new Uri(uri),
            };

            return await _client.SendAsync(request);
        }
    }
}
