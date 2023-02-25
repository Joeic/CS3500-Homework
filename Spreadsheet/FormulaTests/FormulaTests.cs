using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using SpreadsheetUtilities;

/*
 * Course: CS3500 2022 fall
 * Author: Qiaoyi Cai
 * UID: u1285695
 * Date: 9/16/2022
 */
namespace FormulaTests
{

    [TestClass]
    public class FormulaTests
    {
        /// <summary>
        /// test single number
        /// </summary>
        [TestMethod()]
        public void TestNumber()
        {
            Formula f = new Formula("1");
            Assert.AreEqual(1.0, f.Evaluate(s => 0));
        }
        

        /// <summary>
        /// test single variable
        /// </summary>
        [TestMethod()]
        public void TestVariable()
        {
            Formula f = new Formula("a1");
            Assert.AreEqual(1.0, f.Evaluate(s => 1));
        }
        /// <summary>
        /// test simple addition
        /// </summary>
        [TestMethod]
        public void TestSimpleAdd()
        {
            Formula formula = new Formula("1+1.0");
            Assert.AreEqual(2.0, formula.Evaluate(s => 0));
        }
        /// <summary>
        /// test simple substraction
        /// </summary>
        [TestMethod]
        public void TestSimpleSubstract()
        {
            Formula formula = new Formula("11-8");
            Assert.AreEqual(3.0, formula.Evaluate(s => 0));
        }
        /// <summary>
        /// test simple multiplication
        /// </summary>
        [TestMethod]
        public void TestSimpleMultiplication()
        {
            Formula formula = new Formula("11*8");
            Assert.AreEqual(88.0, formula.Evaluate(s => 0));
        }
        /// <summary>
        /// test simple divison
        /// </summary>
        [TestMethod]
        public void TestSimpleDivision()
        {
            Formula formula = new Formula("24/8");
            Assert.AreEqual(3.0, formula.Evaluate(s => 0));
        }
        /// <summary>
        /// test simple parenthesis
        /// </summary>
        [TestMethod()]
        public void TestSimpleParen()
        {
            Formula formula = new Formula("4*(4+2)");
           
            Assert.AreEqual(24.0, formula.Evaluate(s => 0));
        }
        /// <summary>
        /// test simple variable addition
        /// </summary>
        [TestMethod()]
        public void testSimpleVar()
        {
            Formula f = new Formula("10+a1");
            Assert.AreEqual(19.0, f.Evaluate(s => 9));
        }

        /// <summary>
        /// test variable divition
        /// </summary>
        [TestMethod()]
        public void testSimpleVar2()
        {
            Formula f = new Formula("10/a1");
            Assert.AreEqual(5.0, f.Evaluate(s => 2));
        }
        /// <summary>
        /// test variable substract
        /// </summary>
        [TestMethod()]
        public void testSimpleVar3()
        {
            Formula f = new Formula("10-a1");
            Assert.AreEqual(8.0, f.Evaluate(s => 2));
        }
        /// <summary>
        /// test variable multiplication
        /// </summary>
        [TestMethod()]
        public void testSimpleVar4()
        {
            Formula f = new Formula("10*a1");
            Assert.AreEqual(20.0, f.Evaluate(s => 2));
        }

        /// <summary>
        /// test combination of varibale
        /// </summary>
        [TestMethod()]
        public void testSimpleComb()
        {
            Formula f = new Formula("10*a1-2");
            Assert.AreEqual(18.0, f.Evaluate(s => 2));
        }
        /// <summary>
        /// test combination of varibale
        /// </summary>
        [TestMethod()]
        public void testSimpleComb2()
        {
            Formula f = new Formula("10*(a1-1)");
            Assert.AreEqual(10.0, f.Evaluate(s => 2));
        }

        /// <summary>
        /// test if dominator is 0
        /// </summary>
        [TestMethod()]
        public void TestDominatorZero()
        {
            Formula t = new Formula("1/0");
            object error = t.Evaluate(s => 0);
            Assert.IsInstanceOfType(error, typeof(FormulaError));
        }
        /// <summary>
        /// test dominator is 0 in another situation
        /// </summary>
        [TestMethod()]
        public void TestDominatorZero2()
        {
            Formula t = new Formula("20/(3-3)");
            object error = t.Evaluate(s => 0);
            Assert.IsInstanceOfType(error, typeof(FormulaError));
        }
        /// <summary>
        /// test underscore variable
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestWrongConstructor()
        {
            Formula f = new Formula("1_a+b1");
        }
        /// <summary>
        /// test underscore variable
        /// </summary>
        [TestMethod]
        public void TestCorrectConstructor()
        {
            Formula f = new Formula("a+b_1");
        }
        /// <summary>
        /// test underscore variable
        /// </summary>
        [TestMethod]
        public void TestCorrectConstructor2()
        {
            Formula f = new Formula("a+b_1+asdd_____23_s");
        }
        /// <summary>
        /// test empty expresion
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestEmptyConstructor()
        {
            Formula f = new Formula("");
        }
        /// <summary>
        /// test wrong starting token
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestWrongFirstToken()
        {
            Formula f = new Formula(")35-20");
        }
        /// <summary>
        /// test wrong starting token
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestWrongFirstToken2()
        {
            Formula f = new Formula("+35-20");
        }
        /// <summary>
        /// test wrong starting token
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestWrongFirstToken3()
        {
            Formula f = new Formula("-35-20");
        }
        /// <summary>
        /// test wrong starting token
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestWrongFirstToken4()
        {
            Formula f = new Formula("*35-20");
        }
        /// <summary>
        /// test wrong starting token
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestWrongFirstToken5()
        {
            Formula f = new Formula("/35-20");
        }

