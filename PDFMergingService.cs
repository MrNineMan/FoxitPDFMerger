using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
//Add the following name spaces from Foxit library 
using foxit.pdf; 
using foxit.common;
using System.IO; //For file watcher class
using System.Timers; //For Timer class

namespace FoxitPDFMerger
{
    public partial class PDFMergingService : ServiceBase
    {
        FileSystemWatcher watcher; //create new file system watcher to detect changes to the directory
        private static String path = "C:\\downloads"; //hard-coded path   

        Timer timer = new Timer(); //Create a new Timer for service polling

        public PDFMergingService()
        {
            try
            {



                watcher = new FileSystemWatcher(); //initialize  file system watcher
                watcher.IncludeSubdirectories = false; //does not watch for changes in path subfolders
                watcher.Path = path; //Set path
                watcher.NotifyFilter = NotifyFilters.Attributes | 
                NotifyFilters.CreationTime |
                NotifyFilters.DirectoryName |
                NotifyFilters.FileName |
                NotifyFilters.LastAccess |
                NotifyFilters.LastWrite |
                NotifyFilters.Security |
                NotifyFilters.Size;
                //Set types of file attributes to listen for. You can optimize this list accordingly.
                watcher.Filter = "*.pdf"; //sets filters to pdfs

                //Assigns event handlers file changed.

                watcher.Changed += new FileSystemEventHandler(OnChanged);
                
                //Allow the watcher to raise events
                watcher.EnableRaisingEvents = true;
            }
            catch (IOException e)
            {
                Console.WriteLine("A Exception Occurred :" + e);
            }
            catch (Exception oe)
            {
                Console.WriteLine("An Exception Occurred :" + oe);
            }

           
            InitializeComponent();
        }
        public void OnChanged(object source, FileSystemEventArgs e)
        {



            try
            {

                //Initializes Foxit SDK Library
                //You can find “sn” paramater in the “gsdk_sn.txt” (the string after “SN=”).
                //You can find “key” parameter in the “gsdk_key.txt” (the string after “Sign=”). 
                string sn = "hrBJizJACria7/Su9tny8LDEswtXOF72Ir/48Ufy8EK6CPvyFbyH1w==";
                string key = "8f0YFcONvRkN+ldwlpAFW0NF+Q/jOhOBvj26/zxAesuepbv1tfFx4BIOZ+JmjBCKyf2Eg0z6HxMFo1CEybQs/zwgZ/mEdwb3fmyk9wxo4N6uU31YKjEO/Lqm9RsgOPTnD98IK4XZlOPhkApEaRP+1B4P4cBLglhaHTjAH18NSeTe+9AHUUsNvrQ8IPlXrkaQEOo1saDGaBKxB2mKyhc/+Ho30bGUcmScR0xnydcXG513bDLW8zwj3tXOIcVpRA5AQf0/i4fifWebLsApc5lGjZOqMBI0BIGT1B4EROrJBX4GOjK5lXdNhG5iEQYDlIldKeYGojrMmQ6vFPQHW+3rAhxO4hMA6/Iidf2CPzGibgvU0mezuol1xLoUGEpN7i3AO7izaB5sM3T95Em9Q/NcOoZO1v6cxJvWkxSLD3nXXKawgQ2G9Q3BaD7gz0fQhQQUFYwIZg14t7c0AtbY0h5wYAvNySDzMR126UtlrhKH7jxuB7Ie9gs2iuTVOZh2pvofPLGvWS/07hfEmbHjCDsi2aIvI93hM8cd95FZZLyv84pDbVw83bXdQsTojHPyfKbbYXFyc88QlaTelyl2VsYfrFgc6nM6S7oadCrMk3quZOcosJcesmgWgdOiznY/JXq/mX3HoaFqHqkmPs91GK07Vh/h7QJw9bDpCuwE6hhx628DaKmbpOtpn+5l0Nypy926YsZzq9Yywm4kUqNg1UmJqh1CRbcgAdmKNFrHOf7K8dayX9I6AzeCTEDpy7Y+53TdC4xGv2Q0Iy7xa6s7XOSrrLV6vIn0Vybo+z9ohMPgI1cbCYTU4/J256ivRNJmHN7SSzFpt4xlmw8dTltuh+Vd5OkE5WgUN2ce6y0RHH1hOZL5kUQZyAaWdgTVnTKiCat8Uio0lhLWCmz2osdqr/l1+D5BtkPvpoVCo5tlNpnBu3qyXLO72ZsW+UHrJsCsRqms7NrKMzjNSET6V3d7f1vSsgh9w6dTu7E8Vk2c3hjf6TVKOaoJJP14vKt/ZXDoaPp+MkA+b9r8TkmvYe5KH7Px2FryVKQ4sK4l8rbghuhJs0FYr2UKKEfD5MZin02xiCY096FpDDiJ5VOEw6kmjhkvrzNuXJV1NnFVNbEfunzjtuETIwUx6ChXF6N/KhwnUAgGHjn24pJTITtuvOacQceEaCjEDdtF40SjoqLFCXZoN4d5KCpL84muYKBJ41FnMJL5KLJ6QNLX1Gpwt/eYExjvW3PuqdviUWBnUlRR3WJlxFnJDZwrzMEoX9dsk9GoyLH8f5BISUjg2XuyUZVReWK/fV6lXTsnrAjYTJElVXAMlSWVd4qJnhr2BDg=";
                ErrorCode error_code = Library.Initialize(sn, key);
                //If the library fails to initialize, this error and exit the application
                if (error_code != ErrorCode.e_ErrSuccess)
                    return;

                string[] filePaths = Directory.GetFiles(path, "*.pdf",
                                             SearchOption.TopDirectoryOnly);
                if (filePaths.Length > 1)
                {
                    CombineDocumentInfoArray info_array = new CombineDocumentInfoArray();
                    WriteToFile("There are multiple PDFs in this folder");
                    String savepath = path + "\\combined files\\";
                    if (!Directory.Exists(savepath))
                    {
                        Directory.CreateDirectory(savepath);
                    }

                    int option = (int)(Combination.CombineDocsOptions.e_CombineDocsOptionBookmark | Combination.CombineDocsOptions.e_CombineDocsOptionAcroformRename |
                    Combination.CombineDocsOptions.e_CombineDocsOptionStructrueTree | Combination.CombineDocsOptions.e_CombineDocsOptionOutputIntents |
                    Combination.CombineDocsOptions.e_CombineDocsOptionOCProperties | Combination.CombineDocsOptions.e_CombineDocsOptionMarkInfos |
                    Combination.CombineDocsOptions.e_CombineDocsOptionPageLabels | Combination.CombineDocsOptions.e_CombineDocsOptionNames |
                    Combination.CombineDocsOptions.e_CombineDocsOptionObjectStream | Combination.CombineDocsOptions.e_CombineDocsOptionDuplicateStream);

                    for (int i = 0; i < filePaths.Length; i++)
                    {
                        info_array.Add(new CombineDocumentInfo(filePaths[i], ""));
                        WriteToFile(filePaths[i] + " added to array");
                    }

                    Progressive progress = Combination.StartCombineDocuments(savepath + "\\combined.pdf", info_array, option, null);
                    Progressive.State progress_state = Progressive.State.e_ToBeContinued;
                    while (Progressive.State.e_ToBeContinued == progress_state)
                    {
                        progress_state = progress.Continue();
                        WriteToFile("Combinining PDF Files");
                    }
                }

                WriteToFile("File Combination Process Completed at:" + System.DateTime.Now);
            }
            catch (foxit.PDFException ex)
            {
                WriteToFile(ex.StackTrace);

            }
        }
        public void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + System.DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                // Create a file to write to.   
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }

        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            WriteToFile("Service is recall at " + System.DateTime.Now); //called every five seconds
        }
        protected override void OnStart(string[] args)
        {
            WriteToFile("Service is started at " + System.DateTime.Now); //Records when the service was started
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime); 
            timer.Interval = 5000; //number in milisecinds  
            timer.Enabled = true;
        }

        protected override void OnStop()
        {
            WriteToFile("Service is stopped at " + System.DateTime.Now); //Records the time and date of when the service was stopped
        }
    }
}
