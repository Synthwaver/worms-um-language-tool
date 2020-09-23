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

            comboBoxLanguage.ItemsSource = Enum.GetValues(typeof(WormsLanguage));
            comboBoxLanguage.SelectedItem = App.DefaultChosenLanguage;
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
            using (FileStream stream = File.OpenWrite(textBoxPath.Text))
            {
                stream.Seek(App.LanguageBlockOffset, SeekOrigin.Begin);
                using (var writer = new BinaryWriter(stream))
                {
                    writer.Write(langBlockBytes);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog { Filter = "EXE-files|*.exe|All files|*.*" };

            if (dialog.ShowDialog() == true)
            {
                textBoxPath.Text = dialog.FileName;
            }
        }

        private void buttonModify_Click(object sender, RoutedEventArgs e)
        {
            byte[] langBlockBytes;
            try
            {
                langBlockBytes = ReadLanguageBlock(textBoxPath.Text);
            }
            catch
            {
                MessageBox.Show("File read error");
                return;
            }

            LanguageBlock langBlock;
            try
            {
                langBlock = new LanguageBlock(langBlockBytes);
            }
            catch
            {
                MessageBox.Show("The file is invalid");
                return;
            }

            langBlock.SetLanguage((WormsLanguage)comboBoxLanguage.SelectedItem);

            try
            {
                WriteLanguageBlock(langBlock.GetBytes());
            }
            catch
            {
                MessageBox.Show("File write error");
                return;
            }

            MessageBox.Show("Success!");
        }

        private void textBoxPath_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            textBoxPath.ToolTip = textBoxPath.Text;
        }
    }
}
