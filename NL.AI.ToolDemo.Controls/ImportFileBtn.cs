using System.Windows;
using System.Windows.Forms;
using Button = System.Windows.Controls.Button;

namespace NL.AI.ToolDemo.Controls
{
    public class ImportFileBtn : Button
    {
        public string FilePath
        {
            get { return (string)GetValue(FilePathtProperty); }
            set { SetValue(FilePathtProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FilePathtProperty =
            DependencyProperty.Register("FilePath", typeof(string), typeof(ImportFileBtn), new PropertyMetadata(""));


        public string FolderPath
        {
            get { return (string)GetValue(FolderPathProperty); }
            set { SetValue(FolderPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FolderPathProperty =
            DependencyProperty.Register("FolderPath", typeof(string), typeof(ImportFileBtn), new PropertyMetadata(""));

        public string FileType
        {
            get { return (string)GetValue(FileTypeProperty); }
            set { SetValue(FileTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FileTypeProperty =
            DependencyProperty.Register("FileType", typeof(string), typeof(ImportFileBtn), new PropertyMetadata(""));

        public string FileName
        {
            get { return (string)GetValue(FileNameProperty); }
            set { SetValue(FileNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FileNameProperty =
            DependencyProperty.Register("FileName", typeof(string), typeof(ImportFileBtn), new PropertyMetadata(""));

        public string FolderName
        {
            get { return (string)GetValue(FFolderProperty); }
            set { SetValue(FFolderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FFolderProperty =
            DependencyProperty.Register("FolderName", typeof(string), typeof(ImportFileBtn), new PropertyMetadata(""));

        public bool IsFile
        {
            get { return (bool)GetValue(IsFileProperty); }
            set { SetValue(IsFileProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsFileProperty =
            DependencyProperty.Register("IsFileProperty", typeof(bool), typeof(ImportFileBtn), new PropertyMetadata(true));

        /// <summary>
        /// 文件后缀限制
        /// </summary>
        public string FileExtension
        {
            get { return (string)GetValue(FileExtensionProperty); }
            set { SetValue(FileExtensionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FileExtension.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FileExtensionProperty =
            DependencyProperty.Register("FileExtension", typeof(string), typeof(ImportFileBtn), new PropertyMetadata("所有文件 (*.*)|*.*"));

        static ImportFileBtn()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImportFileBtn), new FrameworkPropertyMetadata(typeof(ImportFileBtn)));
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.Loaded += FileDialog_Loaded;
            this.Unloaded += FileDialog_UnLoaded;
        }

        private void FileDialog_Click(object sender, RoutedEventArgs e)
        {
            if (IsFile)
            {
                var d = new System.Windows.Forms.OpenFileDialog();
                OpenFileDialog EMCFileDialog = new OpenFileDialog();
                EMCFileDialog.RestoreDirectory = true;
                EMCFileDialog.Filter = FileExtension;
                if (EMCFileDialog.ShowDialog() == DialogResult.OK)
                {
                    FileName = System.IO.Path.GetFileName(EMCFileDialog.FileName);
                    FilePath = EMCFileDialog.FileName;
                    Command?.Execute(FilePath);
                }
            }
            else
            {
                FolderBrowserDialog openFileDialog = new FolderBrowserDialog();
                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    FolderName = System.IO.Path.GetFileName(openFileDialog.SelectedPath);
                    FolderPath = openFileDialog.SelectedPath;
                    Command?.Execute(FolderPath);
                }
            }
        }

        private void FileDialog_UnLoaded(object sender, RoutedEventArgs e)
        {
            this.Click -= FileDialog_Click;
            this.Loaded -= FileDialog_Loaded;
            this.Unloaded -= FileDialog_UnLoaded;
        }

        private void FileDialog_Loaded(object sender, RoutedEventArgs e)
        {
            this.Click += FileDialog_Click;
        }
    }
}
