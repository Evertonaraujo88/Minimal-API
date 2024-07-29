﻿using MongoDB.Bson.Serialization.Attributes;

namespace Minimal_API.Domains
{
    public class Product
    {
        [BsonId]//define que esta prop é Id do objeto

        //Define o nome do campo no MongoDb como "_id" e o tipo como "ObjectId"
        [BsonElement("_id"), BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string? Id { get; set; }


        [BsonElement("name")]
        public string? Name { get; set; }

        [BsonElement("price")]
        public decimal Price { get; set; }

        //Adiciona um dicionario para atributos adicionais alem dos jah definidos
        public Dictionary<string, string>? AdditionalAttributes { get; set; }

        /// <summary>
        /// Ao ser instancia um obj da classe Product, o atributo AdditionalAttributes jah virah com um novo dicionario e portanto habilitado para adicionar mais atributos
        /// </summary>
        public Product() { 
            
            AdditionalAttributes = new Dictionary<string, string>();

        }
    }
}
