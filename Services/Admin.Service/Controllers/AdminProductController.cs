using Admin.Domain.Interfaces;
using IronBridge.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Admin.Service.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminProductController : ControllerBase
{
    private readonly IProductManager _productManager;

    public AdminProductController(IProductManager productManager)
    {
        _productManager = productManager;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProducts()
    {
        var products = await _productManager.GetAllProductsAsync();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProductById(int id)
    {
        var product = await _productManager.GetProductByIdAsync(id);

        if (product == null)
            return NotFound(new { message = "Product not found" });

        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] CreateProductDto dto)
    {
        var product = await _productManager.CreateProductAsync(dto, dto.CreatedBy);

        if (product == null)
            return Unauthorized(new { message = "Only admins can create products" });

        return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ProductDto>> UpdateProduct(int id, [FromBody] UpdateProductDto dto, [FromQuery] Guid adminId)
    {
        var product = await _productManager.UpdateProductAsync(id, dto, adminId);

        if (product == null)
            return Unauthorized(new { message = "Only admins can update products or product not found" });

        return Ok(product);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProduct(int id, [FromQuery] Guid adminId)
    {
        var result = await _productManager.DeleteProductAsync(id, adminId);

        if (!result)
            return Unauthorized(new { message = "Only admins can delete products or product not found" });

        return NoContent();
    }
}
