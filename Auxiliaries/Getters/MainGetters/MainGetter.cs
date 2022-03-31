using static MathCalc.Auxiliaries.Checker;
namespace MathCalc.Auxiliaries.Getters
{
    using static ScobeModule;
    using static ConstantModule;
    using static ConstraintFuncs;
    using static ConstraintConditions;
   
    static partial class MainGetter
    {
        public static char[] GetOperators(string formula)
        {
            List<char> ops = null;
            int i = 0;
            foreach (char c in formula)
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
        public static Varible[] GetNumbersAndVaribles(string formula,List<string> varibles)
        {
            var constrs=GetVaribleConstraints(formula);
            string[] values = CutConstraints(formula,constrs);
            Varible[] vars = new Varible[values.Length];
            for (int i = 0; i < values.Length; i++)
                vars[i] = Varible.Parse(values[i]);
            return vars;
        }
        public static Dictionary<string, List<int>> GetVaribleIndexes(List<string> varibles, MathFormula formula)
        {
            var indexes = new Dictionary<string, List<int>>();
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
            List<int> arr = new();
            int i = 0;
            var expr_name = MathSpace.expression_name;
            foreach (Varible val in varibles)
            {
                if (val.name == ""+expr_name)
                    arr.Add(i);
                i++;
            }
            return arr.ToArray();
        }
        public static Dictionary<string, int> GetVaribleIds(List<string> varibles)
        {
            var dict = new Dictionary<string, int>(varibles.Count);
            int i = 0;
            foreach (var var_name in varibles)
            {
                dict.Add(var_name, i);
                i++;
            }
            return dict;
        }
        public static string[] GetScobes(string formula)
        { 
            var constrs=ConstraintFuncs.GetScobeConstraints(formula);
            return CutConstraints(formula, constrs);
        }
        
        public static string[] GetFunctionParams(string function)
        {
            int scobe0 = function.IndexOf('(');
            int scobe1 = function.Length - 1;
            string str_params = function.Substring(scobe0 + 1, scobe1 - scobe0 - 1);
            string[] res=str_params.Split(",");
            return res;

        }
        public static string ReplaceConstantsByNumbers(string formula)
        {
            var named_val_list = GetConstraints<NamedValuesContext>(formula,HandleNamedValuesAdding);
            var named_values = CutConstraints(formula,named_val_list);
            var const_names_constrs = SelectConstants(named_values, named_val_list);

            string newFormula = formula;
            int shift=0;
            foreach (var const_pair in const_names_constrs)
            {
                var constrs = const_pair.Value;
                string constant = const_pair.Key;
                int len = constrs[1]-constrs[0]+1;
                double const_value = MathSpace.GetConstant(constant);
                string str_const_val = const_value.ToString().Replace(',', '.');
                for (int i = 0; i < constrs.Count; i+=2)
                {
                    int i0 = constrs[i];
                    newFormula = newFormula.Remove(i0, len);
                    newFormula = newFormula.Insert(i0, str_const_val);
                    shift = str_const_val.Length - len;
                    ShiftPositions(constant, const_names_constrs, i0, shift);
                }

            }
            return newFormula;
        }
        
    }
}
