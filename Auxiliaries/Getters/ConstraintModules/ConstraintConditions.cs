//'named value' is generalized concept of undetermined varible and constant
namespace MathCalc.Auxiliaries.Getters
{
    using static Checker;
    using static ScobeModule;
    public static class ConstraintConditions
    {
        public static bool HandleScobeAdding(string formula,int index,ScobeConstrContext context)
        {
            bool start_read,stop_read=false,read_constr=context.reading_constraint;
            char c = formula[index];
            bool is_letter = IsLetter(formula,index);
            ref int osc=ref context.opened_scobe_count,csc=ref context.closed_scobe_count;
            int formula_len = formula.Length;
            bool del_constr = index - 1 >= 0 && ShouldDeleteConstraint(formula, index, osc, context);
            start_read = !read_constr&&index<formula.Length-3&&(c=='('|| is_letter);
            if (start_read) {
                if (!is_letter)//because it can go here only if 'c' equals '('
                    osc++;
            }
            else 
            {
                osc += c=='('?1:0;
                csc += c==')'?1:0;
                bool default_stop = read_constr && IsOperator(formula, index) && osc == csc;
                stop_read = default_stop||(index==formula_len-1&&read_constr&&!del_constr); 
            }
            return start_read||stop_read;
                
        }
        private static bool ShouldDeleteConstraint(string formula,int index,int osc,ScobeConstrContext context)
        {
            char c = formula[index];
            int formula_len = formula.Length;
            bool is_operator = IsOperator(formula, index);
            bool prev_is_letter = IsLetter(formula, index - 1);//previous symbol is letter
            bool prev_is_num = IsSimpleNumber(formula[index-1]);
            context.delete_constraint = (is_operator || index == formula_len - 1) && (prev_is_letter || prev_is_num) && osc == 0;
            return context.delete_constraint;

        }
        public static bool HandleNamedValuesAdding(string formula,int index,NamedValuesContext context)
        {
            bool start_read, end_read=false;
            ref bool reading = ref context.reading_constraint;
            int formula_len = formula.Length;
            char c = formula[index];
            start_read = !reading&&(IsLetter(formula,index)||c=='@');
            if (!start_read)
            {
                bool is_namedVal_num = reading&&IsSimpleNumber(c);
                bool is_letter = IsLetter(formula,index)||c=='@';
                end_read = reading && (!is_namedVal_num && !is_letter || index == formula_len - 1);
            }
            
            context.delete_constraint =reading&&c=='(';
            return start_read || end_read;
        }
        public static bool HandleNumberAdding(string formula,int index,NumberConstrContext context)
        {
            
            char c = formula[index];
            bool reading = context.reading_constraint;
            bool start_read, stop_read;
            ref bool is_namedVal_part = ref context.isPartOfNamedValue;
            if (!is_namedVal_part)
                is_namedVal_part = IsLetter(formula, index) && c != MathSpace.expression_name;
            else
                is_namedVal_part = c!='(';
            start_read = !reading&&!is_namedVal_part && IsSimpleNumber(c);
            int formula_len = formula.Length;
            
            bool is_border = index == formula_len - 1;
            bool is_num = IsNum(formula,index);
            bool is_operator = IsOperator(formula,index);
            stop_read = (!is_num || is_border)&&reading;
            if (is_operator||stop_read)
                context.isPartOfNamedValue = false;
            return start_read || stop_read;
        }
    }
}
