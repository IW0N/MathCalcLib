using static System.Math;
using MathCalc.Auxiliaries;
using MathCalc.Auxiliaries.Getters;

namespace MathCalc
{
    using static MainGetter;
    using static Checker;
    using static ConstraintFuncs;
    using VarIds = Dictionary<string, int>; //varible indexes pseudoname
    /// <summary>
    /// Class for calculation user string formulas
    /// </summary>
    [Serializable]
    public class MathFormula:FormulaCore
    {
        //formula name of this object
        public readonly string formula;
        public string[] Varibles { get => varibles.ToArray(); }
        //field that compare varible name and its position in the input array
        VarIds varible_ids;
        private MathFormula(string formula, bool replaced_consts, List<string> varibles,VarIds varible_ids)
        {
            this.varible_ids = varible_ids;
            this.formula = formula;
            Initialize(formula, replaced_consts, varibles,varible_ids);
        }
        /// <summary>
        /// Initialize math formula. Don't use next varible-names:+,-,*,^,@ and numeric characters
        /// </summary>
        /// <param name="formula"></param>
        /// <param name="varibles"></param>
        public MathFormula(string formula, List<string> varibles)
        {
            this.formula = formula;
            var var_ids = GetVaribleIds(varibles);
            Initialize(formula, false, varibles,var_ids);
        }
        /// <summary>
        /// Initialize math formula. Don't use next varible-names:+,-,*,^,@ and numeric names
        /// </summary>
        /// <param name="formula"></param>
        /// <param name="varibles"></param>
        public MathFormula(string formula,params string[] varibles)
        {
            
            var vars = new List<string>(varibles);
            var var_ids = GetVaribleIds(vars);
            this.formula = formula;
            Initialize(formula, false, vars,var_ids);
        }
        void Initialize(string formula, bool replaced_consts, List<string> varibles,VarIds vids)
        {
            string newFormula, formula_without_consts, name;
            List<int> scobe_constrs;
            newFormula =ChangeFormula(formula, replaced_consts,out scobe_constrs, out formula_without_consts, out name); 
            InitFields(name, newFormula, varibles, vids);
            string[] next_scobes = name == null ? GetScobes(formula_without_consts,scobe_constrs) : GetFunctionParams(formula_without_consts);
            for (int i = 0; i < next_scobes.Length; i++) {
                int index = expression_indexes[i];
                values[index].expression = new MathFormula(next_scobes[i], true, varibles, vids); 
            }
        }
        void InitFields(string func_name,string newFormula,List<string> varibles,VarIds varible_ids)
        {
            func = func_name;
            this.varible_ids = varible_ids;
            values = GetNumbersAndVaribles(newFormula, varibles);
            determined_values = new double[values.Length];
            if (func_name == null&&values.Length>1)
                operators = GetOperators(newFormula);
            varible_indexes = GetVaribleIndexes(varibles,this);
            expression_indexes = GetExpressionIndexes(values);
            this.varibles=new string[varible_indexes.Count];
            varible_indexes.Keys.CopyTo(this.varibles, 0);
        }
        string ChangeFormula(string formula,bool replaced_consts,out List<int> scobe_constrs,out string formula_without_consts,out string func_name)
        {
            string newFormula = !replaced_consts ? ReplaceConstantsByNumbers(formula) : formula;
            scobe_constrs = GetScobeConstraints(newFormula);
            formula_without_consts = newFormula;
            string fName;
            IsFunction(newFormula, out fName,scobe_constrs);//get function name. If 'new formula', than 'fName' will be equals null
            func_name = fName;
            return ScobeModule.RemoveScobes(newFormula,scobe_constrs);
        }
        protected static void DetermineVaribles(MathFormula formula, double[] varibles)
        {
            foreach (int expr_index in formula.expression_indexes)
            {
                Varible varible = formula.values[expr_index];
                DetermineVaribles(varible.expression, varibles);
            }
            for (int i = 0; i < formula.varibles?.Length; i++)
            {
                string varible_name = formula.varibles[i];
                int var_id = formula.varible_ids[varible_name];
                
                double value = varibles[var_id];
                var indexes = formula.varible_indexes[varible_name];
                foreach (int index in indexes)
                {
                    ref Varible val = ref formula.values[index];
                    val.val = value;
                    formula.determined_values[index] = value;
                }
            }


        }
        public override double Calculate(params double[] input)
        {
            DetermineVaribles(this, input);
            return Calculate(this,input);
        }
        
        protected static double Calculate(MathFormula formula,params double[] input)
        {
            var values = formula.values;
            var expr_indexes = formula.expression_indexes;
            foreach (int expr_index in expr_indexes)
            {
                var varible = values[expr_index];
                double res=Calculate(varible.expression, input);
                values[expr_index].val = res;
                formula.determined_values[expr_index] = res;
            }
            string func = formula.func;
            return func==null?formula.CalculateSimpleFormula():MathSpace.CalculateFormula(func,formula.determined_values);
        }
        protected static double CalculatePair(double a, char action, double b)
        {
            return action switch
            {
                '+' => a + b,
                '-' => a - b,
                '*'=> a*b,
                '/'=>a/b,
                '^'=>Pow(a,b),
                _ => throw new  ArgumentException($"Unknow operator '{action}'"),
            };
        }
        protected double CalculateSimpleFormula()
        {
            double val = values[0];
            if (operators?.Length > 0)
            {
                char oper = operators[0];
                int start_i = oper == '^' ? values.Length - 1 : 1;
                int delta = oper == '^' ? -1 : 1;
                int fin_i = oper == '^' ? 1 : values.Length - 1;
                for (int i = start_i; Continue(i, delta, fin_i); i += delta)
                {
                    char action = operators[i - 1];
                    val = CalculatePair(val, action, values[i].val.Value);
                }
            }
            return val;
        }
        public override string ToString()=>formula;
        
    }
    
}
