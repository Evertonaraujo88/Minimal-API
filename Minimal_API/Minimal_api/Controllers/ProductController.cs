using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Minimal_API.Domains;
using Minimal_API.Services;
using MongoDB.Driver;

namespace Minimal_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ProductController : ControllerBase
    {
        /// <summary>
        /// Armazena os dados de acesso da collection
        /// </summary>
        private readonly IMongoCollection<Product> _product;

        /// <summary>
        /// Construtor que recebe como dependencia o obj da classe MongoDbService
        /// </summary>
        /// <param name="mongoDbService">objeto da classe MongoDbService</param>
        public ProductController(MongoDbService mongoDbService)
        {
            //obtem a collection "product"
            _product = mongoDbService.GetDatabase.GetCollection<Product>("product");

        }

        [HttpGet]
        public async Task<ActionResult<List<Product>>> Get()
        {

            try { 
            
                var products = await _product.Find(FilterDefinition<Product>.Empty).ToListAsync();

                return Ok(products);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost]
        public async Task<ActionResult<Product>> Post([FromBody] Product product)
        {
          

            try
            {
              
                // Insere o novo produto na coleção
                await _product.InsertOneAsync(product);

                // Retorna o produto inserido com status 201 Created
                return CreatedAtAction(nameof(Get), new { id = product.Id }, product);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("O id do produto deve ser fornecido");
            }

            try
            {
                // Tenta encontrar o produto pelo id
                var filter = Builders<Product>.Filter.Eq(p => p.Id, id);
                var result = await _product.DeleteOneAsync(filter);

                if (result.DeletedCount == 0)
                {
                    return NotFound("Produto näo encontrado");
                }

                // Retorna um status 204 No Content em caso de sucesso
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error deleting the product: {ex.Message}");
            }

        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(string id, [FromBody] Product updatedProduct)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("O ID do produto deve ser fornecido");
            }

            if (updatedProduct == null)
            {
                return BadRequest("Os dados do produto devem ser fornecidos");
            }

            try
            {
                // Verifica se o produto com o id fornecido existe
                var filter = Builders<Product>.Filter.Eq(p => p.Id, id);
                var existingProduct = await _product.Find(filter).FirstOrDefaultAsync();

                if (existingProduct == null)
                {
                    return NotFound("O produto não existe!");
                }

                // Atualiza o produto com as novas informações
                var update = Builders<Product>.Update
                    .Set(p => p.Name, updatedProduct.Name)
                    .Set(p => p.Price, updatedProduct.Price);
                // Adicione outras atualizações conforme necessário

                var result = await _product.UpdateOneAsync(filter, update);

                if (result.ModifiedCount == 0)
                {
                    return StatusCode(StatusCodes.Status304NotModified, "Os dados do produto não foram atualizados");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }





    }
}
