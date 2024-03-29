﻿using BPMonitor.Config;
using BPMonitor.Models;
using BPMonitor.ViewModels;
using SQLite;
using System.IO;
using System.Windows;

namespace BPMonitor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            if (!File.Exists(Settings.DBPATH))
                BuildDatabase();

            base.OnStartup(e);
            MainWindow window = new MainWindow();
            var viewModel = new MainWindowViewModel();
            window.DataContext = viewModel;
            window.Show();
        }

        private void BuildDatabase()
        {
            if (!File.Exists(Settings.DBPATH))
                File.Create(Settings.DBPATH).Close();

            SQLiteConnection conn = new SQLiteConnection(Settings.DBPATH, SQLiteOpenFlags.ReadWrite);
            conn.CreateTable<BPReadings>();
            conn.Close();
        }
    }
}
