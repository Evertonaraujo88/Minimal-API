using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Minimal_API.Domains;
using Minimal_API.Services;
using Minimal_API.ViewModel;
using MongoDB.Driver;

namespace Minimal_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class OrderController : ControllerBase
    {

        /// <summary>
        /// Armazena os dados de acesso da collection
        /// </summary>
        private readonly IMongoCollection<Order> _order;
        private readonly IMongoCollection<Client> _client;
        private readonly IMongoCollection<Product> _product;


        /// <summary>
        /// Construtor que recebe como dependencia o obj da classe MongoDbService
        /// </summary>
        /// <param name="mongoDbService">objeto da classe MongoDbService</param>
        public OrderController(MongoDbService mongoDbService)
        {
            //obtem a collection "order"
            _order = mongoDbService.GetDatabase.GetCollection<Order>("order");
            _client = mongoDbService.GetDatabase.GetCollection<Client>("client");
            _product = mongoDbService.GetDatabase.GetCollection<Product>("product");

        }


        [HttpPost]
        public async Task<ActionResult<Order>> Create(OrderViewModel orderViewModel)
        {

            try { 
                
                Order order= new Order();

                order.Id = orderViewModel.Id;
                order.Date= orderViewModel.Date;
                order.Status= orderViewModel.Status;
                order.ProductId = orderViewModel.ProductId;
                order.ClientId = orderViewModel.ClientId;
            
                var client = await _client.Find(x=>x.Id == order.ClientId).FirstOrDefaultAsync();

                if (client == null)
                {
                    return NotFound(); 
                }

                order.Client = client;

                await _order!.InsertOneAsync(order);

                return StatusCode(201, order);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }





    }
}
