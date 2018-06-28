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
using System.Windows.Controls.Primitives;

namespace TextEditorWPFRibbon
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
        }

        public double[] FontSizes
        {
            get
            {
                return new double[] {
                    3.0, 4.0, 5.0, 6.0, 6.5, 7.0, 7.5, 8.0, 8.5, 9.0, 9.5,
                    10.0, 10.5, 11.0, 11.5, 12.0, 12.5, 13.0, 13.5, 14.0, 15.0,
                    16.0, 17.0, 18.0, 19.0, 20.0, 22.0, 24.0, 26.0, 28.0, 30.0,
                    32.0, 34.0, 36.0, 38.0, 40.0, 44.0, 48.0, 52.0, 56.0, 60.0, 64.0, 68.0, 72.0, 76.0,
                    80.0, 88.0, 96.0, 104.0, 112.0, 120.0, 128.0, 136.0, 144.0
                    };
            }
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _fontFamily.ItemsSource = System.Windows.Media.Fonts.SystemFontFamilies;
            _fontSize.ItemsSource = FontSizes;
        }

        private void FontFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FontFamily editValue = (FontFamily)e.AddedItems[0];
            _richTextBox.Selection.ApplyPropertyValue(TextElement.FontFamilyProperty, editValue);
            _richTextBox.Focus();
        }
        private void FontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _richTextBox.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, e.AddedItems[0]);
            _richTextBox.Focus();
        }

        //функция для вызова действи при зажатии комбинации клавиш ctrl+F1
        private void _richTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyboardDevice.Modifiers==ModifierKeys.Control && e.Key==Key.F1)
            {
                MessageBox.Show("HELLO");
            }
        }

        private void _colorPickerForeground_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            _labelForeground.Foreground = new SolidColorBrush((Color)e.NewValue);
        }

        private void _colorPickerBackground_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            _labelBackground.Background = new SolidColorBrush((Color)e.NewValue);
        }

    }

}
