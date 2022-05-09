using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ZoDream.Shared.Controls
{
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ZoDream.Shared.Controls"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ZoDream.Shared.Controls;assembly=ZoDream.Shared.Controls"
    ///
    /// 您还需要添加一个从 XAML 文件所在的项目到此项目的项目引用，
    /// 并重新生成以避免编译错误:
    ///
    ///     在解决方案资源管理器中右击目标项目，然后依次单击
    ///     “添加引用”->“项目”->[浏览查找并选择此项目]
    ///
    ///
    /// 步骤 2)
    /// 继续操作并在 XAML 文件中使用控件。
    ///
    ///     <MyNamespace:FileInput/>
    ///
    /// </summary>
    [TemplatePart(Name = InputName, Type = typeof(TextBox))]
    [TemplatePart(Name = InputName, Type = typeof(ButtonBase))]
    public class FileInput : Control
    {
        public const string InputName = "PART_FileTb";
        public const string OpenName = "PART_OpenBtn";

        static FileInput()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FileInput), new FrameworkPropertyMetadata(typeof(FileInput)));
        }

        public string Filter
        {
            get { return (string)GetValue(FilterProperty); }
            set { SetValue(FilterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Filter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FilterProperty =
            DependencyProperty.Register("Filter", typeof(string), typeof(FileInput), 
                new PropertyMetadata(string.Empty));

        public string FileName
        {
            get { return (string)GetValue(FileNameProperty); }
            set { SetValue(FileNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FileName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FileNameProperty =
            DependencyProperty.Register("FileName", typeof(string), typeof(FileInput), 
                new PropertyMetadata(string.Empty, (d, e) =>
                {
                    (d as FileInput)?.UpdateValue();
                }));

        public event RoutedPropertyChangedEventHandler<string>? FileChanged;

        private TextBox? ValueTb;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ValueTb = GetTemplateChild(InputName) as TextBox;
            var openBtn = GetTemplateChild(OpenName) as ButtonBase;
            if (ValueTb != null)
            {
                ValueTb.TextChanged += FileTb_TextChanged;
                ValueTb.PreviewDragOver += FileTb_PreviewDragOver;
                ValueTb.PreviewDrop += FileTb_PreviewDrop;
            }
            if (openBtn != null)
            {
                openBtn.Click += OpenBtn_Click;
            }
        }


        private void UpdateValue()
        {
            if (ValueTb == null)
            {
                return;
            }
            if (ValueTb.Text == FileName)
            {
                return;
            }
            ValueTb.Text = FileName;
        }

        private void OpenBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFile();
            //if (IsFile)
            //{
            //    OpenFile();
            //}
            //else
            //{
            //    OpenFolder();
            //}
        }

        private void FileTb_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Link;
            e.Handled = true;
        }

        private void FileTb_PreviewDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var file = ((Array)e.Data.GetData(DataFormats.FileDrop))?.GetValue(0)?.ToString();
                if (string.IsNullOrEmpty(file))
                {
                    return;
                }
                var oldVal = FileName;
                FileName = file;
                FileChanged?.Invoke(this, new RoutedPropertyChangedEventArgs<string>(oldVal, FileName));
            }
        }

        //private void OpenFolder()
        //{
        //    var folder = new System.Windows.Forms.FolderBrowserDialog
        //    {
        //        SelectedPath = !string.IsNullOrWhiteSpace(FileName) && Directory.Exists(FileName) ? FileName : AppDomain.CurrentDomain.BaseDirectory,
        //        ShowNewFolderButton = true
        //    };
        //    if (folder.ShowDialog() != System.Windows.Forms.DialogResult.OK)
        //    {
        //        return;
        //    }
        //    FileName = folder.SelectedPath;
        //    FileChanged?.Invoke(this, FileName);
        //}

        private void OpenFile()
        {
            var picker = new Microsoft.Win32.OpenFileDialog
            {
                Title = "选择文件",
                RestoreDirectory = true,
            };
            if (!string.IsNullOrWhiteSpace(Filter))
            {
                picker.Filter = Filter;
            }
            if (picker.ShowDialog() != true)
            {
                return;
            }
            var oldVal = FileName;
            FileName = picker.FileName;
            FileChanged?.Invoke(this, new RoutedPropertyChangedEventArgs<string>(oldVal, picker.FileName));
        }

        private void FileTb_TextChanged(object sender, TextChangedEventArgs e)
        {
            var val = (sender as TextBox)!.Text.Trim();
            FileChanged?.Invoke(this, new RoutedPropertyChangedEventArgs<string>(FileName, val));
            SetCurrentValue(FileNameProperty, val);
        }
    }
}
