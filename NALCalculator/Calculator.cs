using NALCalculator.Extensions;
using Numerics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NALCalculator
{
	public enum Operation
	{
		Add = '+',
		Substract = '−',
		Multiply = '×',
		Divide = '÷'
	}

	public enum Bracket
	{
		Start = '(',
		End = ')'
	}

	public class CalculationException : Exception
	{
		public CalculationException() : base()
		{
		}

		public CalculationException(string message) : base(message)
		{
		}

		public CalculationException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}

	public class CalculationResult
	{
		public BigRational Result { get; set; }
		public string Error { get; private set; }

		public override string ToString()
		{
			throw new ArgumentException("No visible decimals argument given!");
		}

		public string ToString(int precision) => Result.ToString(precision);

		public string ToString(bool allowRational, ushort precisionLimit) => Result.ToString(allowRational, precisionLimit);

		internal CalculationResult(BigRational result)
		{
			Result = result;
			Error = null;
		}

		internal CalculationResult(string error)
		{
			Result = BigRational.MinusOne;
			Error = error;
		}
	}

	public class Calculation
	{
		// Contains Bracket, Operation and BigRational.
		private readonly List<object> values = new List<object>();
		public int Length { get { return values.Count; } }

		public void Set(BigRational rational)
		{
			if (values[values.Count - 1] is Calculation c)
			{
				c.Set(rational);
				return;
			}

			values[values.Count - 1] = rational;
		}

		public override string ToString()
		{
			string output = string.Empty;
			for (int i = 0; i < values.Count; i++)
			{
				object rawVal = values[i];
				if (rawVal is Operation val)
					output += (char)val;
				else if (rawVal is Bracket br)
					output += (char)br;
				else if (rawVal is BigRational bigRat)
					output += bigRat.ToString(true, 20);
				else if (rawVal is Calculation calc)
					output += calc;
				else
					return "<tostring-type-error>";

				output += " ";
			}
			return output;
		}

		public void Next(Operation operation)
		{
			if (values.Count < 1)
				values.Add(BigRational.Zero);
			values.Add(operation);
			values.Add(BigRational.Zero);
		}

		void StartBracket()
		{
			Calculation tmpCalc = new Calculation((BigRational)values[values.Count - 1]);
			values[values.Count - 1] = Bracket.Start;
			values.Add(tmpCalc);
		}

		void EndBracket()
		{
			values.Add(Bracket.End);
		}

		public void AutoBracket()
		{
			object last = values[values.Count - 1];
			if (last is Calculation c)
			{
				c.AutoBracket();
				return;
			}
		}

		List<object> CalculateBrackets(List<object> toCalculate)
		{
			List<object> toC = new List<object>(toCalculate);
			for (int i = 0; i < toC.Count; i++)
			{
				object rawValue = toC[i];
				if (rawValue is Calculation calc)
				{
					toC[i] = calc.Result();

					if (!(toC[i - 1] is Bracket))
						throw new CalculationException("Invalid brackets around calculation.");

					if (toC[i + 1] is Bracket)
						toC.RemoveAt(i + 1); // Before minus so that the list isn't changed before removing this value.
					
					toC.RemoveAt(i - 1);
					i--; // To fix iteration
				}
			}
			return toC;
		}

		List<object> CalculateOperations(List<object> toCalculate, params Operation[] operations)
		{
			List<object> toC = new List<object>(toCalculate);
			for (int i = 0; i < toC.Count; i++)
			{
				object rawValue = toC[i];
				if (rawValue is Operation value)
				{
					if (!operations.Contains(value))
						continue;

					if (toC[i - 1] is CalculationResult res1)
					{
						if (res1.Error == null)
							toC[i - 1] = res1.Result;
						else
							throw new CalculationException(res1.Error);
					}
					if (toC[i + 1] is CalculationResult res2)
					{
						if (res2.Error == null)
							toC[i + 1] = res2.Result;
						else
							throw new CalculationException(res2.Error);
					}

					BigRational v1 = (BigRational)toC[i - 1];
					BigRational v2 = (BigRational)toC[i + 1];
					toC.RemoveAt(i + 1); // Before minus so that the list isn't changed before removing this value.
					toC.RemoveAt(i - 1);
					i--; // To fix iteration

					switch (value)
					{
						case Operation.Multiply:
							toC[i] = v1 * v2;
							break;
						case Operation.Divide:
							toC[i] = v1 / v2;
							break;
						case Operation.Add:
							toC[i] = v1 + v2;
							break;
						case Operation.Substract:
							toC[i] = v1 - v2;
							break;
					}
				}
			}
			return toC;
		}

		public CalculationResult Result()
		{
			List<object> calc = new List<object>(values);
			try
			{
				CalculateBrackets(calc);
			}
			catch (Exception e)
			{
				return new CalculationResult(e.Message);
			}

			try
			{
				calc = CalculateOperations(calc, Operation.Multiply, Operation.Divide);
			}
			catch (DivideByZeroException)
			{
				return new CalculationResult("Division by zero.");
			}
			catch (Exception e)
			{
				return new CalculationResult(e.Message);
			}

			try
			{
				calc = CalculateOperations(calc, Operation.Add, Operation.Substract);
			}
			catch (Exception e)
			{
				return new CalculationResult(e.Message);
			}

			if (calc.Count != 1)
				return new CalculationResult("<count-error>");
			if (!(calc[0] is BigRational))
				return new CalculationResult("<type-error>");

			return new CalculationResult((BigRational)calc[0]);
		}

		public Calculation(BigRational initValue)
		{
			values.Add(initValue);
		}
	}
}
