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
        public ObservableCollection<Character> Characters;
        public ObservableCollection<Comic> Comics;

        private BitmapImage CurrentImage;

        public MainPage()
        {
            this.InitializeComponent();
            Characters = new ObservableCollection<Character>();
            Comics = new ObservableCollection<Comic>();
            CurrentImage = new BitmapImage();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            DetailsGrid.Visibility = Visibility.Collapsed;

            MainProgressRing.IsActive = true;
            MainProgressRing.Visibility = Visibility.Visible;

            try {
                while (Characters.Count < 20) {
                    await MarvelFacade.UpdateCharacterListAsync(Characters);
                }
            }
            catch (Exception) {

            }

            MainProgressRing.IsActive = false;
            MainProgressRing.Visibility = Visibility.Collapsed;

            AttributeTextBlock.Visibility = Visibility.Visible;
            ListHeadingTextBlock.Visibility = Visibility.Visible;
        }

        private async void CharactersListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (DetailsGrid.Visibility == Visibility.Collapsed) DetailsGrid.Visibility = Visibility.Visible;

            MainProgressRing.IsActive = true;
            MainProgressRing.Visibility = Visibility.Visible;

            var selectedCharacter = e.ClickedItem as Character;

            var imageSource = new BitmapImage();
            var imageLocation = new Uri(selectedCharacter.thumbnail.large, UriKind.Absolute);
            imageSource.UriSource = imageLocation;

            CurrentImage = imageSource;

            CharacterDetailsImage.Source = imageSource;

            CharacterNameTextBlock.Text = selectedCharacter.name;
            DescriptionTextBlock.Text = selectedCharacter.description;

            await MarvelFacade.UpdateComicListAsync(Comics, selectedCharacter.id);

            ImageGrid.Visibility = Visibility.Visible;
            FeaturedTextBlock.Visibility = Visibility.Visible;

            MainProgressRing.IsActive = false;
            MainProgressRing.Visibility = Visibility.Collapsed;
        }

        private void CharacterDetailsImage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            FlyoutImage.Source = CurrentImage;
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
            MainGridBlurStoryboard.Begin();
        }

        private void CharacterImageFlyout_Closed(object sender, object e)
        {
            MainGridShowStoryboard.Begin();
        }

        private void ComicImage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
            MainGridBlurStoryboard.Begin();
        }
    }
}
