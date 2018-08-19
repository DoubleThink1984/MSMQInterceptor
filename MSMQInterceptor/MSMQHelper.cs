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
            //string filePath;
            message.UnpackageMessageBody();
            //var doc = GetXmlDocFromMsg(message);
            XmlDocument doc = (XmlDocument)message.Body;

            doc.Save(fileName);
        }
                
        private static XmlDocument GetXmlDocFromMsg(Message message)
        {
            XmlDocument doc = new XmlDocument();
            //message.Formatter = new ActiveXMessageFormatter();
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

        public static void PackageMessageBody(this Message message, string filePath)
        {            
            try
            {
                string tmpUri = @"C:\Users\devin.PSC\Documents\TmpDirectory\";
                string fileName = message.Label + ".xml";

                // check if msg body is too large
                if (FileExceedsMaxSize(filePath))
                {
                    // Set body xml with link to temporary file location
                    message.Body = GetTempPathBody(tmpUri);
                    // Save file to temp directory
                    if (File.Exists(filePath))
                    {
                        EnsureDirectoryExists(tmpUri);
                        File.Copy(filePath + fileName, tmpUri);
                    }
                }
                else
                {
                    // If file is not too large, set it as the message body.
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(filePath);
                    message.Body = xmlDoc;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// Checks if given directory exists. If not, creates directory
        /// </summary>
        /// <param name="path">Path of directory</param>
        private static void EnsureDirectoryExists(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            catch (Exception)
            {
                throw;
            }            
        }

        //public static void PackageMessageBody(this Message message, string docPath)
        //{
        //    // get message body as xml
        //    var doc = new XmlDocument();
        //    //if (message.BodyStream != null)
        //    //{
        //    //    doc = GetXmlDocFromMsg(message);
        //    //}
        //    //else if (message.Body != null)
        //    //{
        //    //    doc = (XmlDocument)message.Body;
        //    //}
        //    doc = (XmlDocument)message.Body;
        //    //else {
        //    //    return;
        //    //}
        //    string tmpUri = @"C:\Users\devin.PSC\Documents\TmpDirectory\" + message.Label + ".xml";
        //    // check if msg body is too large

        //    if (doc)
        //    {
        //        // If too large, save in tmp directory.
        //        try
        //        {

        //        doc.Save(tmpUri);
        //        }
        //        catch (Exception e)
        //        {

        //            throw;
        //        }
        //        message.Body = GetTempPathBody(tmpUri);
        //    }
        //    // set message body with new xml with tmp directory path
        //}

        public static void UnpackageMessageBody(this Message message)
        {
            try
            {
                var doc = GetXmlDocFromMsg(message);

                XmlNodeList x = doc.GetElementsByTagName("LargeFileURI");
                if (x.Count > 0)
                {
                    string filePath = x[0].InnerText;
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(filePath);
                    message.Body = xmlDoc;
                }
                else
                {
                    message.Body = doc;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static XmlDocument GetTempPathBody(string docPath)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement elememnt = (XmlElement)doc.AppendChild(doc.CreateElement("LargeFileURI"));
            elememnt.InnerText = docPath;

            return doc;
        }


        private static bool FileExceedsMaxSize(string filePath)
        {
            try
            {
                var docLength = new System.IO.FileInfo(filePath).Length;
                decimal maxFileSize = Decimal.Multiply(3.8m, Decimal.Multiply(1024, 1024));
                return docLength > maxFileSize;
            }
            catch (Exception e)
            {
                //MsmqHelper.LogError(e);
                return true;
            }
        }

        private static bool FileExceedsMaxSize(long length)
        {   
            try
            {
                decimal maxFileSize = Decimal.Multiply(3.8m, Decimal.Multiply(1024, 1024));
                var dcmLength = Convert.ToDecimal(length);
                return dcmLength > maxFileSize;
            }
            catch (Exception)
            {
                //MsmqHelper.LogError(e);
                throw;
            }
        }
    }
}