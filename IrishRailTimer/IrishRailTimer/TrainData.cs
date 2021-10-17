
namespace IrishRailTimer
{
    public class TrainData
    {
        public static string StationDataByStationCode = "http://api.irishrail.ie/realtime/realtime.asmx/getStationDataByCodeXML?StationCode=";
        public string DueTime { get; set; }
        public string Destination { get; set; }
        public string Origin { get; set; }
        public string Direction { get; set; }

        public TrainData(string rawTrainData)
        {
            string[] splitTrainData = rawTrainData.Split('>');

            for (int i = 0; i < splitTrainData.Length; ++i)
            {
                string currentString = splitTrainData[i];

                if (currentString.Contains("/Duein"))
                {
                    DueTime = StringSplitter.ExtractTrainDetail(currentString) + " mins";
                }
                else if (currentString.Contains("/Origintime"))
                {

                }
                else if (currentString.Contains("/Destinationtime"))
                {

                }
                else if (currentString.Contains("/Destination"))
                {
                    Destination = StringSplitter.ExtractTrainDetail(currentString);
                }
                else if (currentString.Contains("/Origin"))
                {
                    Origin = StringSplitter.ExtractTrainDetail(currentString);
                }
                else if (currentString.Contains("/Direction"))
                {
                    Direction = StringSplitter.ExtractTrainDetail(currentString);
                }
            }
        }
    }
}
