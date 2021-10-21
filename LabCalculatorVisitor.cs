using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;

namespace LabCalculator
{
    class LabCalculatorVisitor : ExpressionGrammarBaseVisitor<double>
    {
        public override double VisitCompileUnit(ExpressionGrammarParser.CompileUnitContext context)
        {
            return Visit(context.expression());
        }

        //IdentifierExpression


        public override double VisitParenthesizedExpression(ExpressionGrammarParser.ParenthesizedExpressionContext context)
        {
            return Visit(context.expression());
        }

        public override double VisitExponentExpression(ExpressionGrammarParser.ExponentExpressionContext context)
        {
            var left = WalkLeft(context);
            var right = WalkRight(context);

            Debug.WriteLine("{0} ^ {1}", left, right);
            return System.Math.Pow(left, right);
        }

        public override double VisitDivExpression(ExpressionGrammarParser.DivExpressionContext context)
        {
            try
            {
                var left = WalkLeft(context);
                var right = WalkRight(context);

                if (right == 0) throw new Exception("Divide by 0");

                Debug.WriteLine("{0} div {1}", left, right);

                return (left - (left % right)) / right;
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return double.PositiveInfinity;
        }

        public override double VisitModExpression(ExpressionGrammarParser.ModExpressionContext context)
        {
            try
            {
                var left = WalkLeft(context);
                var right = WalkRight(context);
                if (right == 0) throw new Exception("Divide by 0");

                Debug.WriteLine("{0} mod {1}", left, right);
                return left % right;
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return double.PositiveInfinity;
        }
        public override double VisitDivideExpression(ExpressionGrammarParser.DivideExpressionContext context)
        {
            try
            {
                var left = WalkLeft(context);
                var right = WalkRight(context);
                if (right == 0) throw new Exception("Divide by 0");

                Debug.WriteLine("{0} / {1}", left, right);
                return left / right;
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return double.PositiveInfinity;
        }
        public override double VisitMultiplyExpression(ExpressionGrammarParser.MultiplyExpressionContext context)
        {
            var left = WalkLeft(context);
            var right = WalkRight(context);

            Debug.WriteLine("{0} * {1}", left, right);
            return left * right;
        }
        public override double VisitNumberExpression(ExpressionGrammarParser.NumberExpressionContext context)
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US", false);

            var result = Convert.ToDouble(context.GetText());
            Debug.WriteLine(result);

            return result;
        }
        public override double VisitAddExpression(ExpressionGrammarParser.AddExpressionContext context)
        {
            var left = WalkLeft(context);
            var right = WalkRight(context);

            Debug.WriteLine("{0} + {1}", left, right);
            return left + right;
        }
        public override double VisitSubtractExpression(ExpressionGrammarParser.SubtractExpressionContext context)
        {
            var left = WalkLeft(context);
            var right = WalkRight(context);

            Debug.WriteLine("{0} - {1}", left, right);
            return left - right;
        }

        public override double VisitMinusExpression(ExpressionGrammarParser.MinusExpressionContext context)
        {
            var expression = WalkLeft(context);
            return -1 * expression;
        }

        public override double VisitPlusExpression([NotNull] ExpressionGrammarParser.PlusExpressionContext context)
        {
            var expression = WalkLeft(context);
            return expression;
        }

        private double WalkLeft(ExpressionGrammarParser.ExpressionContext context)
        {
            return Visit(context.GetRuleContext<ExpressionGrammarParser.ExpressionContext>(0));
        }

        private double WalkRight(ExpressionGrammarParser.ExpressionContext context)
        {
            return Visit(context.GetRuleContext<ExpressionGrammarParser.ExpressionContext>(1));
        }
    }
}
