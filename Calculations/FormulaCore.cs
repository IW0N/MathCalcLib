using System;
using System.Collections.Generic;
using System.Text;
namespace MathCalc
{
    //Parent class of MathFormula contains the most its fields
    [Serializable]
    public abstract class FormulaCore
    {
        //function name of this formula
        protected string func;
        //array, that contains all varibles and theres next expressions
        protected internal Varible[] values;
        //varible values, that already determinated
        protected double[] determined_values;
        //all operators of formula without scobes and expressions
        protected char[] operators;
        //all varibles, that use in main formula.
        protected string[] varibles;
        //indexes of appropriate varible in the 'values' 
        protected Dictionary<string, List<int>> varible_indexes;
        //indexes of expressions of value in the 'values'
        protected int[] expression_indexes;
        public abstract double Calculate(params double[] input);
        
        
    }
}
