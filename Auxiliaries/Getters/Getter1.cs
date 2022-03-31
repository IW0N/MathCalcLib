using System;
using System.Collections.Generic;
using System.Text;

namespace MathCalc.Auxiliaries.Getters
{
    using static MathCalc.Auxiliaries.Checker;
    
    static class Getter1
    {
        internal static readonly Dictionary<char, int> operator_rank = new Dictionary<char, int>()
        {
            {'+',0},{'-',0 },{'*',1},{'/',1},{'^',2}
        };
        //Method Get smallest rank in the formula. Returns -1 if formula hasn't operator 
        internal static int GetSmallestRank(string formula)
        {
            bool zero = formula.Contains('+') || formula.Contains('-');
            if (zero)
                return 0;
            bool one = formula.Contains('*') || formula.Contains('/');
            if (one)
                return 1;
            bool two = formula.Contains('^');
            if (two)
                return 2;
            return -1;
        }
        public static List<int> GetCorrectedRangesOfSimpleFormula(string formula)
        {
            List<int> ranges = new List<int>();
            
            int sml_rank = GetSmallestRank(formula);

            if (sml_rank != -1)
            {
                var dict = GetNumberedOperators(formula);
                bool read_expr = false;
                int i0 = 0;
                int x = 0;
                int last_index = 0;
                foreach (var kvp in dict)
                {
                    char oper = kvp.Value;
                    int index = kvp.Key;
                    int rank = operator_rank[oper];
                    if (rank > sml_rank && !read_expr)
                    {
                        i0 = last_index;
                        i0 += x > 0 ? 1 : 0;
                        read_expr = true;
                    }
                    if ((rank == sml_rank || x == dict.Count - 1) && read_expr)
                    {
                        int i1 = rank == sml_rank ? index - 1 : formula.Length - 1;
                        ranges.Add(i0); ranges.Add(i1);
                        read_expr = false;
                    }
                    last_index = index;
                    x++;
                }
            }
            return ranges;
        }
        //Method compare index of operator and the operator itself.
        static Dictionary<int,char> GetNumberedOperators(string formula)
        {
            var dict = new Dictionary<int,char>();
            int i = 0;
            foreach(char c in formula)
            {
                bool oper=IsOperator(formula, i);
                if (oper)
                    dict.Add(i,c);
                i++;
            }
            return dict;
           
        }
        public static Dictionary<char, List<int>> GetVaribleIndexes(List<char> varibles,MathFormula formula)
        {
            Dictionary<char, List<int>> indexes = new Dictionary<char, List<int>>();
            var values = formula.values;
            for (int i = 0; i < values.Length; i++)
            {
                var val = values[i];
                if (varibles.Contains(val.name))
                {
                    if (indexes.ContainsKey(val.name))
                        indexes[val.name].Add(i);
                    else
                        indexes.Add(val.name, new List<int>() { i });
                }
            }
            return indexes;
        }
        public static int[] GetExpressionIndexes(Varible[] varibles)
        {
            List<int> arr = new List<int>();
            int i = 0;
            var expr_name = MathSpace.expression_name;
            foreach (Varible val in varibles)
            {
                if (val.name==expr_name)
                    arr.Add(i);
                i++;
            }
            return arr.ToArray();
        }
    }
}
