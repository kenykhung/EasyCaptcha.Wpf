using System.Windows;

namespace EasyCaptcha.Wpf.TestProgram
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Captcha.CreateCaptcha(EasyCaptcha.Wpf.Captcha.LetterOption.Alphanumeric, 5);
            AnswerTextBlock.Text = Captcha.CaptchaText;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Captcha.CreateCaptcha(EasyCaptcha.Wpf.Captcha.LetterOption.Alphanumeric, 5);
            AnswerTextBlock.Text = Captcha.CaptchaText;
        }
    }
}
