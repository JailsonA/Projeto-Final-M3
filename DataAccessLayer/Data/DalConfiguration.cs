using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Data
{
    public static class DalConfiguration
    {
        private static string _connectionString;

        public static string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_connectionString))
                {
                    // Obtenha o diretório base do projeto (não do assembly)
                    var projectDirectory = Directory.GetCurrentDirectory();

                    // Combine o diretório base com o caminho para o arquivo dalsettings.json
                    var jsonFilePath = Path.Combine(projectDirectory, "dalsettings.json");

                    // Configure a ConnectionString from the absolute path to the JSON file
                    var builder = new ConfigurationBuilder()
                        .AddJsonFile(jsonFilePath, optional: true, reloadOnChange: true);

                    var configuration = builder.Build();
                    _connectionString = configuration.GetConnectionString("DefaultConnection");
                }

                return _connectionString;
            }
        }
    }
}