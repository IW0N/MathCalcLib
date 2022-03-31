namespace MathCalc.Auxiliaries.Getters
{
    public static partial class ConstraintFuncs
    {
        
        public static List<int> GetConstraints<T>(string formula, ContextCondition<T> condition)
            where T : ConstraintContext, new()
        {
            T? context = new();
            List<int> constraints = new();
            const int def_i0 = -1;//default i0
            int i0 = def_i0;
            bool condition_invoked;
            int len = formula.Length;
            ref bool reading_constr = ref context.reading_constraint,
                del_constr = ref context.delete_constraint;
            for (int i = 0; i < formula.Length; i++)
            {
                char c = formula[i];
                condition_invoked = condition.Invoke(formula, i, context);
                bool need_define_i0 = condition_invoked && !reading_constr && !del_constr;
                bool add_indexes = condition_invoked && reading_constr && !del_constr;
                if (need_define_i0)
                {
                    i0 = i;
                    reading_constr = true;
                    if (i == len - 1 && IsNotScobeContext(context))
                        constraints.AddConstr(i0, c == ')' ? i - 1 : i);
                }
                else if (add_indexes)
                {
                    int delta = i < len - 1 || (c == ')' && IsNotScobeContext(context)) ? 1 : 0;
                    constraints.AddConstr(i0, i - delta);
                    context.Reset(def_i0, ref i0);
                }
                else if (del_constr)
                    context.Reset(def_i0, ref i0);

            }
            return constraints;
        }
        public static string[] CutConstraints(string formula, List<int> constraints)
        {
            string[] arr = new string[constraints.Count / 2];
            for (int i = 0; i < constraints.Count - 1; i += 2)
            {
                int index0 = constraints[i], index1 = constraints[i + 1];
                string value = formula.Substring(index0, index1 - index0 + 1);
                arr[i / 2] = value;
            }
            return arr;
        }
        static bool IsNotScobeContext<T>(T obj) => obj is not ScobeConstrContext;
        public static Dictionary<string,List<int>> SelectConstants(string[] named_values,List<int> constrs)
        {
        
            Dictionary<string, List<int>> consts = new();
            int i = 0;
            foreach (string val in named_values)
            {
                bool contains = consts.ContainsKey(val);
                if (MathSpace.ExistConstant(val) && !contains)
                    consts.Add(val, new List<int>() { constrs[2 * i], constrs[2 * i+1]  });
                else if (contains)
                    consts[val].AddConstr(constrs[2 * i], constrs[2 * i + 1]);
                i++;
            }
            return consts;
        }
        
        public static Varible[] SelectVaribles(string[] values,List<string> varibles)
        {
            List<Varible> vars = new List<Varible>();
            for (int i = 0; i < values.Length; i++)
            {
                
                values[i] = values[i].Replace('.', ',');
                var value = values[i];
                if (double.TryParse(value,out _)||varibles.Contains(value))
                    vars.Add(Varible.Parse(values[i]));
            }
            return vars.ToArray();
        }
        //Selects all named values of current level
        public static List<int> SelectCurrentLevelConstraints(List<int> scobe_constraints,List<int> vals_constraints)
        {
            List<int> vals_constrs = new();
            const int def_sc_i = -1;//this constants might be negative
            int scobe_i = scobe_constraints.Count>0?0:def_sc_i, scobe_ctrs_len=scobe_constraints.Count;
            int scobe_i0 = scobe_i>def_sc_i? scobe_constraints[scobe_i]:def_sc_i;
            int scobe_i1 = scobe_i > def_sc_i ?scobe_constraints[scobe_i+1] :def_sc_i;
                        
            for (int var_i=0;var_i<vals_constraints.Count;var_i+=2)
            {
                int i0 = vals_constraints[var_i];
                int i1 = vals_constraints[var_i + 1];
                if (i1 > scobe_i1 && scobe_i + 2 < scobe_ctrs_len)
                {
                    scobe_i0 = scobe_constraints[scobe_i += 2];
                    scobe_i1 = scobe_constraints[scobe_i + 1];
                }
                if (!(i0 > scobe_i0 && i1 < scobe_i1))
                    vals_constrs.AddConstr(i0, i1);
                
                
            } 
            return vals_constrs;
        }
        public static List<int> JoinConstraints(List<int> named_values_constrs, List<int> num_constraints)
        {
            List<int> constrs = new();
            constrs.AddRange(named_values_constrs);
            constrs.AddRange(num_constraints);
            constrs.Sort();
            return constrs;
        }

    }
    static class Ext
    {
        public static void AddConstr(this List<int> arr,int i0,int i1)
        {
            arr.Add(i0);arr.Add(i1);
        }
    }
}
