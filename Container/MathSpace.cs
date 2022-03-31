using System;
using System.Collections.Generic;
using MathNet.Numerics;
using static System.Math;

namespace MathCalc
{
    using static SpecialFunctions;
    public static class MathSpace
    {
        static Dictionary<string, MathFormula> user_formulas = new Dictionary<string, MathFormula>();
        static readonly Dictionary<string, Func<double, double>> base_formulas = new Dictionary<string, Func<double, double>>()
        {
            {"sin",Sin},{"cos",Cos},{"tan",Tan},{"cot",(input)=>1/Tan(input)},
            {"arcsin",Asin},{"arccos",Acos},{"arctan",Atan },{"arccot",(input)=>PI/2-Atan(input) },
            {"ln",Log },{"lg",Log10 },{"sqrt",Sqrt },{"cbrt",Cbrt},{"factorial",Gamma }
        };
        static Dictionary<string, double> user_consts = new Dictionary<string, double>();
        static readonly Dictionary<string, double> base_consts = new Dictionary<string, double>() 
        {
            {"pi",PI },{"e",E } 
        };
        internal const char expression_name ='@';
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
            base_formulas.ContainsKey(func_name) || user_formulas.ContainsKey(func_name)||func_name=="log";
        public static double CalculateFormula(string formula_name, params double[] input)
        {
            if (base_formulas.ContainsKey(formula_name))
            {
                var func = base_formulas[formula_name];
                return func.Invoke(input[0]);
            }
            else if (user_formulas.ContainsKey(formula_name))
            {
                var func = user_formulas[formula_name];
                return func.Calculate(input);
            }
            else if (formula_name == "log")
                return Log(input[1], input[0]);
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
                string message = base_consts.ContainsKey(function_name) ? base_message : exeption_message;
                throw new ArgumentException(message);
            }

        }
        public static Container Export() => new Container { constants = user_consts, formulas = user_formulas };
        public static void Import(Container container)
        {
            user_formulas = container.formulas;
            user_consts = container.constants;
        }

        
    }
}
