using Amazon.Runtime;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Amazon.S3;
using Amazon.SQS;
using MySqlX.XDevAPI;
using Newtonsoft.Json;
using SQSConsumer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AppReadQueueSQS
{
    internal class App
    {
        private readonly ISQSManager _sqsManager;
        private readonly IDatabaseManager _databaseManager;




        public App(ISQSManager sqsManager, IDatabaseManager databaseManager)
        {
            _sqsManager = sqsManager;
            _databaseManager = databaseManager;
        }

        public async Task Run()
        {
            AmazonS3Client s3Client = new AmazonS3Client("AKIARA7BJEBXM25CC2YM", "5Siqls246BdXaxpcB4DG30ajG8FKpa3I7j0tpZbo", Amazon.RegionEndpoint.USEast2);

            while (true)
            {
                try
                {
                    var message = await _sqsManager.ReceiveMessageFromQueue();
                    if (message != null)
                    {
                        Root deserializedObject = JsonConvert.DeserializeObject<Root>(message.Body);
                        var s3Event = Amazon.S3.Util.S3EventNotification.ParseJson(message.Body);
                        var s3Entity = s3Event.Records?[0].S3;

                        if (s3Event != null)
                        {
                            var request = new GetObjectMetadataRequest
                            {
                                BucketName = s3Entity.Bucket.Name,
                                Key = s3Entity.Object.Key
                            };

                            GetObjectMetadataResponse response = await s3Client.GetObjectMetadataAsync(request);

                            DateTime lastModified = response.LastModified;
                            // Agora tenho a data e hora da última modificação do arquivo
                            deserializedObject.Records[0].s3.@object.lastModified = lastModified;
                        }
                        FileData fileData = new FileData();
                        fileData.key = deserializedObject.Records[0].s3.@object.key;
                        fileData.size = deserializedObject.Records[0].s3.@object.size;
                        fileData.LastModified = deserializedObject.Records[0].s3.@object.lastModified;
                        await _databaseManager.UpdateOrInsertIntoDatabase(fileData);
                        await _sqsManager.DeleteMessageFromQueue(message.ReceiptHandle);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }

                await Task.Delay(1000);
            }
        }



        public class UserIdentity
        {
            public string principalId { get; set; }
        }

        public class RequestParameters
        {
            public string sourceIPAddress { get; set; }
        }

        public class ResponseElements
        {
            public string x_amz_request_id { get; set; }
            public string x_amz_id_2 { get; set; }
        }

        public class OwnerIdentity
        {
            public string principalId { get; set; }
        }

        public class Bucket
        {
            public string name { get; set; }
            public OwnerIdentity ownerIdentity { get; set; }
            public string arn { get; set; }
        }

        public class S3Object
        {
            public string key { get; set; }
            public int size { get; set; }
            public string eTag { get; set; }
            public string sequencer { get; set; }

            public DateTime lastModified { get; set; }
        }

        public class S3
        {
            public string s3SchemaVersion { get; set; }
            public string configurationId { get; set; }
            public Bucket bucket { get; set; }
            public S3Object @object { get; set; }
        }

        public class Record
        {
            public string eventVersion { get; set; }
            public string eventSource { get; set; }
            public string awsRegion { get; set; }
            public DateTime eventTime { get; set; }
            public string eventName { get; set; }
            public UserIdentity userIdentity { get; set; }
            public RequestParameters requestParameters { get; set; }
            public ResponseElements responseElements { get; set; }
            public S3 s3 { get; set; }
        }

        public class Root
        {
            public List<Record> Records { get; set; }
        }

    }
}
