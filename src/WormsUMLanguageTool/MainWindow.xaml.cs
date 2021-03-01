using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;

namespace WormsUMLanguageTool
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ComboBoxLanguage.ItemsSource = Enum.GetValues(typeof(WormsLanguage));
            ComboBoxLanguage.SelectedItem = App.DefaultChosenLanguage;

            string wormsPath = TryFindWormsPath();

            if (wormsPath != null)
            {
                TextBoxPath.Text = wormsPath;
            }
        }

        private byte[] ReadLanguageBlock(string filePath)
        {
            using (FileStream stream = File.OpenRead(filePath))
            {
                stream.Seek(App.LanguageBlockOffset, SeekOrigin.Begin);
                using (var reader = new BinaryReader(stream))
                {
                    return reader.ReadBytes(LanguageBlock.BlockLength);
                }
            }
        }

        private void WriteLanguageBlock(byte[] langBlockBytes)
        {
            using (FileStream stream = File.OpenWrite(TextBoxPath.Text))
            {
                stream.Seek(App.LanguageBlockOffset, SeekOrigin.Begin);
                using (var writer = new BinaryWriter(stream))
                {
                    writer.Write(langBlockBytes);
                }
            }
        }

        private string TryFindWormsPath()
        {
            string path = TryFindWormsSteamPath();

            if (path != null)
            {
                return path;
            }

            return File.Exists(App.WormsExeFileName) ? Path.GetFullPath(App.WormsExeFileName) : null;
        }

        private string TryFindWormsSteamPath()
        {
            string wormsInstallLocation = Registry.GetValue(
                @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 70600", "InstallLocation", null) as string;

            if (!string.IsNullOrWhiteSpace(wormsInstallLocation))
            {
                string wormsPath = Path.Combine(wormsInstallLocation, App.WormsExeFileName);

                if (File.Exists(wormsPath))
                    return wormsPath;
            }

            string steamPath = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Valve\Steam", "InstallPath", null) as string ??
                               Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Valve\Steam", "InstallPath", null) as string;

            if (!string.IsNullOrWhiteSpace(steamPath))
            {
                string wormsPath = Path.Combine(steamPath, App.WormsSteamDirRelativePath, App.WormsExeFileName);

                if (File.Exists(wormsPath))
                    return wormsPath;
            }

            return null;
        }

        private void MakeBackup(string filePath)
        {
            string backupFileName = $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss}_{Path.GetFileName(filePath)}";
            string backupDirPath = Path.Combine(Path.GetDirectoryName(filePath) ?? string.Empty, App.BackupDirName);

            Directory.CreateDirectory(backupDirPath);
            File.Copy(filePath, Path.Combine(backupDirPath, backupFileName), true);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog { Filter = "EXE-files|*.exe|All files|*.*" };

            if (File.Exists(TextBoxPath.Text))
            {
                dialog.InitialDirectory = Path.GetDirectoryName(TextBoxPath.Text) ?? Directory.GetCurrentDirectory();
            }

            if (dialog.ShowDialog() == true)
            {
                TextBoxPath.Text = dialog.FileName;
            }
        }

        private void ButtonModify_Click(object sender, RoutedEventArgs e)
        {
            byte[] langBlockBytes;
            try
            {
                langBlockBytes = ReadLanguageBlock(TextBoxPath.Text);
            }
            catch
            {
                MessageBox.Show("File read error", string.Empty, MessageBoxButton.OK,MessageBoxImage.Error);
                return;
            }

            LanguageBlock langBlock;
            try
            {
                langBlock = new LanguageBlock(langBlockBytes);
            }
            catch
            {
                MessageBox.Show("The file is invalid", string.Empty, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            langBlock.SetLanguage((WormsLanguage)ComboBoxLanguage.SelectedItem);

            if (MakeBackupCheckbox.IsChecked == true)
            {
                try
                {
                    MakeBackup(TextBoxPath.Text);
                }
                catch
                {
                    MessageBox.Show("Unable to make backup", string.Empty, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            try
            {
                WriteLanguageBlock(langBlock.GetBytes());
            }
            catch
            {
                MessageBox.Show("File write error", string.Empty, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBox.Show("Success!");
        }

        private void TextBoxPath_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            TextBoxPath.ToolTip = TextBoxPath.Text;
        }
    }
}
