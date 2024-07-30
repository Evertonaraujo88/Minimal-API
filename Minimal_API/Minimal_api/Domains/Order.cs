using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Minimal_API.Domains
{
    public class Order
    {

        [BsonId]//define que esta prop é Id do objeto

        //Define o nome do campo no MongoDb como "_id" e o tipo como "ObjectId"
        [BsonElement("_id"), BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("date")]
        public DateTime Date { get; set; }

        [BsonElement("status")]
        public string? Status { get; set; }



        //referência aos produtos do pedido

        //referencia para que eu consiga cadasrtar um pedido com os produtos
        [BsonElement("productId")]
        [JsonIgnore] //ignora a lista na hora do get, pois, na lista de produtos ja contem os id's
        public List<string> ProductId { get; set; }

        //referecia para que quando eu liste os pedidos, venham os dados de cada produto(lista)
        public List<Product>? Products { get; set; }




        //referência aos cliente que fizeram os pedidos

        //referencia para que eu consiga cadastrar um pedido com o cliente
        [BsonElement("clientId")]
        [JsonIgnore]
        public string? ClientId { get; set; }

        //referencia para que eu liste os pedidos, venham os dados do cliente
        public Client? Client { get; set; }




        //Adiciona um dicionario para atributos adicionais alem dos jah definidos
        public Dictionary<string, string>? AdditionalAttributes { get; set; }

        /// <summary>
        /// Ao ser instancia um obj da classe Order, o atributo AdditionalAttributes jah virah com um novo dicionario e portanto habilitado para adicionar mais atributos
        /// </summary>
        public Order()
        {

            AdditionalAttributes = new Dictionary<string, string>();

        }

    }
}
