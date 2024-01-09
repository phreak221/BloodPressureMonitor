using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using BPMonitor.Config;
using BPMonitor.Models;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using SQLite;

namespace BPMonitor.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ObservableCollection<ViewModelBase> _workspaces;

        public ICommand MenuExit { get; set; }
        public ICommand MenuAddNew { get; set; }
        public ICommand MenuCreateReport { get; set; }

        public MainWindowViewModel()
        {
            MenuExit = new RelayCommand(ExitBPMonitor);
            MenuAddNew = new RelayCommand(AddNewBpReading);
            MenuCreateReport = new RelayCommand(OpenRunReport);
            //MenuCreateReport = new RelayCommand(CreateBpReport);
            ShowMainWindow("Ready to add a new reading?");
        }

        private void OpenRunReport(object obj)
        {
            _workspaces.Clear();
            BPReportViewModel workspace = new BPReportViewModel(_workspaces);
            _workspaces.Add(workspace);
            SetActiveWorkspace(workspace);
        }

        private void CreateBpReport(object obj)
        {
            string saveToPath = "";
            //var folderBrowserDialog = new FolderBrowserDialog();
            //folderBrowserDialog.ShowNewFolderButton = true;
            //folderBrowserDialog.Description = "Save Blood Pressure Readings";
            //DialogResult result = folderBrowserDialog.ShowDialog();
            //if (result == DialogResult.OK)
            //{
            //    saveToPath = folderBrowserDialog.SelectedPath;
            //}

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Blood Pressure Reading";
            saveFileDialog.Filter = "PDF|*.pdf";
            DialogResult result = saveFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                saveToPath = saveFileDialog.FileName;
            }

            //PdfWriter writer = new PdfWriter($"{saveToPath}\\BPReadings.pdf");
            if (string.IsNullOrEmpty(saveToPath)) return;
            PdfWriter writer = new PdfWriter(saveToPath);
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
            //Cell cellStatus = new Cell(1, 1)
            //    .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
            //    .SetTextAlignment(TextAlignment.CENTER)
            //    .SetWidth(300)
            //    .Add(new Paragraph("Status"));
            //table.AddCell(cellStatus);

            SQLiteConnection conn = new SQLiteConnection(Settings.DBPATH, SQLiteOpenFlags.ReadWrite, false);
            List<BPReadings> bpList = conn.Table<BPReadings>().Where(x => !x.BPOmitted).ToList();
            foreach (BPReadings bpr in bpList)
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
                //Cell cellBpStatus = new Cell(1, 1)
                //    .SetTextAlignment(TextAlignment.CENTER)
                //    .Add(new Paragraph(bpr.BPStatus));
                //table.AddCell(cellBpStatus);
            }
            doc.Add(table);
            doc.Close();

            ShowMainWindow("Report completed successfully.");
        }

        public ObservableCollection<ViewModelBase> Workspaces
        {
            get
            {
                if (_workspaces == null)
                {
                    _workspaces = new ObservableCollection<ViewModelBase>();
                    _workspaces.CollectionChanged += OnWorkspacesChanged;
                }
                return _workspaces;
            }
        }

        private void OnWorkspacesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

        }

        public void ShowMainWindow(string alertMsg = "")
        {
            Workspaces.Clear();
            MainViewModel workspace = new MainViewModel(Workspaces, alertMsg);
            Workspaces.Add(workspace);
            SetActiveWorkspace(workspace);
        }

        private void AddNewBpReading(object obj)
        {
            Workspaces.Clear();
            AddReadingViewModel workspace = new AddReadingViewModel(Workspaces);
            Workspaces.Add(workspace);
            SetActiveWorkspace(workspace);
        }

        private void ExitBPMonitor(object obj)
        {
            App.Current.MainWindow.Close();
        }

        public void SetActiveWorkspace(ViewModelBase workspace)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(Workspaces);
            collectionView?.MoveCurrentTo(workspace);
        }
    }
}
