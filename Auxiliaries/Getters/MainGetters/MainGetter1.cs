using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathCalc.Auxiliaries.Getters
{
    using static Checker;
    static partial class MainGetter
    {
        public static string[] GetScobes(string formula,List<int> constraints)
        {
            string[] scobes = ConstraintFuncs.CutConstraints(formula,constraints);
            for (int i=0;i<scobes.Length;i++)
            {
                string scobe = scobes[i];
                if(!IsFunction(scobe,out _,constraints) && scobe[0]=='(')
                {
                    scobe = scobe.Remove(0, 1);
                    scobe = scobe.Remove(scobe.Length - 1, 1);
                    scobes[i] = scobe;
                }
            }
            return scobes;

        }
    }
}
