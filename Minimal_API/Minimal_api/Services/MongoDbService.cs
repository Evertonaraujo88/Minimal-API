using MongoDB.Driver;

namespace Minimal_API.Services
{
    public class MongoDbService
    {

        /// <summary>
        /// Armazena a configuração da aplicação
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Armazena uma referência ao MongoDb
        /// </summary>
        private readonly IMongoDatabase _database;

        /// <summary>
        /// Recebe a config da aplicação como parâmetro
        /// </summary>
        /// <param name="configuration">objeto configuration</param>
        public MongoDbService(IConfiguration configuration)
        {
            //Atribui a config recebida em _configuration
            _configuration = configuration;

            //Obtem a string de conexão através do _configuration 
            var connectionString = _configuration.GetConnectionString("DbConnection");

            //cria um ibj MongoUrl que recebe como parametro a string de conexão
            var mongoUrl = MongoUrl.Create(connectionString);

            //cria um client MongoClient oara se conectar ao MongoDb
            var mongoClient = new MongoClient(mongoUrl);

            //Obtem a referência ao banco com o nome especificado na string de conexao
            _database = mongoClient.GetDatabase(mongoUrl.DatabaseName);
                                                //mongodb://localhost:27017/ProductDatabase_Manha
        }


        /// <summary>
        /// Propriedade para acessarr o banco de dados
        /// Retorna a referência ao bd
        /// </summary>
        public IMongoDatabase GetDatabase => _database;
    }
}
