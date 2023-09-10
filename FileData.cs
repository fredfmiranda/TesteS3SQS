using System;
using System.Collections.Generic;
using System.Text;

namespace AppReadQueueSQS
{

    public class FileData
    {
        public string key { get; set; }
        public int size { get; set; }
        public DateTime LastModified { get; set; }
    }
}
