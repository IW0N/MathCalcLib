using System;
using System.Collections.Generic;
using System.Text;

namespace MathCalc.Auxiliaries.Getters
{
    using static Getter1;
    using static Checker;
    static class Getter3
    {
        public static string[] GetScobes(string simpleFormula,string full_formula,List<char> varibles)
        {
            if (HasOperator(simpleFormula))
            {
                int rank = GetSmallestRank(simpleFormula);

                char[] separotrs = rank switch
                {
                    0 => new char[2] { '+', '-' },
                    1 => new char[2] { '*', '/' },
                    2 => new char[1] { '^' },
                    _ => throw new Exception()
                };
                string[] fs = simpleFormula.Split(separotrs);
                var scobes = FiltrateScobes(fs);
                var unProccesedScobes = Getter2.GetScobes(full_formula, varibles);
                return CorrectScobes(full_formula, scobes, unProccesedScobes).ToArray();
            }
            else
                return new string[0];
        }
        static List<string> FiltrateScobes(string[] scobes)
        {
            List<string> scs = new List<string>(scobes);
            string expression = MathSpace.expression_name.ToString();
            for(int i=0;i<scs.Count;i++)
            {
                string scobe = scs[i];
                if (scobe!=expression&&!HasOperator(scobe))
                {
                    scs.RemoveAt(i);
                    i--;
                }
            }
            return scs;
        }
        static List<string> CorrectScobes(string full_formula,List<string> scobes,string[] unProcessedScobes)
        {
            int scobeIndex = 0;
            char expr =MathSpace.expression_name;
            for (int i=0;i<scobes.Count;i++)
            {
                string scobe = scobes[i];
               
                while(scobe.Contains(expr))
                {
                    int expr_index = scobe.IndexOf(expr);
                    string val = unProcessedScobes[scobeIndex];
                    scobe = scobe.Remove(expr_index, 1);
                    scobe = scobe.Insert(expr_index, val);
                    scobes[i] = scobe;
                    scobeIndex++;
                }
            }
            return scobes;
        }
    }
}
