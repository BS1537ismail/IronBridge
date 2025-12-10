using IronBridge.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Product.Domain.Interfaces;

namespace Product.Service.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductManager _productService;

    public ProductController(IProductManager productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProducts()
    {
        var products = await _productService.GetAllProductsAsync();
        return Ok(products);
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveProducts()
    {
        var products = await _productService.GetActiveProductsAsync();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);

        if (product == null)
            return NotFound(new { message = "Product not found" });

        return Ok(product);
    }

    [HttpGet("category/{category}")]
    public async Task<IActionResult> GetProductsByCategory(string category)
    {
        var products = await _productService.GetProductsByCategoryAsync(category);
        return Ok(products);
    }

    //[HttpPost]
    //public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto dto)
    //{
    //    var product = await _productService.CreateProductAsync(dto);
    //    return CreatedAtAction(nameof(GetProductById), new { id = product!.Id }, product);
    //}

    //[HttpPut("{id}")]
    //public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDto dto)
    //{
    //    var product = await _productService.UpdateProductAsync(id, dto);

    //    if (product == null)
    //        return NotFound(new { message = "Product not found" });

    //    return Ok(product);
    //}

    //[HttpDelete("{id}")]
    //public async Task<IActionResult> DeleteProduct(int id)
    //{
    //    var result = await _productService.DeleteProductAsync(id);

    //    if (!result)
    //        return NotFound(new { message = "Product not found" });

    //    return NoContent();
    //}

    //[HttpPatch("{id}/stock")]
    //public async Task<IActionResult> UpdateStock(int id, [FromQuery] int quantity)
    //{
    //    var result = await _productService.UpdateStockAsync(id, quantity);

    //    if (!result)
    //        return BadRequest(new { message = "Insufficient stock or product not found" });

    //    return NoContent();
    //}
}
