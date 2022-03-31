using static MathCalc.Auxiliaries.Checker;
namespace MathCalc.Auxiliaries.Getters
{
    using static ConstraintFuncs;
    using static ConstraintConditions;
    internal static class ConstantModule
    {

      
            
        internal static Dictionary<string, List<int>> GetConstants(string formula, List<int> constraints)
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
                    list = constant_names_constraints[constant_name];
                    list.Add(i0);

                }
                else
                {
                    list = new List<int>() { len, i0 };
                    constant_names_constraints.Add(constant_name, list);
                }
            }
            return constant_names_constraints;
        }
        internal static void ShiftPositions(string started_const, Dictionary<string, List<int>> dict, int old_position, int newShift)
        {
            bool change_pos = false;
            foreach (var kvp in dict)
            {
                change_pos = kvp.Key == started_const || change_pos;
                if (change_pos)
                {
                    var contraints = dict[kvp.Key];
                    for (int i = 0; i < contraints.Count; i++)
                    {
                        int pos = contraints[i];
                        if (pos > old_position)
                            contraints[i] += newShift;
                    }
                }
            }
        }
    }
}
