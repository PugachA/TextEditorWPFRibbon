
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using Microsoft.Win32;
using System.IO;
using System.ComponentModel; // CancelEventArgs

namespace TextEditorWPFRibbon
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string File_Name = ""; //храним имя файла для сохранения
        string File_Format = ""; //храним формат файла для сохранения
        bool flag_change = false; //поднимается когда в файл вносятся изменения
        bool flag_SaveCancel = false; //флаг для обработки нажатия отмены при сохранении
        bool flag_FontSizeChange = false; //нужен
        bool flag_FontFamilyChange = false; //флаг для отслеживание выбора стиля шрифта
        bool flag_KeyF7 = true; // флаг для обработки нажатия F7 

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(MainWindow_Loaded); // выполняем метод MainWindow_Loaded при загрузке приложения
        }

        #region Properties

        public double[] FontSizes //добавляем размеры шрифта
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
        #endregion

        #region Event Handlers

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _fontFamily.ItemsSource = System.Windows.Media.Fonts.SystemFontFamilies; //добавляем стили шрифтов
            _fontSize.ItemsSource = FontSizes; //добавляем размеры шрифтов
            _richTextBox.Focus(); //делаем RTB активным при запуске
            UpdateVisualState(); // обновляем отображение всех настроек
        }

        private void DataWindow_Closing(object sender, CancelEventArgs e) //обработка закрытия приложения
        {
            if (flag_change)
            {
                MessageBoxResult result = MessageBox.Show("Файл не сохранён или изменения в файле не были сохранены. Сохранить?",
                                                      "Текстовый редактор",
                                                      MessageBoxButton.YesNoCancel,
                                                      MessageBoxImage.Question);

                if (result == MessageBoxResult.Cancel)
                {
                    e.Cancel = true; //предотвращаем закрытие приложения
                }
                if (result == MessageBoxResult.Yes)
                {
                    _buttonSave_Click(sender, new RoutedEventArgs());
                    if (flag_SaveCancel == true)
                    {
                        e.Cancel = true; //предотвращаем закрытие приложения
                    }
                }

            }
        }

        private void FontFamily_SelectionChanged(object sender, SelectionChangedEventArgs e) //обработка выбора нового шрифта
        {
            ComboBox comboBox = (ComboBox)sender;
            if (comboBox.SelectedItem != null && flag_FontFamilyChange == true)
            {
                _richTextBox.Selection.ApplyPropertyValue(TextElement.FontFamilyProperty, (FontFamily)comboBox.SelectedItem);//применяем выбранный шрифт
                _richTextBox.Focus(); //делаем RTB активным
            }
        }
        private void FontSize_SelectionChanged(object sender, SelectionChangedEventArgs e) //обработка выбора нового размера шрифта
        {
            ComboBox comboBox = (ComboBox)sender;
            if (comboBox.SelectedItem != null && flag_FontSizeChange == true)
            {
                _richTextBox.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, e.AddedItems[0]); //применяем выбранный размер шрифта
                _richTextBox.Focus(); //делаем RTB активным
            }
        }

        //функция для вызова действи при зажатии комбинации клавиш F7
        private void _richTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F7)
            {
                if (flag_KeyF7)
                {
                    _spellcheck.IsChecked = true;
                    flag_KeyF7 = false;
                }
                else
                {
                    _spellcheck.IsChecked = false;
                    flag_KeyF7 = true;
                }
            }
        }

        private void _colorPickerForeground_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            _labelForeground.Background = new SolidColorBrush((Color)e.NewValue); //отображаем выбранный цвет в label
            _richTextBox.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush((Color)e.NewValue));//изменение цвета символов
            _colorPickerForeground.IsEnabled = false; //прошлось так сделать, чтобы после выбора цвета RTB становился активным (Ещё для этого добавлена обработки MouseUp)
        }

        private void _colorPickerBackground_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            _labelBackground.Background = new SolidColorBrush((Color)e.NewValue); //отображаем выбранный цвет в label
            _richTextBox.Selection.ApplyPropertyValue(TextElement.BackgroundProperty, new SolidColorBrush((Color)e.NewValue));//изменение цвета фона текста 
            _colorPickerBackground.IsEnabled = false; //прошлось так сделать, чтобы после выбора цвета RTB становился активным (Ещё для этого добавлена обработки MouseUp)
        }

        private void _fontSize_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            flag_FontSizeChange = true; //поднимаем флаг при нажатии мышки на Combobox
        }

        private void _fontFamily_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            flag_FontFamilyChange = true; //поднимаем флаг при нажатии мышки на Combobox
        }

        private void _spellcheck_Unchecked(object sender, RoutedEventArgs e)
        {
            _richTextBox.SpellCheck.IsEnabled = false;//включение проверки орфографии
        }

        private void _spellcheck_Checked(object sender, RoutedEventArgs e)
        {
            _richTextBox.SpellCheck.IsEnabled = true; //включение проверки орфографии
        }

        private void _buttonBullets_Checked(object sender, RoutedEventArgs e)
        {
            _buttonNumbering.IsChecked = false; //чтобы был активен только один формат списка
        }

        private void _buttonNumbering_Checked(object sender, RoutedEventArgs e)
        {
            _buttonBullets.IsChecked = false; //чтобы был активен только один формат списка
        }

        private void Low_Click(object sender, RoutedEventArgs e)
        {
            _richTextBox.Selection.Text = _richTextBox.Selection.Text.ToLower(); //приведение к нижнему регистру
        }

        private void Up_Click(object sender, RoutedEventArgs e)
        {
            _richTextBox.Selection.Text = _richTextBox.Selection.Text.ToUpper(); //приведение к верхнему регистру
        }

        private void _colorPickerForeground_MouseUp(object sender, MouseButtonEventArgs e) //Возникает при освобождении кнопки мыши
        {
            _colorPickerForeground.IsEnabled = true;
        }

        private void _colorPickerBackground_MouseUp(object sender, MouseButtonEventArgs e) //Возникает при освобождении кнопки мыши
        {
            _colorPickerBackground.IsEnabled = true;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void _richTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            flag_change = true; //поднимаем флаг, потому что были изменения
        }

        private void _richTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            UpdateVisualState(); //обновляем состояние всех элементов отображения информации о тексте 
        }

        #endregion

        #region Document Processing

        private void _buttonSave_Click(object sender, RoutedEventArgs e)
        {
            TextRange doc = new TextRange(_richTextBox.Document.ContentStart, _richTextBox.Document.ContentEnd); //создаем контейнер для документа

            if (File_Name == "")
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Text Files (*.txt)|*.txt|RichText Files (*.rtf)|*.rtf|XAML Files (*.xaml)|*.xaml|All files (*.*)|*.*"; //форматы сохранения файлов
                if (sfd.ShowDialog() == true)
                {
                    using (FileStream fs = File.Create(sfd.FileName))
                    {
                        //if (sfd.DialogResult=false)
                        if (Path.GetExtension(sfd.FileName).ToLower() == ".rtf")
                        {
                            doc.Save(fs, DataFormats.Rtf);
                            File_Format = DataFormats.Rtf;
                        }
                        else if (Path.GetExtension(sfd.FileName).ToLower() == ".txt")
                        {
                            doc.Save(fs, DataFormats.Text);
                            File_Format = DataFormats.Text;
                        }
                        else
                        {
                            doc.Save(fs, DataFormats.Xaml);
                            File_Format = DataFormats.Xaml;
                        }
                        File_Name = sfd.FileName;
                        flag_change = false;
                        flag_SaveCancel = false;
                    }
                }
                else
                {
                    flag_SaveCancel = true;
                }
            }
            else
            {
                using (FileStream fs = File.Create(File_Name))
                {
                    doc.Save(fs, File_Format);
                    flag_change = false;
                }

            }
        }

        private void _buttonSaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Text Files (*.txt)|*.txt|RichText Files (*.rtf)|*.rtf|XAML Files (*.xaml)|*.xaml|All files (*.*)|*.*";
            if (sfd.ShowDialog() == true)
            {
                TextRange doc = new TextRange(_richTextBox.Document.ContentStart, _richTextBox.Document.ContentEnd);
                using (FileStream fs = File.Create(sfd.FileName))
                {
                    if (Path.GetExtension(sfd.FileName).ToLower() == ".rtf")
                    {
                        doc.Save(fs, DataFormats.Rtf);
                        File_Format = DataFormats.Rtf;
                    }
                    else if (Path.GetExtension(sfd.FileName).ToLower() == ".txt")
                    {
                        doc.Save(fs, DataFormats.Text);
                        File_Format = DataFormats.Text;
                    }
                    else
                    {
                        doc.Save(fs, DataFormats.Xaml);
                        File_Format = DataFormats.Xaml;
                    }
                    File_Name = sfd.FileName;
                    flag_change = false;
                }
            }
        }

        private void Open_File() //вспомогательный метод для открытия файла
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Text Files (*.txt)|*.txt|RichText Files (*.rtf)|*.rtf|XAML Files (*.xaml)|*.xaml|All files (*.*)|*.*"; //форматы загружаемых файлов

            if (ofd.ShowDialog() == true)
            {
                TextRange doc = new TextRange(_richTextBox.Document.ContentStart, _richTextBox.Document.ContentEnd); //создаем контейнер для документа

                //открытие файла
                using (FileStream fs = new FileStream(ofd.FileName, FileMode.Open))
                {
                    if (Path.GetExtension(ofd.FileName).ToLower() == ".rtf")
                    {
                        doc.Load(fs, DataFormats.Rtf);
                        File_Format = DataFormats.Rtf;
                    }
                    else if (Path.GetExtension(ofd.FileName).ToLower() == ".txt")
                    {
                        doc.Load(fs, DataFormats.Text);
                        File_Format = DataFormats.Text;
                    }
                    else
                    {
                        doc.Load(fs, DataFormats.Xaml);
                        File_Format = DataFormats.Xaml;
                    }
                    File_Name = ofd.FileName;
                    flag_change = false;
                }
            }
        }

        private void _buttonOpen_Click(object sender, RoutedEventArgs e)
        {
            if (flag_change)
            {
                MessageBoxResult result = MessageBox.Show("Файл не сохранён или изменения в файле не были сохранены. Сохранить?",
                                                      "Текстовый редактор",
                                                      MessageBoxButton.YesNoCancel,
                                                      MessageBoxImage.Question);
                if (result == MessageBoxResult.No)
                {
                    Open_File(); //открываем файл
                }
                if (result == MessageBoxResult.Yes)
                {
                    _buttonSave_Click(sender, e);
                    if (flag_SaveCancel == false)
                    {
                        Open_File(); //открываем файл
                    }
                }

            }
            else
            {
                Open_File(); //открываем файл
            }
        }

        private void _buttonNew_Click(object sender, RoutedEventArgs e)
        {
            if (flag_change)
            {
                MessageBoxResult result = MessageBox.Show("Файл не сохранён или изменения в файле не были сохранены. Сохранить?",
                                                      "Текстовый редактор",
                                                      MessageBoxButton.YesNoCancel,
                                                      MessageBoxImage.Question);
                if (result == MessageBoxResult.No)
                {
                    _richTextBox.Document.Blocks.Clear(); //очищаем richtextbox
                    flag_change = false;
                }
                if (result == MessageBoxResult.Yes)
                {
                    _buttonSave_Click(sender, e);
                    if (flag_SaveCancel == false)
                    {
                        _richTextBox.Document.Blocks.Clear(); //очищаем richtextbox
                        flag_change = false;
                    }
                }

            }
            else
            {
                _richTextBox.Document.Blocks.Clear(); //очищаем richtextbox
                flag_change = false;
            }
        }

        private void _buttonPrint_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog pd = new PrintDialog();
            if ((pd.ShowDialog() == true))
            {

                FlowDocument doc = _richTextBox.Document;

                // Сохраняем все имеющиеся настройки
                double pageHeight = doc.PageHeight;
                double pageWidth = doc.PageWidth;
                Thickness pagePadding = doc.PagePadding;
                double columnWidth = doc.ColumnWidth;

                // Приводим страницу FlowDocument в соответствие с печатной страницей
                doc.PageHeight = pd.PrintableAreaHeight;
                doc.PageWidth = pd.PrintableAreaWidth;
                doc.PagePadding = new Thickness(94.4882, 75.5906, 75.5906, 75.5906);


                // Используем один столбец
                doc.ColumnWidth = (doc.PageWidth -
                    -doc.PagePadding.Left - doc.PagePadding.Right);

                // Печатаем документ
                pd.PrintDocument((((IDocumentPaginatorSource)doc).DocumentPaginator), "printing as paginator");

                // Восстанавливаем старые настройки
                doc.PageHeight = pageHeight;
                doc.PageWidth = pageWidth;
                doc.PagePadding = pagePadding;
                doc.ColumnWidth = columnWidth;
            }
        }

        #endregion

        #region Methods

        private void UpdateVisualState() //метод для визуального отображения настроек текста
        {
            UpdateToggleButtonState();
            UpdateSelectedFontFamily();
            UpdateSelectedFontSize();
            UpdateSelectionListType();
        }

        private void UpdateToggleButtonState() //обновление состояний  всех ToggleButtons
        {
            UpdateItemCheckedState(_buttonBold, TextElement.FontWeightProperty, FontWeights.Bold);
            UpdateItemCheckedState(_buttonItalic, TextElement.FontStyleProperty, FontStyles.Italic);
            UpdateItemCheckedState(_buttonUnderline, Inline.TextDecorationsProperty, TextDecorations.Underline);

            UpdateItemCheckedState(_buttonAlignLeft, Paragraph.TextAlignmentProperty, TextAlignment.Left);
            UpdateItemCheckedState(_buttonAlignCenter, Paragraph.TextAlignmentProperty, TextAlignment.Center);
            UpdateItemCheckedState(_buttonAlignRight, Paragraph.TextAlignmentProperty, TextAlignment.Right);
            UpdateItemCheckedState(_buttonAlignJustify, Paragraph.TextAlignmentProperty, TextAlignment.Right);
        }

        private void UpdateItemCheckedState(ToggleButton button, DependencyProperty formattingProperty, object expectedValue) // метод для обновления состояния ToggleButton
        {
            object currentValue = _richTextBox.Selection.GetPropertyValue(formattingProperty);
            button.IsChecked = (currentValue == DependencyProperty.UnsetValue) ? false : currentValue != null && currentValue.Equals(expectedValue);
        }

        private void UpdateSelectedFontFamily() //проверить условие
        {
            object value = _richTextBox.Selection.GetPropertyValue(TextElement.FontFamilyProperty);
            FontFamily currentFontFamily = (FontFamily)((value == DependencyProperty.UnsetValue) ? null : value);
            if (currentFontFamily != null)
            {
                _fontFamily.SelectedItem = currentFontFamily; //выводим в Combobox шрифт текста
                flag_FontFamilyChange = false; // опускаем флаг, чтобы выведенное значение не применялось к тексту.
            }
        }

        private void UpdateSelectedFontSize()
        {
            object value = _richTextBox.Selection.GetPropertyValue(TextElement.FontSizeProperty);
            _fontSize.SelectedValue = (value == DependencyProperty.UnsetValue) ? null : value; //выводим в Combobox  размер шрифта текста
            flag_FontSizeChange = false; // опускаем флаг, чтобы выведенное значение не применялось к тексту.
        }

        private void UpdateSelectionListType() //обновления формата списка
        {
            Paragraph startParagraph = _richTextBox.Selection.Start.Paragraph;
            Paragraph endParagraph = _richTextBox.Selection.End.Paragraph;
            if (startParagraph != null && endParagraph != null && (startParagraph.Parent is ListItem) && (endParagraph.Parent is ListItem) && object.ReferenceEquals(((ListItem)startParagraph.Parent).List, ((ListItem)endParagraph.Parent).List))
            {
                TextMarkerStyle markerStyle = ((ListItem)startParagraph.Parent).List.MarkerStyle;
                if (markerStyle == TextMarkerStyle.Disc) //bullets
                {
                    _buttonBullets.IsChecked = true;
                }
                else if (markerStyle == TextMarkerStyle.Decimal) //numbers
                {
                    _buttonNumbering.IsChecked = true;
                }
            }
            else
            {
                _buttonBullets.IsChecked = false;
                _buttonNumbering.IsChecked = false;
            }
        }

        #endregion
    }
}
