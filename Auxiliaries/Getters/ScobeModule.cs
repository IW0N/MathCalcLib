using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MathCalc.Auxiliaries.Checker;
namespace MathCalc.Auxiliaries.Getters
{
    
    public static class ScobeModule
    {
        public static Dictionary<char, int> ranks = new()
        {
            {'+',0 }, {'-',0},
            {'*',1 }, {'/',1 },
            {'^',2 }
        };
        public static string RemoveScobes(string formula,List<int> constrs)
        {
            string newFormula = formula;
            string expr = MathSpace.expression_name.ToString();
            if (!IsFunction(formula,out _,constrs))
            {
                //CorrectScobeConstraints(ref constrs, ref newFormula);
                int shift = 0;

                for (int i = 0; i < constrs.Count - 1; i += 2)
                {
                    int i0 = constrs[i];
                    int i1 = constrs[i + 1];
                    newFormula = newFormula.Remove(i0 - shift, i1 - i0 + 1);
                    newFormula = newFormula.Insert(i0 - shift, expr);
                    shift += i1 - i0;
                }
                

            }
            else
            {
                string[] parametres = MainGetter.GetFunctionParams(formula);
                int i = 0;
                foreach (string param in parametres)
                {
                    char adding_param_symb = i==parametres.Length-1?')':',';
                    newFormula = newFormula.Replace(param+adding_param_symb, expr+adding_param_symb);
                    i++;
                }
            }
            return newFormula;
        }
        
    }
}
