using Microsoft.Net.Http.Headers;
using System.Text.Json;


using System.Text;
using HotelUI.Models;
using HotelUI.Exceptions;
using HotelUI.Models.Admin;
using HotelUI.Models.Contact;
using Microsoft.VisualBasic;
using System.Net.Http;
using HotelUI.Models.Reservation;
using System.Net.Http.Headers;
using Microsoft.Extensions.Options;



namespace HotelUI.Services
{
    public class CrudService : ICrudService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private HttpClient _client;
        private const string baseUrl = "https://localhost:7119/api/";
        public CrudService(IHttpContextAccessor httpContextAccessor)
        {
            _client = new HttpClient();
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<PaginatedResponse<TResponse>> GetAllPaginated<TResponse>(string path, int page)
        {
            _client.DefaultRequestHeaders.Remove(HeaderNames.Authorization);
            _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, _httpContextAccessor.HttpContext.Request.Cookies["token"]);

            using (var response = await _client.GetAsync(baseUrl + path + "?page=" + page))
            {
                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
                    return JsonSerializer.Deserialize<PaginatedResponse<TResponse>>(await response.Content.ReadAsStringAsync(), options);
                }
                else throw new HttpException(response.StatusCode);
            }
        }

        public async Task<TResponse> Get<TResponse>(string path)
        {
            _client.DefaultRequestHeaders.Remove(HeaderNames.Authorization);
            _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, _httpContextAccessor.HttpContext.Request.Cookies["token"]);

            using (var response = await _client.GetAsync(baseUrl + path))
            {
                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
                    return JsonSerializer.Deserialize<TResponse>(await response.Content.ReadAsStringAsync(), options);
                }
                else { throw new HttpException(response.StatusCode); }
            }
        }
        public async Task<CreateResponse> Create<TRequest>(TRequest request, string path)
        {
            _client.DefaultRequestHeaders.Remove(HeaderNames.Authorization);
            _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, _httpContextAccessor.HttpContext.Request.Cookies["token"]);

            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            using (HttpResponseMessage response = await _client.PostAsync(baseUrl + path, content))
            {
                var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };

                if (response.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<CreateResponse>(await response.Content.ReadAsStringAsync(), options);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    ErrorResponse errorResponse = JsonSerializer.Deserialize<ErrorResponse>(await response.Content.ReadAsStringAsync(), options);
                    throw new ModelException(System.Net.HttpStatusCode.BadRequest, errorResponse);
                }
                else
                {
                    ErrorResponse errorResponse = JsonSerializer.Deserialize<ErrorResponse>(await response.Content.ReadAsStringAsync(), options);
                    throw new HttpException(response.StatusCode, errorResponse.Message);
                }
            }
        }

        public async Task Update<TRequest>(TRequest request, string path)
        {
            _client.DefaultRequestHeaders.Remove(HeaderNames.Authorization);
            _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, _httpContextAccessor.HttpContext.Request.Cookies["token"]);


            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            using (HttpResponseMessage response = await _client.PutAsync(baseUrl + path, content))
            {
                var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };

                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    ErrorResponse errorResponse = JsonSerializer.Deserialize<ErrorResponse>(await response.Content.ReadAsStringAsync(), options);
                    throw new ModelException(System.Net.HttpStatusCode.BadRequest, errorResponse);
                }
                else if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse errorResponse = JsonSerializer.Deserialize<ErrorResponse>(await response.Content.ReadAsStringAsync(), options);
                    throw new HttpException(response.StatusCode, errorResponse.Message);
                }
            }
        }

        public async Task Delete(string path)
        {
            _client.DefaultRequestHeaders.Remove(HeaderNames.Authorization);
            _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, _httpContextAccessor.HttpContext.Request.Cookies["token"]);

            using (var response = await _client.DeleteAsync(baseUrl + path))
            {
                if (!response.IsSuccessStatusCode)
                    throw new HttpException(response.StatusCode);
            }
        }

        public async Task<CreateResponse> CreateFromForm<TRequest>(TRequest request, string path)
        {
            _client.DefaultRequestHeaders.Remove(HeaderNames.Authorization);
            _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, _httpContextAccessor.HttpContext.Request.Cookies["token"]);

            MultipartFormDataContent content = new MultipartFormDataContent();
            foreach (var prop in request.GetType().GetProperties())
            {
                var val = prop.GetValue(request);

                if (val is IFormFile file)
                {
                    var strContent = new StreamContent(file.OpenReadStream());
                    strContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                    content.Add(strContent, prop.Name, file.FileName);
                }
                else if (val is List<IFormFile> fileList)
                {
                    foreach (var file2 in fileList)
                    {
                        var strContent = new StreamContent(file2.OpenReadStream());
                        strContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file2.ContentType);
                        content.Add(strContent, prop.Name, file2.FileName);
                    }
                }
                else if (val is List<int> numbers)
                {
                    foreach (var item in numbers)
                    {
                        content.Add(new StringContent(item.ToString()), prop.Name);
                    }
                }
                else if (val is DateTime dateTime)
                    content.Add(new StringContent(dateTime.ToLongDateString()), prop.Name);
                else if (val is not null)
                    content.Add(new StringContent(val.ToString()), prop.Name);
            }

            using (HttpResponseMessage response = await _client.PostAsync(baseUrl + path, content))
            {
                var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };

                if (response.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<CreateResponse>(await response.Content.ReadAsStringAsync(), options);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    ErrorResponse errorResponse = JsonSerializer.Deserialize<ErrorResponse>(await response.Content.ReadAsStringAsync(), options);
                    throw new ModelException(System.Net.HttpStatusCode.BadRequest, errorResponse);
                }
                else
                {
                    ErrorResponse errorResponse = JsonSerializer.Deserialize<ErrorResponse>(await response.Content.ReadAsStringAsync(), options);
                    throw new HttpException(response.StatusCode, errorResponse.Message);
                }
            }
        }

        public async Task UpdateFormForm<TRequest>(TRequest request, string path)
        {
            _client.DefaultRequestHeaders.Remove(HeaderNames.Authorization);
            _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, _httpContextAccessor.HttpContext.Request.Cookies["token"]);

            MultipartFormDataContent content = new MultipartFormDataContent();
            foreach (var prop in request.GetType().GetProperties())
            {
                var val = prop.GetValue(request);

                if (val is IFormFile file)
                {
                    var strContent = new StreamContent(file.OpenReadStream());
                    strContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                    content.Add(strContent, prop.Name, file.FileName);
                }
                else if (val is List<IFormFile> fileList)
                {
                    foreach (var file2 in fileList)
                    {
                        var strContent = new StreamContent(file2.OpenReadStream());
                        strContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file2.ContentType);
                        content.Add(strContent, prop.Name, file2.FileName);
                    }
                }
                else if (val is List<int> numbers)
                {
                    foreach (var item in numbers)
                    {
                        content.Add(new StringContent(item.ToString()), prop.Name);
                    }
                }
                else if (val is DateTime dateTime)
                    content.Add(new StringContent(dateTime.ToLongDateString()), prop.Name);
                else if (val is not null)
                    content.Add(new StringContent(val.ToString()), prop.Name);
            }

            using (HttpResponseMessage response = await _client.PutAsync(baseUrl + path, content))
            {
                var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };

                if (response.IsSuccessStatusCode)
                {
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    ErrorResponse errorResponse = JsonSerializer.Deserialize<ErrorResponse>(await response.Content.ReadAsStringAsync(), options);
                    throw new ModelException(System.Net.HttpStatusCode.BadRequest, errorResponse);
                }
                else
                {
                    ErrorResponse errorResponse = JsonSerializer.Deserialize<ErrorResponse>(await response.Content.ReadAsStringAsync(), options);
                    throw new HttpException(response.StatusCode, errorResponse.Message);
                }
            }
        }

        public async Task<CreateResponseForAdmin> CreateForAdmins<TRequest>(TRequest request, string path)
        {
            _client.DefaultRequestHeaders.Remove(HeaderNames.Authorization);
            _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, _httpContextAccessor.HttpContext.Request.Cookies["token"]);

            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            using (HttpResponseMessage response = await _client.PostAsync(baseUrl + path, content))
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                if (response.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<CreateResponseForAdmin>(await response.Content.ReadAsStringAsync(), options);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    ErrorResponse errorResponse = JsonSerializer.Deserialize<ErrorResponse>(await response.Content.ReadAsStringAsync(), options);
                    throw new ModelException(System.Net.HttpStatusCode.BadRequest, errorResponse);
                }
                else
                {
                    ErrorResponse errorResponse = JsonSerializer.Deserialize<ErrorResponse>(await response.Content.ReadAsStringAsync(), options);
                    throw new HttpException(response.StatusCode, errorResponse.Message);
                }
            }
        }


        public async Task SendMessageToUser(AdminAndIUserInteraction interaction)
        {
            _client.DefaultRequestHeaders.Remove(HeaderNames.Authorization);
            _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, _httpContextAccessor.HttpContext.Request.Cookies["token"]);

            var jsonContent = JsonSerializer.Serialize(interaction);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            using (var response = await _client.PostAsync(baseUrl + "contacts/admin/message", content))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpException(response.StatusCode);
                }
            }
        }


        public async Task Status(string path)
        {
            _client.DefaultRequestHeaders.Remove(HeaderNames.Authorization);
            _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, _httpContextAccessor.HttpContext.Request.Cookies["token"]);

            using (HttpResponseMessage response = await _client.PutAsync(baseUrl + path, null))
            {
                if (!response.IsSuccessStatusCode)
                {
                    var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    throw new HttpException(response.StatusCode, errorResponse?.Message);
                }
            }
        }


        public async Task<TResponse> GetAsyncBranchesIncome<TResponse>(string endpoint)
        {
            _client.DefaultRequestHeaders.Remove(HeaderNames.Authorization);
            _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, _httpContextAccessor.HttpContext.Request.Cookies["token"]);

            using (var response = await _client.GetAsync(baseUrl + endpoint))
            {
                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    return JsonSerializer.Deserialize<TResponse>(await response.Content.ReadAsStringAsync(), options);
                }
                else
                {
                    throw new HttpException(response.StatusCode);
                }
            }
        }



        public async Task<Dictionary<string, int>> GetOrdersPricePerYearAsync()
        {
            _client.DefaultRequestHeaders.Remove(HeaderNames.Authorization);
            _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, _httpContextAccessor.HttpContext.Request.Cookies["token"]);

            using (var response = await _client.GetAsync(baseUrl + "reservations/last-12-months-income"))
            {
                var json = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        // JSON verisini Dictionary olarak deserialize et
                        return JsonSerializer.Deserialize<Dictionary<string, int>>(json);
                    }
                    catch (JsonException ex)
                    {
                        // JSON deseralize hatasını logla
                        Console.WriteLine($"JSON Deserialization Error: {ex.Message}");
                        throw;
                    }
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    throw new HttpException(response.StatusCode, errorMessage);
                }
            }
        }

        public async Task<int> GetTotalReservationsCountAsync(string path)
        {
            _client.DefaultRequestHeaders.Remove(HeaderNames.Authorization);
            _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, _httpContextAccessor.HttpContext.Request.Cookies["token"]);

            using (var response = await _client.GetAsync(baseUrl + path))
            {
                var json = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<ReservationCountResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return result?.TotalReservations ?? 0;

                }
                else { throw new HttpException(response.StatusCode); }
            }
        }

        public async Task<double> GetTotalPriceReservationsAsync(string path)
        {
            _client.DefaultRequestHeaders.Remove(HeaderNames.Authorization);
            _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, _httpContextAccessor.HttpContext.Request.Cookies["token"]);

            using (var response = await _client.GetAsync(baseUrl + path))
            {
                var json = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<ReservationTotalPriceResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return result?.TotalPrice ?? 0;

                }
                else { throw new HttpException(response.StatusCode); }
            }
        }


        

    }
}
