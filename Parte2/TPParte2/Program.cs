using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Libmongocrypt;
using MongoDB.Driver.Core;
using MongoDB.Driver.Encryption;
using MongoDB.Driver.GeoJsonObjectModel;
using MongoDB.Driver.Linq;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace TPParte2
{
    class Pais
    {
        public string nombrePais { get; set; }
        public string capitalPais { get; set; }
        public string region { get; set; }
        public long poblacion { get; set; }
        public double latitud { get; set; }
        public double longitud { get; set; }
        public int codigoPais { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            for (int i = 1; i <= 300; i++)
            {
                string JSON = $"https://restcountries.eu/rest/v2/callingcode/{i}";
                WebRequest request;
                WebResponse response;
                Stream dataStream;
                StreamReader reader;
                string responseFromServer;
                Pais pais = null;
                bool hayDatos = true;
                request = WebRequest.Create(JSON);
                response = request.GetResponse();
                dataStream = response.GetResponseStream();
                reader = new StreamReader(dataStream);
                responseFromServer = reader.ReadToEnd();
                pais = Newtonsoft.Json.JsonConvert.DeserializeObject<Pais>(responseFromServer);
                
                hayDatos = pais.nombrePais != null;

                if (hayDatos)
                {
                    MongoClient mongoClient = CrearConexion();

                    if (mongoClient != null)
                    {
                        Console.WriteLine("Conexion Exitosa");
                        List<string> databases = mongoClient.ListDatabaseNames().ToList();

                        if (databases.Contains("paises_db"))
                        {
                            Console.WriteLine("Base de datos existente");
                            List<string> colecciones = mongoClient.GetDatabase("paises_db").ListCollectionNames().ToList();

                            if (colecciones.Contains("paises"))
                            {
                                Console.WriteLine("Coleccion existente");
                                IMongoCollection<Pais> coleccion = mongoClient.GetDatabase("paises_db").GetCollection<Pais>("paises");
                                var filter = Builders<Pais>.Filter.Eq("codigoPais", pais.codigoPais);
                                var resultado = coleccion.Find(filter).ToList();

                                if (resultado.Count == 0)
                                {
                                    Console.WriteLine("No existe el pais");
                                    coleccion.InsertOne(pais);
                                }
                                else
                                {
                                    Console.WriteLine("El pais ya existe");
                                }
                            }
                            else
                            {
                                Console.WriteLine("No existe la coleccion");
                                IMongoCollection<Pais> coleccion = mongoClient.GetDatabase("TPParte2").GetCollection<Pais>("Paises");
                                coleccion.InsertOne(pais);
                            }
                        }
                        else
                        {
                            Console.WriteLine("No existe la base de datos");
                            mongoClient.GetDatabase("paises_db").CreateCollection("paises");
                            IMongoCollection<Pais> coleccion = mongoClient.GetDatabase("paises_db").GetCollection<Pais>("paises");
                            coleccion.InsertOne(pais);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error: Conexión no establecida");
                    }
                }
                else
                {
                    continue;
                }            
            }           
        }

        private static MongoClient CrearConexion()
        {
            MongoClient mongo = null;
            try
            {
                mongo = new MongoClient("mongodb://localhost:27017");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return mongo;
        }
    }
}
