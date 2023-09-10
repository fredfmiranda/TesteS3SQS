using System;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Amazon.Runtime;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace AppUploadS3Bucket
{
    class Program
    {
        private static string bucketName = string.Empty;
        private static string directoryPath = string.Empty;
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.USEast2; 
        private static IAmazonS3 s3Client;

        public static void Main()
        {


           var configuration = new ConfigurationBuilder()
          .SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile("appSettings.json", optional: true, reloadOnChange: true)
          .Build();

            string awsAccessKey = configuration["AWS:AccessKey"];
            string awsSecretKey = configuration["AWS:SecretKey"];
            bucketName = configuration["AWS:BucketName"];
            directoryPath = configuration["DirectoryPath"]; 
            var credentials = new BasicAWSCredentials(awsAccessKey, awsSecretKey);
            s3Client = new AmazonS3Client(credentials, bucketRegion);
            UploadFilesAsync().Wait();
        }

        private static async Task UploadFilesAsync()
        {
            try
            {
                var fileTransferUtility = new TransferUtility(s3Client);

                // Obtém todos os arquivos no diretório especificado
                var files = Directory.GetFiles(directoryPath);

                foreach (var filePath in files)
                {
                   await fileTransferUtility.UploadAsync(filePath, bucketName);
                }
                Console.WriteLine("Arquivos atualizados no Bucket S3 com sucesso!");
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine($"Erro: {e.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Erro desconhecido: {e.Message}");
            }
        }
    }
}

