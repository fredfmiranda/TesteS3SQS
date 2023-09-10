using Amazon.SQS.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AppReadQueueSQS
{

    public interface ISQSManager
    {
        Task<Message> ReceiveMessageFromQueue();
        Task DeleteMessageFromQueue(string receiptHandle);
    }
}
