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
    public class UserController : ControllerBase
    {

        /// <summary>
        /// Armazena os dados de acesso da collection
        /// </summary>
        private readonly IMongoCollection<User> _user;


        /// <summary>
        /// Construtor que recebe como dependencia o obj da classe MongoDbService
        /// </summary>
        /// <param name="mongoDbService">objeto da classe MongoDbService</param>
        public UserController(MongoDbService mongoDbService)
        {
            //obtem a collection "user"
            _user = mongoDbService.GetDatabase.GetCollection<User>("user");

        }

        [HttpGet]
        public async Task<ActionResult<List<User>>> Get()
        {

            try
            {

                var users = await _user.Find(FilterDefinition<User>.Empty).ToListAsync();

                return Ok(users);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost]
        public async Task<ActionResult<User>> Post([FromBody] User user)
        {


            try
            {

                // Insere o novo produto na coleção
                await _user.InsertOneAsync(user);

                // Retorna o produto inserido com status 201 Created
                return CreatedAtAction(nameof(Get), new { id = user.Id }, user);
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
                var filter = Builders<User>.Filter.Eq(p => p.Id, id);
                var result = await _user.DeleteOneAsync(filter);

                if (result.DeletedCount == 0)
                {
                    return NotFound("Usuário näo encontrado");
                }

                // Retorna um status 204 No Content em caso de sucesso
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error deleting the product: {ex.Message}");
            }

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> BuscarPorId(string id)
        {
            try
            {
                var filter = Builders<User>.Filter.Eq(u => u.Id, id);
                var user = await _user.Find(filter).FirstOrDefaultAsync();

                if (user == null)
                {
                    return NotFound("Usuário não encontrado");
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, [FromBody] User user)
        {
            try
            {
                var filter = Builders<User>.Filter.Eq(u => u.Id, id);
                var update = Builders<User>.Update
                    .Set(u => u.Name, user.Name)
                    .Set(u => u.Email, user.Email)
                    .Set(u => u.Password, user.Password)
                    .Set(u => u.AdditionalAttributes, user.AdditionalAttributes);

                var result = await _user.UpdateOneAsync(filter, update);

                if (result.MatchedCount == 0)
                {
                    return NotFound("Usuário não encontrado");
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
