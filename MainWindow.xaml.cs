using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PieLaunch.Windows;
using System.IO;
using System.Runtime.InteropServices;

namespace PieLaunch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ApplicationsDataGrid.SelectionChanged += ApplicationsDataGrid_SelectionChanged;
            PathTextBox.TextChanged += PathTextBox_TextChanged;
            RefreshApplicationList();
        }

        private void RefreshApplicationList()
        {
            var windows = WindowManager.GetTaskbarVisibleWindows();
            ApplicationsDataGrid.ItemsSource = windows;
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshApplicationList();
        }

        private void ApplicationsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool hasSelection = ApplicationsDataGrid.SelectedItem != null;
            LaunchSelectedButton.IsEnabled = hasSelection;
            MaximizeButton.IsEnabled = hasSelection;
            MinimizeButton.IsEnabled = hasSelection;
            RestoreButton.IsEnabled = hasSelection;

            // Enable snap buttons
            SnapLeftButton.IsEnabled = hasSelection;
            SnapRightButton.IsEnabled = hasSelection;
            SnapTopButton.IsEnabled = hasSelection;
            SnapBottomButton.IsEnabled = hasSelection;
            SnapTopLeftButton.IsEnabled = hasSelection;
            SnapTopRightButton.IsEnabled = hasSelection;
            SnapBottomLeftButton.IsEnabled = hasSelection;
            SnapBottomRightButton.IsEnabled = hasSelection;
        }

        private void LaunchSelectedButton_Click(object sender, RoutedEventArgs e)
        {
            if (ApplicationsDataGrid.SelectedItem is WindowManager.WindowInfo selectedWindow)
            {
                try
                {
                    if (string.IsNullOrEmpty(selectedWindow.ExecutablePath))
                    {
                        // If we don't have the executable path, just focus the window
                        WindowManager.FocusWindow(selectedWindow.Handle);
                    }
                    else
                    {
                        // Use the launch or focus method
                        WindowManager.LaunchOrFocusApp(selectedWindow.ExecutablePath);
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Error: {ex.Message}", "Launch Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (ApplicationsDataGrid.SelectedItem is WindowManager.WindowInfo selectedWindow)
            {
                try
                {
                    WindowManager.MaximizeWindow(selectedWindow.Handle);
                    RefreshApplicationList();
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Error maximizing window: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (ApplicationsDataGrid.SelectedItem is WindowManager.WindowInfo selectedWindow)
            {
                try
                {
                    WindowManager.MinimizeWindow(selectedWindow.Handle);
                    RefreshApplicationList();
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Error minimizing window: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void RestoreButton_Click(object sender, RoutedEventArgs e)
        {
            if (ApplicationsDataGrid.SelectedItem is WindowManager.WindowInfo selectedWindow)
            {
                try
                {
                    WindowManager.RestoreWindow(selectedWindow.Handle);
                    RefreshApplicationList();
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Error restoring window: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SnapLeftButton_Click(object sender, RoutedEventArgs e)
        {
            if (ApplicationsDataGrid.SelectedItem is WindowManager.WindowInfo selectedWindow)
            {
                try
                {
                    WindowManager.SnapWindowToLeftHalf(selectedWindow.Handle);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Error snapping window: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SnapRightButton_Click(object sender, RoutedEventArgs e)
        {
            if (ApplicationsDataGrid.SelectedItem is WindowManager.WindowInfo selectedWindow)
            {
                try
                {
                    WindowManager.SnapWindowToRightHalf(selectedWindow.Handle);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Error snapping window: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SnapTopButton_Click(object sender, RoutedEventArgs e)
        {
            if (ApplicationsDataGrid.SelectedItem is WindowManager.WindowInfo selectedWindow)
            {
                try
                {
                    WindowManager.SnapWindowToTopHalf(selectedWindow.Handle);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Error snapping window: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SnapBottomButton_Click(object sender, RoutedEventArgs e)
        {
            if (ApplicationsDataGrid.SelectedItem is WindowManager.WindowInfo selectedWindow)
            {
                try
                {
                    WindowManager.SnapWindowToBottomHalf(selectedWindow.Handle);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Error snapping window: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SnapTopLeftButton_Click(object sender, RoutedEventArgs e)
        {
            if (ApplicationsDataGrid.SelectedItem is WindowManager.WindowInfo selectedWindow)
            {
                try
                {
                    WindowManager.SnapWindowToTopLeft(selectedWindow.Handle);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Error snapping window: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SnapTopRightButton_Click(object sender, RoutedEventArgs e)
        {
            if (ApplicationsDataGrid.SelectedItem is WindowManager.WindowInfo selectedWindow)
            {
                try
                {
                    WindowManager.SnapWindowToTopRight(selectedWindow.Handle);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Error snapping window: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SnapBottomLeftButton_Click(object sender, RoutedEventArgs e)
        {
            if (ApplicationsDataGrid.SelectedItem is WindowManager.WindowInfo selectedWindow)
            {
                try
                {
                    WindowManager.SnapWindowToBottomLeft(selectedWindow.Handle);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Error snapping window: {ex.Message}", "Error",
                       MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SnapBottomRightButton_Click(object sender, RoutedEventArgs e)
        {
            if (ApplicationsDataGrid.SelectedItem is WindowManager.WindowInfo selectedWindow)
            {
                try
                {
                    WindowManager.SnapWindowToBottomRight(selectedWindow.Handle);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Error snapping window: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void PathTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            LaunchButton.IsEnabled = !string.IsNullOrWhiteSpace(PathTextBox.Text) &&
                                   File.Exists(PathTextBox.Text);
        }

        private void PathTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter && LaunchButton.IsEnabled)
            {
                LaunchButton_Click(sender, e);
            }
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*",
                FilterIndex = 1
            };

            if (openFileDialog.ShowDialog() == true)
            {
                PathTextBox.Text = openFileDialog.FileName;
            }
        }

        private void LaunchButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WindowManager.LaunchOrFocusApp(PathTextBox.Text);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error: {ex.Message}", "Launch Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}