namespace EvaluatorTester
{
    [TestClass]
    public class EvaluatorsTest
    {
        /// <summary>
        /// test single addition operation
        /// </summary>
        [TestMethod]
        public void TestSimpleAddition()
        {
            String exp = "1+32";

            static int var(string input)
            {
                return 0;
            }

            Assert.AreEqual(33, FormulaEvaluator.Evaluator.Evaluate(exp, var));
        }
        /// <summary>
        /// test helper method of if the token is varibale
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static bool isVariable(String token)
        {
            if (char.IsLetter(token[0]))
            {
                for (int i = 0; i < token.Length - 1; i++)
                {
                    if (char.IsDigit(token[i]) && char.IsLetter(token[i + 1]))
                    {

                        return false;
                        throw new Exception("It's wrong variable");

                    }
                    else
                    {
                        continue;
                    }
                }
            }
            else
            {
                return false;
            }

            return true;
        }
        /// <summary>
        /// test if the token is variable
        /// </summary>
        [TestMethod]
        public void TestIfItsVar()
        {
            String exp = "AAAAAAA1111";

            Assert.AreEqual(true, isVariable(exp));
        }

        /// <summary>
        /// test single substraction
        /// </summary>
        [TestMethod]
        public void TestSubtraction()
        {
            String exp = "8-7";

            static int var(string input)
            {
                return 0;
            }

            Assert.AreEqual(1, FormulaEvaluator.Evaluator.Evaluate(exp, var));
        }

        /// <summary>
        /// test single multiplication
        /// </summary>
        [TestMethod]
        public void TestMultiplication()
        {
            String exp = "8*7";

            static int var(string input)
            {
                return 0;
            }

            Assert.AreEqual(56, FormulaEvaluator.Evaluator.Evaluate(exp, var));
        }

        /// <summary>
        /// test single division
        /// </summary>
        [TestMethod]
        public void Testdivision()
        {
            String exp = "14/7";

            static int var(string input)
            {
                return 0;
            }

            Assert.AreEqual(2, FormulaEvaluator.Evaluator.Evaluate(exp, var));
        }

        /// <summary>
        /// test single number is inputed
        /// </summary>
        [TestMethod]
        public void TestSimgleNumber()
        {
            String exp = "6";

            static int var(string input)
            {
                return 0;
            }

            Assert.AreEqual(6, FormulaEvaluator.Evaluator.Evaluate(exp, var));
        }

        /// <summary>
        /// test single parenthesis
        /// </summary>
        [TestMethod]
        public void Testparenthesis()
        {
            String exp = "(2+1)*2";

            static int var(string input)
            {
                return 0;
            }

            Assert.AreEqual(6, FormulaEvaluator.Evaluator.Evaluate(exp, var));
        }

        /// <summary>
        /// test whitespaces
        /// </summary>
        [TestMethod]
        public void TestWhiteSpace()
        {
            String exp = "  (  2  +1)*  2";

            static int var(string input)
            {
                return 0;
            }

            Assert.AreEqual(6, FormulaEvaluator.Evaluator.Evaluate(exp, var));
        }


        /// <summary>
        /// test additions
        /// </summary>
        [TestMethod]
        public void TestAdditions()
        {
            String exp1 = "5+1";
            String exp2 = "5+3+7+9";
            String exp3 = "1+2+3+4+5";
            String exp4 = "0+0";
            String exp5 = "10+1+2";

            static int var(string input)
            {
                return 0;
            }

            Assert.AreEqual(6, FormulaEvaluator.Evaluator.Evaluate(exp1, var));
            Assert.AreEqual(24, FormulaEvaluator.Evaluator.Evaluate(exp2, var));
            Assert.AreEqual(15, FormulaEvaluator.Evaluator.Evaluate(exp3, var));
            Assert.AreEqual(0, FormulaEvaluator.Evaluator.Evaluate(exp4, var));
            Assert.AreEqual(13, FormulaEvaluator.Evaluator.Evaluate(exp5, var));
        }

        /// <summary>
        /// test substractions
        /// </summary>
        [TestMethod]
        public void TestSubtractions()
        {
            String exp1 = "5-1";
            String exp2 = "9-10";
            String exp3 = "0-0";
            String exp4 = "20-10-5-3";
            String exp5 = "0-10-15";

            static int var(string input)
            {
                return 0;
            }

            Assert.AreEqual(4, FormulaEvaluator.Evaluator.Evaluate(exp1, var));
            Assert.AreEqual(-1, FormulaEvaluator.Evaluator.Evaluate(exp2, var));
            Assert.AreEqual(0, FormulaEvaluator.Evaluator.Evaluate(exp3, var));
            Assert.AreEqual(2, FormulaEvaluator.Evaluator.Evaluate(exp4, var));
            Assert.AreEqual(-25, FormulaEvaluator.Evaluator.Evaluate(exp5, var));
        }

