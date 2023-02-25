// Skeleton implementation written by Joe Zachary for CS 3500, September 2013.
// Version 1.1 (Fixed error in comment for RemoveDependency.)
// Version 1.2 - Daniel Kopta 
//               (Clarified meaning of dependent and dependee.)
//               (Clarified names in solution/project structure.)


using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// (s1,t1) is an ordered pair of strings
    /// t1 depends on s1; s1 must be evaluated before t1
    /// 
    /// A DependencyGraph can be modeled as a set of ordered pairs of strings.  Two ordered pairs
    /// (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 equals t2.
    /// Recall that sets never contain duplicates.  If an attempt is made to add an element to a 
    /// set, and the element is already in the set, the set remains unchanged.
    /// 
    /// Given a DependencyGraph DG:
    /// 
    ///    (1) If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
    ///        (The set of things that depend on s)    
    ///        
    ///    (2) If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
    ///        (The set of things that s depends on) 
    //
    // For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    //     dependents("a") = {"b", "c"}
    //     dependents("b") = {"d"}
    //     dependents("c") = {}
    //     dependents("d") = {"d"}
    //     dependees("a") = {}
    //     dependees("b") = {"a"}
    //     dependees("c") = {"a"}
    //     dependees("d") = {"b", "d"}
    /// </summary>
    /// 

    ///
    /// Author: Qiaoyi Cai
    /// UID:1285695
    /// Date:9/9/2022
    /// 
    ///
    public class DependencyGraph
    {
        Dictionary<string, HashSet<string>> dependents;
        Dictionary<string, HashSet<string>> dependees;

        private int dependentsSize;
        private int dependeesSize;
        /// <summary>
        /// Creates an empty DependencyGraph.
        /// </summary>
        /// 
        public DependencyGraph()
        {
            dependents = new Dictionary<string, HashSet<string>>();
            dependees = new Dictionary<string, HashSet<string>>();
            dependentsSize = 0;
            dependeesSize = 0;

        }
        /// <summary>
        /// The number of ordered pairs in the DependencyGraph.
        /// </summary>
        public int Size
        {
            get
            {
                return dependeesSize;
            }
        }

        /// <summary>
        /// The size of dependees(s).
        /// This property is an example of an indexer.  If dg is a DependencyGraph, you would
        /// invoke it like this:
        /// dg["a"]
        /// It should return the size of dependees("a")
        /// </summary>
        public int this[string s]
        {
            get
            {
                // return dependeesSize;

                if (dependees.ContainsKey(s))
                {
                    return dependees[s].Count;
                }
                return 0;
            }
        }

        /// <summary>
        /// Reports whether dependents(s) is non-empty.
        /// </summary>
        public bool HasDependents(string s)
        {
            return dependents.ContainsKey(s) && dependents[s].Count > 0;
        }

        /// <summary>
        /// Reports whether dependees(s) is non-empty.
        /// </summary>
        public bool HasDependees(string s)
        {
            return dependees.ContainsKey(s) && dependees[s].Count > 0;
        }

        /// <summary>
        /// Enumerates dependents(s).
        /// </summary>
        public IEnumerable<string> GetDependents(string s)
        {
            if (dependents.ContainsKey(s))
            {
                return dependents[s];
            }
            else
            {
                //return a empty list
                return new List<String>();
            }
        }

        /// <summary>
        /// Enumerates dependees(s).
        /// </summary>
        public IEnumerable<string> GetDependees(string s)
        {

            if (dependees.ContainsKey(s))
            {
                //return a empty list
                return dependees[s];
            }

            return new List<String>();
        }


        /// <summary>
        /// <para>Adds the ordered pair (s,t), if it doesn't exist</para>
        /// 
        /// <para>This should be thought of as:</para>   
        /// 
        ///   t depends on s
        ///
        /// </summary>
        /// <param name="s"> s must be evaluated first. T depends on S</param>
        /// <param name="t"> t cannot be evaluated until s is</param>        /// 
        public void AddDependency(string s, string t)
        {
            if (dependents.ContainsKey(s))
            {
                if (!dependents[s].Contains(t))
                {
                    dependents[s].Add(t);
                    dependentsSize++;
                }
            }
            else if (!dependents.ContainsKey(s))
            {
                //if dependents doesn't have the key, then creat List<string> tempDependententsList = new List<string>();e a new list and assign into key

                // tempDependententsList.Add(t);
                HashSet<String> tempDependententsList = new HashSet<String>();
                tempDependententsList.Add(t);
                dependents.Add(s, tempDependententsList);
                dependentsSize++;
            }

            if (dependees.ContainsKey(t))
            {
                if (!dependees[t].Contains(s))
                {
                    dependees[t].Add(s);
                    dependeesSize++;
                }
            }
            else if (!dependees.ContainsKey(t))
            {
                //if dependees doesn't have the key, then create a new list and assign into key
                // List<string> tempDependeesList = new List<string>();
                HashSet<String> tempDependeesList = new HashSet<String>();
                tempDependeesList.Add(s);
                dependees.Add(t, tempDependeesList);
                dependeesSize++;
            }

        }

        /// <summary>
        /// Removes the ordered pair (s,t), if it exists
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        public void RemoveDependency(string s, string t)
        {
            if (dependents.ContainsKey(s))
            {
                //remove both dependents and dependees
                dependents[s].Remove(t);
                removeDependees(t, s);
                dependentsSize--;
                dependeesSize--;
            }

        }
        /// <summary>
        /// helper method that remove dependees of ordered pair
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        private void removeDependees(String s, String t)
        {
            if (dependees.ContainsKey(s))
            {
                if (dependees[s].Contains(t))
                {
                    dependees[s].Remove(t);
                }
            }

        }
        /// <summary>
        /// Removes all existing ordered pairs of the form (s,r).  Then, for each
        /// t in newDependents, adds the ordered pair (s,t).
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            if (HasDependents(s))
            {

                HashSet<string> list = new HashSet<string>(dependents[s]);
                foreach (String temp in list)
                {
                    //remove each dependent in the list of the key
                    RemoveDependency(s, temp);
                }

                dependentsSize = 0;

                foreach (string dependent in newDependents)
                {
                    //add each new dependent in the list of the key
                    AddDependency(s, dependent);
                }
            }
            else
            {
                foreach (string dependent in newDependents)
                {
                    // create new key and assign list to key
                    AddDependency(s, dependent);
                }
            }
        }

        /// <summary>
        /// Removes all existing ordered pairs of the form (r,s).  Then, for each 
        /// t in newDependees, adds the ordered pair (t,s).
        /// </summary>
        public void ReplaceDependees(string s, IEnumerable<string> newDependees)
        {
            if (dependees.ContainsKey(s))
            {

                HashSet<string> list = new HashSet<string>(dependees[s]);
                foreach (String temp in list)
                {
                    //remove each dependees in the list of the key
                    RemoveDependency(temp, s);
                }
                foreach (string dependee in newDependees)
                {
                    //add each new dependent in the list of the key
                    AddDependency(dependee, s);
                }
            }
            else
            {
                foreach (string dependee in newDependees)
                {
                    // create new key and assign list to key
                    AddDependency(dependee, s);
                }
            }
        }

    }

}