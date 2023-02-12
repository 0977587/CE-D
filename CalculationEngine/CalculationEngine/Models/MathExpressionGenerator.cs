using System.Linq.Expressions;
using System;
using System.Diagnostics.CodeAnalysis;

namespace CalculationEngine.Models
{
    [ExcludeFromCodeCoverage]

    public class MathExpressionGenerator
    {
        public const string SupportedOps = "+-*/^";
        private static readonly ParameterExpression Parameter = Expression.Parameter(typeof(double), "x");

        public static Expression<Func<double, double>> GetExpression(string s)
        {
            ParameterExpression parameterExpression = Expression.Parameter(typeof(double), "x");
            Expression result = GetExpressionInternal(s);
            return Expression.Lambda<Func<double, double>>(result, Parameter);
        }

        private static Expression GetExpressionInternal(string s)
        {
            double constant;
            if (s == "x")
                return Parameter;
            if (double.TryParse(s, out constant))
                return Expression.Constant(constant, typeof(double));
            foreach (char op in SupportedOps)
            {
                var split = s.Split(new[] { op }, StringSplitOptions.RemoveEmptyEntries);
                if (split.Length > 1)
                {
                    var expression = GetExpressionInternal(split[0]);
                    for (int i = 1; i < split.Length; i++)
                    {
                        expression = RunOp(expression, GetExpressionInternal(split[i]), op);
                    }
                    return expression;
                }
            }
            throw new NotImplementedException("never throws");
        }

        private static Expression RunOp(Expression a, Expression b, char op)
        {
            switch (op)
            {
                case '+':
                    return Expression.Add(a, b);
                case '-':
                    return Expression.Subtract(a, b);
                case '/':
                    return Expression.Divide(a, b);
                case '*':
                    return Expression.Multiply(a, b);
                case '^':
                    return Expression.Power(a, b);
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
