using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using ChartEditor.Base;
using ChartEditor.Base.Command;
using Label = System.Windows.Controls.Label;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace ChartEditor.ViewModel
{
    internal class MainViewModel : BaseViewModel
    {
        //Коллекция элементов на Canvas
        private ObservableCollection<FrameworkElement> _elements = new ObservableCollection<FrameworkElement>();
        //Выбранный элемент
        private FrameworkElement _selectedElement;

        public ObservableCollection<FrameworkElement> Elements
        {
            get { return _elements; }
            set
            {
                _elements = value;
                RaisePropertyChanged();
            }
        }

        public FrameworkElement SelectedElement
        {
            get
            {
                if (_selectedElement == null) return null;
                var type = _selectedElement.GetType();
                if (type.Name == "Label")
                {
                    IsLabel = Visibility.Visible;
                    IsFigure = Visibility.Collapsed;
                    IsImage = Visibility.Collapsed;
                    return (Label) _selectedElement;
                }
                if (type.Name == "Ellipse")
                {
                    IsLabel = Visibility.Collapsed;
                    IsFigure = Visibility.Visible;
                    IsImage = Visibility.Collapsed;
                    return (Ellipse) _selectedElement;
                }
                if (type.Name == "Polygon")
                {
                    IsLabel = Visibility.Collapsed;
                    IsFigure = Visibility.Visible;
                    IsImage = Visibility.Collapsed;
                    return (Polygon) _selectedElement;
                }
                if (type.Name == "Image")
                {
                    IsLabel = Visibility.Collapsed;
                    IsFigure = Visibility.Collapsed;
                    IsImage = Visibility.Visible;
                    return (Image) _selectedElement;
                }
                return _selectedElement;
            }
            set
            {
                _selectedElement = value;
                RaisePropertyChanged();
                RaisePropertyChanged("Font");
            }
        }

        /// <summary>
        ///     преобразование в SolidColorBrush
        /// </summary>
        /// <param name="colorDialog"></param>
        /// <returns></returns>
        public SolidColorBrush ConvertColor(ColorDialog colorDialog)
        {
            var wpfColor = Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G,
                colorDialog.Color.B);
            return new SolidColorBrush(wpfColor);
        }

        /// <summary>
        ///     Событие для получения выбранного элемента
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="mouseButtonEventArgs"></param>
        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            SelectedElement = sender as FrameworkElement;
        }

        #region Видимость панелей свойств

        private Visibility _islabel = Visibility.Collapsed;

        public Visibility IsLabel
        {
            get { return _islabel; }
            set
            {
                _islabel = value;
                RaisePropertyChanged();
            }
        }

        private Visibility _isImage = Visibility.Collapsed;

        public Visibility IsImage
        {
            get { return _isImage; }
            set
            {
                _isImage = value;
                RaisePropertyChanged();
            }
        }

        private Visibility _isFigure = Visibility.Collapsed;

        public Visibility IsFigure
        {
            get { return _isFigure; }
            set
            {
                _isFigure = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Добавление текста

        private DelegateCommand _addText;

        public ICommand AddTextButtonClick
        {
            get
            {
                if (_addText == null)
                {
                    _addText = new DelegateCommand(AddText);
                }
                return _addText;
            }
        }

        private void AddText()
        {
            var lbl = new Label {Content = "Текст"};
            lbl.MouseLeftButtonUp += OnMouseLeftButtonUp;
            Elements.Add(lbl);
        }

        #endregion

        #region Добавление Эллипса

        private DelegateCommand _addEllipse;

        public ICommand AddEllipseButtonClick
        {
            get
            {
                if (_addEllipse == null)
                {
                    _addEllipse = new DelegateCommand(AddEllipse);
                }
                return _addEllipse;
            }
        }

        private void AddEllipse()
        {
            var rectangle = new Ellipse
            {
                Height = 100,
                Width = 100,
                Fill = new SolidColorBrush(Colors.BlueViolet),
                Name = "Ellipse" + Elements.Count
            };
            rectangle.MouseLeftButtonUp += OnMouseLeftButtonUp;
            Elements.Add(rectangle);
        }

        #endregion

        #region Добавление Треугольника

        private DelegateCommand _addtriangle;

        public ICommand AddTriangleButtonClick
        {
            get
            {
                if (_addtriangle == null)
                {
                    _addtriangle = new DelegateCommand(AddTriangle);
                }
                return _addtriangle;
            }
        }

        private void AddTriangle()
        {
            var q = new Point(0, 0);
            var w = new Point(100, 100);
            var e = new Point(0, 100);

            var pc = new PointCollection();
            pc.Add(q);
            pc.Add(w);
            pc.Add(e);

            var triangle = new Polygon
            {
                Points = pc,
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 1,
                Fill = new SolidColorBrush(Colors.BlueViolet)
            };

            triangle.MouseLeftButtonUp += OnMouseLeftButtonUp;
            Elements.Add(triangle);
        }

        #endregion

        #region Добавление изображения

        private DelegateCommand _addImage;

        public ICommand AddImageButtonClick
        {
            get
            {
                if (_addImage == null)
                {
                    _addImage = new DelegateCommand(AddImage);
                }
                return _addImage;
            }
        }

        private void AddImage()
        {
            var dialig = new OpenFileDialog();

            if (dialig.ShowDialog() == true)
            {
                var converter = new ImageSourceConverter();
                var imageSource = (ImageSource) converter.ConvertFromString(dialig.FileName);
                var img = new Image {Source = imageSource};
                img.MouseLeftButtonUp += OnMouseLeftButtonUp;
                Elements.Add(img);
            }
        }

        #endregion

        #region Удаление элемента

        private DelegateCommand _deleteElement;
        public ICommand DeleteElementButtonClick
        {
            get
            {
                if (_deleteElement == null)
                {
                    _deleteElement = new DelegateCommand(DeleteElement);
                }
                return _deleteElement;
            }
        }

        private void DeleteElement()
        {
            Elements.Remove(SelectedElement);
        }

        #endregion

        //Панель свойств

        #region Изменение изображения

        private DelegateCommand _changeImage;

        public ICommand ChangeImageButtonClick
        {
            get
            {
                if (_changeImage == null)
                {
                    _changeImage = new DelegateCommand(ChangeImage);
                }
                return _changeImage;
            }
        }

        private void ChangeImage()
        {
            var dialig = new OpenFileDialog();

            if (dialig.ShowDialog() == true)
            {
                var converter = new ImageSourceConverter();
                var imageSource = (ImageSource) converter.ConvertFromString(dialig.FileName);

                ((Image) SelectedElement).Source = imageSource;
            }
        }

        #endregion

        #region Цвет текста

        private string _fontColor;

        public string SelectedColor
        {
            get { return _fontColor; }
            set
            {
                _fontColor = value;
                RaisePropertyChanged();
            }
        }

        private DelegateCommand _changeFontColor;

        public ICommand ChangeFontColor
        {
            get
            {
                if (_changeFontColor == null)
                {
                    _changeFontColor = new DelegateCommand(ChangeFontColorDialog);
                }
                return _changeFontColor;
            }
        }

        private void ChangeFontColorDialog()
        {
            var colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                ((Label) _selectedElement).Foreground = ConvertColor(colorDialog);
            }
        }

        #endregion

        #region Цвет бордера

        private DelegateCommand _changeBorderColor;

        public ICommand ChangeBorderColor
        {
            get
            {
                if (_changeBorderColor == null)
                {
                    _changeBorderColor = new DelegateCommand(ChangeBorderColorDialog);
                }
                return _changeBorderColor;
            }
        }

        private void ChangeBorderColorDialog()
        {
            var colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                ((Shape) _selectedElement).Stroke = ConvertColor(colorDialog);
            }
        }

        #endregion

        #region Цвет заливки

        private DelegateCommand _changeFillColor;

        public ICommand ChangeFillColor
        {
            get
            {
                if (_changeFillColor == null)
                {
                    _changeFillColor = new DelegateCommand(ChangeFillColorDialog);
                }
                return _changeFillColor;
            }
        }

        private void ChangeFillColorDialog()
        {
            var colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                ((Shape) _selectedElement).Fill = ConvertColor(colorDialog);
            }
        }

        #endregion

        #region Шрифты

        private readonly InstalledFontCollection fontList = new InstalledFontCollection();

        private List<string> _fonts;

        public List<string> Fonts
        {
            get
            {
                if (_fonts == null)
                {
                    _fonts = new List<string>();
                    foreach (var ff in fontList.Families)
                    {
                        _fonts.Add(ff.Name);
                    }
                }
                return _fonts;
            }
            set
            {
                _fonts = value;
                RaisePropertyChanged();
            }
        }

        public string Font
        {
            get
            {
                if (_selectedElement == null) return null;
                var type = _selectedElement.GetType();
                if (type.Name == "Label")
                {
                    IsLabel = Visibility.Visible;
                    IsFigure = Visibility.Collapsed;
                    IsImage = Visibility.Collapsed;
                    return ((Label) _selectedElement).FontFamily.ToString();
                }
                return null;
            }
            set
            {
                var ffd = new FontFamily(value);
                ((Label) _selectedElement).FontFamily = ffd;

                RaisePropertyChanged();
            }
        }

        #endregion
    }
}