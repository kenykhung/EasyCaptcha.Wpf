# EasyCaptcha.Wpf
WPF Captcha usercontrol<br/><br/>
![image](https://github.com/kenykhung/EasyCaptcha.Wpf/blob/master/sample.gif)

Nuget Install:  
>Install-Package EasyCaptcha.Wpf

Declair in Xaml:  
```C#
    xmlns:easy="clr-namespace:EasyCaptcha.Wpf;assembly=EasyCaptcha.Wpf"
    <easy:Captcha x:Name="MyCaptcha" Width="200" Height="50"/>
````

Usage:  
```C#
    Captcha.CreateCaptcha(Captcha.LetterOption.Alphanumeric, 5);
    var answer = MyCaptcha.CaptchaText;
````

Different types:
```C#
    public enum LetterOption
    {
        Number,
        Alphabet,
        Alphanumeric,
    }
````

Build:
```C#
    build\build.cmd
````
