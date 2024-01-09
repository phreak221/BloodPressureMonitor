using BPMonitor.Config;
using BPMonitor.Models;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;

namespace BPMonitor.ViewModels
{
    class BPReportViewModel : ViewModelBase
    {
        public ICommand RunReportButton { get; set; }
        public ICommand PrintReportButton { get; set; }
        public ICommand DoneButton { get; set; }
        private readonly ObservableCollection<ViewModelBase> _workspaces;

        private ObservableCollection<BPReadings> _bpReadings;
        public ObservableCollection<BPReadings> BpReadings
        {
            get => _bpReadings;
            set
            {
                if (_bpReadings == value) return;
                _bpReadings = value;
                OnPropertyChanged("BpReadings");
            }
        }

        private BPReadings _selectedReading;
        public BPReadings SelectedReading
        {
            get => _selectedReading;
            set
            {
                if (_selectedReading == value) return;
                _selectedReading = value;
                OnPropertyChanged("SelectedReading");
            }
        }

        private DateTime _readingStartDate;
        public DateTime ReadingStartDate
        {
            get => _readingStartDate;
            set
            {
                if (_readingStartDate == value) return;
                _readingStartDate = value;
                OnPropertyChanged("ReadingStartDate");
            }
        }

        private DateTime _readingEndDate;
        public DateTime ReadingEndDate
        {
            get => _readingEndDate;
            set
            {
                if (_readingEndDate == value) return;
                _readingEndDate = value;
                OnPropertyChanged("ReadingEndDate");
            }
        }

        public BPReportViewModel(ObservableCollection<ViewModelBase> workspaces)
        {
            _workspaces = workspaces;
            RunReportButton = new RelayCommand(RunReport);
            PrintReportButton = new RelayCommand(PrintReport);
            DoneButton = new RelayCommand(Done);
            ReadingStartDate = DateTime.Now;
            ReadingEndDate = DateTime.Now;
            
        }

        private void PrintReport(object obj)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Blood Pressure Readind";
            saveFileDialog.Filter = "PDF|*.pdf";
            DialogResult result = saveFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                string saveToPath = saveFileDialog.FileName;
                if (string.IsNullOrEmpty(saveToPath)) return;
                BuildReport(saveToPath);
            }
        }

        private void Done(object obj)
        {
            _workspaces.Clear();
            MainViewModel workspace = new MainViewModel(_workspaces, "");
            _workspaces.Add(workspace);
            SetActiveWorkspace(workspace);
        }

        private void RunReport(object obj)
        {
            SQLiteConnection conn = new SQLiteConnection(Settings.DBPATH, SQLiteOpenFlags.ReadWrite, false);
            List<BPReadings> bplist = conn.Table<BPReadings>().Where(x => !x.BPOmitted).ToList();
            List<BPReadings> filteredBpList = bplist.Where(x => (Convert.ToDateTime(x.BPTime)) <= ReadingEndDate && (Convert.ToDateTime(x.BPTime)) >= ReadingStartDate).ToList();
            BpReadings = new ObservableCollection<BPReadings>(filteredBpList);
            conn.Close();
        }

        public void SetActiveWorkspace(ViewModelBase workspace)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(_workspaces);
            collectionView?.MoveCurrentTo(workspace);
        }

        private void BuildReport(string savePath)
        {
            PdfWriter writer = new PdfWriter(savePath);
            PdfDocument pdf = new PdfDocument(writer);
            Document doc = new Document(pdf);
            Paragraph header = new Paragraph("Blood Pressure Readings")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(20);
            doc.Add(header);
            Paragraph subHeader = new Paragraph($"Scott McCall - {DateTime.Now}")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(13);
            doc.Add(subHeader);

            LineSeparator ls = new LineSeparator(new SolidLine());
            doc.Add(ls);
            doc.Add(new Paragraph(""));

            Table table = new Table(4, false);
            Cell cellTime = new Cell(1, 1)
                .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetWidth(200)
                .Add(new Paragraph("Date/Time"));
            table.AddCell(cellTime);
            Cell cellSystolic = new Cell(1, 1)
                .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetWidth(200)
                .Add(new Paragraph("Systolic"));
            table.AddCell(cellSystolic);
            Cell cellDiastolic = new Cell(1, 1)
                .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetWidth(200)
                .Add(new Paragraph("Diastolic"));
            table.AddCell(cellDiastolic);
            Cell cellPulseRate = new Cell(1, 1)
                .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetWidth(200)
                .Add(new Paragraph("Pulse Rate"));
            table.AddCell(cellPulseRate);
            SQLiteConnection conn = new SQLiteConnection(Settings.DBPATH, SQLiteOpenFlags.ReadWrite, false);
            //List<BPReadings> bpList = conn.Table<BPReadings>().Where(x => !x.BPOmitted).ToList();
            foreach (BPReadings bpr in BpReadings)
            {
                Cell cellBpTime = new Cell(1, 1)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .Add(new Paragraph(bpr.BPTime.ToString()));
                table.AddCell(cellBpTime);
                Cell cellBpSystolic = new Cell(1, 1)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .Add(new Paragraph(bpr.BPSystolic.ToString()));
                table.AddCell(cellBpSystolic);
                Cell cellBpDiastolic = new Cell(1, 1)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .Add(new Paragraph(bpr.BPDiastolic.ToString()));
                table.AddCell(cellBpDiastolic);
                Cell cellBpPulseRate = new Cell(1, 1)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .Add(new Paragraph(bpr.BPPulseRate.ToString()));
                table.AddCell(cellBpPulseRate);
            }
            doc.Add(table);
            doc.Close();
        }
    }
}
