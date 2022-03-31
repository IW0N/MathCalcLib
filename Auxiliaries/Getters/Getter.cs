using System;
using System.Collections.Generic;
namespace MathCalc.Auxiliaries.Getters
{
    using static MathCalc.Auxiliaries.Checker;
    using static Getter2;
    static class Getter
    {
        public static char[] GetOperators(string formula, List<char> varibles, bool without_scobes = true)
        {
            List<char> ops = null;
            string formula2 = !without_scobes ? RemoveScobes(formula, varibles) : formula;
            int i = 0;
            foreach (char c in formula2)
            {
                if (IsOperator(formula, i))
                {
                    if (ops == null)
                        ops = new List<char>();
                    ops.Add(c);
                }
                i++;
            }
            return ops.ToArray();
        }
        public static Varible[] GetNumbersAndVaribles(string formula, List<char> varibles, bool without_scobes = true)
        {
            List<Varible> vars = new List<Varible>();
            string formula2 = !without_scobes ? RemoveScobes(formula, varibles) : formula;
            string num = "";
            bool read_num = false;
            for (int i = 0; i < formula2.Length; i++)
            {
                char c = formula2[i];
                if (IsVarible(formula, varibles, i)||c==MathSpace.expression_name)
                    vars.Add(c);
                else
                {
                    if (IsNum(formula2, i))
                    {
                        num += c;
                        read_num = true;
                        if (i == formula.Length - 1)
                            vars.Add(double.Parse(num.Replace('.', ',')));
                    }
                    else if (read_num)
                    {
                        vars.Add(double.Parse(num.Replace('.',',')));
                        num = "";
                        read_num = false;
                    }

                }
            }
            return vars.ToArray();
        }
        //Method that get ranges of constant names in formula
        static List<int> GetConstantConstraints(string formula, List<char> varibles)
        {
            List<int> constraints = new List<int>();
            bool read_const = false,isChar;
            for (int i = 0; i < formula.Length; i++)
            {

                char c = formula[i];
                isChar = c!=','&&c!='('&&c!=')'&&IsChar(formula,i,varibles);
                if (isChar && !read_const)
                {
                    constraints.Add(i);
                    read_const = true;
                }
                else if ((!isChar || i == formula.Length - 1)&&read_const)
                {
                    if (c != '(')
                    {
                        int i1 = !isChar ? i - 1 : i;
                        constraints.Add(i1);
                    }
                    else
                        constraints.RemoveAt(constraints.Count - 1);
                    read_const = false;
                    
                }
            }
            return constraints;
        }
        static Dictionary<string,List<int>> GetConstants(string formula, List<int> constraints)
        {
            var constant_names_constraints = new Dictionary<string, List<int>>();
            int len;
            List<int> list;
            for (int i = 0; i < constraints.Count; i += 2)
            {
                int i0 = constraints[i];
                int i1 = constraints[i + 1];
                len = i1 - i0 + 1;
                string constant_name = formula.Substring(i0, len);
                if (constant_names_constraints.ContainsKey(constant_name))
                {
                    list=constant_names_constraints[constant_name];
                    list.Add(i0);
                   
                }
                else
                {
                    list = new List<int>() {len,i0};
                    constant_names_constraints.Add(constant_name,list);
                }
            }
            return constant_names_constraints;
        }
        static void ShiftPositions(string started_const, Dictionary<string,List<int>> dict,int old_position,int newShift)
        {
            bool change_pos=false;
            foreach (var kvp in dict)
            {
                change_pos = kvp.Key == started_const||change_pos;
                if (change_pos)
                {
                    var contraints =dict[kvp.Key];
                    for(int i=1;i<contraints.Count;i++)
                    {
                        int pos = contraints[i];
                        if (pos>old_position)
                            contraints[i] += newShift;
                    }
                }
            }
        }
        //This method replace constants in the global formula (formula, that was gotten from user) by appropriate value 
        public static string ReplaceConstantsByNumbers(string formula, List<char> varibles)
        {
            var constraints = GetConstantConstraints(formula, varibles);
            var const_names_constrs = GetConstants(formula, constraints);
            string newFormula = formula;
            int shift;
            foreach (var const_pair in const_names_constrs)
            {
                var constrs = const_pair.Value;
                string constant = const_pair.Key;
                int len = constrs[0];
                double const_value = MathSpace.GetConstant(constant);
                string str_const_val = const_value.ToString().Replace(',','.');
                for (int i=1;i<constrs.Count;i++)
                {
                    int i0 =constrs[i];
                    newFormula=newFormula.Remove(i0, len);
                    newFormula=newFormula.Insert(i0, str_const_val);
                    shift = str_const_val.Length - len;
                    ShiftPositions(constant, const_names_constrs, i0,shift);
                }
               
            }
            return newFormula;
        }
        public static string[] GetFunctionParams(string function)
        {
            int scobe0 = function.IndexOf('(');
            int scobe1 = function.Length - 1;
            string str_params = function.Substring(scobe0+1,scobe1-scobe0-1);
            return str_params.Split(",");
            
        }
        
    }
}
