
namespace IrishRailTimer
{
    public class Station
    {
        public string StationName { get; set; }
        public string StationCode { get; set; }
        public string StationAlias { get; set; }
        public string StationLongitude { get; set; }
        public string StationLatitude { get; set; }
        public string StationID { get; set; }


        public Station(string name, string code)
        {
            StationName = name;
            StationCode = code;
        }

        public Station(string rawData)
        {
            string[] details = rawData.Split(' ');

            for (int i = 0; i < details.Length; ++i)
            {
                if (details[i] != "")
                {
                    string selectedDetail = details[i];

                    if (selectedDetail.Contains("StationDesc"))
                    {
                        StationName = StringSplitter.ExtractValue(selectedDetail);
                    }
                    else if (selectedDetail.Contains("StationAlias"))
                    {
                        StationAlias = StringSplitter.ExtractValue(selectedDetail);
                    }
                    else if (selectedDetail.Contains("StationLatitude"))
                    {
                        StationLatitude = StringSplitter.ExtractValue(selectedDetail);
                    }
                    else if (selectedDetail.Contains("StationLongitude"))
                    {
                        StationLongitude = StringSplitter.ExtractValue(selectedDetail);
                    }
                    else if (selectedDetail.Contains("StationCode"))
                    {
                        StationCode = StringSplitter.ExtractValue(selectedDetail);
                    }
                    else if (selectedDetail.Contains("StationId"))
                    {
                        StationID = StringSplitter.ExtractValue(selectedDetail);
                    }
                }
            }
            string RawData = rawData;
        }
    }
}
