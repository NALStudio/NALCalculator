using NALCalculator.Extensions;
using NALCalculator.Numerics;
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
        Divide = '÷',
        Exponent = '^',
        Unspecified = '?'
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

        public string ToString(int precision)
        {
            return Result.ToString(precision);
        }

        public string ToString(bool allowRational, ushort precisionLimit)
        {
            return Result.ToString(allowRational, precisionLimit);
        }

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
        const Operation replaceUnspecifiedBehaviour = Operation.Add;

        // Contains Bracket, Operation and BigRational.
        private readonly List<object> values = new List<object>();
        public int Length => values.Count;

        bool valueSetAfterNext = false;

        public void Set(BigRational rational)
        {
            valueSetAfterNext = true;

            if (values[values.Count - 1] is Calculation c)
            {
                c.Set(rational);
                return;
            }

            if (values.Count > 1 && values[values.Count - 2] is Operation op && op == Operation.Unspecified)
                values[values.Count - 2] = replaceUnspecifiedBehaviour;

            values[values.Count - 1] = rational;
        }

        public override string ToString()
        {
            string output = string.Empty;
            for (int i = 0; i < values.Count; i++)
            {
                object rawVal = values[i];
                switch (rawVal)
                {
                    case Operation val:
                        output += (char)(val != Operation.Unspecified ? val : Operation.Add);
                        break;
                    case Bracket br:
                        output += (char)br;
                        break;
                    case BigRational bigRat:
                        output += bigRat.ToString(true, 20);
                        break;
                    case Calculation calc:
                        output += calc.ToString();
                        break;
                    default:
                        return "<tostring-type-error>";
                }

                if (!output.EndsWith(' '))
                    output += " ";
            }
            return output;
        }

        public string ToString(bool noOperationsNull)
        {
            return noOperationsNull && values.Count < 2 ? null : ToString();
        }

        public void Next(Operation operation)
        {
            if (values[values.Count - 1] is Calculation c)
            {
                c.Next(operation);
                return;
            }

            if (!valueSetAfterNext && values.Count > 1)
            {
                if (values[values.Count - 2] is Operation op)
                {
                    if (op == Operation.Multiply && operation == Operation.Multiply)
                        values[values.Count - 2] = Operation.Exponent;
                    else
                        values[values.Count - 2] = operation;
                }
                else
                {
                    Debug.WriteLine("No operation found to override!");
                }
                return;
            }
            valueSetAfterNext = false;


            if (values.Count < 1)
                values.Add(BigRational.Zero);

            if (values.Count > 1 && values[values.Count - 2] is Operation o && o == Operation.Unspecified)
                values[values.Count - 2] = replaceUnspecifiedBehaviour;

            values.Add(operation);
            values.Add(BigRational.Zero);
        }

        public void StartBracket()
        {
            Calculation tmpCalc = new Calculation((BigRational)values[values.Count - 1]);
            if (values.Count < 1)
                values.Add(Bracket.Start);
            else
                values[values.Count - 1] = Bracket.Start;
            values.Add(tmpCalc);
        }

        public void EndBracket()
        {
            values.Add(Bracket.End);
        }

        List<object> CalculateBrackets(List<object> toCalculate)
        {
            List<object> toC = new List<object>(toCalculate);
            for (int i = 0; i < toC.Count; i++)
            {
                if (toC[i] is Calculation calc)
                {
                    CalculationResult tmpres = calc.Result();
                    if (tmpres.Error != null)
                        throw new CalculationException(tmpres.Error);
                    toC[i] = tmpres.Result;

                    if (i == 0)
                        throw new CalculationException("No bracket before first calculation!");
                    if (!(toC[i - 1] is Bracket))
                        throw new CalculationException("No brackets around calculation.");

                    if (i + 1 < toC.Count && toC[i + 1] is Bracket)
                        toC.RemoveAt(i + 1); // Before minus one so that the list isn't changed before removing this value.
                    
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

                    BigRational v1 = (BigRational)toC[i - 1];
                    BigRational v2 = (BigRational)toC[i + 1];
                    toC.RemoveAt(i + 1); // Before minus one so that the list isn't changed before removing this value.
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
                        case Operation.Exponent:
                            if (v2.GetFractionPart() != BigRational.Zero)
                                throw new CalculationException("Decimal number exponents are not supported yet.");
                            toC[i] = BigRational.Pow(v1, v2.GetWholePart());
                            break;
                        case Operation.Unspecified:
                            throw new CalculationException("Unspecified operations should be replaced before calling CalculateOperations!");
                        default:
                            throw new CalculationException("Invalid operation type!");
                    }
                }
            }
            return toC;
        }

        public CalculationResult Result()
        {
            List<object> calc = new List<object>(values);
            for (int i = 0; i < calc.Count; i++)
            {
                if (calc[i] is Operation o && o == Operation.Unspecified)
                    calc[i] = replaceUnspecifiedBehaviour;
            }

            try
            {
                calc = CalculateBrackets(calc);
                calc = CalculateOperations(calc, Operation.Exponent);
                calc = CalculateOperations(calc, Operation.Multiply, Operation.Divide);
                calc = CalculateOperations(calc, Operation.Add, Operation.Substract);
            }
            catch (DivideByZeroException)
            {
                return new CalculationResult("Division by zero.");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return new CalculationResult(e.Message);
            }

            if (calc.Count != 1)
                return new CalculationResult($"<count-error>: Count: {calc.Count}, Value types: {string.Join(", ", calc.Select(v => v.GetType().Name))}");

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
