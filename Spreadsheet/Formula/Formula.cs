// Skeleton written by Profs Zachary, Kopta and Martin for CS 3500
// Read the entire skeleton carefully and completely before you
// do anything else!

// Change log:
// Last updated: 9/8, updated for non-nullable types

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

/*
 * Course: CS3500 2022 fall
 * Date: 9/16/2022
 */
namespace SpreadsheetUtilities
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  The allowed symbols are non-negative numbers written using double-precision 
    /// floating-point syntax (without unary preceeding '-' or '+'); 
    /// variables that consist of a letter or underscore followed by 
    /// zero or more letters, underscores, or digits; parentheses; and the four operator 
    /// symbols +, -, *, and /.  
    /// 
    /// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
    /// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable; 
    /// and "x 23" consists of a variable "x" and a number "23".
    /// 
    /// Associated with every formula are two delegates:  a normalizer and a validator.  The
    /// normalizer is used to convert variables into a canonical form, and the validator is used
    /// to add extra restrictions on the validity of a variable (beyond the standard requirement 
    /// that it consist of a letter or underscore followed by zero or more letters, underscores,
    /// or digits.)  Their use is described in detail in the constructor and method comments.
    /// </summary>
    public class Formula { 
    

        private List<String> tokensList;
        Func<string, string> normalize;

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer is the identity function, and the associated validator
        /// maps every string to true.  
        /// </summary>
        public Formula(String formula) :
            this(formula, s => s, s => true)
        {
        }
        /// <summary>
        /// helper method determing if the token is opening parenthesisw
        /// </summary>
        /// <param name="param"></param> input token
        /// <returns></returns> return true if the token is opening parenthesis, return false if not
        private bool checkLeftPara(String param)
        {
            String lpPattern = @"\(";
            if (Regex.IsMatch(param, lpPattern) == false)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        /// <summary>
        /// helper method that determine if the token is closing parenthesis
        /// </summary> 
        /// <param name="param"></param> input token
        /// <returns></returns>  return true if the token is closing parenthesis, return false if not
        private bool checkRightPara(String param)
        {
            String rpPattern = @"\)";
            if (Regex.IsMatch(param, rpPattern) == false)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// helper method determing if the token is operator
        /// </summary>
        /// <param name="param"></param> input token
        /// <returns></returns>  return true if the token is operator , return false if not
        private bool checkOperator(String param)
        {
            if (param == "+" || param == "-" || param == "*" || param == "/")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// helper method determing if the token is number
        /// </summary>
        /// <param name="param"></param> input token
        /// <returns></returns>  return true if the token is number , return false if not
        private bool checkNum(String param)
        {
            double test;
            if (double.TryParse(param, out test) == false)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically incorrect,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer and validator are the second and third parameters,
        /// respectively.  
        /// 
        /// If the formula contains a variable v such that normalize(v) is not a legal variable, 
        /// throws a FormulaFormatException with an explanatory message. 
        /// 
        /// If the formula contains a variable v such that isValid(normalize(v)) is false,
        /// throws a FormulaFormatException with an explanatory message.
        /// 
        /// Suppose that N is a method that converts all the letters in a string to upper case, and
        /// that V is a method that returns true only if a string consists of one letter followed
        /// by one digit.  Then:
        /// 
        /// new Formula("x2+y3", N, V) should succeed
        /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
        /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
        /// </summary>
        /// 
        public Formula(String formula, Func<string, string> normalize, Func<string, bool> isValid)
        {
            //check if the expression is empty at very first
            if (String.IsNullOrEmpty(formula))
            {
                throw new FormulaFormatException("The inputed expression is empty or null, please add characters to expression ");
            }
            tokensList = new List<String>(GetTokens(formula));

            int opParenNum = 0;
            int cloParenNum = 0;
            this.normalize = normalize;
            Queue<String> paraQueue = new Queue<string>();
            //if the expresion doesn't meet starting token rule
            if (!startingTokenRule(tokensList[0]))
            {
                throw new FormulaFormatException("The expression doesn't obey the starting token rule, please check if the first character is number, variable or opening parenthesis");
            }
            //if the expresion doesn't meet ending token rule
            if (!endingTokenRule(tokensList[tokensList.Count - 1]))
            {
                throw new FormulaFormatException("The expression doesn't obey the starting token rule, please check if the first character is number, variable or opening parenthesis");
            }
            for (int i = 0; i < tokensList.Count; i++)
            {
                String token = tokensList[i];


                if (token == "(")
                {
                    paraQueue.Enqueue(token);
                    opParenNum++;
                }

                if (token == ")")
                {
                    if (paraQueue.Count > 0)
                    {
                        paraQueue.Dequeue();
                        cloParenNum++;
                    }
                    else
                    {
                        throw new FormulaFormatException("The expression doesn't obey the Parenthesis/Operator Following Rule, Please check the expression format");
                    }
                }
                //check if the expression meet the  parenthesis/operator following rule
                if (i + 1 < tokensList.Count)
                {
                    if (checkLeftPara(tokensList[i]) || checkOperator(tokensList[i]))
                    {
                        if (checkNum(tokensList[i + 1]) == false && isVar(tokensList[i + 1]) == false && checkLeftPara(tokensList[i + 1]) == false)
                        {
                            throw new FormulaFormatException("Any token that immediately follows an opening parenthesis or an operator must be either a number, a variable, or an opening parenthesis.");
                        }
                    }
                }
                //check if the expression meet the Extra Following Rule
                if (i + 1 < tokensList.Count)
                {
                    if (checkNum(tokensList[i]) || isVar(tokensList[i]) || checkRightPara(tokensList[i]))
                    {
                        if (checkOperator(tokensList[i + 1]) == false && checkRightPara(tokensList[i + 1]) == false)
                        {
                            throw new FormulaFormatException("Any token that immediately follows a number, a variable, or a closing parenthesis must be either an operator or a closing parenthesis.");
                        }
                    }
                }
                if (isVar(token))
                {

                    if (isValid(normalize(token)) == false)
                    {
                        throw new FormulaFormatException(" the varibale is not valid, please enter a valid variable");
                    }
                    tokensList[i] = normalize(tokensList[i]);
                }


            }
            if (opParenNum != cloParenNum)
            {
                throw new FormulaFormatException("error in parenthesis, please check each opening parenthesis macthed closing parenthesis");
            }

        }
 

        /// <summary>
        /// helper method to determind if the expression obey the ending Token Rule
        /// </summary>
        /// <param name="lastToken"></param> The last token of an expression
        /// <returns></returns> return true if the last token is number, variable, or closing parenthesis, return false if not
        private static bool endingTokenRule(String lastToken)
        {
            if ((Double.TryParse(lastToken, out double numericValue) || isVar(lastToken) || lastToken == ")"))
            {
                return true;

            }
            return false;
        }
        /// <summary>
        /// helper method to determind if the expression obey the Starting Token Rule
        /// </summary>
        ///  /// <param name="firstToken"></param>  The first token of an expression
        /// <returns></returns> return true if the first token is number, variable, or opening parenthesis, return false if not
        private static bool startingTokenRule(String firstToken)
        {

            if ((Double.TryParse(firstToken, out double numericValue) || isVar(firstToken) || firstToken == "("))
            {
                return true;

            }
            return false;
        }

        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables.  When a variable symbol v needs to be determined, it should be looked up
        /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to 
        /// the constructor.)
        /// 
        /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters 
        /// in a string to upper case:
        /// 
        /// new Formula("x+7", N, s => true).Evaluate(L) is 11
        /// new Formula("x+7").Evaluate(L) is 9
        /// 
        /// Given a variable symbol as its parameter, lookup returns the variable's value 
        /// (if it has one) or throws an ArgumentException (otherwise).
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.  
        /// The Reason property of the FormulaError should have a meaningful explanation.
        ///
        /// This method should never throw an exception.
        /// </summary>
        public object Evaluate(Func<string, double> lookup)
        {
            Stack<String> operatorStack = new Stack<String>();
            Stack<Double> valueStack = new Stack<Double>();
            string[] substrings = tokensList.Cast<string>().ToArray<string>();
            // remove the whitespaces form expression

            double numericValue;

            foreach (String tokenInExp in substrings)
            {
                if (Double.TryParse(tokenInExp, out numericValue)) //if token is double                                                               
                {

                    Object object1 = floatingCalculation(tokenInExp, valueStack, operatorStack);
                    if (object1 is FormulaError)
                    {
                        return new FormulaError("");
                    }


                }
                else if (!String.IsNullOrEmpty(tokenInExp) && isVar(tokenInExp))//if token is variable
                {
                    try
                    {
                        String stringVariableValue = lookup(tokenInExp).ToString();
                    }
                    catch (Exception)
                    {
                        return new FormulaError("");
                    }

                    double tokenVaribaleValue = lookup(tokenInExp);
                    String stringTokenVariableValue = tokenVaribaleValue.ToString();
                    floatingCalculation(stringTokenVariableValue, valueStack, operatorStack);


                }
                // if the token is + or -
                else if (tokenInExp.Equals("+") || tokenInExp.Equals("-"))
                {
                    if (operatorStack.isOnTop("+") || operatorStack.isOnTop("-"))
                    {
                        pValueStackTwiceAndPopOpStack(valueStack, operatorStack);
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
                        pValueStackTwiceAndPopOpStack(valueStack, operatorStack);
                    if (operatorStack.isOnTop("("))
                        operatorStack.Pop();

                    if (operatorStack.isOnTop("*") || operatorStack.isOnTop("/"))
                        if (pValueStackTwiceAndPopOpStack(valueStack, operatorStack) is FormulaError)
                        {
                            return new FormulaError("");
                        }

                }
                else if (tokenInExp.Equals("") || tokenInExp.Equals(" "))
                    continue;
            }
            // if there is 1 element in value stack
            if (operatorStack.Count == 0 && valueStack.Count == 1)
            {
                double result = valueStack.Pop();

                return result;
            }
            //if there is 2 elements in value stack and 1 element in operator stack
            //else if (operatorStack.Count == 1 && valueStack.Count == 2)
            else
            {
                pValueStackTwiceAndPopOpStack(valueStack, operatorStack);
                double result = valueStack.Pop();

                return result;
            }


        }

        /// <summary>
        /// Enumerates the normalized versions of all of the variables that occur in this 
        /// formula.  No normalization may appear more than once in the enumeration, even 
        /// if it appears more than once in this Formula.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
        /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
        /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
        /// </summary>
        public IEnumerable<String> GetVariables()
        {
            HashSet<String> variables = new HashSet<String>();
            for (int i = 0; i < tokensList.Count; i++)
            {
                if (isVar(tokensList[i]))
                {
                    variables.Add(tokensList[i]);
                }
            }
            return variables;
        }

        /// <summary>
        /// Returns a string containing no spaces which, if passed to the Formula
        /// constructor, will produce a Formula f such that this.Equals(f).  All of the
        /// variables in the string should be normalized.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
        /// new Formula("x + Y").ToString() should return "x+Y"
        /// </summary>
        public override string ToString()
        {
            String formulaToString = "";
            foreach (String token in tokensList)
            {
                formulaToString = formulaToString + token;
            }
            return formulaToString;
        }

        /// <summary>
        /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
        /// whether or not this Formula and obj are equal.
        /// 
        /// Two Formulae are considered equal if they consist of the same tokens in the
        /// same order.  To determine token equality, all tokens are compared as strings 
        /// except for numeric tokens and variable tokens.
        /// Numeric tokens are considered equal if they are equal after being "normalized" 
        /// by C#'s standard conversion from string to double, then back to string. This 
        /// eliminates any inconsistencies due to limited floating point precision.
        /// Variable tokens are considered equal if their normalized forms are equal, as 
        /// defined by the provided normalizer.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///  
        /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
        /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
        /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
        /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
        /// </summary>
        public override bool Equals(object? obj)
        {
            if ((obj == null) || !(obj is Formula))
            {
                return false;
            }
            //obj is Formula type now
            Formula formula = (Formula)obj;
            if (tokensList.Count != formula.tokensList.Count)
            {
                return false;
            }
            else
            {

                for (int i = 0; i < tokensList.Count; i++)
                {
                    if ((Double.TryParse(tokensList[i], out double thisNum)) && (Double.TryParse(formula.tokensList[i], out double fromulaNum)))
                    {
                        if (thisNum != fromulaNum)
                        {
                            return false;
                        }
                    }

                    else if (!(this.normalize(this.tokensList[i]).Equals(formula.normalize(formula.tokensList[i]))))
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        /// Note that f1 and f2 cannot be null, because their types are non-nullable
        /// </summary>
        public static bool operator ==(Formula f1, Formula f2)
        {
            if (!(f1 is (null)) && !(f2 is (null)))
            {
                if (f1.Equals(f2))
                {
                    return true;
                }
            }
            return false;

        }

        /// <summary>
        /// Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// Note that f1 and f2 cannot be null, because their types are non-nullable
        /// </summary>
        public static bool operator !=(Formula f1, Formula f2)
        {
            if (!(f1.Equals(f2)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
        /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two 
        /// randomly-generated unequal Formulae have the same hash code should be extremely small.
        /// </summary>
        public override int GetHashCode()
        {
            //return this.toString.GetHashCode(); 
            String temp = "";
            for (int i = 0; i < tokensList.Count; i++)
            {
                if ((Double.TryParse(tokensList[i], out double thisNum)))
                {

                    temp += thisNum;

                }

                else if (isVar(this.tokensList[i]))
                {
                    temp += normalize(this.tokensList[i]);
                }
            }


            return temp.GetHashCode();
        }
        /// <summary>
        /// pop value stack twice and pop operator stack once
        /// </summary>
        private static Object pValueStackTwiceAndPopOpStack(Stack<Double> valueStack, Stack<String> operatorStack)
        {
            try
            {
                double firstValue = Convert.ToDouble(valueStack.Pop());
                double secondValue = Convert.ToDouble(valueStack.Pop());
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
                        return new FormulaError("dominator can't be 0");
                }
                return 0;
            }

            catch (Exception)
            {
                return 0;
                
            }
        }

        /// <summary>
        /// if the token is double, do the algorithm action 
        /// </summary>
        /// <param name="tokenInExpression"></param> inputed token
        /// <exception cref="ArgumentException"></exception> if the expression is wrong
        private static Object floatingCalculation(String tokenInExpression, Stack<Double> valueStack, Stack<String> operatorStack)
        {
            if (operatorStack.isOnTop("*") || operatorStack.isOnTop("/"))
            {
                double tokenNumberValue = Double.Parse(tokenInExpression);
                double value = Convert.ToDouble(valueStack.Pop());

                if (operatorStack.isOnTop("*"))
                {
                    operatorStack.Pop();
                    double newValue = tokenNumberValue * value;
                    valueStack.Push(newValue);
                }
                else if (operatorStack.isOnTop("/"))
                {
                    operatorStack.Pop();
                    if (tokenNumberValue != 0)
                    {
                        double newValue = value / tokenNumberValue;
                        valueStack.Push(newValue);
                    }
                    else
                    {
                        return new FormulaError("sss");
                    }
                    
                }
                return 0;
            }
            else
            {
                valueStack.Push(Convert.ToDouble(tokenInExpression));
                return 0;
            }

        }
        /// <summary>
        /// helper method that determines if the single token is varibale or not
        /// </summary>
        /// <param name="singleToken"></param> inputed single token 
        /// <returns></returns> return true if the inputed token is variable, if not, return false
        private static bool isVar(String singleToken)
        {
            String varPattern = @"[a-zA-Z][0-9a-zA-Z]*";
            if (Regex.IsMatch(singleToken, varPattern) == false)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        /// <summary>
        /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
        /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }

        }
    }

    /// <summary>
    /// Used to report syntactic errors in the argument to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// Used as a possible return value of the Formula.Evaluate method.
    /// </summary>
    public struct FormulaError
    {
        /// <summary>
        /// Constructs a FormulaError containing the explanatory reason.
        /// </summary>
        /// <param name="reason"></param>
        public FormulaError(String reason)
            : this()
        {
            Reason = reason;
        }

        /// <summary>
        ///  The reason why this FormulaError was created.
        /// </summary>
        public string Reason { get; private set; }
    }

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

