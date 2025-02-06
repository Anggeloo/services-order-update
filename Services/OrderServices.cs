using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using services_order_update.Database;
using services_order_update.Models;
using System.Net.Http;
using NewtonsoftJson = Newtonsoft.Json.JsonConvert;

namespace services_order_update.Services
{
    public class OrderServices
    {

        private readonly DBContext _context;
        private readonly HttpClient _httpClient;

        public OrderServices(DBContext context, HttpClient httpClient)
        {
            _context = context;
            _httpClient = httpClient;
        }

        public async Task<Orders> UpdateOrderAsync(string codice, Orders order)
        {
            var result = await _context.Orders.FirstOrDefaultAsync(x => x.OrderCode == codice);

            if (result == null)
            {
                return null;
            }

            result.UpdatedAt = DateTime.Now;
            result.TeamCode = order.TeamCode;
            result.ProductCode = order.ProductCode;
            result.Description = order.Description;
            result.Quantity = order.Quantity;
            result.Priority = order.Priority;
            result.OrderStatus = order.OrderStatus;
            result.AssignmentDate = order.AssignmentDate;
            result.CompletionDate = order.CompletionDate;
            result.Notes = order.Notes;

            await _context.SaveChangesAsync();

            return result;
        }

        public async Task<bool> CheckIfOrderExistsAsync(string codice)
        {
            var exist = await _context.Orders.FirstOrDefaultAsync(x => x.OrderCode == codice);
            if (exist == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public async Task<bool> CheckIfProductExistsAsync(string codice)
        {
            //var url = $"http://localhost:3000/products/byCodiceProducto?codice={codice}"; // Local
            var url = $"http://app_producto_search:3000/products/byCodiceProducto?codice={codice}";
            try
            {
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = NewtonsoftJson.DeserializeObject<ApiResponse<JArray>>(responseContent);
                    if (apiResponse != null && apiResponse.Status == "success")
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> CheckIfWorkTeamExistsAsync(string codice)
        {
            //var url = $"http://localhost:8180/work-team/{codice}"; // Local
            var url = $"http://app_work_team_search:8080/work-team/{codice}";
            try
            {
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = NewtonsoftJson.DeserializeObject<ApiResponse<JObject>>(responseContent);
                    if (apiResponse != null && apiResponse.Status == "success")
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

}


