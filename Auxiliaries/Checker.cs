using System;
using System.Collections.Generic;
using System.Text;
using MathCalc.Auxiliaries.Getters;
namespace MathCalc.Auxiliaries
{
    using static Getter2;
    static class Checker
    {
        public static bool IsOperator(string formula, int index)
        {
            char val = formula[index];
            bool isSpecialOperator = false;
            if (index - 2 >= 0)
            {
                char previous_val = formula[index - 1];
                isSpecialOperator = (val == '+' || val == '-') && index - 1 > 0 &&
                (previous_val == 'E' || previous_val == 'e') && IsNum(formula, index - 2);
            }
            return ((val == '+' || val == '-') && !isSpecialOperator) || val == '*' || val == '/' || val == '^';
        }
        public static bool IsNum(string formula, int char_index)
        {
            char val = formula[char_index];
            bool isJustNum = int.TryParse(val.ToString(), out _);

            if (!isJustNum)
            {
                bool isPoint = val == '.';
                if (!isPoint)
                {
                    bool lastSymbIsNum = char_index - 1 >= 0 && int.TryParse(formula[char_index - 1].ToString(), out _);
                    bool isSpecialE = (val == 'E' || val == 'e') && lastSymbIsNum;
                    if (char_index - 1 >= 0 && !isSpecialE)
                    {

                        char previousSymb = formula[char_index - 1];
                        bool previousSymbIsE = previousSymb == 'E' || previousSymb == 'e';
                        bool pre_previousSymbIsNum = char_index - 2 >= 0 && int.TryParse(formula[char_index - 2].ToString(), out _);
                        bool isSpecialOperator = (val == '+' || val == '-') && previousSymbIsE && pre_previousSymbIsNum;
                        return isSpecialOperator;
                    }
                    else if (!isSpecialE && char_index + 1 < formula.Length && char_index - 1 >= 0)
                    {
                        char next_val = formula[char_index + 1];
                        char previous_val = formula[char_index - 1];
                        bool isSpecialMinus = val == '-' && (previous_val == '(' || IsOperator(formula, char_index - 1))
                            && int.TryParse(next_val.ToString(), out _);
                        return isSpecialMinus;
                    }
                    return isSpecialE;

                }
                return isPoint;
            }
            return isJustNum;
        }
        public static bool IsChar(string formula, int index, List<char> varibles) => !IsNum(formula, index) && !IsOperator(formula, index) && !IsVarible(formula, varibles, index);
        public static bool IsVarible(string formula, List<char> varibles, int index)
        {
           
            bool prevSymbIsOperator = index - 1 >= 0 ? (IsOperator(formula, index - 1) || formula[index - 1] == '(' || formula[index-1]==',') : true;
            bool nextSymbIsOperator = index + 1 < formula.Length ? (IsOperator(formula, index + 1) || formula[index + 1] == ')'||formula[index+1]==',') : true;
            bool currentSymbIsVarible = varibles.Contains(formula[index]);
            return prevSymbIsOperator && currentSymbIsVarible && nextSymbIsOperator;
        }
        public static bool IsFunction(string formula, out string name,List<char> varibles)
        {
            var scobes = GetScobes(formula,varibles);
            name = null;
            if (scobes.Length != 1)
                return false;
            else
            {
                int last_scobe_index = formula.LastIndexOf(')');
                int first_scobe_index = formula.IndexOf('(');
                if (last_scobe_index > -1 && first_scobe_index > -1)
                {
                    string str = formula.Remove(first_scobe_index, last_scobe_index - first_scobe_index + 1);
                    bool exist=MathSpace.ExitstFunction(str);
                    if (exist)
                        name = str;
                    return exist;
                }
            }
            return false;
        }
        public static bool IsFunction(string formula, List<char> varibles) =>
            IsFunction(formula, out _,varibles);
        //Determines, that formula is only one expreesion or scobe
        public static bool ScobeHasOneExpr(string formula, List<int> scobe_cosntraints)
        {
            int lastIndex = formula.Length - 1;
            var constrs = scobe_cosntraints;
            bool res= constrs.Count == 2 && constrs[0] == 0 && constrs[1] == lastIndex && formula[0] == '(' && formula[lastIndex] == ')';
            return res;
        }
        public static bool Continue(int i, int delta, int i_fin) =>
            delta > 0 ? i <= i_fin : i >= i_fin;
        public static bool HasOperator(string formula)=> 
            Getter1.GetSmallestRank(formula) > - 1;
        
        
        
    }
}
