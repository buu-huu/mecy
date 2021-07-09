using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WinSCP;

namespace MecyApplication
{
    /// <summary>
    /// Downloading of data and storing connection constants.
    /// </summary>
    static class OpenDataDownloader
    {
        public const string HOST_NAME = "opendata.dwd.de";
        public const string USER_NAME = "anonymous";
        public const string PASSWORD  = "anonymous";
        public const string DWD_MESO_DIRECTORY = "weather/radar/mesocyclones/";
        public const string DATA_FOLDER_NAME = @"Mecy\dwd_data";
        public static string LOCAL_DOWNLOAD_PATH = System.Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\" + DATA_FOLDER_NAME;

        /// <summary>
        /// Downloads all mesocyclone XML files from opendata server.
        /// </summary>
        public static void DownloadAllData()
        {
            Console.WriteLine(LOCAL_DOWNLOAD_PATH);

            DeleteAllData();
            System.IO.Directory.CreateDirectory(LOCAL_DOWNLOAD_PATH); // Ignore if directory exists...

            SessionOptions sessionOptions = new SessionOptions
            {
                Protocol = Protocol.Ftp,
                HostName = HOST_NAME,
                UserName = USER_NAME,
                Password = PASSWORD,
            };

            using (Session session = new Session())
            {
                session.Open(sessionOptions);
                session.GetFiles(DWD_MESO_DIRECTORY + "*", LOCAL_DOWNLOAD_PATH + "\\").Check();
                session.Close();
            }
        }

        /// <summary>
        /// Deletes the folder for XML files.
        /// </summary>
        private static void DeleteAllData()
        {
            if (Directory.Exists(LOCAL_DOWNLOAD_PATH))
            {
                Directory.Delete(LOCAL_DOWNLOAD_PATH, true);
                Directory.CreateDirectory(LOCAL_DOWNLOAD_PATH);
            }
        }

        /// <summary>
        /// Checks if the DWD opendata server is up.
        /// </summary>
        /// <returns>Opendata server up</returns>
        public static bool CheckServerConnection()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(String.Format("https://{0}", HOST_NAME));
            request.Timeout = 2000;
            request.Method = "HEAD";
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    return (response.StatusCode == HttpStatusCode.OK);
                }
            }
            catch (WebException)
            {
                return false;
            }
        }
    }
}
