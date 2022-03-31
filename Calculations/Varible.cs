using System;
using System.Collections.Generic;
using System.Text;

namespace MathCalc
{
#pragma warning disable CS0660
    public struct Varible
    {
        public readonly char name;
        internal double? val { get; set; }
        public MathFormula expression;
        private Varible(char name, double? value)
        {
            this.name = name;
            val = value;
            expression = null;
        }
        public static implicit operator Varible(double val) => new Varible('\0', val);
        public static implicit operator Varible(char var_name) => new Varible(var_name, null);
        public static implicit operator double(Varible varible)
        {
           
            if (varible.val.HasValue)
                return varible.val.Value;
            else
                throw new ArgumentNullException($"Value of varible '{varible.name}' doesn't determine");
        }
        public static bool operator != (Varible a, Varible b) => a.val != b.val;
        public static bool operator == (Varible a,Varible b)=> !(a!=b);
        public bool TryGetValue(out double value)
        {
            value = val.HasValue ? val.Value : 0;
            return val.HasValue;

        }
        public override string ToString()=>
            val==null ? name.ToString():val.Value.ToString();
       
    }
}
