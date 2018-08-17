using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Messaging;
using System.Messaging.Design;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MSMQInterceptor
{
    public static class MSMQHelper
    {
        static readonly string folderPath = @"C:\Users\devin.PSC\Documents\TestMessageXml\";
        static readonly string queuePath = @"dell47\private$\test2";

        public static void ProcessMessageQueue()
        {
            using (MessageQueue msgQ = new MessageQueue(queuePath))
            {
                if (msgQ != null)
                {
                    Message[] messages = msgQ.GetAllMessages();

                    foreach (var message in messages)
                    {
                        ProcessMessage(message);
                    }
                }                
            }                        
        }

        public static void ProcessMessageQueue(string queuePath)
        {
            using (MessageQueue msgQ = new MessageQueue(queuePath))
            {
                if (msgQ != null)
                {
                    Message[] messages = msgQ.GetAllMessages();

                    foreach (var message in messages)
                    {
                        ProcessMessage(message);
                    }
                }
            }
        }

        public static void ProcessMessage(Message message)
        {
            var fileName = folderPath + message.LookupId.ToString() + ".xml";
            var doc = GetXmlDocFromMsg(message);
            doc.Save(fileName);
        }
                
        private static XmlDocument GetXmlDocFromMsg(Message message)
        {
            XmlDocument doc = new XmlDocument();
            message.Formatter = new ActiveXMessageFormatter();
            using (var sw = new StreamReader(message.BodyStream))
            {
                try
                {
                    var msgBody = sw.ReadToEnd();
                    if (!string.IsNullOrWhiteSpace(msgBody))
                    {
                        doc.LoadXml(msgBody);
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.Write(e.Message);
                }
                return doc;
            }
        }

        private static XmlDocument GetXmlDocFromString(string xmlString)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlString);
            return doc;
        }
    }
}