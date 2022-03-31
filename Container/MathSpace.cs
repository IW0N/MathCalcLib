using System;
using System.Collections.Generic;
using MathNet.Numerics;
using static System.Math;

namespace MathCalc
{
    using static SpecialFunctions;
    /// <summary>
    /// It contains the all functions and constants. 
    /// Base functions: sin,cos,tan,cot,arcsin,arccos,arctan,arccot,ln,lg,sqrt,cbrt,factorial,log,abs,mod
    /// Base constants: pi,e
    /// </summary>
    public static class MathSpace
    {
        static Dictionary<string, MathFormula> user_formulas = new Dictionary<string, MathFormula>();
        static readonly Dictionary<string, Func<double, double>> base_formulas = new Dictionary<string, Func<double, double>>()
        {
            {"sin",Sin},{"cos",Cos},{"tan",Tan},{"cot",(input)=>1/Tan(input)},
            {"arcsin",Asin},{"arccos",Acos},{"arctan",Atan },{"arccot",(input)=>PI/2-Atan(input) },
            {"sinh",Sinh},{"cosh",Cosh},{"tanh",Tanh},{"coth",(a)=>1/Tanh(a)},
            {"arcsinh",Asinh},{"arccosh",Acosh},{"arctanh",Atanh},{"arccoth",(a)=>Atanh(1/a)},
            {"ln",Log },{"lg",Log10 },{"sqrt",Sqrt },{"cbrt",Cbrt},{"factorial",Gamma },{"abs",Abs},
            {"log2",Log2}
        };
        static readonly Dictionary<string, Func<double, double, double>> base_formulas_2args = new()
        {
            { "log",(a,b)=> Log(b,a) },{"mod",(a,b)=>a%b }
        };
        static Dictionary<string, double> user_consts = new Dictionary<string, double>();
        static readonly Dictionary<string, double> base_consts = new Dictionary<string, double>() 
        {
            {"pi",PI },{"e",E } 
        };
        internal const char expression_name ='@';
        internal const char varible_name = '#';
        public static double GetConstant(string const_name)
        {
            
            if (base_consts.ContainsKey(const_name))
                return base_consts[const_name];
            else if (user_consts.ContainsKey(const_name))
                return user_consts[const_name];
            else
                throw new Exception($"Constant '{const_name}' doesn't exist");
        }
        internal static bool ExitstFunction(string func_name) =>
            base_formulas.ContainsKey(func_name) || user_formulas.ContainsKey(func_name)|| base_formulas_2args.ContainsKey(func_name);
        internal static bool ExistConstant(string const_name) =>
            base_consts.ContainsKey(const_name) || user_consts.ContainsKey(const_name);
        public static double CalculateFormula(string formula_name, params double[] input)
        {
            if (base_formulas.ContainsKey(formula_name))
            {
                var func = base_formulas[formula_name];
                return func.Invoke(input[0]);
            }
            else if(base_formulas_2args.ContainsKey(formula_name))
            {
                var func = base_formulas_2args[formula_name];
                return func.Invoke(input[0],input[1]);
            }
            else if (user_formulas.ContainsKey(formula_name))
            {
                var func = user_formulas[formula_name];
                return func.Calculate(input);
            }
            else
                throw new Exception($"Function '{formula_name}' doesn't exist");
        }
        /// <summary>
        /// Method, that just set constant into the module.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="val"></param>
        /// <remarks>WARNING!Function name can't contains +,-,*,^,@ and numeric characters</remarks>
        public static void SetConstant(string name, double val) => user_consts.Add(name, val);
        /// <summary>
        /// Method, that just set function into the module.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="val"></param>
        /// <remarks>WARNING!Function name can't contains +,-,*,^,@ and numeric characters</remarks>
        public static void SetFunction(string name, MathFormula formula) => user_formulas.Add(name, formula);
        public static void RemoveConstant(string constant)
        {
            if (user_consts.ContainsKey(constant))
                user_consts.Remove(constant);
            else
            {
                string base_message = $"You can't delete the base constant '{constant}' ";
                string exeption_message = $"Constant '{constant}' doesn't exist";
                string message = base_consts.ContainsKey(constant) ? base_message : exeption_message;
                throw new ArgumentException(message);
            }
            
        }
        public static void RemoveFunction(string function_name)
        {
            if (user_formulas.ContainsKey(function_name))
                user_formulas.Remove(function_name);
            else
            {
                string base_message = $"You can't delete the base function '{function_name}'";
                string exeption_message = $"Function '{function_name}' doesn't exist";
                bool base_formulas_has = base_formulas.ContainsKey(function_name);
                bool base_formulas2_has = base_formulas_2args.ContainsKey(function_name);
                string message =  base_formulas_has||base_formulas2_has? base_message : exeption_message;
                throw new ArgumentException(message);
            }

        }
        public static void ExportTo(string filePath) 
        {
            var container=new Container { constants = user_consts, formulas = user_formulas };
            container.Set(filePath);
        }
        public static void ImportFrom(string filePath)
        {
            Container container=Container.Get(filePath);
            user_formulas = container.formulas;
            user_consts = container.constants;
        }
        public static string[] GetAllFunctions()
        {
            var base_keys=base_formulas.Keys;
            var base2_keys = base_formulas_2args.Keys;
            var user_keys = user_formulas.Keys;
            var base_collection=GetCollection(base_keys,base2_keys);
            return GetCollection(base_collection, user_keys);
        }
        public static string[] GetAllConstants()=> 
            GetCollection(base_consts.Keys, user_consts.Keys);   
        private static string[] GetCollection(ICollection<string> base_collection,ICollection<string> user_collection)
        {
            var funcs = new string[base_collection.Count + user_collection.Count];
            base_collection.CopyTo(funcs, 0);
            user_collection.CopyTo(funcs, base_collection.Count);
            return funcs;
        }
        
        
    }
}
