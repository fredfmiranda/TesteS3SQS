using SQSConsumer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AppReadQueueSQS
{
    public interface IDatabaseManager
    {
        Task UpdateOrInsertIntoDatabase(FileData fileData);
    }
}
