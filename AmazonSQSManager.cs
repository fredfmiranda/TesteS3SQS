using Amazon.SQS.Model;
using Amazon.SQS;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Amazon.Runtime;

namespace AppReadQueueSQS
{
    public class AmazonSQSManager : ISQSManager
    {
        private static readonly AmazonSQSClient sqsClient = new AmazonSQSClient(
           new BasicAWSCredentials(AppSettings.Get("AWS:AccessKey"), AppSettings.Get("AWS:SecretKey")), Amazon.RegionEndpoint.USEast2
           );

        private string QueueUrlValue = AppSettings.Get("QueueUrlS3");

        public async Task<Message> ReceiveMessageFromQueue()
        {
            var receiveMessageRequest = new ReceiveMessageRequest
            {
                QueueUrl = QueueUrlValue,
                MaxNumberOfMessages = 1,
                WaitTimeSeconds = 0
            };

            var receiveMessageResponse = await sqsClient.ReceiveMessageAsync(receiveMessageRequest);

            return receiveMessageResponse.Messages.Count > 0 ? receiveMessageResponse.Messages[0] : null;
        }

        public async Task DeleteMessageFromQueue(string receiptHandle)
        {
            var deleteMessageRequest = new DeleteMessageRequest
            {
                QueueUrl = QueueUrlValue,
                ReceiptHandle = receiptHandle
            };

            await sqsClient.DeleteMessageAsync(deleteMessageRequest);
        }
    }
}
