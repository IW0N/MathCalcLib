using System;
using System.Collections.Generic;
using System.Text;
using MathCalc.Auxiliaries.Getters;
namespace MathCalc.Auxiliaries
{
    public static class Checker
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
            return  val == '*' || val == '/' || val == '^'|| ((val == '+' || val == '-') && !isSpecialOperator);
        }
        public static bool IsNum(string formula, int char_index)
        {
            char val = formula[char_index];
            bool isJustNum = IsSimpleNumber(val);

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
                        bool pre_previousSymbIsNum = char_index - 2 >= 0 && IsSimpleNumber(formula[char_index-2]);
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
        public static bool IsFunction(string formula, out string name,List<int> scobes=null)
        {
            scobes = scobes==null?ConstraintFuncs.GetScobeConstraints(formula):scobes;
            name = null;
            if (scobes.Count>2)
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
            IsFunction(formula, out _);
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
        
        public static bool IsLetter(string formula, int index,ref bool reading_value)
        {
            char c = formula[index];
            bool is_number = IsNum(formula,index),is_scobe=c=='('||c==')';
            bool is_letter = !IsOperator(formula,index)&&!is_number&&!is_scobe;
            bool is_special_e;
            if (index < formula.Length - 1 && index > 0)
            {
                char prev_val = formula[index - 1];
                char next_val = formula[index + 1];
                is_special_e = (c == 'E' || c == 'e') && IsSimpleNumber(prev_val) && (next_val == '+' || next_val == '-');
                is_letter = is_letter && !is_special_e;
            }
            if (!is_letter)
                is_letter = is_letter || (is_number && reading_value);
            else
                reading_value = true;
            return is_letter && c != '.' && c != ',' && c != MathSpace.expression_name;
        }
        public static bool IsLetter(string formula,int index)
        {
            bool a=false; return IsLetter(formula, index,ref a);
        }
        public static bool IsSimpleNumber(char c)
            => byte.TryParse(c.ToString(), out _);


    }
}
