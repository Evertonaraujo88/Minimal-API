using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Minimal_API.Domains;
using Minimal_API.Services;
using MongoDB.Driver;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Minimal_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ClientController : ControllerBase
    {
     
        private readonly IMongoCollection<Client> _client;

        private readonly IMongoCollection<User> _user;

        // Construtor que inicializa as coleções MongoDB através do MongoDbService
        public ClientController(MongoDbService mongoDbService)
        {
            _client = mongoDbService.GetDatabase.GetCollection<Client>("client");
            _user = mongoDbService.GetDatabase.GetCollection<User>("user");
        }

        // Método para criar um novo Client
        [HttpPost]
        public async Task<IActionResult> Create(Client newClient)
        {
            if (newClient.UserId != null)
            {
                // Verifica se o User já existe no banco de dados
                var existingUser = await _user.Find(u => u.Id == newClient.UserId.Id).FirstOrDefaultAsync();
                if (existingUser == null)
                {
                    // Se o User não existir, insere o novo User
                    await _user.InsertOneAsync(newClient.UserId);
                }
                else
                {
                    // Se o User existir, associa o User existente ao novo Client
                    newClient.UserId = existingUser;
                }
            }
            // Insere o Client na coleção "client"
            await _client.InsertOneAsync(newClient);

            // Retorna o Client criado com o status 201 Created
            return CreatedAtAction(nameof(GetById), new { id = newClient.Id }, newClient);
        }

        // Método para obter todos os Clients
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // Busca todos os Clients na coleção "client"
            var clients = await _client.Find(client => true).ToListAsync();

            // Itera sobre cada Client e busca os detalhes completos do User correspondente
            foreach (var client in clients)
            {
                if (client.UserId != null)
                {
                    var user = await _user.Find(u => u.Id == client.UserId.Id).FirstOrDefaultAsync();
                    client.UserId = user;
                }
            }

            // Retorna a lista de Clients com os Users preenchidos
            return Ok(clients);
        }

        // Método para obter um Client específico pelo ID
        [HttpGet("{id:length(24)}", Name = "GetClient")]
        public async Task<IActionResult> GetById(string id)
        {
            // Busca um Client pelo ID
            var client = await _client.Find<Client>(client => client.Id == id).FirstOrDefaultAsync();
            if (client == null)
            {
                return NotFound();
            }

            // Busca os detalhes completos do User correspondente, se houver
            if (client.UserId != null)
            {
                var user = await _user.Find(u => u.Id == client.UserId.Id).FirstOrDefaultAsync();
                client.UserId = user;
            }

            // Retorna o Client com o User preenchido
            return Ok(client);
        }

        // Método para atualizar um Client
        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, Client updatedClient)
        {
            // Busca o Client existente pelo ID
            var client = await _client.Find<Client>(c => c.Id == id).FirstOrDefaultAsync();
            if (client == null)
            {
                return NotFound();
            }

            // Se o User embutido no Client for atualizado, substitui o User na coleção "user"
            if (updatedClient.UserId != null)
            {
                await _user.ReplaceOneAsync(u => u.Id == updatedClient.UserId.Id, updatedClient.UserId);
            }

            // Substitui o Client na coleção "client"
            await _client.ReplaceOneAsync(c => c.Id == id, updatedClient);
            return NoContent();
        }

        // Método para deletar um Client
        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            // Busca o Client pelo ID
            var client = await _client.Find<Client>(c => c.Id == id).FirstOrDefaultAsync();
            if (client == null)
            {
                return NotFound();
            }

            // Se o Client tiver um User embutido, remove o User da coleção "user"
            if (client.UserId != null)
            {
                await _user.DeleteOneAsync(u => u.Id == client.UserId.Id);
            }

            // Remove o Client da coleção "client"
            await _client.DeleteOneAsync(c => c.Id == id);
            return NoContent();
        }
    }
}
