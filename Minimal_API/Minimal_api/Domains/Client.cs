using MongoDB.Bson.Serialization.Attributes;

namespace Minimal_API.Domains
{
    public class Client
    {

        [BsonId]//define que esta prop é Id do objeto

        //Define o nome do campo no MongoDb como "_id" e o tipo como "ObjectId"
        [BsonElement("_id"), BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("userId")]
        public User? UserId { get; set; }

        [BsonElement("cpf")]
        public string? Cpf { get; set; }

        [BsonElement("phone")]
        public int? Phone { get; set; }

        [BsonElement("adress")]
        public string? Adress { get; set; }


        //Adiciona um dicionario para atributos adicionais alem dos jah definidos
        public Dictionary<string, string>? AdditionalAttributes { get; set; }

        /// <summary>
        /// Ao ser instancia um obj da classe Client, o atributo AdditionalAttributes jah virah com um novo dicionario e portanto habilitado para adicionar mais atributos
        /// </summary>
        public Client()
        {

            AdditionalAttributes = new Dictionary<string, string>();

        }

    }
}
