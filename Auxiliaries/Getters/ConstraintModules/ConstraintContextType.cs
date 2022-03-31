namespace MathCalc.Auxiliaries.Getters
{
    public delegate bool ContextCondition<T>(string formula, int index, T context) where T:ConstraintContext;
    public abstract class ConstraintContext 
    {
        public bool reading_constraint=false;
        public bool delete_constraint = false;
        protected virtual void Reset()
        {
            reading_constraint = false;
            delete_constraint = false;
        }
        public void Reset(int def_i0,ref int i0)
        {
            Reset();
            i0 = def_i0;
        }
    }
    public class ScobeConstrContext : ConstraintContext
    {
        public int opened_scobe_count=0;
        public int closed_scobe_count = 0;
        public const int def_smallest_rank = -1;
        public int smallest_operator_rank = def_smallest_rank;
      
        protected override void Reset()
        {
            base.Reset();
            opened_scobe_count = 0;
            closed_scobe_count = 0;
            smallest_operator_rank = def_smallest_rank;
        }
    }
    public class NamedValuesContext:ConstraintContext { }
    public class NumberConstrContext:ConstraintContext
    {
        public bool isPartOfNamedValue=false;
        protected override void Reset()
        {
            base.Reset();
            isPartOfNamedValue = false;
        }
    }
}