        /// <summary>
        /// test wrong last token
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestWrongLastToken()
        {
            Formula f = new Formula("35-20(");
        }
        /// <summary>
        /// test wrong last token
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestWrongLastToken2()
        {
            Formula f = new Formula("35-20+");
        }
        /// <summary>
        /// test wrong last token
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestWrongLastToken3()
        {
            Formula f = new Formula("35-20-");
        }
        /// <summary>
        /// test wrong last token
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestWrongLastToken4()
        {
            Formula f = new Formula("35-20*");
        }
        /// <summary>
        /// test wrong last token
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestWrongLastToken5()
        {
            Formula f = new Formula("35-20/");
        }
        /// <summary>
        /// test wrong last token
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestUndefiendTokens()
        {
            Formula f = new Formula("30+34=");
        }
        /// <summary>
        /// test wrong last token
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestUndefiendTokens2()
        {
            Formula f = new Formula("30+34&@");
        }
        /// <summary>
        /// test undefined variable
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        [TestMethod()]
        public void TestUndefinedVariable()
        {
            Formula f = new Formula("10+a1");
            object error = f.Evaluate(s => { throw new ArgumentException("Undefined Variable occurs"); });
            Assert.IsInstanceOfType(error, typeof(SpreadsheetUtilities.FormulaError));
        }
        /// <summary>
        /// test wrong parenthesis/operator following rule
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestInvalidExpression()
        {
            Formula f = new Formula("10+a1))");
        }
        /// <summary>
        /// test wrong parenthesis/operator following rule
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestInvalidExpression2()
        {
            Formula f = new Formula("((+10+a1");
        }
        /// <summary>
        /// test wrong parenthesis/operator following rule
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestInvalidExpression3()
        {
            Formula f = new Formula("((()))))");
        }
        /// <summary>
        /// test invalid variable
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestInvalidVariable1()
        {
            Formula f = new Formula("asdf1 + asdfg2", s => s.ToUpper(), s => false);
        }
        /// <summary>
        /// test simple equals
        /// </summary>
        [TestMethod()]
        public void TestEquals()
        {
            Formula f1 = new Formula("a1+b1");
            Formula f2 = new Formula("a1+b1");
            Assert.IsTrue(f1.Equals(f2));
        }
        /// <summary>
        /// test equals with space and double
        /// </summary>
        [TestMethod()]
        public void TestEquals2()
        {
            Formula f1 = new Formula("a1+b1+1");
            Formula f2 = new Formula("a1+   b1 + 1.0");
            Assert.IsTrue(f1.Equals(f2));
        }
        /// <summary>
        /// test operator == 
        /// </summary>
        [TestMethod()]
        public void TestEqualsOpera()
        {
            Formula f1 = new Formula("a1+b1+1");
            Formula f2 = new Formula("a1+   b1 + 1.0");
            Assert.IsTrue(f1 == f2);
        }
        /// <summary>
        /// test wrong situation of operator ==
        /// </summary>
        [TestMethod()]
        public void TestWrongEqualsOpera2()
        {
            Formula f1 = new Formula("a1+b1+1");
            Formula f2 = new Formula("a1+   a1 + 1.0");
            Assert.IsFalse(f1 == f2);
        }

        /// <summary>
        /// test operator !=
        /// </summary>
        [TestMethod()]
        public void TestWrongEqualsOpera3()
        {
            Formula f1 = new Formula("a1+b1+1");
            Formula f2 = new Formula("a1+   a1 + 1.0");
            Assert.IsTrue(f1 != f2);
        }
        /// <summary>
        /// test simple toString
        /// </summary>
        [TestMethod()]
        public void TestToString()
        {
            Formula f = new Formula("1*34");
            Formula tester = new Formula("1*34");
            Assert.IsTrue(f.Equals(tester));
        }
        /// <summary>
        /// test simple getHashCode
        /// </summary>
        [TestMethod()]
        public void TestGetHashCode()
        {
            Formula f1 = new Formula("1*34");
            Formula tester = new Formula("1*34");
            Assert.IsTrue(f1.GetHashCode() == tester.GetHashCode());
        }
        /// <summary>
        /// test getHashcode at wrong situation
        /// </summary>
        [TestMethod()]
        public void TestGetHashCode2()
        {
            Formula f1 = new Formula("1*34");
            Formula tester = new Formula("1*34+232");
            Assert.IsFalse(f1.GetHashCode() == tester.GetHashCode());
        }
        /// <summary>
        /// test getvariables
        /// </summary>
        [TestMethod()]
        public void TestGerVariable()
        {
            Formula f = new Formula("2*5+a1-b1");
            int numVar = f.GetVariables().Count();
            Assert.AreEqual(2,numVar);
        }




     
    }
}