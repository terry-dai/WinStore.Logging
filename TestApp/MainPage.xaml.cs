using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WinStore.Logging;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TestApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            LoggingHelper.Verbose("button content is '{0}', clicked", (sender as Button).Content);

            var item = await ApplicationData.Current.LocalFolder.TryGetItemAsync(LoggingHelper.GetLogFilename());
            if (item != null)
                this.filePath.Text = item.Path;

            LoggingHelper.SetEventLevel(EventLevel.Error);
        }


        private void Grid_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            LoggingHelper.Error("Grid_PointerPressed Error test");
        }
    }
}
