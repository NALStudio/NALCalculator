using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
        public MainPage()
        {
            this.InitializeComponent();

			ButtonHelper.Init(this);
        }

		private void B0_Click(object sender, RoutedEventArgs e)
		{
			ButtonHelper.Number(0);
		}

		private void B1_Click(object sender, RoutedEventArgs e)
		{
			ButtonHelper.Number(1);
		}

		private void B2_Click(object sender, RoutedEventArgs e)
		{
			ButtonHelper.Number(2);
		}

		private void B3_Click(object sender, RoutedEventArgs e)
		{
			ButtonHelper.Number(3);
		}

		private void B4_Click(object sender, RoutedEventArgs e)
		{
			ButtonHelper.Number(4);
		}

		private void B5_Click(object sender, RoutedEventArgs e)
		{
			ButtonHelper.Number(5);
		}

		private void B6_Click(object sender, RoutedEventArgs e)
		{
			ButtonHelper.Number(6);
		}

		private void B7_Click(object sender, RoutedEventArgs e)
		{
			ButtonHelper.Number(7);
		}

		private void B8_Click(object sender, RoutedEventArgs e)
		{
			ButtonHelper.Number(8);
		}

		private void B9_Click(object sender, RoutedEventArgs e)
		{
			ButtonHelper.Number(9);
		}

		private void BComma_Click(object sender, RoutedEventArgs e)
		{
			ButtonHelper.Comma();
		}

		private void BPlus_Click(object sender, RoutedEventArgs e)
		{
			ButtonHelper.Operation(Operation.Add);
		}

		private void BMinus_Click(object sender, RoutedEventArgs e)
		{
			ButtonHelper.Operation(Operation.Substract);
		}

		private void BMultiply_Click(object sender, RoutedEventArgs e)
		{
			ButtonHelper.Operation(Operation.Multiply);
		}

		private void BDivide_Click(object sender, RoutedEventArgs e)
		{
			ButtonHelper.Operation(Operation.Divide);
		}

		private void BPercent_Click(object sender, RoutedEventArgs e)
		{
			ButtonHelper.Percent();
		}

		private void BInverse_Click(object sender, RoutedEventArgs e)
		{
			ButtonHelper.Inverse();
		}

		private void BEquals_Click(object sender, RoutedEventArgs e)
		{
			ButtonHelper.Result();
		}

		private void BClear_Click(object sender, RoutedEventArgs e)
		{
			ButtonHelper.Clear();
		}
	}
}
