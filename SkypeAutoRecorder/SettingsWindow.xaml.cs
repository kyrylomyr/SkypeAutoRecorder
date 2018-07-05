using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Navigation;
using Microsoft.Win32;
using SkypeAutoRecorder.Configuration;
using Button = System.Windows.Controls.Button;
using Clipboard = System.Windows.Clipboard;

namespace SkypeAutoRecorder
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : INotifyPropertyChanged
    {
        private const string AUTOSTART_REGISTRY_KEY = "Software\\Microsoft\\Windows\\CurrentVersion\\Run";
        private const string AUTOSTART_VALUE_NAME = Settings.APPLICATION_NAME;

        public static RoutedCommand OkCommand = new RoutedCommand();

        /// <summary>
        /// Shows how many validation errors filter list view contains.
        /// </summary>
        private int _filtersValidationErrors;

        private bool _autostart;

        public event PropertyChangedEventHandler PropertyChanged;

        public SettingsWindow(Settings currentSettings)
        {
            InitializeComponent();

            NewSettings = currentSettings ?? new Settings();
            MainGrid.DataContext = NewSettings;

            // Check if application is present in Run registry section and its file name is valid.
            // ReSharper disable once PossibleNullReferenceException
            var currentValue = (string)Registry.CurrentUser.OpenSubKey(AUTOSTART_REGISTRY_KEY).GetValue(AUTOSTART_VALUE_NAME, null);
            Autostart = !string.IsNullOrEmpty(currentValue) && currentValue == Assembly.GetEntryAssembly().Location;
        }

        public bool Autostart
        {
            get
            {
                return _autostart;
            }
            set
            {
                if (_autostart != value)
                {
                    _autostart = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Autostart"));
                }
            }
        }

        public Settings NewSettings { get; set; }

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, e);
        }

        private void okCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            // Update registry Run section for autostart.
            if (Autostart)
            {
                // Enable autostart - add registry record.
                // ReSharper disable once PossibleNullReferenceException
                Registry.CurrentUser.OpenSubKey(AUTOSTART_REGISTRY_KEY, true).SetValue(
                    AUTOSTART_VALUE_NAME, Assembly.GetEntryAssembly().Location);
            }
            else
            {
                // Disable autostart - remove registry record.
                // ReSharper disable once PossibleNullReferenceException
                Registry.CurrentUser.OpenSubKey(AUTOSTART_REGISTRY_KEY, true).DeleteValue(AUTOSTART_VALUE_NAME, false);
            }

            DialogResult = true;
        }

        private void okCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ValidationHelper.InputsAreValid(sender as DependencyObject) && _filtersValidationErrors <= 0;
        }

        private void addButtonClick(object sender, RoutedEventArgs e)
        {
            NewSettings.Filters.Add(new Filter());
        }

        private void removeButtonClick(object sender, RoutedEventArgs e)
        {
            var filter = (Filter)((Button)sender).Tag;
            NewSettings.Filters.Remove(filter);
        }

        private void onPlaceholderClick(object sender, RequestNavigateEventArgs e)
        {
            Clipboard.SetText(((Hyperlink)sender).Tag.ToString());
            e.Handled = true;
        }

        private void browseFilterFolderButtonClick(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ((Filter)((Button)sender).Tag).RawFileName = dialog.SelectedPath;
            }
        }

        private void browseDefaultFolderButtonClick(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                NewSettings.DefaultRawFileName = dialog.SelectedPath;
            }
        }

        private void resetToDefaultButtonClick(object sender, RoutedEventArgs e)
        {
            NewSettings = new Settings();
            Autostart = false;
            MainGrid.DataContext = NewSettings;
        }

        private void filtersListError(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
            {
                _filtersValidationErrors++;
            }
            else
            {
                _filtersValidationErrors--;
            }
        }
    }
}
