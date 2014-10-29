﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using CefSharp;
using CefSharp.Wpf;
using Newtonsoft.Json;
using System.IO;

namespace EpochLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        public class ServerTable
        {
            public class ServerTableProxy
            {
                public string test;



                public string TestFunc()
                {
                    return test;
                }

            }


            private readonly ServerTableProxy _proxy;

            public ServerTable()
            {
                _proxy = new ServerTableProxy();

            }


            public void Register(IWebBrowser browser)
            {
                browser.RegisterJsObject("servers", _proxy);
                _proxy.test = "a";
            }

        }
        



	    public class BoundMessager
	    {
		    private MainWindow _window;

		    public delegate void LauncherWindowEvent();

		    public event LauncherWindowEvent CloseEvent;
		    public event LauncherWindowEvent MinimizeEvent;
		    public event LauncherWindowEvent MaximizeEvent;


		    public BoundMessager(MainWindow window)
		    {
			    _window = window;

		    }


		    public void HandleMessage(string type, string json)
		    {
			    MessageBox.Show(type, json);
		    }

		    public void Minimize()
		    {
				var temp = MinimizeEvent;
			    if (temp != null)
			    {
					new Task(() => temp()).Start();
			    }
		    }

			public void Close()
			{
				var temp = CloseEvent;
				if (temp != null)
				{
					new Task(() => temp()).Start();
				}
			}

		    public void Maximize()
		    {
			    var temp = MaximizeEvent;
			    if (temp != null)
			    {
					new Task(() => temp()).Start();
			    }
		    }
	    }

	    public readonly ChromiumWebBrowser WebView;
        public readonly BoundMessager Messager;
        public readonly ServerTable Servers;

        public MainWindow()
        {
            InitializeComponent();

            WindowSettings jsonSettings;
            if(File.Exists("window.json"))
            {
                jsonSettings = JsonConvert.DeserializeObject<WindowSettings>(File.ReadAllText("window.json"));
            }
            else
            {
                jsonSettings = new WindowSettings
                {
                    width = 1148,
                    height = 608,
                    enableComposting = false,
                    uri = "http://cdn.bmrf.me/UI.html"
                };
            }

			var settings = new CefSettings
			{
				PackLoadingDisabled = false
			};

	        if (!Cef.Initialize(settings)) return;


	        WebView = new ChromiumWebBrowser
	        {
		        BrowserSettings = new BrowserSettings
		        {
			        FileAccessFromFileUrlsAllowed = true,
					WebGlDisabled = true,
					JavaDisabled = true,	
                    UniversalAccessFromFileUrlsAllowed = true,
                    WebSecurityDisabled = false,
                    PluginsDisabled = false,
                    AcceleratedCompositingDisabled = !jsonSettings.enableComposting,
                   
		        },

	        };

			Browser.Children.Add(WebView);
            //WebView.LoadError += WebView_LoadError;

            //WebView.RequestHandler = new LocalFileResourceHandler();
            WebView.Address = "http://cdn.bmrf.me/UI.html"; //Jamie. Point me at the WebUI folder. 
            Messager = new BoundMessager(this);
			Messager.CloseEvent += MessagerOnCloseEvent;
			Messager.MinimizeEvent += MessagerMinimizeEvent;
			Messager.MaximizeEvent += MessagerOnMaximizeEvent;
            WebView.RegisterJsObject("launcher", Messager);
            Servers = new ServerTable();
            Servers.Register(WebView);

            WebView.ShowDevTools();

        }

        void WebView_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(WebView.ShowDevTools));
        }

        void WebView_LoadError(object sender, LoadErrorEventArgs e)
        {
            //MessageBox.Show("Load Error");
        }


	    private void MessagerOnMaximizeEvent()
	    {
			Dispatcher.Invoke(() =>
			{
				WindowState = WindowState.Maximized;
			});
	    }

	    private void MessagerOnCloseEvent()
	    {
		    Dispatcher.Invoke(Close);
	    }

	    private void MessagerMinimizeEvent()
	    {
		    Dispatcher.Invoke(() =>
		    {
			    WindowState = WindowState.Minimized;
		    });
	    }

	    private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
	    }
    }
}
