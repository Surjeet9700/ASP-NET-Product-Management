using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAspNetCoreApp.Data;
using MyAspNetCoreApp.Models;

namespace MyAspNetCoreApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(ApplicationDbContext context, ILogger<ProductsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Products
        /// <summary>
        /// Get all products
        /// </summary>
        /// <returns>List of all products</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            try
            {
                _logger.LogInformation("Fetching all products");
                var products = await _context.Products.ToListAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching products");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/Products/5
        /// <summary>
        /// Get a specific product by ID
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Product details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            try
            {
                _logger.LogInformation("Fetching product with ID: {ProductId}", id);
                var product = await _context.Products.FindAsync(id);

                if (product == null)
                {
                    _logger.LogWarning("Product with ID {ProductId} not found", id);
                    return NotFound(new { message = $"Product with ID {id} not found" });
                }

                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching product with ID: {ProductId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/Products
        /// <summary>
        /// Create a new product
        /// </summary>
        /// <param name="product">Product object</param>
        /// <returns>Created product</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Check if SKU already exists
                var existingSku = await _context.Products.AnyAsync(p => p.SKU == product.SKU);
                if (existingSku)
                {
                    return BadRequest(new { message = "Product with this SKU already exists" });
                }

                product.CreatedDate = DateTime.UtcNow;
                product.UpdatedDate = null;

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Product created with ID: {ProductId}", product.Id);

                return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT: api/Products/5
        /// <summary>
        /// Update an existing product
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="product">Updated product object</param>
        /// <returns>No content on success</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product product)
        {
            try
            {
                if (id != product.Id)
                {
                    return BadRequest(new { message = "Product ID mismatch" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingProduct = await _context.Products.FindAsync(id);
                if (existingProduct == null)
                {
                    _logger.LogWarning("Product with ID {ProductId} not found for update", id);
                    return NotFound(new { message = $"Product with ID {id} not found" });
                }

                // Check if SKU already exists for another product
                var duplicateSku = await _context.Products
                    .AnyAsync(p => p.SKU == product.SKU && p.Id != id);
                if (duplicateSku)
                {
                    return BadRequest(new { message = "Product with this SKU already exists" });
                }

                // Update properties
                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;
                existingProduct.Price = product.Price;
                existingProduct.Category = product.Category;
                existingProduct.Brand = product.Brand;
                existingProduct.StockQuantity = product.StockQuantity;
                existingProduct.SKU = product.SKU;
                existingProduct.IsActive = product.IsActive;
                existingProduct.ImageUrl = product.ImageUrl;
                existingProduct.UpdatedDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Product with ID {ProductId} updated successfully", id);

                return NoContent();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    _logger.LogError(ex, "Concurrency error updating product with ID: {ProductId}", id);
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product with ID: {ProductId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE: api/Products/5
        /// <summary>
        /// Delete a product
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>No content on success</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                _logger.LogInformation("Attempting to delete product with ID: {ProductId}", id);
                var product = await _context.Products.FindAsync(id);

                if (product == null)
                {
                    _logger.LogWarning("Product with ID {ProductId} not found for deletion", id);
                    return NotFound(new { message = $"Product with ID {id} not found" });
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Product with ID {ProductId} deleted successfully", id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product with ID: {ProductId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
