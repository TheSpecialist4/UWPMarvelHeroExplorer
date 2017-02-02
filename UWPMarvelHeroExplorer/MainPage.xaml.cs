using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UWPMarvelHeroExplorer.Models;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UWPMarvelHeroExplorer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = false;
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MainSplitView.IsPaneOpen = !MainSplitView.IsPaneOpen;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (HomeListBoxItem.IsSelected) {
                MainFrame.Navigate(typeof(Homepage));
                HeadingTextBlock.Text = "Home";
            } else {
                MainFrame.Navigate(typeof(AboutPage));
                HeadingTextBlock.Text = "About";
            }
            MainSplitView.IsPaneOpen = false;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            HeadingTextBlock.Text = "Home";
            MainFrame.Navigate(typeof(Homepage));
        }
    }
}
