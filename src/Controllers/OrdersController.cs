using Microsoft.AspNetCore.Mvc;
using services_order_update.Services;
using services_order_update.Models;


[ApiController]
[Route("[controller]")]
public class OrdersController : ControllerBase
{
    private readonly OrderServices _orderService;
    private readonly HttpClient _httpClient;

    public OrdersController(OrderServices orderService, HttpClient httpClient)
    {
        _orderService = orderService;
        _httpClient = httpClient;
    }

    [HttpPut("update/{codice}")]
    public async Task<IActionResult> CreateOrder(string codice, [FromBody] Orders order)
    {
        if (order == null)
        {
            return BadRequest(new ApiResponse<string>("Error", null, "Invalid order data"));
        }

        var exitsOrder = await _orderService.CheckIfOrderExistsAsync(codice);

        if (exitsOrder == false)
        {
            return BadRequest(new ApiResponse<string>("Error", null, "The order code does not exist"));
        }

        var existProduct = await _orderService.CheckIfProductExistsAsync(order.ProductCode);

        if (existProduct == false)
        {
            return BadRequest(new ApiResponse<string>("Error", null, "The product code is incorrect"));
        }

        var existWorkTeam = await _orderService.CheckIfWorkTeamExistsAsync(order.TeamCode);

        if (existWorkTeam == false)
        {
            return BadRequest(new ApiResponse<string>("Error", null, "The work team code is incorrect"));
        }


        var updatedOrder = await _orderService.UpdateOrderAsync(codice, order);

        if (updatedOrder == null)
        {
            return StatusCode(400, new ApiResponse<string>("Error", null, "Order was updated but could not be retrieved"));
        }

        return Ok(new ApiResponse<Orders>("success", updatedOrder, "Order updated successfully"));
    }
}
