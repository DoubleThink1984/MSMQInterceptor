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
        static readonly string queuePath = @"dell47\private$\test2";

        static void Main(string[] args)
        {
            DirectoryWatcher.Run();
        }

        public static void SendMessages(string filePath)
        {
            try
            {
                bool isFinishedTransfering = false;
                while (!isFinishedTransfering)
                {
                    isFinishedTransfering = IsFileTransferComplete(filePath);
                }

                string fileName = Path.GetFileNameWithoutExtension(filePath);
                XmlDocument xmlDoc= new XmlDocument();
                xmlDoc.Load(filePath);
                var msg = new Message()
                {
                    Formatter = new XmlMessageFormatter(new String[] { "System.String,mscorlib" }),
                    Label = fileName,
                    Body = xmlDoc
                };
                //msg.Body = File.WriteAllText(filePath)
                msg.PackageMessageBody();

                using (MessageQueue messageQueue = GetQueue("Queue_TEST"))
                {
                    //messageQueue.SetPermissions("Everyone", MessageQueueAccessRights.FullControl);
                    // Send Message to queue
                    messageQueue.Send(msg);
                }
            }
            catch (Exception e)
            {
                //MsmqHelper.LogError(e);
                //throw;
            }
        }

        private static MessageQueue GetQueue(string queueName)
        {
            MessageQueue messageQueue = null;
            //string fullQueueName = ConfigHelper.GetAppSettingOrEmptyString(queueName).ToFullMsmqQueuePath();
            try
            {
                if (MessageQueue.Exists(queuePath))
                {
                    messageQueue = new MessageQueue(queuePath);
                    messageQueue.Label = "Testing Queue";
                }
                else
                {
                    // Create the Queue
                    MessageQueue.Create(queuePath);
                    messageQueue = new MessageQueue(queuePath);
                    messageQueue.Label = "Newly Created Queue";
                }
            }
            catch (Exception e)
            {
                //MsmqHelper.LogError(e);
            }
            return messageQueue;
        }

        private static bool IsFileTransferComplete(string filePath)
        {
            try
            {
                // check file length 
                var initialLength = new System.IO.FileInfo(filePath).Length;
                // Wait 1 second
                System.Threading.Thread.Sleep(1000);
                // check current length of file. If initialLength and second length are equal, file is finished transfereing
                var secondLengthMeasurement = new System.IO.FileInfo(filePath).Length;

                return initialLength == secondLengthMeasurement;
            }
            catch (Exception e)
            {
                //MsmqHelper.LogError(e);
                return true;
            }
        }

        private static bool FileExceedsMaxSize(string filePath)
        {
            decimal maxFileSize = Decimal.Multiply(3.8m, Decimal.Multiply(1024, 1024));

            try
            {
                var fileLength = new System.IO.FileInfo(filePath).Length;

                return fileLength > maxFileSize;
            }
            catch (Exception e)
            {
                //MsmqHelper.LogError(e);
                return true;
            }
        }
    }
}
