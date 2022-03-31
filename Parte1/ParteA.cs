using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using MySql.Data.MySqlClient;

namespace Lab3TP4
{
    class Country
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
                Country country = null;
                bool hayDatos = true;
                bool existePais = true;
                MySqlConnection conexionDb = null;
                request = WebRequest.Create(JSON);
                response = request.GetResponse();
                dataStream = response.GetResponseStream();
                reader = new StreamReader(dataStream);
                responseFromServer = reader.ReadToEnd();
                country = JsonConvert.DeserializeObject<Country>(responseFromServer);
                string servidor, db, usuario, password, cadenaConexion = "";
                try
                {
                    servidor = "localhost";
                    db = "tp2lab4";
                    usuario = "root";
                    password = "mysql";
                    cadenaConexion = "Database=" + db +
                                            "; Data Source=" + servidor +
                                            "; User Id= " + usuario +
                                            "; Password=" + password + "";


                    try
                    {
                        conexionDb = new MySqlConnection(cadenaConexion);
                    }
                    catch (MySqlException ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                    }
                }
                catch
                {
                    hayDatos = false;
                }

                if (hayDatos)
                {

                    conexionDb = new MySqlConnection(cadenaConexion);
                    conexionDb.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT * FROM pais WHERE codigoPais = " + country.codigoPais, conexionDb);
                    MySqlDataReader dbReader = cmd.ExecuteReader();
                    
                    if (dbReader.Read())
                    {
                        existePais = true;
                    }
                    else
                    {
                        existePais = false;
                    }

                    if (existePais)
                    {
                       cmd = new MySqlCommand("UPDATE " +
                                                            "pais " +
                                                            "SET " +
                                                            "nombrePais = '" + country.nombrePais + "', " +
                                                            "capitalPais = '" + country.capitalPais + "', " +
                                                            "region = '" + country.region + "', " +
                                                            "poblacion = " + country.poblacion + ", " +
                                                            "latitud = " + country.latitud + ", " +
                                                            "longitud = " + country.longitud +
                                                            " WHERE codigoPais = " + country.codigoPais, conexionDb);
                        cmd.ExecuteReader();
                    }
                    else
                    {
                        cmd = new MySqlCommand("INSERT INTO " +
                                                            "pais " +
                                                            "(codigoPais, nombrePais, capitalPais, region, poblacion, latitud, longitud) " +
                                                            "VALUES " +
                                                            "(" + country.codigoPais + ", '" + country.nombrePais + "', '" + country.capitalPais + "', '" + country.region + "', " + country.poblacion + ", " + country.latitud + ", " + country.longitud + ")", conexionDb);
                        cmd.ExecuteReader();
                    }
                }
                else
                {
                    continue;
                }
            }
        }
    }
}
