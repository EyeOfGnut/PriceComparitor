using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Price_Comparitor
{
    public class CSVWriter
    {
        private Queue<String> csvQueue;
        private CancellationToken cToken;
        private Task fileWriter = null;
        private string csvFile;

        public CSVWriter(CancellationToken ct, string file)
        {
            cToken = ct;
            csvQueue = new Queue<string>();
            csvFile = file;
        }

        public TaskStatus Status()
        {
            return fileWriter.Status;
        }

        public void Enqueue(string str)
        {
            lock (csvQueue)
            {
                csvQueue.Enqueue(str);
                if (fileWriter == null)
                {
                    fileWriter = new Task(() =>
                    {
                        try { saveCsv(); }
                        catch (AggregateException ex)
                        {
                            Console.WriteLine("Cancelled write: " + ex.Message);
                        }
                    }, cToken, TaskCreationOptions.None);

                    fileWriter.Start();
                }
                else
                {
                    fileWriter = fileWriter.ContinueWith(x =>
                    {
                        try { saveCsv(); }
                        catch (AggregateException ex)
                        {
                            Console.WriteLine("Cancelled write: " + ex.Message);
                        }
                    }, cToken, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
                }
            }
        }

        private void saveCsv()
        {
            StreamWriter sWriter = null;
            try
            {
                sWriter = new StreamWriter(csvFile, true);
                sWriter.WriteLine(csvQueue.Dequeue());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                sWriter.Close();
            }
        }
    }
}