        /// <summary>
        /// test multiplications
        /// </summary>
        [TestMethod]
        public void TestMultiplications()
        {
            String exp1 = "5*1";
            String exp2 = "5*0";
            String exp3 = "0*0";
            String exp4 = "1*5*2";
            String exp5 = "2*2*2";

            static int var(string input)
            {
                return 0;
            }

            Assert.AreEqual(5, FormulaEvaluator.Evaluator.Evaluate(exp1, var));
            Assert.AreEqual(0, FormulaEvaluator.Evaluator.Evaluate(exp2, var));
            Assert.AreEqual(0, FormulaEvaluator.Evaluator.Evaluate(exp3, var));
            Assert.AreEqual(10, FormulaEvaluator.Evaluator.Evaluate(exp4, var));
            Assert.AreEqual(8, FormulaEvaluator.Evaluator.Evaluate(exp5, var));
        }

        /// <summary>
        /// test dicisions
        /// </summary>
        [TestMethod]
        public void TestDivisions()
        {
            String exp1 = "5/2";
            String exp2 = "10/2";
            String exp3 = "8/2/4";
            String exp4 = "20/4/2";
            String exp5 = "2";
            String exp6 = "14/7";


            static int var(string input)
            {
                return 0;
            }
         Assert.AreEqual(2, FormulaEvaluator.Evaluator.Evaluate(exp1, var));
            Assert.AreEqual(5, FormulaEvaluator.Evaluator.Evaluate(exp2, var));
            Assert.AreEqual(1, FormulaEvaluator.Evaluator.Evaluate(exp3, var));
          Assert.AreEqual(2, FormulaEvaluator.Evaluator.Evaluate(exp4, var));
            Assert.AreEqual(2, FormulaEvaluator.Evaluator.Evaluate(exp5, var));
            Assert.AreEqual(2, FormulaEvaluator.Evaluator.Evaluate(exp6, var));

        }

        /// <summary>
        /// test parenthesis
        /// </summary>
        [TestMethod]
        public void Testparenthesises()
        {
            String exp1 = "(5/2)+6*(2+7)";
            String exp2 = "(10+2)/2";
            String exp3 = "(1-1)*50";
            String exp4 = "(12-6)/3+1";
            String exp5 = "5+(3+7)/5*6";

            static int var(string input)
            {
                return 0;
            }
            Assert.AreEqual(56, FormulaEvaluator.Evaluator.Evaluate(exp1, var));
            Assert.AreEqual(6, FormulaEvaluator.Evaluator.Evaluate(exp2, var));
            Assert.AreEqual(0, FormulaEvaluator.Evaluator.Evaluate(exp3, var));
            Assert.AreEqual(3, FormulaEvaluator.Evaluator.Evaluate(exp4, var));
            Assert.AreEqual(17, FormulaEvaluator.Evaluator.Evaluate(exp5, var));
        }

        /// <summary>
        /// test delegates
        /// </summary>
        [TestMethod]
        public void Testdelegates()
        {
            String exp1 = "(5/2)+6*(2+A1) ";
            String exp2 = "(AA11+2)/2";
            String exp3 = "(1-1)*BBB1";
            String exp4 = "(12-6)/CSJ1+1";
            String exp5 = "5+(3+7)/B2*6";

            static int var(string input)
            {
                if(input == "A1")
                return 7;
                if (input == "AA11")
                    return 10;
                if (input == "BBB1")
                    return 50;
                if (input == "CSJ1")
                    return 3;
                if (input == "B2")
                    return 5;
                return 0;
            }
            Assert.AreEqual(56, FormulaEvaluator.Evaluator.Evaluate(exp1, a => { return 7; }));
            Assert.AreEqual(6, FormulaEvaluator.Evaluator.Evaluate(exp2, var));
            Assert.AreEqual(0, FormulaEvaluator.Evaluator.Evaluate(exp3, var));
            Assert.AreEqual(3, FormulaEvaluator.Evaluator.Evaluate(exp4, var));
            Assert.AreEqual(17, FormulaEvaluator.Evaluator.Evaluate(exp5, var));
        }

    }
}