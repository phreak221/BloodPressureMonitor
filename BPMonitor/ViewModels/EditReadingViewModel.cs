using BPMonitor.Config;
using BPMonitor.Models;
using SQLite;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;

namespace BPMonitor.ViewModels
{
    public class EditReadingViewModel : ViewModelBase
    {
        public ICommand SubmitReadingButton { get; set; }
        public ICommand CancelReadingButton { get; set; }
        public ICommand BtnSetBpTime { get; set; }

        private readonly ObservableCollection<ViewModelBase> _workspaces;

        private int _bpid;
        public int BPID
        {
            get { return _bpid; }
            set
            {
                if (_bpid == value) return;
                _bpid = value;
                OnPropertyChanged("BPID");
            }
        }

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

        private bool _isOmitted;
        public bool IsOmitted
        {
            get => _isOmitted;
            set
            {
                if (_isOmitted == value) return;
                _isOmitted = value;
                OnPropertyChanged("IsOmitted");
            }
        }

        public EditReadingViewModel(ObservableCollection<ViewModelBase> workspaces, int bpId)
        {
            SubmitReadingButton = new RelayCommand(SubmitReading);
            CancelReadingButton = new RelayCommand(CancelReading);
            BtnSetBpTime = new RelayCommand(SetBpTime);

            _workspaces = workspaces;

            BPID = bpId;
            SetEditProperties(bpId);
        }

        private void SubmitReading(object obj)
        {
            using (SQLiteConnection conn = new SQLiteConnection(Settings.DBPATH, SQLiteOpenFlags.ReadWrite, false))
            {
                string msg = "";
                try
                {
                    BPReadings bpreadings = conn.Table<BPReadings>().FirstOrDefault(x => x.BPID == BPID);
                    if (bpreadings == null) return;

                    int systolic = int.Parse(BPSystolic);
                    int diastolic = int.Parse(BPDiastolic);

                    bpreadings.BPTime = BPTime;
                    bpreadings.BPSystolic = int.Parse(BPSystolic);
                    bpreadings.BPDiastolic = int.Parse(BPDiastolic);
                    bpreadings.BPPulseRate = int.Parse(BPPulseRate);
                    bpreadings.BPStatus = GetPressureStatus(systolic, diastolic);
                    bpreadings.BPOmitted = IsOmitted;
                    conn.Update(bpreadings);

                    msg = "Changes to reading successful.";
                }
                catch (Exception)
                {
                    msg = "There was a problem updating reading.";
                }
                finally
                {
                    GoToMainView(msg);
                }
            }
        }

        private void CancelReading(object obj)
        {
            GoToMainView("Changes to reading cancelled by user.");
        }

        private void SetBpTime(object obj)
        {
            //BPTime = String.Format("{0:g}", DateTime.Now);
            BPTime = DateTime.Now;
        }

        private void SetEditProperties(int bpId)
        {
            using (SQLiteConnection conn = new SQLiteConnection(Settings.DBPATH, SQLiteOpenFlags.ReadWrite, false))
            {
                BPReadings bpreading = conn.Table<BPReadings>().FirstOrDefault(x => x.BPID == bpId);
                if (bpreading == null) return;

                BPTime = bpreading.BPTime;
                BPSystolic = bpreading.BPSystolic.ToString();
                BPDiastolic = bpreading.BPDiastolic.ToString();
                BPPulseRate = bpreading.BPPulseRate.ToString();
                IsOmitted = bpreading.BPOmitted;
            }
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

        private void GoToMainView(string alertMsg)
        {
            _workspaces.Clear();
            MainViewModel workspace = new MainViewModel(_workspaces, alertMsg);
            _workspaces.Add(workspace);
            SetActiveWorkspace(workspace);
        }

        private void SetActiveWorkspace(ViewModelBase workspace)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(_workspaces);
            collectionView?.MoveCurrentTo(workspace);
        }
    }
}
