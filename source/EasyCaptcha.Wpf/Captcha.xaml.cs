using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace EasyCaptcha.Wpf
{
    /// <summary>
    /// Interaction logic for Captcha.xaml
    /// </summary>
    public partial class Captcha
    {
        private static readonly Random Random = new Random();
        private const string EncodedNoiseImage = "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";
        private readonly Image _noiseImage;

        public Captcha()
        {
            InitializeComponent();

            if (_noiseImage == null)
            {
                _noiseImage = GetNoiseImage();
            }
        }

        public string CaptchaText { get; set; } = string.Empty;

        public enum LetterOption
        {
            Number,
            Alphabet,
            Alphanumeric,
        }

        public void CreateCaptcha(LetterOption letterOption, int numberOfLetters)
        {
            
            if (numberOfLetters <= 0) return;

            var captchaText = string.Empty;
            CaptchaContainerGrid.ColumnDefinitions.Clear();
            CaptchaContainerGrid.Children.Clear();

            _noiseImage.SetValue(Grid.ColumnSpanProperty, numberOfLetters);
            CaptchaContainerGrid.Children.Add(_noiseImage);

            for (var i = 0; i < numberOfLetters; i++)
            {
                var textBlock = new TextBlock();
                var text = GetRandomLetter(letterOption);
                textBlock.Text = text;
                captchaText += text;
                textBlock.FontSize = GetRandomFontSize();
                textBlock.Margin = GetRandomMargin(numberOfLetters);
                var rotateTransform = new RotateTransform {Angle = GetRandomAngle()};
                textBlock.RenderTransformOrigin = new Point(0.5, 0.5);
                textBlock.LayoutTransform = rotateTransform;
                if (i != numberOfLetters - 1)
                {
                    textBlock.SetValue(Grid.ColumnSpanProperty, 2);
                }
                Grid.SetColumn(textBlock, i);
                CaptchaContainerGrid.ColumnDefinitions.Add(new ColumnDefinition());
                CaptchaContainerGrid.Children.Add(textBlock);
            }

            var path = GetRandomPath((int)Width, (int)Height);
            path.SetValue(Grid.ColumnSpanProperty, numberOfLetters);
            Grid.SetColumn(path, 0);
            CaptchaContainerGrid.Children.Add(path);

            CaptchaText = captchaText;
        }

        private static string GetRandomLetter(LetterOption letterOption)
        {
            var text = "A";

            switch (letterOption)
            {
                case LetterOption.Alphabet:
                    text = GetRandomAlphabet();
                    break;
                case LetterOption.Number:
                    text = GetRandomNumber();
                    break;
                case LetterOption.Alphanumeric:
                    text = GetRandomAlphanumeric();
                    break;
            }

            return text;
        }

        private static string GetRandomNumber()
        {
            return Random.Next(0, 9).ToString();
        }

        private static string GetRandomAlphabet()
        {
            var charactersAvailable = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            return charactersAvailable[Random.Next(0, charactersAvailable.Length-1)].ToString();
        }

        private static string GetRandomAlphanumeric()
        {
            var charactersAvailable = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();
            return charactersAvailable[Random.Next(0, charactersAvailable.Length - 1)].ToString();
        }

        private static int GetRandomFontSize()
        {
            return Random.Next(15, 35);
        }

        private Thickness GetRandomMargin(int numberOfLetters)
        {
            if (numberOfLetters <= 0 || CaptchaContainerGrid == null || Width <= 0 || Height <= 0 ||
                double.IsInfinity(Width) || double.IsInfinity(Height))
            {
                return new Thickness(0);
            }

            var maxLeftMargin = (int)(Width / (numberOfLetters * 2));
            var maxTopMargin = (int)(Height / 3);

            return new Thickness(Random.Next(0, maxLeftMargin), Random.Next(0, maxTopMargin), 0, 0);
        }

        private static double GetRandomAngle()
        {
            return Random.Next(-40, 40);
        }

        private static Image GetNoiseImage()
        {
            var binary = System.Convert.FromBase64String(EncodedNoiseImage);
            
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = new MemoryStream(binary);
            bitmapImage.EndInit();

            return new Image {Source = bitmapImage, Opacity = 0.8, Stretch = Stretch.Fill};
        }

        private static System.Windows.Shapes.Path GetRandomPath(int maxWidth, int maxHeight)
        {
            var pathFigure = new PathFigure { StartPoint = new Point(Random.Next(0, maxWidth / 10), Random.Next(0, maxHeight)) };
            var pointCollection = new PointCollection(9)
            {
                new Point(Random.Next(maxWidth / 10, maxWidth * 2 / 10), Random.Next(0, maxHeight)),
                new Point(Random.Next(maxWidth * 2 / 10, maxWidth * 3 / 10), Random.Next(0, maxHeight)),
                new Point(Random.Next(maxWidth * 3 / 10, maxWidth * 4 / 10), Random.Next(0, maxHeight)),
                new Point(Random.Next(maxWidth * 4 / 10, maxWidth * 5 / 10), Random.Next(0, maxHeight)),
                new Point(Random.Next(maxWidth * 5 / 10, maxWidth * 6 / 10), Random.Next(0, maxHeight)),
                new Point(Random.Next(maxWidth * 6 / 10, maxWidth * 7 / 10), Random.Next(0, maxHeight)),
                new Point(Random.Next(maxWidth * 7 / 10, maxWidth * 8 / 10), Random.Next(0, maxHeight)),
                new Point(Random.Next(maxWidth * 8 / 10, maxWidth * 9 / 10), Random.Next(0, maxHeight)),
                new Point(Random.Next(maxWidth * 9 / 10, maxWidth), Random.Next(0, maxHeight))
            };

            var bezierSegment = new PolyBezierSegment {Points = pointCollection};
            var pathSegmentCollection = new PathSegmentCollection {bezierSegment};
            pathFigure.Segments = pathSegmentCollection;
            var pathFigureCollection = new PathFigureCollection {pathFigure};
            var pathGeometry = new PathGeometry {Figures = pathFigureCollection};
            var path = new System.Windows.Shapes.Path { Stroke = Brushes.Black, StrokeThickness = 1, Data = pathGeometry };
            
            return path;
        }
    }
}
