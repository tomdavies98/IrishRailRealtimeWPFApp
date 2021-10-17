using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Threading;
using System.ComponentModel;
using System.Windows.Input;

namespace IrishRailTimer
{
    public class MainViewModel : INotifyPropertyChanged
    {   
        public MainModel MainModel { get; set; }
        public ICommand Search { get; set; }
        public bool MonitorThreadRunning = true;

        private string _errorMessage = "";
        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
            set
            {
                _errorMessage = value;
                NotifyPropertyChanged(nameof(ErrorMessage));
            }
        }

        private string _lastUpdated = "-";
        public string LastUpdated
        {
            get { return _lastUpdated; }
            set
            {
                _lastUpdated = "Last Updated: " + value;
                NotifyPropertyChanged(nameof(LastUpdated));
            }
        }

        private ObservableCollection<TrainData> _trainsDetails = new ObservableCollection<TrainData>();

        public ObservableCollection<TrainData> TrainsDetails
        {
            get { return _trainsDetails; }
            set
            {
                _trainsDetails = value;
                NotifyPropertyChanged(nameof(TrainsDetails));
            }
        }

        private ObservableCollection<Station> _stations = new ObservableCollection<Station>();
        public ObservableCollection<Station> Stations
        {
            get { return _stations; }
            set
            {
                _stations = value;
                NotifyPropertyChanged(nameof(Stations));
            }
        }

        private ObservableCollection<string> _stationCodes = new ObservableCollection<string>();

        public ObservableCollection<string> StationCodes
        {
            get { return _stationCodes; }
            set
            {
                _stationCodes = value;
                NotifyPropertyChanged(nameof(StationCodes));
            }
        }

        private ObservableCollection<string> _stationNameCodes = new ObservableCollection<string>();
        public ObservableCollection<string> StationNameCodes
        {
            get { return _stationNameCodes; }
            set
            {
                _stationNameCodes = value;
                NotifyPropertyChanged(nameof(StationNameCodes));
            }
        }

        private string _selectedStationNameCode = "";
        public string SelectedStationNameCode
        {
            get { return _selectedStationNameCode; }
            set
            {
                _selectedStationNameCode = value;
                NotifyPropertyChanged(nameof(SelectedStationNameCode));
            }
        }

        public MainViewModel(MainModel mainModel)
        {
            MainModel = mainModel;
            Search = new RelayButtonCommand(async o => await StationSearch());
            SetStationInfo();

            Thread monitorThread = new Thread(StopUpdateMonitor);
            monitorThread.Start();
        }

        private async void SetStationInfo()
        {
            var stationInfo = await MainModel.GetAllStationsAsync();
            StationNameCodes = stationInfo.Item1;
            Stations = stationInfo.Item2;
        }

        public async void StopUpdateMonitor()
        {
            try
            {
                while (MonitorThreadRunning)
                {
                    var selectedStationCode = MainModel.GetCodeFromNameCode(SelectedStationNameCode);
                    if (!string.IsNullOrEmpty(selectedStationCode))
                    {
                        await StationSearch();
                    }

                    Thread.Sleep(60000);
                }
            }
            catch(Exception e)
            {
                throw e;
            }
        }        
        
        public async Task StationSearch()
        {
            var selectedStationCode = MainModel.GetCodeFromNameCode(SelectedStationNameCode);
            var trainData = await MainModel.StationSearch(selectedStationCode);
            TrainsDetails = trainData.Item1;
            LastUpdated = trainData.Item2;

            if(LastUpdated.Contains(DateTime.MinValue.ToString("h:mm:ss tt")) && TrainsDetails.Count == 0)
            {
                ErrorMessage = "There is no available real time data from the Irish Rail Api, try again during operating hours!";
                LastUpdated = "No live data :(";
            }
            else
            {
                ErrorMessage = "";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
