using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace NALCalculator
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
		Grid grid;
		double defaultTopMargin;

        public MainPage()
        {
            this.InitializeComponent();

			grid = (Grid)FindName("MainGrid");
			defaultTopMargin = grid.Margin.Top;

			CoreApplication.GetCurrentView().TitleBar.LayoutMetricsChanged += TitleBar_LayoutMetricsChanged;

			ButtonHelper.Init(this);
        }

		private void TitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
		{
			grid.Margin = new Thickness(grid.Margin.Left, sender.Height + defaultTopMargin, grid.Margin.Right, grid.Margin.Bottom);
		}

		private void B0_Click(object _, RoutedEventArgs __)
		{
			ButtonHelper.Number(0);
		}

		private void B1_Click(object _, RoutedEventArgs __)
		{
			ButtonHelper.Number(1);
		}

		private void B2_Click(object _, RoutedEventArgs __)
		{
			ButtonHelper.Number(2);
		}

		private void B3_Click(object _, RoutedEventArgs __)
		{
			ButtonHelper.Number(3);
		}

		private void B4_Click(object _, RoutedEventArgs __)
		{
			ButtonHelper.Number(4);
		}

		private void B5_Click(object _, RoutedEventArgs __)
		{
			ButtonHelper.Number(5);
		}

		private void B6_Click(object _, RoutedEventArgs __)
		{
			ButtonHelper.Number(6);
		}

		private void B7_Click(object _, RoutedEventArgs __)
		{
			ButtonHelper.Number(7);
		}

		private void B8_Click(object _, RoutedEventArgs __)
		{
			ButtonHelper.Number(8);
		}

		private void B9_Click(object _, RoutedEventArgs __)
		{
			ButtonHelper.Number(9);
		}

		private void BComma_Click(object _, RoutedEventArgs __)
		{
			ButtonHelper.Comma();
		}

		private void BPlus_Click(object _, RoutedEventArgs __)
		{
			ButtonHelper.Operation(Operation.Add);
		}

		private void BMinus_Click(object _, RoutedEventArgs __)
		{
			ButtonHelper.Operation(Operation.Substract);
		}

		private void BMultiply_Click(object _, RoutedEventArgs __)
		{
			ButtonHelper.Operation(Operation.Multiply);
		}

		private void BDivide_Click(object _, RoutedEventArgs __)
		{
			ButtonHelper.Operation(Operation.Divide);
		}

		private void BPercent_Click(object _, RoutedEventArgs __)
		{
			ButtonHelper.Percent();
		}

		private void BInverse_Click(object _, RoutedEventArgs __)
		{
			ButtonHelper.Inverse();
		}

		private void BEquals_Click(object _, RoutedEventArgs __)
		{
			ButtonHelper.Result();
		}

		private void BClear_Click(object sender, RoutedEventArgs e)
		{
			ButtonHelper.Clear();
		}
	}
}
