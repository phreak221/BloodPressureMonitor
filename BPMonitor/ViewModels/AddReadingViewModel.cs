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
    public class AddReadingViewModel : ViewModelBase
    {
        public ICommand AddNewReadingButton { get; set; }
        public ICommand CancelReadingButton { get; set; }
        public ICommand BtnSetBpTime { get; set; }

        private ObservableCollection<ViewModelBase> _workspaces;

        private DateTime _bpTime;
        public DateTime BPTime
        {
            get { return _bpTime; }
            set
            {
                if (_bpTime == value) return;
                _bpTime = value;
                OnPropertyChanged("BPTime");
            }
        }

        private string _bpSystolic;
        public string BPSystolic
        {
            get { return _bpSystolic; }
            set
            {
                if (_bpSystolic == value) return;
                _bpSystolic = value;
                OnPropertyChanged("BPSystolic");
            }
        }

        private string _bpDiastolic;
        public string BPDiastolic
        {
            get { return _bpDiastolic; }
            set
            {
                if (_bpDiastolic == value) return;
                _bpDiastolic = value;
                OnPropertyChanged("BPDiastolic");
            }
        }

        private string _bpPulseRate;
        public string BPPulseRate
        {
            get { return _bpPulseRate; }
            set
            {
                if (_bpPulseRate == value) return;
                _bpPulseRate = value;
                OnPropertyChanged("BPPulseRate");
            }
        }

        public AddReadingViewModel(ObservableCollection<ViewModelBase> workspaces)
        {
            AddNewReadingButton = new RelayCommand(AddReading);
            CancelReadingButton = new RelayCommand(CancelReading);
            BtnSetBpTime = new RelayCommand(SetDateTime);
            _workspaces = workspaces;

            //BPTime = string.Format("{0:g}", DateTime.Now);
            BPTime = DateTime.Now;
            BPSystolic = string.Empty;
            BPDiastolic = string.Empty;
            BPPulseRate = string.Empty;
        }

        private void AddReading(object obj)
        {
            using (SQLiteConnection conn = new SQLiteConnection(Settings.DBPATH, SQLiteOpenFlags.ReadWrite, false))
            {
                string msg = "";
                try
                {
                    int systolic = int.Parse(BPSystolic);
                    int diastolic = int.Parse(BPDiastolic);

                    BPReadings bpReadings = new BPReadings();
                    bpReadings.BPTime = BPTime;
                    bpReadings.BPSystolic = int.Parse(BPSystolic);
                    bpReadings.BPDiastolic = int.Parse(BPDiastolic);
                    bpReadings.BPPulseRate = int.Parse(BPPulseRate);
                    bpReadings.BPStatus = GetPressureStatus(systolic, diastolic);

                    conn.Insert(bpReadings);
                    msg = "New reading successfully added.";
                }
                catch (Exception)
                {
                    msg = "There was a problem adding new reading.";
                }
                finally
                {
                    GoToMainView(msg);
                }
            }
        }

        private void CancelReading(object obj)
        {
            GoToMainView("Add new reading cancelled by user.");
        }

        private void SetDateTime(object obj)
        {
            //BPTime = string.Format("{0:g}", DateTime.Now);
            BPTime = DateTime.Now;
        }

        private void GoToMainView(string alertMsg)
        {
            _workspaces.Clear();
            MainViewModel workspace = new MainViewModel(_workspaces, alertMsg);
            _workspaces.Add(workspace);
            SetActiveWorkspace(workspace);
        }

        //private string GetPressureStatus(int systolic, int diastolic)
        //{
        //    if (systolic < 120 && diastolic < 80) return "Normal";
        //    if ((systolic >= 120 && systolic <= 129) && diastolic < 80) return "Elevated";
        //    if ((systolic >= 130 && systolic <= 139) || (diastolic >= 80 && diastolic <= 89)) return "Hypertension - Stage 1";
        //    if (systolic >= 140 || diastolic >= 90) return "Hypertension - Stage 2";
        //    if (systolic > 180 && diastolic > 120) return "High Crisis";
        //    return string.Empty;
        //}

        public void SetActiveWorkspace(ViewModelBase workspace)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(_workspaces);
            collectionView?.MoveCurrentTo(workspace);
        }
    }
}
