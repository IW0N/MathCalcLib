using System.Collections.Generic;
namespace MathCalc.Auxiliaries.Getters
{
    using static MathCalc.Auxiliaries.Checker;
    using static Getter1;
    static class Getter2
    {
        static void CorrectScobeConstraints(ref List<int> constrs, ref string formula, List<char> varibles)
        {
            if (ScobeHasOneExpr(formula, constrs))
            {
                int lastIndex = formula.Length - 1;
                formula = formula.Remove(0, 1);
                formula = formula.Remove(lastIndex - 1, 1);
                constrs = GetScobeConstraints(formula, varibles);
            }
            
            else if(constrs.Count==0)
                constrs = GetCorrectedRangesOfSimpleFormula(formula);

        }
        static List<int> GetScobeConstraints(string formula, List<char> varibles)
        {
            List<int> scobeConstraints = new List<int>();
            int openedScobeCount = 0;
            int closedScobeCount = 0;
            int i = 0, i0 = 0;
            bool readScobe = false, readFunction = false;
            char expr_name = MathSpace.expression_name;
            foreach (char c in formula)
            {
                openedScobeCount = c == '(' ? openedScobeCount + 1 : openedScobeCount;
                closedScobeCount = c == ')' ? closedScobeCount + 1 : closedScobeCount;
                readFunction = readFunction||(c != '(' && c != ')' && c!=expr_name &&IsChar(formula, i, varibles));
                if ((openedScobeCount == 1 || readFunction) && !readScobe)
                {
                    i0 = i;
                    readScobe = true;
                }
                else if (openedScobeCount==closedScobeCount&&openedScobeCount>0)
                {
                    scobeConstraints.Add(i0);
                    scobeConstraints.Add(i);
                    readScobe = false;
                    readFunction = false;
                    openedScobeCount = 0;
                    closedScobeCount = 0;
     
                }
                
                i++;
            }
            return scobeConstraints;
        }
        //Method remove all scobe and expression in formula and replced there by expression constant 
        public static string RemoveScobes(string formula, List<char> varibles)
        {
            string newFormula = formula;
            string expr =MathSpace.expression_name.ToString();
            if (!IsFunction(formula, varibles))
            {
                var constrs = GetScobeConstraints(formula, varibles);
                CorrectScobeConstraints(ref constrs, ref newFormula, varibles);
                int shift = 0;

                for (int i = 0; i < constrs.Count - 1; i += 2)
                {
                    int i0 = constrs[i];
                    int i1 = constrs[i + 1];
                    newFormula = newFormula.Remove(i0 - shift, i1 - i0 + 1);
                    newFormula = newFormula.Insert(i0 - shift, expr);
                    shift += i1 - i0;
                }
                /*constrs=GetCorrectedRangesOfSimpleFormula(newFormula,constrs);
                if (constrs.Count > 0)
                    newFormula = RemoveScobes(newFormula, varibles);*/

            }
            else
            {
                string[] parametres = Getter.GetFunctionParams(formula);
                foreach (string param in parametres)
                {
                    newFormula = newFormula.Replace(param, expr);
                }
            }
            return newFormula;
        }
        public static string RemoveScobes(string[] scobes, string formula)
        {
            string newFormula = formula;
            string expr = MathSpace.expression_name.ToString();
            foreach (string scobe in scobes)
            {
                newFormula = newFormula.Replace(scobe, expr);
            }
            return newFormula;
        }

        public static string[] GetScobes(string formula, List<char> varibles)
        {
            var constrs = GetScobeConstraints(formula, varibles);
            CorrectScobeConstraints(ref constrs, ref formula, varibles);
            return GetScobes(formula,constrs);

        }
        static string[] GetScobes(string formula,List<int> constraints)
        {
            string[] scobes = new string[constraints.Count / 2];

            for (int i = 0; i < constraints.Count - 1; i += 2)
            {
                int i0 = constraints[i];
                int i1 = constraints[i + 1];
                scobes[i / 2] = formula.Substring(i0, i1 - i0 + 1);
            }
            return scobes;
        }
        public static Dictionary<char,int> GetVaribleIds(List<char> varibles)
        {
            var dict = new Dictionary<char, int>(varibles.Count);
            int i = 0;
            foreach (var var_name in varibles)
            {
                dict.Add(var_name, i);
                i++;
            }
            return dict;
        }
       
    }
}
