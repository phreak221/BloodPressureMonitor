using BPMonitor.Config;
using BPMonitor.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace BPMonitor.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private ObservableCollection<ViewModelBase> _workspaces;

        public ICommand AddNewButton { get; set; }
        public ICommand EditButton { get; set; }
        public ICommand HideLegendButton { get; set; }
        public ICommand ShowOmittedButton { get; set; }

        private ObservableCollection<BPReadings> _bpReadings;
        public ObservableCollection<BPReadings> BpReadings
        {
            get { return _bpReadings; }
            set
            {
                if (_bpReadings == value) return;
                _bpReadings = value;
                OnPropertyChanged("BpReadings");
            }
        }

        private ObservableCollection<PressureReading> _pressureReadings;
        public ObservableCollection<PressureReading> PressureReadings
        {
            get => _pressureReadings;
            set
            {
                if (_pressureReadings == value) return;
                _pressureReadings = value;
                OnPropertyChanged("PressureReadings");
            }
        }

        private BPReadings _selectedReading;
        public BPReadings SelectedReading
        {
            get { return _selectedReading; }
            set
            {
                if (_selectedReading == value) return;
                _selectedReading = value;
                OnPropertyChanged("SelectedReading");
            }
        }

        private PressureReading _selectedBp;
        public PressureReading SelectedBp
        {
            get => _selectedBp;
            set
            {
                if (_selectedBp == value) return;
                _selectedBp = value;
                OnPropertyChanged("SelectedBp");
            }
        }

        private string _legendBtnName;
        public string LegendBtnName
        {
            get => _legendBtnName;
            set
            {
                if (_legendBtnName == value) return;
                _legendBtnName = value;
                OnPropertyChanged("LegendBtnName");
            }
        }

        private string _isVisible;
        public string IsVisible
        {
            get => _isVisible;
            set
            {
                if (_isVisible == value) return;
                _isVisible = value;
                OnPropertyChanged("IsVisible");
            }
        }

        private string _messageAlert;
        public string MessageAlert
        {
            get => _messageAlert;
            set
            {
                if (_messageAlert == value) return;
                _messageAlert = value;
                OnPropertyChanged("MessageAlert");
            }
        }

        private bool _isToggled = true;

        public MainViewModel(ObservableCollection<ViewModelBase> workspaces, string alertMsg = "")
        {
            _workspaces = workspaces;
            AddNewButton = new RelayCommand(AddNewReading);
            EditButton = new RelayCommand(EditReading);
            HideLegendButton = new RelayCommand(HideLegend);
            ShowOmittedButton = new RelayCommand(ShowOmitted);
            LegendBtnName = "Show Legend";
            IsVisible = "Collapsed";
            _isToggled = false;
            BuildBPReadingList();
            MessageAlert = alertMsg;
        }

        private void ShowOmitted(object obj)
        {
            
        }

        private void HideLegend(object obj)
        {
            if (_isToggled)
            {
                LegendBtnName = "Show Legend";
                IsVisible = "Collapsed";
                _isToggled = false;
            }
            else
            {
                LegendBtnName = "Hide Legend";
                IsVisible = "Visible";
                _isToggled = true;
            }
        }

        private void AddNewReading(object obj)
        {
            _workspaces.Clear();
            AddReadingViewModel workspace = new AddReadingViewModel(_workspaces);
            _workspaces.Add(workspace);
            SetActiveWorkspace(workspace);
        }

        private void EditReading(object obj)
        { 
            if (SelectedBp == null)
            {
                MessageAlert = "A reading must be selected first.";
                return;
            }

            _workspaces.Clear();
            //EditReadingViewModel workspace = new EditReadingViewModel(_workspaces, SelectedReading.BPID);
            EditReadingViewModel workspace = new EditReadingViewModel(_workspaces, SelectedBp.BPID);
            _workspaces.Add(workspace);
            SetActiveWorkspace(workspace);
        }

        private void BuildBPReadingList()
        {
            SQLiteConnection conn = new SQLiteConnection(Settings.DBPATH, SQLiteOpenFlags.ReadWrite, false);
            //BpReadings = new ObservableCollection<BPReadings>(conn.Table<BPReadings>().ToList());
            List<PressureReading> pressurelist = new List<PressureReading>();
            List<BPReadings> bplist = conn.Table<BPReadings>().Where(x => !x.BPOmitted).ToList();
            foreach (BPReadings bp in bplist)
            {
                pressurelist.Add(new PressureReading()
                {
                    BPID = bp.BPID,
                    BPReadingDate = bp.BPTime.ToShortDateString(),
                    BPReadingTime = bp.BPTime.ToShortTimeString(),
                    BPSystolic = bp.BPSystolic,
                    BPDiastolic = bp.BPDiastolic,
                    BPPulseRate = bp.BPPulseRate,
                    BPStatus = bp.BPStatus,
                    BPOmitted = bp.BPOmitted
                });
            }
            PressureReadings = new ObservableCollection<PressureReading>(pressurelist);
            //BpReadings = new ObservableCollection<BPReadings>(bplist);
            conn.Close();
        }

        public void SetActiveWorkspace(ViewModelBase workspace)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(_workspaces);
            collectionView?.MoveCurrentTo(workspace);
        }
    }
}
