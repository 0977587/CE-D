using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CalculationEngine.Models
{
    public class Calculation
    {
        public Calculation LeftHandValue { get; set; }
        public Calculation RightHandValue { get; set; }
        public float? CurrentValue { get; set; }
        public Func<float, float, float> Function { get; set; }
        public float Solve()
        {
            if (CurrentValue.HasValue)
            {
                return CurrentValue.Value;
            }
            if (!LeftHandValue.CurrentValue.HasValue)
            {
                LeftHandValue.CurrentValue = LeftHandValue.Solve();
            }
            if (!RightHandValue.CurrentValue.HasValue)
            {
                RightHandValue.CurrentValue = RightHandValue.Solve();
            }
            CurrentValue = Function(LeftHandValue.CurrentValue.Value, RightHandValue.CurrentValue.Value);
            return CurrentValue.Value;
        }

        public void test()
        {
            var RightHandValue = new Calculation()
            {
                LeftHandValue = new Calculation() { CurrentValue = 3 },
                RightHandValue = new Calculation() { CurrentValue = 5 },
                Function = (left, right) => left + right
            };
        }

        public void setOperation(string expression)
        {
            switch (expression)
            {
                case "add":
                    Function = (left, right) => left + right;
                    return;
                case "sub":
                    Function = (left, right) => left - right;
                    return;
                case "mul":
                    Function = (left, right) => left * right;
                    return;
                case "div":
                    Function = (left, right) => left / right;
                    return;
                case "mod":
                    Function = (left, right) => left % right;
                    return;
                case "exp":
                    Function = (left, right) => (int)left ^ (int)right;
                    return;
                default:
                    Function = (left, right) => 0;
                    return; 
            }
            /* Addition +,
            Subtraction -,
            Multiplication *,
            Division /,
            Remainder %,
            Exponentiation * **/
        }

        public string getOperation(string expression)
        {
            switch (expression)
            {
                case "add":
                    return "+" ;
                case "sub":
                    return "-";
                case "mul":
                    return "*";
                case "div":
                    return "/";
                case "mod":
                    return "%";
                case "exp":
                    return "^";
                default:
                    return "";
            }
            /* Addition +,
            Subtraction -,
            Multiplication *,
            Division /,
            Remainder %,
            Exponentiation * **/
        }

    }
}

