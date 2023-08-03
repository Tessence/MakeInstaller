using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MakeInstall
{
    public class MakerWindow : Window, MakerHolder
    {
        protected Maker maker = null;

        protected MakerMessage onUIMakerMessage;
        protected OnSingleMessage onTaskFinished;
        protected OnSingleMessage onTitle;
        protected bool _isBaked = false;
        protected RichTextBox base_logTextBox;
        protected ProgressBar base_progressBar;
        protected TextBlock base_progressMessage;
        protected bool isRunning = false;

        public MakerWindow()
        { 
            onUIMakerMessage = DispatchUIMessage;
            onTaskFinished = OnTaskFinished;
            onTitle = SetTitle;
            Loaded += OnWindowLoaded;
            Closed += OnWindowClosed;

            maker = CreateMaker();
            maker.onMessage = OnMakerMessage;
            maker.onWindowTitle = OnSetTitle;
        }
        protected virtual Maker CreateMaker()
        {
            return new Maker(null);
        }

        protected virtual void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            maker.Load();
            maker.Prepare();
        }

        protected virtual void OnWindowClosed(object sender, EventArgs e)
        {
            maker.Unload();
        }

        private void SetTitle(string title)
        {
            //this.Title = title;
        }

        public void OnPage(string page)
        {

        }

        public void OnSetTitle(string title)
        {
            this.Dispatcher.Invoke(onTitle, title);
        }

        public void OnMakerMessage(string msg, int progress, bool alert)
        {
            this.Dispatcher.Invoke(onUIMakerMessage, msg, progress, alert);
        }

        private void DispatchUIMessage(string msg, int progress, bool alert)
        {
            if (alert)
            {
                MessageBox.Show(msg, "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            //logTextBlock.Text += msg + "\n";
            if (progress > 0 && base_progressBar != null && base_progressMessage != null)
            {
                base_progressBar.Value = progress;
                base_progressMessage.Text = msg;
            }
            var d = DateTime.Now.ToString("[ yyyy-MM-dd HH:mm:ss ] ");
            base_logTextBox.AppendText(d + msg + "\n");
            base_logTextBox.ScrollToEnd();
        }

        protected virtual void OnTaskFinished(string msg)
        {

        }

        protected int Start( string configPath)
        {
            //if (!maker.IsLoaded())
            //{
            //    return;
            //}
            if (isRunning)
            {
                return 1;
            }
           
            maker.configFile = configPath;

            isRunning = true;
            Task task = new Task(() =>
            {
                maker.Run();
                //var exeType = Maker.GetExeType();
                //try
                //{
                //    switch (exeType)
                //    {
                //        case ExeType.Packer: maker.RunMake(); break;
                //        case ExeType.Packed: maker.RunInstall(); break;
                //        case ExeType.Uninstaller: maker.RunUinstall(); break;
                //    }
                //}
                //catch (Exception ex)
                //{
                //    MessageBox.Show(ex.Message);
                //}
                this.Dispatcher.Invoke(onTaskFinished, "ok");
                isRunning = false;
            });
            task.Start();
            return 0;
        }
         

        protected void SetIcon(Image image)
        {
            
        }
    }
}
