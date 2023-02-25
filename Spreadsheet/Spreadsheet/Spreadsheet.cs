using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using SpreadsheetUtilities;

using SS;
using static System.Net.Mime.MediaTypeNames;
/*
 * Course: CS3500 2022 fall
 * Author: Qiaoyi Cai
 * Date: 9/30/2022
 */
namespace SS
{
    /// <summary>
    /// An AbstractSpreadsheet object represents the state of a simple spreadsheet.  A 
    /// spreadsheet consists of an infinite number of named cells.
    /// 
    /// A string is a valid cell name if and only if:
    ///   (1) its first character is an underscore or a letter
    ///   (2) its remaining characters (if any) are underscores and/or letters and/or digits
    /// Note that this is the same as the definition of valid variable from the PS3 Formula class.
    /// 
    /// For example, "x", "_", "x2", "y_15", and "___" are all valid cell  names, but
    /// "25", "2x", and "&" are not.  Cell names are case sensitive, so "x" and "X" are
    /// different cell names.
    /// 
    /// A spreadsheet contains a cell corresponding to every possible cell name.  (This
    /// means that a spreadsheet contains an infinite number of cells.)  In addition to 
    /// a name, each cell has a contents and a value.  The distinction is important.
    /// 
    /// The contents of a cell can be (1) a string, (2) a double, or (3) a Formula.  If the
    /// contents is an empty string, we say that the cell is empty.  (By analogy, the contents
    /// of a cell in Excel is what is displayed on the editing line when the cell is selected).
    /// 
    /// In a new spreadsheet, the contents of every cell is the empty string.
    ///  
    /// We are not concerned with values in PS4, but to give context for the future of the project,
    /// the value of a cell can be (1) a string, (2) a double, or (3) a FormulaError.  
    /// (By analogy, the value of an Excel cell is what is displayed in that cell's position
    /// in the grid). 
    /// 
    /// If a cell's contents is a string, its value is that string.
    /// 
    /// If a cell's contents is a double, its value is that double.
    /// 
    /// If a cell's contents is a Formula, its value is either a double or a FormulaError,
    /// as reported by the Evaluate method of the Formula class.  The value of a Formula,
    /// of course, can depend on the values of variables.  The value of a variable is the 
    /// value of the spreadsheet cell it names (if that cell's value is a double) or 
    /// is undefined (otherwise).
    /// 
    /// Spreadsheets are never allowed to contain a combination of Formulas that establish
    /// a circular dependency.  A circular dependency exists when a cell depends on itself.
    /// For example, suppose that A1 contains B1*2, B1 contains C1*2, and C1 contains A1*2.
    /// A1 depends on B1, which depends on C1, which depends on A1.  That's a circular
    /// dependency.
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {
        //Dictionary connecting cell name and cell class
        Dictionary<string, Cell> cellMaps;
        // dependency graph of cell
        DependencyGraph dependencyGraph;

        /// <summary>
        /// helper method determine if the token is variable
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private bool isVar(String token)
        {
            return Regex.IsMatch(token, @"^[a-zA-Z]+[1-9]{1}[0-9]*$");
        }

        /// <summary>
        /// helper method that is delegates to get the cell's value
        /// </summary>
        /// <param name="name"></param> cell's name
        /// <returns></returns> double value
        /// <exception cref="ArgumentException"></exception>
        private double lookup(String name)
        {
            double decimalNum;

            if (double.TryParse(this.cellMaps[name].value.ToString(), out decimalNum))
            {
                return decimalNum;
            }
            else
            {
                throw new ArgumentException();
            }
        }
        /// <summary>
        /// helper method that is the cell's name is formula, use lookup helper method to get the cell's value
        /// </summary>
        /// <param name="name"></param> cell's name
        /// <returns></returns> return the value, double or string
        private object calculateCell(String name)
        {
            //if the content is formula, use lookup method
            if (this.cellMaps[name].content is Formula)
            {

#pragma warning disable CS8604 // Possible null reference argument.
                return (new Formula(this.cellMaps[name].content.ToString())).Evaluate(lookup);
#pragma warning restore CS8604 // Possible null reference argument.
            }
            else if (this.cellMaps[name].content is double)
            {
                return (double)this.cellMaps[name].content;
            }
            else
            {
                return this.cellMaps[name].content;
            }
        }

        /// <summary>
        /// helper method that track the cell if it's changed
        /// </summary>
        public override bool Changed
        {
            get;
            protected set;
        }

        /// <summary>
        /// base constructor
        /// </summary>
        public Spreadsheet() : base(s => true, s => s, "default")
        {
            Changed = false;
            cellMaps = new Dictionary<string, Cell>();
            dependencyGraph = new DependencyGraph();
        }

        /// <summary>
        /// constructor of Spreadsheet
        /// </summary>
        /// <param name="isValid"></param> delegate to determind if the cell is valid
        /// <param name="normalize"></param> normalize the cell
        /// <param name="version"></param> version information
        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version)
           : base(isValid, normalize, version)

