namespace MathCalc.Auxiliaries.Getters
{
    using static ConstraintConditions;
    using static Checker;
    public static partial class ConstraintFuncs
    {
        static Dictionary<char, int> ranks = new()
        {
            { '+', 0 },
            { '-', 0 },
            { '*', 1 },
            { '/',1},
            { '^',2}
        };
        public static List<int> GetVaribleConstraints(string formula)
        {
            var nv_constrs = GetConstraints<NamedValuesContext>(formula, HandleNamedValuesAdding);
            var num_constrs = GetConstraints<NumberConstrContext>(formula, HandleNumberAdding);
            return JoinConstraints(nv_constrs, num_constrs);
        }
        private static int GetNextScobe_i(int next_i,List<int> unproccesed_scobe_constraints,int formula_len) =>
            next_i < unproccesed_scobe_constraints.Count ? unproccesed_scobe_constraints[next_i] : formula_len;
        private static int GetSmallestOperatorRank(string formula,List<int> unproccesed_scobe_constraints)
        {
#if DEBUG
            int a = 0;
#endif
            int len=formula.Length;
            
            int scobe_i0 = 0, next_scobe_i = GetNextScobe_i(0,unproccesed_scobe_constraints,len);
            int smallest_rank=3;//set initialize number, that bigger, than maximum operator rank,2 
            for(int i=0;i<len;i++)
            {
                if (i < next_scobe_i) {
                    char c = formula[i];
                    bool is_operator= IsOperator(formula, i);
                    smallest_rank =is_operator&&smallest_rank>ranks[c]?ranks[c]:smallest_rank;
                }
                else { 
                    scobe_i0++;
                    i = GetNextScobe_i(scobe_i0,unproccesed_scobe_constraints,len);
                    next_scobe_i = GetNextScobe_i(++scobe_i0,unproccesed_scobe_constraints,len);
                }
            }
            return smallest_rank;

        }
        private static List<int> GetAddedScobeConstraints(string formula, List<int> scobe_func_constraints, int smallest_rank)
        {
            const int def_i0 = 0,def_oper_range=3;
            List<int> new_constraints = new List<int>();
            int reading_operator_range = def_oper_range, len = formula.Length,i0 = 0,scobe_iter=0,rank;
            int next_scobe_index = GetNextScobe_i(0,scobe_func_constraints,len);
            int imaginary_i0 = 0, operator_quanity = 0 ;
            bool stop_reading_constraint = false,no_state_stop=false,reading=false;
            for (int i=0;i<len;i++)
            {
                if (i == next_scobe_index) {
                    i = GetNextScobe_i(++scobe_iter,scobe_func_constraints,len)-1;
                    next_scobe_index = GetNextScobe_i(++scobe_iter,scobe_func_constraints,len);
                    continue;
                }
                else
                {
                    bool is_operator = IsOperator(formula,i);
                    if (is_operator)
                    {
                        rank = ranks[formula[i]];
                        if (rank > smallest_rank && !reading) {
                            i0 = imaginary_i0; reading = true;
                        }
                        imaginary_i0 = i+1;
                        stop_reading_constraint = reading&&(rank==smallest_rank||i==len-1);
                    }
                }
                no_state_stop = reading&&i==len-1;
                if (stop_reading_constraint||no_state_stop)
                {
                    new_constraints.AddConstr(i0, !no_state_stop?i-1:i);
                    i0 = def_i0;
                    reading = false;
                    stop_reading_constraint = false;
                }
                
            }
            return new_constraints;
            
        }
        
        private static int FindElementIndex(int element,List<int> list)
        {
            int count = list.Count;
            for (int i=1;i<count;i++)
            {
                if (element < list[i])
                    return i - 1;
                else if (i == count - 1 && element >= list[i])
                    return i;
            }
            return -1;
        }
        private static List<int> JoinResidualConstrs(List<int> main,List<int> residual) 
        {
            for (int i=0;i<residual.Count;i++)
            {
                if (!main.Contains(residual[i]))
                    main.Add(residual[i]);
            }
            main.Sort();
            return main;
        }
        private static List<int> JoinScobeConstraints(List<int> base_scobes,List<int> other_scobe_constrs)
        {
            List<int> full_constraints = base_scobes;
            
            for (int i=0;i<other_scobe_constrs.Count;i+=2)
            {
                int a = other_scobe_constrs[i];
                int b = other_scobe_constrs[i + 1];
                int border_a = FindElementIndex(a, base_scobes);
                int border_b = FindElementIndex(b,base_scobes);
                if (border_a!=border_b&&border_a>=0&&border_b>0)
                {
                    full_constraints.RemoveRange(border_a, border_b - border_a + 1);
                    full_constraints.InsertRange(border_a, new int[2] {a,b});
                }

            }
            full_constraints = JoinResidualConstrs(full_constraints,other_scobe_constrs);
            return full_constraints;
        }
        public static List<int> GetScobeConstraints(string formula)
        {
            var scobes = GetConstraints<ScobeConstrContext>(formula,HandleScobeAdding);
            int smallest_rank = GetSmallestOperatorRank(formula,scobes);
            var other_scobes = GetAddedScobeConstraints(formula,scobes,smallest_rank);
            return JoinScobeConstraints(scobes,other_scobes);
        }
    }
}
