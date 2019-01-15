using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x415

namespace Index
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        #region Props
        public ObservableCollection<StorageFile> Files1 { get; set; } = new ObservableCollection<StorageFile>();
        public ObservableCollection<StorageFile> Files2 { get; set; } = new ObservableCollection<StorageFile>();
        #endregion

        public static event EventHandler IconOptionChanged;

        public MainPage()
        {
            this.InitializeComponent();
            Color color = new Color() { R = 0x7F, G = 0x7A, B = 0x6F, A = 0xFF };
            Application.Current.Resources["SystemControlHighlightListAccentLowBrush"] = new SolidColorBrush(color);
            Application.Current.Resources["SystemControlHighlightListAccentMediumBrush"] = new SolidColorBrush(color);
        }

        #region Events Handlers
        private async void Folder1_Button_ClickAsync(object sender, RoutedEventArgs e)
        {
            var folder = await TakeFolderAccess(this.Folder1Name_TextBlock);
            if (folder != null)
            {
                LoadFilesList(folder, Files1, Progress1_Grid);
            } 
        }

        private async void Folder2_Button_ClickAsync(object sender, RoutedEventArgs e)
        {
            var folder = await TakeFolderAccess(this.Folder2Name_TextBlock);
            if (folder != null)
            {
                LoadFilesList(folder, Files2, Progress2_Grid);
            }    
        }

        private void Menu_SplitView_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            var xPosition = e.GetCurrentPoint(Menu_SplitView).Position.X;
            if (xPosition <= Menu_SplitView.CompactPaneLength)
            {
                Menu_SplitView.IsPaneOpen = true;
                OpenPane_Storyboard.Begin();
            }
            else if (xPosition > Menu_SplitView.OpenPaneLength)
            {
                Menu_SplitView.IsPaneOpen = false;
                ClosePane_Storyboard.Begin();
            }
        }

        private void IconSize_RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender == NoneIcon_RadioButton)
            {
                Globals.PictureMode = PictureMode.None;
            }
            else if (sender == SmallIcon_RadioButton)
            {
                Globals.PictureMode = PictureMode.Small;
            }
            else if (sender == BigIcon_RadioButton)
            {
                Globals.PictureMode = PictureMode.Big;
            }

            if (IconOptionChanged != null)
            {
                IconOptionChanged(this, null);
            }
        }

        private void AscendingSort_Button_Click(object sender, RoutedEventArgs e)
        {
            Files1 = new ObservableCollection<StorageFile>(Files1.OrderBy(u => u.Name));
            Files2 = new ObservableCollection<StorageFile>(Files2.OrderBy(u => u.Name));
            Files1_ListBox.ItemsSource = Files1;
            Files2_ListBox.ItemsSource = Files2;
        }

        private void DescendingSort_Button_Click(object sender, RoutedEventArgs e)
        {
            Files1 = new ObservableCollection<StorageFile>(Files1.OrderByDescending(u => u.Name));
            Files2 = new ObservableCollection<StorageFile>(Files2.OrderByDescending(u => u.Name));
            Files1_ListBox.ItemsSource = Files1;
            Files2_ListBox.ItemsSource = Files2;
        }

        private void Reduct_Button_Click(object sender, RoutedEventArgs e)
        {
            //Hide Leyout
            Menu_SplitView.Visibility = Visibility.Collapsed;
            DuplicateReductionLoading_Grid.Visibility = Visibility.Visible;
            FilesReduction_ProgressRing.IsActive = true;

            //Skip all that have name eqivalent
            var newFiles1 = Files1.Where( u => !Files2.Any(v => v.Name == u.Name ) ).ToList();
            var newFiles2 = Files2.Where( u => !Files1.Any(v => v.Name == u.Name ) ).ToList();

            Files1 = new ObservableCollection<StorageFile>(newFiles1);
            Files2 = new ObservableCollection<StorageFile>(newFiles2);
            Files1_ListBox.ItemsSource = Files1;
            Files2_ListBox.ItemsSource = Files2;

            //Show Leyout
            Menu_SplitView.Visibility = Visibility.Visible;
            DuplicateReductionLoading_Grid.Visibility = Visibility.Collapsed;
            FilesReduction_ProgressRing.IsActive = false;
        }

        private void Files1_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var slectedItemNumber = (sender as ListBox).SelectedItems?.Count;
            Selected1_TextBlock.Text = slectedItemNumber.ToString();
        }

        private void Files2_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var slectedItemNumber = (sender as ListBox).SelectedItems?.Count;
            Selected2_TextBlock.Text = slectedItemNumber.ToString();
        }

        private void SelectAll1_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Files1_ListBox.SelectedItems.Count < Files1_ListBox.Items.Count)
            {
                Files1_ListBox.SelectAll();
            }
            else
            {
                Files1_ListBox.SelectedIndex = -1;
            }
        }

        private void SelectAll2_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Files2_ListBox.SelectedItems.Count < Files2_ListBox.Items.Count)
            {
                Files2_ListBox.SelectAll();
            }
            else
            {
                Files2_ListBox.SelectedIndex = -1;
            }
        }
        #endregion

        #region Helpers
        private async Task<StorageFolder> TakeFolderAccess(TextBlock textBlock)
        {
            var folderPicker = new Windows.Storage.Pickers.FolderPicker();
            folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
            folderPicker.FileTypeFilter.Add("*");

            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                textBlock.Text = "Wybrany folder: " + folder.Name;
                return folder;
            }
            else
            {
                textBlock.Text = "Anulowano";
                return null;
            }
        }

        private void ToggleLoadingVisibility(Grid grid)
        {
            if (grid.Visibility == Visibility.Collapsed)
            {
                grid.Visibility = Visibility.Visible;
            }
            else
            {
                grid.Visibility = Visibility.Collapsed;
            }

            var stackPanel = grid.Children.First(u => u.GetType() == typeof(StackPanel)) as StackPanel;
            var progressRing = stackPanel.Children.First(u => u.GetType() == typeof(ProgressRing)) as ProgressRing;
            progressRing.IsActive = !progressRing.IsActive;
        }

        #endregion

        #region Others
        private async Task LoadFilesList(StorageFolder folder, ICollection<StorageFile> files, Grid grid)
        {
            ToggleLoadingVisibility(grid);
            files.Clear();
            await GetFilesFromFolderAsync(folder, files);
            ToggleLoadingVisibility(grid);
        }

        private async Task GetFilesFromFolderAsync(StorageFolder folder, ICollection<StorageFile> files)
        {
            var items = await folder.GetItemsAsync();
            foreach (var item in items)
            {
                if (item.IsOfType(StorageItemTypes.File))
                {
                    files.Add(item as StorageFile);
                }
                else if (item.IsOfType(StorageItemTypes.Folder))
                {
                    await GetFilesFromFolderAsync(item as StorageFolder, files);
                }
                else
                {
                    throw new Exception("Wird Exception");
                }
            }

        }
        #endregion

    }
}
