using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IrishRailTimer
{
    public class MainModel
    {
        public MainModel()
        {
        }

        public async Task<Tuple<ObservableCollection<TrainData>, string>> StationSearch(string selectedStationNameCode)
        {
            var selectedStationCode = GetCodeFromNameCode(selectedStationNameCode);
            if (selectedStationCode != "")
            {
                string apiUrl = GetStationByStationCodeURL(selectedStationCode);
                string response = await GetStationDataByCode(apiUrl);
                string[] rawTrainData = SplitDataByTrain(response);

                if (rawTrainData != null)
                {
                    ObservableCollection<TrainData> trainsDetails = new ObservableCollection<TrainData>();
                    //Ignoring first line
                    for (int i = 1; i < rawTrainData.Length; ++i)
                    {
                        string currentTrain = rawTrainData[i].Trim();
                        currentTrain = currentTrain.Replace("\r\n", string.Empty);

                        TrainData trainDetails = new TrainData(currentTrain);
                        trainsDetails.Add(trainDetails);
                    }

                    var lastUpdated = DateTime.Now.ToString("h:mm:ss tt");
                    return Tuple.Create(trainsDetails, lastUpdated);
                }
            }

            return Tuple.Create(new ObservableCollection<TrainData>(), DateTime.MinValue.ToString("h:mm:ss tt"));
        }

        public string[] SplitDataByTrain(string rawData)
        {
            if (rawData.Contains("<objStationData>"))
            {
                string[] splitData = rawData.Split(new string[] { "<objStationData>" }, StringSplitOptions.None);
                return splitData;
            }

            return null;
        }

        public string GetCodeFromNameCode(string nameCode)
        {
            var sep = '-';

            if (nameCode.Contains(sep.ToString()))
            {
                var res = nameCode.Split(sep);
                if (res.Length > 1)
                {
                    return Regex.Replace(res[1], @"\s+", "");
                }

                return "";
            }

            return Regex.Replace(nameCode, @"\s+", "");
        }

        public string GetStationByStationCodeURL(string stationCode)
        {
            return TrainData.StationDataByStationCode + stationCode;
        }

        public async Task<string> GetStationDataByCode(string url)
        {
            var rawValue = await GetContentRequest(url);
            return rawValue;
        }

        public async Task<string> GetContentRequest(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = await client.GetAsync(url))
                {
                    using (HttpContent content = response.Content)
                    {
                        string res = await content.ReadAsStringAsync();
                        return res;
                    }
                }
            }
        }

        public async Task<Tuple<ObservableCollection<string>, ObservableCollection<Station>>> GetAllStationsAsync()
        {
            try
            {
                string getStationsCall = "http://api.irishrail.ie/realtime/realtime.asmx/getAllStationsXML";
                string rawString = await GetContentRequest(getStationsCall);
                Console.WriteLine(rawString);
                ObservableCollection<Station> stations = ParseRawStationData(rawString);
                List<string> stationNameCodes = new List<string>();
                List<string> noNameStationCodes = new List<string>();

                for (int i = 0; i < stations.Count; ++i)
                {
                    var stationCode = (stations[i]?.StationCode);
                    var stationName = stations[i]?.StationName;

                    if (!string.IsNullOrEmpty(stationName) & !string.IsNullOrEmpty(stationCode))
                    {
                        stationNameCodes.Add($"{stationName} - {stationCode}");
                    }
                    else if (!string.IsNullOrEmpty(stationCode))
                    {
                        noNameStationCodes.Add(stationCode);
                    }
                }

                stationNameCodes.Sort();
                stationNameCodes = (stationNameCodes.Concat(noNameStationCodes)).ToList();

                return Tuple.Create(new ObservableCollection<string>(stationNameCodes), new ObservableCollection<Station>(stations));
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public ObservableCollection<Station> ParseRawStationData(string rawStationData)
        {
            ObservableCollection<Station> stations = new ObservableCollection<Station>();
            string[] stationDatas = rawStationData.Split(new string[] { "<objStation>" }, StringSplitOptions.None);
            try
            {
                for (int i = 1; i < stationDatas.Length; ++i)
                {
                    string currentString = stationDatas[i].Trim();
                    currentString = currentString.Replace("\r\n", string.Empty);
                    Station station = new Station(currentString);

                    stations.Add(station);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return stations;
        }
    }
}
