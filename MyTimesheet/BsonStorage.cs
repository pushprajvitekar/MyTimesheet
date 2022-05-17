using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System.IO;

namespace MyTimesheet
{
    public static class BsonStorage
    {
        public const string Store = "Store";
        public static void AddToStore(TimeSheetMonth timeSheetReport)
        {
            string fileFullPath = GetPath(timeSheetReport.Year, timeSheetReport.Month);
            SerializeFile(timeSheetReport, fileFullPath);
        }

        private static void SerializeFile(TimeSheetMonth timeSheetReport, string fileFullPath)
        {
            FileInfo fileInfo = new FileInfo(fileFullPath);
            if (!fileInfo.Exists)
                Directory.CreateDirectory(fileInfo.Directory.FullName);
            using (MemoryStream ms = new MemoryStream())
            {
                using (BsonDataWriter writer = new BsonDataWriter(ms))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(writer, timeSheetReport);
                    writer.Flush();

                    ms.Seek(0, SeekOrigin.Begin);
                    using (FileStream fs = new FileStream(fileFullPath, FileMode.OpenOrCreate))
                    {
                        ms.CopyTo(fs);
                        fs.Flush();
                    }

                }

            }
        }

        private static string GetPath(int year, string month)
        {
            return Path.Combine(Store, year.ToString(), month);
        }

        public static TimeSheetMonth FetchFromStore(int year, string month)
        {
            string fileFullPath = GetPath(year, month);
            FileInfo fileInfo = new FileInfo(fileFullPath);
            if (!fileInfo.Exists)
            {
                
                var tsm= new TimeSheetMonth(month, year);
                AddToStore(tsm);
                return tsm;
            }
            return DeserializeFile<TimeSheetMonth>(fileFullPath);
        }

        private static T DeserializeFile<T>(string fileFullPath)
        {
            FileInfo fileInfo = new FileInfo(fileFullPath);
            if (!fileInfo.Exists)
                //throw new FileNotFoundException("record not found!!", fileFullPath);
                return default;
            JsonSerializer serializer = new JsonSerializer();
            BsonDataReader reader = new BsonDataReader(File.OpenRead(fileFullPath));
            return serializer.Deserialize<T>(reader);
        }
    }
}
