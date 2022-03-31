using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
namespace MathCalc
{

    /// <summary>
    /// Struct for saving the user math entities (formulas and constants)
    /// </summary>
    [Serializable]
    struct Container
    {
        internal Dictionary<string, MathFormula> formulas { get; set; }
        internal Dictionary<string, double> constants { get; set; }
        public static Container Get(string path)
        {
            using (FileStream file = new FileStream(path,FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                Container container=(Container)formatter.Deserialize(file);
                return container;
            }
        }
        public void Set(string path)
        {
            using (FileStream file = new FileStream(path, FileMode.OpenOrCreate))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(file, this);
            }
        }
    }
    
}
