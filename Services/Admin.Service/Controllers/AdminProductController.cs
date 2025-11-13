using IronBridge.Shared.DTOs;
using IronBridge.Shared.Enums;
using Microsoft.AspNetCore.Mvc;
using Admin.Service.Services;

namespace Admin.Service.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminProductController : ControllerBase
{
    private readonly IProductHttpClient _productClient;
    private readonly IUserHttpClient _userClient;

    public AdminProductController(IProductHttpClient productClient, IUserHttpClient userClient)
    {
        _productClient = productClient;
        _userClient = userClient;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProducts()
    {
        var products = await _productClient.GetAllProductsAsync();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProductById(int id)
    {
        var product = await _productClient.GetProductByIdAsync(id);

        if (product == null)
            return NotFound(new { message = "Product not found" });

        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] CreateProductDto dto)
    {
        // Verify user is admin
        var user = await _userClient.GetUserByIdAsync(dto.CreatedBy);

        if (user == null || user.Role != UserRole.Admin)
            return Unauthorized(new { message = "Only admins can create products" });

        var product = await _productClient.CreateProductAsync(dto);

        if (product == null)
            return BadRequest(new { message = "Failed to create product" });

        return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ProductDto>> UpdateProduct(int id, [FromBody] UpdateProductDto dto, [FromQuery] Guid adminId)
    {
        // Verify user is admin
        var user = await _userClient.GetUserByIdAsync(adminId);

        if (user == null || user.Role != UserRole.Admin)
            return Unauthorized(new { message = "Only admins can update products" });

        var product = await _productClient.UpdateProductAsync(id, dto);

        if (product == null)
            return NotFound(new { message = "Product not found or update failed" });

        return Ok(product);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProduct(int id, [FromQuery] Guid adminId)
    {
        // Verify user is admin
        var user = await _userClient.GetUserByIdAsync(adminId);

        if (user == null || user.Role != UserRole.Admin)
            return Unauthorized(new { message = "Only admins can delete products" });

        var result = await _productClient.DeleteProductAsync(id);

        if (!result)
            return NotFound(new { message = "Product not found or delete failed" });

        return NoContent();
    }
}
