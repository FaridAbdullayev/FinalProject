using HotelUI.Models.Admin;
using Microsoft.Net.Http.Headers;
using System.Text.Json;

namespace HotelUI.Services
{

    public class LayoutService : ILayoutService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private HttpClient _client;
        public LayoutService(IHttpContextAccessor httpContextAccessor)
        {
            _client = new HttpClient();
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<UserProfileResponse> GetProfile()
        {
            _client.DefaultRequestHeaders.Remove(HeaderNames.Authorization);
            _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, _httpContextAccessor.HttpContext.Request.Cookies["token"]);
            using (var response = await _client.GetAsync("https://localhost:7119/api/auth/profileLayout"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
                    var data = JsonSerializer.Deserialize<UserProfileResponse>(await response.Content.ReadAsStringAsync(), options);

                    return data;
                }
            }
            return null;
        }
    }
}
