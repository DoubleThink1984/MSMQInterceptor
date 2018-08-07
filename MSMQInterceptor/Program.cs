using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MSMQInterceptor
{
    class Program
    {
        static readonly string quePath = @"dell47\private$\testqueue";

        static void Main(string[] args)
        {
            SendMessages();
            MSMQHelper.ProcessMessageQueue(quePath);
        }

        static void SendMessages()
        {            
            MessageQueue messageQueue = null;
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(Directory.GetCurrentDirectory() + @"\XMLFiles\TestFile.xml");
            if (MessageQueue.Exists(quePath))
            {
                messageQueue = new MessageQueue(quePath);
                messageQueue.Label = "Testing Queue";
            }
            else
            {
                // Create the Queue
                MessageQueue.Create(quePath);
                messageQueue = new MessageQueue(quePath);
                messageQueue.Label = "Newly Created Queue";
            }
            System.Messaging.Message msg = new System.Messaging.Message
            {
                Body = xmlDoc,
                Formatter = new XmlMessageFormatter(new String[] { "System.String,mscorlib" })
            };

            messageQueue.Send(msg);
        }
    }
}
