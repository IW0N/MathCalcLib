using System;
using System.Collections.Generic;
using System.Text;

namespace MathCalc
{
#pragma warning disable CS0660
    [Serializable]
    public struct Varible
    {
        public readonly string name;
        internal double? val { get; set; }
        public MathFormula? expression;
        
        private Varible(string name,double? val)
        {
            this.name = name;
            this.val = val;
            expression = null;
        }
        public static implicit operator Varible(double val) => new("", val);
        public static implicit operator Varible(string var_name)=>new(var_name,null);
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
            value = val??0;
            return val.HasValue;
        }
        public override string ToString()=>
            val==null ? name.ToString():val.Value.ToString();
        public static Varible Parse(string value)
        {
            double num;
            Varible vrb=double.TryParse(value.Replace('.',','),out num)?new Varible("",num):new Varible(value,null);
            return vrb;
        }
    }
}
