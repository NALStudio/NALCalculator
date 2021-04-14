using NALCalculator.Extensions;
using NALCalculator.Numerics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace NALCalculator
{
	public static class ButtonHelper
	{
		static TextBlock clc = null;
		static TextBlock txt = null;
		static TextBlock rslt = null;

		static BigRational rational;
		static CalculationResult result;
		static int fractionIndex = 0;
		static Calculation calculation;
		static string calculationString;
		public static int VisibleDecimals { get; private set; }
		public static int ResultDecimals { get; private set; }

		static void Reset()
		{
			rational = BigRational.Zero;
			result = null;
			fractionIndex = 0;
			calculation = new Calculation(rational);
			calculationString = string.Empty;
			VisibleDecimals = 10;
			ResultDecimals = 50;
		}

		public static void Init(MainPage mainPage)
		{
			clc = (TextBlock)mainPage.FindName("CalculationText");
			txt = (TextBlock)mainPage.FindName("NumberText");
			rslt = (TextBlock)mainPage.FindName("ResultText");

			Reset();

			UpdateText();

			rslt.Text = string.Empty;
		}

		public static void Number(byte number)
		{
			if (calculationString.EndsWith('%'))
				return;

			if (fractionIndex < 1)
			{
				rational *= 10;
				rational += number;
			}
			else if (fractionIndex < int.MaxValue - 1)
			{
				BigRational modNumber = new BigRational(new BigInteger(number));
				modNumber /= BigInteger.Pow(new BigInteger(10), fractionIndex);
				rational += modNumber;

				fractionIndex++;
			}

			calculation.Set(rational);

			calculationString += number.ToString();

			UpdateText();
		}

		public static void Operation(Operation operation)
		{
			calculation.Next(operation);

			calculationString = string.Empty;
			rational = BigRational.Zero;
			fractionIndex = 0;

			UpdateText();
		}

		public static void Comma()
		{
			if (fractionIndex > 0)
				return;

			fractionIndex++;

			calculationString += ",";
			UpdateText();
		}

		public static void Percent()
		{
			if (calculationString.Length < 1)
				calculationString += "0";

			switch (calculationString[calculationString.Length - 1])
			{
				case '%':
					return;
				case ',':
					Number(0);
					break;
			}

			calculationString += '%';
			rational /= 100;
			calculation.Set(rational);

			UpdateText();
		}

		public static void Inverse()
		{
			rational *= -1;
			calculation.Set(rational);

			if (rational >= 0 && calculationString.StartsWith('-'))
				calculationString = calculationString.TrimStart('-');
			else if (!calculationString.StartsWith('-'))
				calculationString = calculationString.Insert(0, "-");

			UpdateText();
		}

		public static void Result()
		{
			if (result.Error != null)
				return;

			rational = result.Result;
			calculation = new Calculation(rational);
			calculationString = result.ToString(VisibleDecimals);
			string[] tmpSplit = calculationString.Split(',', 2);
			fractionIndex = tmpSplit.Length < 2 ? 0 : tmpSplit[tmpSplit.Length - 1].Length + 1;

			UpdateText();

			rslt.Text = string.Empty;
		}

		public static void Clear()
		{
			Reset();

			UpdateText();
		}

		public static void SetResultText(int? decimals)
		{
			if (decimals is int d)
				ResultDecimals = d;
			rslt.Text = result.Error ?? result.ToString(ResultDecimals);
		}

		static void UpdateText()
		{
			result = calculation.Result();
			SetResultText(null);

			if (calculationString.Length > 0 && !(calculationString.StartsWith('<') && calculationString.EndsWith('>')))
			{
				string[] calcSplit = calculationString.Split(',');

				int iterStart = calcSplit[0].Length - 3;
				int iterEnd = 0;
				if (calcSplit[0].EndsWith('%'))
					iterStart--;
				if (calcSplit[0].StartsWith('-'))
					iterEnd++;

				for (int i = iterStart; i > iterEnd; i -= 3)
					calcSplit[0] = calcSplit[0].Insert(i, " ");

				txt.Text = string.Join(',', calcSplit);
			}
			else
			{
				txt.Text = "0";
			}

			clc.Text = calculation.ToString();
		}
	}
}