        {
            Changed = false;
            this.IsValid = isValid;
            this.Normalize = normalize;
            this.Version = version;
            cellMaps = new Dictionary<string, Cell>();
            dependencyGraph = new DependencyGraph();
        }

        /// <summary>
        /// constructor that read the file path
        /// </summary>
        /// <param name="filePath"></param> filepath that save the file
        /// <param name="isValid"></param> check the cell is valid
        /// <param name="normalize"></param> normalize the cell
        /// <param name="version"></param> version information
        public Spreadsheet(String filePath, Func<string, bool> isValid, Func<string, string> normalize, string version)
            : base(isValid, normalize, version)
        {
            Changed = false;
            this.IsValid = isValid;
            this.Normalize = normalize;
            this.Version = version;
            cellMaps = new Dictionary<string, Cell>();
            dependencyGraph = new DependencyGraph();
            //read the filepath to check if any problems on opning, close and reading
            readFile(filePath);
        }

        /// <summary>
        /// traver the cells to see if any problems in opening, reading and closing
        /// </summary>
        /// <param name="filePath"></param> filepath
        /// <exception cref="SpreadsheetReadWriteException"></exception>
        private void readFile(String filePath)
        {
            try
            {
                String getAll = File.ReadAllText(filePath);
                //deserialize the json file to dictionary
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                JsonCell jsonCell = JsonConvert.DeserializeObject<JsonCell>(getAll);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                Dictionary<String, Cell> dictionary = jsonCell.cells;
                // if the versin is not matched, throw exception 
                if (jsonCell.version != this.Version)
                {
                    throw new SpreadsheetReadWriteException("");
                }
                foreach (String tempName in dictionary.Keys)
                {
                    //assign the temp cell
                    String cellName = tempName;
                    String cellContent = dictionary[tempName].contentString;
                    //if the cell is not valid, throw exception immediately
                    if (isValid(cellName) == false)
                    {
                        throw new SpreadsheetReadWriteException("");
                    }
                    //if the cellContent's first char is =
                    if (cellContent.First() == '=')
                    {
                        try
                        {
                            Formula temp = new Formula(cellContent.Substring(1), this.Normalize, this.IsValid);
                            if (this.cellMaps.ContainsKey(cellName))
                            {
                                throw new SpreadsheetReadWriteException("");
                            }
                            try
                            {
                                SetContentsOfCell(cellName, cellContent);
                            }
                            catch (CircularException)
                            {
                                throw new SpreadsheetReadWriteException("");
                            }
                            catch (InvalidNameException)
                            {
                                throw new SpreadsheetReadWriteException("");
                            }
                            catch (FormulaFormatException)
                            {
                                throw new SpreadsheetReadWriteException("");
                            }
                        }
                        catch
                        {
                            throw new SpreadsheetReadWriteException("");
                        }
                    }
                    //other wise simply use SetContentofCell
                    else
                    {
                        SetContentsOfCell(cellName, cellContent);
                    }
                }               
            }
            catch
            {
                throw new SpreadsheetReadWriteException("");
            }
        }

        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            List<String> result = new List<string>();
            foreach (String temp in cellMaps.Keys)
            {
                // check the cell is not null or empty
                if (cellMaps[temp] != null)
                {
                    if (cellMaps[temp].content.ToString() != "")
                    {
                        result.Add(temp);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// If name is invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        /// </summary>
        public override object GetCellContents(string name)
        {
            if (name == null || !isValid(name))
            {
                throw new InvalidNameException();
            }
            //if the cellMaps contains the name of key
            if (cellMaps.ContainsKey(this.Normalize(name)))
            {
                Cell cell = cellMaps[this.Normalize(name)];
                return cell.content;
            }
            else return "";
        }

        /// <summary>
        /// If name is invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes number.  The method returns a
        /// list consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        protected override IList<string> SetCellContents(string name, double number)
        {
            if (name == null || !isValid(name))
            {
                throw new InvalidNameException();
            }
            //create new cell
            Cell newCell = new Cell(number);
            List<String> cellsName = new List<String>();
            //repalce the dependency graph's dependents(empty)
            this.dependencyGraph.ReplaceDependents(name, cellsName);
            List<String> result = new List<String>(GetCellsToRecalculate(name));
            this.cellMaps[name] = newCell;
            this.cellMaps[name].value = number;
            Changed = true;
            foreach (String temp in result)
            {
                try
                {
                    this.cellMaps[temp].value = calculateCell(temp);
                }
                catch
                {
                    this.cellMaps[temp].value = new FormulaError();
                }
            }
            return result;
        }

        /// <summary>
        /// If name is invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes text.  The method returns a
        /// list consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        protected override IList<string> SetCellContents(string name, string text)
        {
            if (name == null || !isValid(name))
            {
                throw new InvalidNameException();
            }
            //create new cell
            Cell newCell = new Cell(text);
            List<String> cellsName = new List<string>();
            //repalce the dependency graph's dependents(empty)
            this.dependencyGraph.ReplaceDependents(name, cellsName);
            List<String> result = new List<String>(GetCellsToRecalculate(name));
            this.cellMaps[name] = newCell;
            this.cellMaps[name].value = text;

            foreach (String temp in result)
            {
                try
                {
                    this.cellMaps[temp].value = calculateCell(temp);
                }
                catch
                {
                    this.cellMaps[temp].value = new FormulaError();
                }
            }
            Changed = true;
            return result;
        }

        /// <summary>
        /// If name is invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if changing the contents of the named cell to be the formula would cause a 
        /// circular dependency, throws a CircularException, and no change is made to the spreadsheet.
        /// 
        /// Otherwise, the contents of the named cell becomes formula.  The method returns a
        /// list consisting of name plus the names of all other cells whose value depends,
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        protected override IList<string> SetCellContents(string name, Formula formula)
        {
            if (name == null || !this.IsValid(name))
            {
                throw new InvalidNameException();
            }
            // cerate new cell
            Cell newCell = new Cell(formula);
            IEnumerable<String> cellsName = (IEnumerable<String>)formula.GetVariables();
            this.dependencyGraph.ReplaceDependents(name, cellsName);
            //test if has circular exception
            List<String> result = new List<String>(GetCellsToRecalculate(name));
            this.cellMaps[name] = newCell;
            foreach (String temp in result)
            {
                try
                {
                    this.cellMaps[temp].value = calculateCell(temp);
                }
                catch
                {
                    this.cellMaps[temp].value = new FormulaError();
                }
            }
            Changed = true;
            return result;
        }

        /// <summary>
        /// Returns an enumeration, without duplicates, of the names of all cells whose
        /// values depend directly on the value of the named cell.  In other words, returns
        /// an enumeration, without duplicates, of the names of all cells that contain
        /// formulas containing name.
        /// 
        /// For example, suppose that
        /// A1 contains 3
        /// B1 contains the formula A1 * A1
        /// C1 contains the formula B1 + A1
        /// D1 contains the formula B1 - C1
        /// The direct dependents of A1 are B1 and C1
        /// </summary>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            List<String> directDependents = new List<String>(dependencyGraph.GetDependees(name));
            return directDependents;
        }
        /// <summary>
        /// helper method determing if the cell is valid
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        private static bool isValid(String cell)
        {
            String valCell = @"^[a-zA-Z_](?: [a-zA-Z_]|\d)*$";
            if (Regex.IsMatch(cell, valCell) == true)
                return true;
            else return false;
        }

        /// <summary>
        /// Writes the contents of this spreadsheet to the named file using a JSON format.
        /// The JSON object should have the following fields:
        /// "Version" - the version of the spreadsheet software (a string)
        /// "cells" - an object containing 0 or more cell objects
        ///           Each cell object has a field named after the cell itself 
        ///           The value of that field is another object representing the cell's contents
        ///               The contents object has a single field called "stringForm",
        ///               representing the string form of the cell's contents
        ///               - If the contents is a string, the value of stringForm is that string
        ///               - If the contents is a double d, the value of stringForm is d.ToString()
        ///               - If the contents is a Formula f, the value of stringForm is "=" + f.ToString()
        /// 
        /// For example, if this spreadsheet has a version of "default" 
        /// and contains a cell "A1" with contents being the double 5.0 
        /// and a cell "B3" with contents being the Formula("A1+2"), 
        /// a JSON string produced by this method would be:
        /// 
        /// {
        ///   "cells": {
        ///     "A1": {
        ///       "stringForm": "5"
        ///     },
        ///     "B3": {
        ///       "stringForm": "=A1+2"
        ///     }
        ///   },
        ///   "Version": "default"
        /// }
        /// 
        /// If there are any problems opening, writing, or closing the file, the method should throw a
        /// SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        public override void Save(string filename)
        {
            try
            {
                //use private class
                JsonCell jsonCell = new JsonCell(cellMaps, this.Version);
                jsonCell.version = this.Version;
                jsonCell.cells = cellMaps;
                foreach (String temp in jsonCell.cells.Keys)
                {
                    // if the cell is formula, add char '='
                    if (jsonCell.cells[temp].content is Formula)
                    {
                        jsonCell.cells[temp].contentString = "=" + jsonCell.cells[temp].content.ToString();
                    }
                    else
                    {
                        jsonCell.cells[temp].contentString = jsonCell.cells[temp].content.ToString();
                    }
                }
                //serialize the Jsoncell private class
                String toJson = JsonConvert.SerializeObject(jsonCell);
                File.WriteAllText(filename, toJson);
                Changed = false;
            }
            catch (Exception)
            {
                throw new SpreadsheetReadWriteException("");
            }
        }

        /// <summary>
        /// If name is invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the value (as opposed to the contents) of the named cell.  The return
        /// value should be either a string, a double, or a SpreadsheetUtilities.FormulaError.
        /// </summary>
        public override object GetCellValue(string name)
        {
            //if name is not valid
            if (name == null || isVar(name) == false)
            {
                throw new InvalidNameException();
            }
            if (this.cellMaps.ContainsKey(name) == false)
            {
                return "";
            }
            else
            {
                return this.cellMaps[name].value;
            }
        }

        /// <summary>
        /// If name is invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if content parses as a double, the contents of the named
        /// cell becomes that double.
        /// 
        /// Otherwise, if content begins with the character '=', an attempt is made
        /// to parse the remainder of content into a Formula f using the Formula
        /// constructor.  There are then three possibilities:
        /// 
        ///   (1) If the remainder of content cannot be parsed into a Formula, a 
        ///       SpreadsheetUtilities.FormulaFormatException is thrown.
        ///       
        ///   (2) Otherwise, if changing the contents of the named cell to be f
        ///       would cause a circular dependency, a CircularException is thrown,
        ///       and no change is made to the spreadsheet.
        ///       
        ///   (3) Otherwise, the contents of the named cell becomes f.
        /// 
        /// Otherwise, the contents of the named cell becomes content.
        /// 
        /// If an exception is not thrown, the method returns a list consisting of
        /// name plus the names of all other cells whose value depends, directly
        /// or indirectly, on the named cell. The order of the list should be any
        /// order such that if cells are re-evaluated in that order, their dependencies 
        /// are satisfied by the time they are evaluated.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        public override IList<string> SetContentsOfCell(string name, string content)
        {
            if (content == null)
            {
                throw new ArgumentNullException();
            }
            if (name == null || this.IsValid(name) == false)
            {
                throw new InvalidNameException();
            }
            double tempDouble;
            bool doubleNum = Double.TryParse(content, out tempDouble);
            //if content is double
            if (doubleNum == true)
            {
                return SetCellContents(name, tempDouble);
            }
            else
            {
                //if content is formula
                if (content.StartsWith("="))
                {
                    String realContent = content.Substring(1);
                    Formula formula = new Formula(realContent, Normalize, IsValid);
                    return SetCellContents(name, formula);
                }
                else
                {
                    return SetCellContents(name, content);
                }
            }

        }

        /// <summary>
        /// private class that contains cellMaps and version information
        /// </summary>
        [JsonObject(MemberSerialization.OptIn)]
        private class JsonCell
        {
            /// <summary>
            /// Dictionary
            /// </summary>
            [JsonProperty]
            public Dictionary<String, Cell> cells { set; get; }
            /// <summary>
            /// version information
            /// </summary>
            [JsonProperty(PropertyName = "Version")]
            public string version { set; get; }

            /// <summary>
            /// constructor of JsonCell containing cellMaps and version information
            /// </summary>
            /// <param name="cells"></param>
            /// <param name="version"></param>
            public JsonCell(Dictionary<String, Cell> cells, String version)
            {
                this.cells = cells;
                this.version = version;
            }
        }

        /// <summary>
        /// private clss
        /// </summary>
        /// [JsonObject(MemberSerialization.OptIn)]
        /// 
        [JsonObject(MemberSerialization.OptIn)]
        private class Cell
        {
            /// <summary>
            /// get and set content
            /// </summary>
            public String cellName { set; get; }
            [JsonProperty(PropertyName = "stringForm")]
            public String contentString { set; get; }
            public object content { get; set; }

            public object value { get; set; }
            /// <summary>
            /// cell constructor for double type
            /// </summary>

            /// <param name="number"></param> double type
            public Cell(double number)
            {
                content = number;
            }
            /// <summary>
            /// cell constructor for string type
            /// </summary>
            /// <param name="text"></param> string type
            public Cell(string text)
            {
                content = text;
            }
            /// <summary>
            /// cell constructor for formula type
            /// </summary>
            /// <param name="formula"></param> formula type
            public Cell(Formula formula)
            {
                content = formula;
            }

            /// <summary>
            /// set content
            /// </summary>
            /// <param name="content"></param>
            public Cell(object content)
            {
                this.content = content;
            }
            /// <summary>
            /// default constructor
            /// </summary>
            public Cell()
            {
            }
        }
    }
}
