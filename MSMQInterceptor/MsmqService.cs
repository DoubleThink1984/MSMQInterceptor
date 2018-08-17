using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace MSMQInterceptor
{
    class MsmqService
    {
        private string _queueName = "//Default//QueueName";
        private string _tempDirector = "//Default//Directory";

        public MsmqService(string tempDirectory, string queueName)
        {
            this._queueName = queueName;
            this._tempDirector = tempDirectory;
        }

        public MsmqService()
        {

        }

        public void SendMessage(Message msg)
        {
            // Evaluate file body

            // If to large, save file in tmp directory and save path to message body

            //send message
        }

        //public Message GetMessage()
        //{

        //}
    }
}
