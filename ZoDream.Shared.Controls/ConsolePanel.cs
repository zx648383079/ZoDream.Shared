using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
    ///     <MyNamespace:ConsolePanel/>
    ///
    /// </summary>
    [TemplatePart(Name = InnerTbName, Type = typeof(TextBox))]
    public class ConsolePanel : Control
    {
        public const string InnerTbName = "PART_InnerTb";
        static ConsolePanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ConsolePanel), new FrameworkPropertyMetadata(typeof(ConsolePanel)));
        }

        private int lastLineStart = 0;
        private TextBox? InnerTb;


        /// <summary>
        /// 最大显示字数
        /// </summary>
        public int MaxLength {
            get { return (int)GetValue(MaxLengthProperty); }
            set { SetValue(MaxLengthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxLength.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxLengthProperty =
            DependencyProperty.Register("MaxLength", typeof(int), typeof(ConsolePanel), new PropertyMetadata(1000));



        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            InnerTb = GetTemplateChild(InnerTbName) as TextBox;
            if (ContextMenu is null)
            {
                var menu = new ContextMenu();
                var item = new MenuItem()
                {
                    Header = "Clear"
                };
                item.Click += (s, e) =>
                {
                    Clear();
                };
                menu.Items.Add(item);
                ContextMenu = menu;
            }
        }

        public void AppendLine(string line)
        {
            if (InnerTb is null)
            {
                return;
            }
            var val = InnerTb.Text;
            if (val.Length > MaxLength)
            {
                var i = val.IndexOf('\n', val.Length - Math.Max(MaxLength - 200, 0));
                if (i < 0)
                {
                    val = "";
                }
                else
                {
                    val = val[(i + 1)..];
                }
            }
            lastLineStart = val.Length;
            InnerTb.Text = val + line + "\n";
            ScrollToEnd();
        }

        public void AppendLine(string line, bool hasTime)
        {
            if (InnerTb is null)
            {
                return;
            }
            if (hasTime)
            {
                AppendLine($"[{FormatTime()}]{line}");
            }
            else
            {
                AppendLine(line);
            }
        }

        public void ReplaceLine(string line)
        {
            if (InnerTb is null)
            {
                return;
            }
            InnerTb.Text = InnerTb.Text.Substring(0, lastLineStart) + line + "\n";
            ScrollToEnd();
        }

        public void ScrollToEnd()
        {
            if (InnerTb is null)
            {
                return;
            }
            InnerTb.ScrollToEnd();
        }

        public void Clear()
        {
            if (InnerTb is null)
            {
                return;
            }
            InnerTb.Text = string.Empty;
        }

        public static string FormatTime(DateTime date)
        {
            return $"{TwoPad(date.Hour)}:{TwoPad(date.Minute)}:{TwoPad(date.Second)}";
        }

        public static string FormatTime()
        {
            return FormatTime(DateTime.Now);
        }

        public static string TwoPad(object v)
        {
            return v.ToString()!.PadLeft(2, '0');
        }
    }
}
