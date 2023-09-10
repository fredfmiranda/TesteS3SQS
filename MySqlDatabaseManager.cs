using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using SQSConsumer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AppReadQueueSQS
{
    public class MySqlDatabaseManager : IDatabaseManager
    {
        public async Task UpdateOrInsertIntoDatabase(FileData fileData)
        {
            string connectionMySQL = AppSettings.Get("connectionString");
            using (var connection = new MySqlConnection(connectionMySQL))
            {
                await connection.OpenAsync();

                var query = "SELECT lastmodified FROM Files WHERE filename = @filename";
                var cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@filename", fileData.key);

                var lastModifiedInDb = await cmd.ExecuteScalarAsync() as DateTime?;

                if (lastModifiedInDb != null && lastModifiedInDb >= fileData.LastModified)
                {
                    Console.WriteLine("Registro já existente tendo um lastmodified mais recente ou igual, não será atualizado.");
                    LogNonUpdatableRecord(fileData);  // Chama o método para logar o registro e retorna ao processamento principal
                    return;
                }

                query = @"
                INSERT INTO Files (filename, filesize, lastmodified)
                VALUES (@filename, @filesize, @lastmodified)
                ON DUPLICATE KEY UPDATE filesize = @filesize, lastmodified = @lastmodified";

                cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@filename", fileData.key);
                cmd.Parameters.AddWithValue("@filesize", fileData.size);
                cmd.Parameters.AddWithValue("@lastmodified", fileData.LastModified);

                await cmd.ExecuteNonQueryAsync();
            }
        }

        private void LogNonUpdatableRecord(FileData fileData)
        {
            string logMessage = $"[{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}] Este registro não foi atualizado. Filename: {fileData.key}, Filesize: {fileData.size}, Last Modified: {fileData.LastModified.ToString("dd/MM/yyyy")}\n";
            File.AppendAllText("RegistrosNaoAtualizados.log", logMessage);  // Adiciona a mensagem ao arquivo de log
        }
    }
}
