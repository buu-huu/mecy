using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WinSCP;

namespace MecyInformation
{
    static class OpenDataDownloader
    {
        public const string HOST_NAME = "opendata.dwd.de";
        public const string USER_NAME = "anonymous";
        public const string PASSWORD  = "anonymous";
        public const string DWD_MESO_DIRECTORY = "weather/radar/mesocyclones/";
        public const string LOCAL_DOWNLOAD_PATH = @"xml_data";

        public static void DownloadAllData()
        {
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
                session.GetFiles(DWD_MESO_DIRECTORY+"*", LOCAL_DOWNLOAD_PATH+"\\").Check();
                session.Close();
            }
        }

        private static void DeleteAllData()
        {
            if (Directory.Exists(LOCAL_DOWNLOAD_PATH))
            {
                Directory.Delete(LOCAL_DOWNLOAD_PATH, true);
                Directory.CreateDirectory(LOCAL_DOWNLOAD_PATH);
            }
        }
    }
}
