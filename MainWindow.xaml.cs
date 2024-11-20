using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace DPaste
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Storyboard Sb = new Storyboard();
        private readonly string[] Ua = Properties.Resources.UserAgents.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
        private IEasingFunction Smooth { get; set; } = new QuadraticEase {
            EasingMode = EasingMode.EaseInOut
        }; 
        public MainWindow()
        {
            InitializeComponent();
            string Dir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string DPTPath = Path.Combine(Dir, "DPasteTool");
            if (!Directory.Exists(DPTPath)) { Directory.CreateDirectory(DPTPath); }
            else
            {
            }
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ExitB_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        private void MinimizeB_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        async private void Progress_Click(object sender, RoutedEventArgs e)
        {
            if (Input.Text.Replace(" ", "").Length == 0) {
                PGD.Content = "You must fill out the text box!";
                return;
            }
            Progress.IsEnabled = false;
            Progress.Content = "...";
            try
            {
                string cnt = Input.Text;
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < cnt.Length; i += 32766)
                {
                    int ccs = Math.Min(32766, cnt.Length - i);
                    string cca = cnt.Substring(i, ccs);
                    builder.Append(cca);
                }
                var handler = new HttpClientHandler { AllowAutoRedirect = true };
                using (var wc = new HttpClient())
                {
                    var x1 = new MultipartFormDataContent
                    {
                        { new StringContent(builder.ToString(), Encoding.UTF8, "application/x-www-form-urlencoded"), "content" }
                    };
                    Random random = new Random();
                    int s2 = random.Next(0, Ua.Length);
                    wc.DefaultRequestHeaders.Add("User-Agent", Ua[s2]);
                    HttpResponseMessage res = await wc.PostAsync("https://dpaste.com/", x1);
                    var finaluri = res.RequestMessage.RequestUri.ToString();
                    if (!finaluri.Equals("https://dpaste.com/") && finaluri.StartsWith("https://dpaste.com/"))
                    {
                        PGD.Content = "Uploaded successfully! Link copied to clipboard";
                        Clipboard.SetText(finaluri + ".txt");
                    }
                    else
                    {
                        PGD.Content = "Either you get IP banned, or it failed to upload";
                    }
                }
                Input.Text = "";
            }
            catch (Exception ex) {
                PGD.Content = "Error: " + ex.ToString();
            }
            ThicknessAnimation Animation = new ThicknessAnimation()
            {
                From = new Thickness(15, 0, 0, 15),
                To = new Thickness(15, 0, 0, 25),
                Duration = new Duration(TimeSpan.FromMilliseconds(150)),
                AutoReverse = true
            };
            Storyboard.SetTarget(Animation, PGD);
            Storyboard.SetTargetProperty(Animation, new PropertyPath(MarginProperty));
            Sb.Children.Add(Animation);
            Sb.Begin();
            Sb.Children.Remove(Animation);
            Progress.IsEnabled = true;
            Progress.Content = "Upload";
        }

        private void SettingsB_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
