using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UWPMarvelHeroExplorer.Models;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UWPMarvelHeroExplorer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public ObservableCollection<Character> Characters;

        public MainPage()
        {
            this.InitializeComponent();
            Characters = new ObservableCollection<Character>();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            MainProgressRing.IsActive = true;
            MainProgressRing.Visibility = Visibility.Visible;

            await MarvelFacade.UpdateCharacterListAsync(Characters);

            MainProgressRing.IsActive = false;
            MainProgressRing.Visibility = Visibility.Collapsed;
            AttributeTextBlock.Visibility = Visibility.Visible;
        }
    }
}
