using System;
using System.Collections;
using System.Text.RegularExpressions;

namespace FormulaEvaluator
{
    /// <summary>
    /// author:Qiaoyi Cai
    /// date:9/5/2022
    /// The class Evaluator creates the method Evalate to calculate the 
    /// input math expression
    /// </summary>
    public static class Evaluator
    {
        public static Stack<int> valueStack = new Stack<int>();
        public static Stack<string> operatorStack = new Stack<string>();
        public delegate int Lookup(String v);
        /// <summary>
        /// the method would passin math expression and return the integer result
        /// </summary>
        /// <param name="exp"></param> String expression of math
        /// <param name="variableEvaluator"></param> delegate method to return variable value
        /// <returns></returns> the integer result of expression
        /// <exception cref="ArgumentException"></exception> when the expected token or stack is not occured
        /// <exception cref="Exception"></exception>
        public static int Evaluate(String exp, Lookup variableEvaluator)
        {
            string cleanString = String.Concat(exp.Where(c => !Char.IsWhiteSpace(c)));
            // remove the whitespaces form expression
            string[] substrings = Regex.Split(cleanString, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");
            if (cleanString == "")
                throw new ArgumentException("undefined expression");
            int numericValue;

            foreach (String tokenInExp in substrings)
            {
                if (int.TryParse(tokenInExp, out numericValue))//if token is integer
                    IntegerCalculation(tokenInExp);
                else if (!String.IsNullOrEmpty(tokenInExp) && isVariable(tokenInExp))//if token is variable
                {
                    try
                    {
                        String stringVariableValue = variableEvaluator(tokenInExp).ToString();

                        if (!int.TryParse(stringVariableValue, out numericValue))// if variable doesn't exist
                            throw new Exception("varible doesn't exist");
                        int tokenVaribaleValue = variableEvaluator(tokenInExp);
                        String stringTokenVariableValue = tokenVaribaleValue.ToString();
                        IntegerCalculation(stringTokenVariableValue);
                    }
                    catch (ArgumentException)// if illegal variable happens
                    {
                        operatorStack.Clear();
                        valueStack.Clear();
                        throw new ArgumentException("illegal Variable");
                    }
                }
                // if the token is + or -
                else if (tokenInExp.Equals("+") || tokenInExp.Equals("-"))
                {
                    if (operatorStack.isOnTop("+") || operatorStack.isOnTop("-"))
                    {
                        pValueStackTwiceAndPopOpStack();
                    }
                    operatorStack.Push(tokenInExp);
                }
                //if token is * or /
                else if (tokenInExp.Equals("*") || tokenInExp.Equals("/"))
                    operatorStack.Push(tokenInExp);
                //if token is (
                else if (tokenInExp.Equals("("))
                    operatorStack.Push(tokenInExp);
                //if token is )
                else if (tokenInExp.Equals(")"))
                {
                    if (operatorStack.isOnTop("+") || operatorStack.isOnTop("-"))
                        pValueStackTwiceAndPopOpStack();
                    if (operatorStack.isOnTop("("))
                        operatorStack.Pop();
                    else
                    {
                        operatorStack.Clear();
                        valueStack.Clear();
                        throw new ArgumentException("expression has errors of format");
                    }
                    if (operatorStack.isOnTop("*") || operatorStack.isOnTop("/"))
                        pValueStackTwiceAndPopOpStack();
                }
                else if (tokenInExp.Equals("") || tokenInExp.Equals(" "))
                    continue;
            }
            // if there is 1 element in value stack
            if (operatorStack.Count == 0 && valueStack.Count == 1)
            {
                int result = valueStack.Pop();
                operatorStack.Clear();
                valueStack.Clear();
                return result;
            }
            //if there is 2 elements in value stack and 1 element in operator stack
            else if (operatorStack.Count == 1 && valueStack.Count == 2)
            {
                pValueStackTwiceAndPopOpStack();
                int result = valueStack.Pop();
                operatorStack.Clear();
                valueStack.Clear();
                return result;
            }
            //if errors occured
            else
            {
                operatorStack.Clear();
                valueStack.Clear();
                throw new ArgumentException("errors in expression after scaned all tokens");
            }

        }

        /// <summary>
        /// Determine if the token is variable
        /// </summary> determine if the token is variable 
        /// <param name="token"></param> input string
        /// <returns></returns> if the token is vairbale, return true, if not, return false
        private static bool isVariable(String token)
        {
            String varPattern = @"[a-zA-Z]+\d+";
            if (Regex.IsMatch(token, varPattern) == false)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        /// <summary>
        /// pop value stack twice and pop operator stack once
        /// </summary>
        private static void pValueStackTwiceAndPopOpStack()
        {
            try
            {
                int firstValue = Convert.ToInt32(valueStack.Pop());
                int secondValue = Convert.ToInt32(valueStack.Pop());
                String operators = (string)operatorStack.Pop();

                if (operators.Equals("+"))
                    valueStack.Push(firstValue + secondValue);
                else if (operators.Equals("-"))
                    valueStack.Push(secondValue - firstValue);
                else if (operators.Equals("*"))
                    valueStack.Push(firstValue * secondValue);
                else if (operators.Equals("/"))
                {
                    if (firstValue != 0)
                        valueStack.Push(secondValue / firstValue);
                    else
                        throw new Exception("dominator can't be 0");
                }
            }
            catch (Exception)
            {
                Console.WriteLine("there are no 2 values in value stack");
            }
        }
        /// <summary>
        /// if the token is integer, do the algorithm action 
        /// </summary>
        /// <param name="tokenInExpression"></param> inputed token
        /// <exception cref="ArgumentException"></exception> if the expression is wrong
        private static void IntegerCalculation(String tokenInExpression)
        {
            if (operatorStack.isOnTop("*") || operatorStack.isOnTop("/"))
            {
                int tokenNumberValue = Int32.Parse(tokenInExpression);
                int value = Convert.ToInt32(valueStack.Pop());

                if (operatorStack.isOnTop("*"))
                {
                    operatorStack.Pop();
                    int newValue = tokenNumberValue * value;
                    valueStack.Push(newValue);
                }
                else if (operatorStack.isOnTop("/"))
                {
                    operatorStack.Pop();
                    if (tokenNumberValue != 0)
                    {
                        int newValue = value / tokenNumberValue;
                        valueStack.Push(newValue);
                    }
                    else
                        throw new ArgumentException("Wrong expression");
                }
            }
            else
                valueStack.Push(Convert.ToInt32(tokenInExpression));
        }
    }
    /// <summary>
    /// extension for stack, which helps to determine if wanted operator is not top of stack
    /// </summary>
    static class stackExtensions
    {
        /// <summary>
        /// the method determine is the top of operator stack is 
        /// the imputed operation, if yes return true, if not vice versa
        /// </summary>
        /// <param name="stack"></param> operator stack 
        /// <param name="operators"></param> determing operator
        /// <returns></returns>
        public static bool isOnTop(this Stack<string> stack, String operators)
        {
            return stack.Count > 0 && stack.Peek().Equals(operators);
        }
    }
}