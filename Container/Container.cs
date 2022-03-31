using System;
using System.Collections.Generic;
using MathCalc;
namespace MathCalc
{

    /// <summary>
    /// Struct for saving the user math entities (formulas and constants)
    /// </summary>
    [Serializable]
    public struct Container
    {
        internal Dictionary<string, MathFormula> formulas { get; set; }
        internal Dictionary<string, double> constants { get; set; }
    }
}
