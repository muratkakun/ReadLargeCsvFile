using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ReadLargeCsvFile
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));
            path = path + "\\Test Csv\\test.csv";  
            List<SongPlayer> parsedData = parseCSV(path);

            if (parsedData != null)
            {
                var distinctSongCountList = parsedData.Select(
                        x => new { x.CLIENT_ID, x.SONG_ID }
                    ).Distinct();

                var distincClient = distinctSongCountList.GroupBy(info => info.CLIENT_ID )
                           .Select(group => new
                           {
                               CLIENT_ID = group.Key,
                               Count = group.Count()
                           }).OrderBy(x => x.CLIENT_ID);

                
                var distincSongAndClient = distincClient.GroupBy(info => info.Count)
                                 .Select(group => new
                                 {
                                      DISTINCT_PLAY_COUNT = group.Count(),
                                     CLIENT_COUNT = group.Key
                                 }).OrderBy(x => x.DISTINCT_PLAY_COUNT);


                int clientIdCount = distincSongAndClient.Where(x => x.CLIENT_COUNT == 346).Count();
                int maxDistincPlayCount = distincSongAndClient.Max(x => x.DISTINCT_PLAY_COUNT);

                Console.Write("clientIdCount_346 = " + clientIdCount  + Environment.NewLine);
                Console.Write("maxDistincPlayCount = " + maxDistincPlayCount);

            }

            Console.ReadKey();
        }

        public class SongPlayer
        {
            private string pLAY_TS;

            public string PLAY_ID { get; set; }
            public string SONG_ID { get; set; }
            public string CLIENT_ID { get; set; }
            public string PLAY_TS
            {
                get { return pLAY_TS; }

                set
                {
                    if (value.Length > 10)
                        pLAY_TS = value.Substring(0, 10);
                }
            }
        }

        public static List<SongPlayer> parseCSV(string path)
        {
            List<SongPlayer> parsedData = new List<SongPlayer>();
            string[] fields;

            try
            {
                Parallel.ForEach(File.ReadLines(path), line =>
                {
                    fields = line.Split('\t');
                    lock (parsedData)
                    {
                        parsedData.Add(new SongPlayer()
                        {
                            PLAY_ID = fields[0],
                            SONG_ID = fields[1],
                            CLIENT_ID = fields[2],
                            PLAY_TS = fields[3]
                        });
                    }
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            parsedData.RemoveAt(0);

            //parsedData = parsedData.Take(1000).ToList();

            return parsedData;
        }
    }
}
