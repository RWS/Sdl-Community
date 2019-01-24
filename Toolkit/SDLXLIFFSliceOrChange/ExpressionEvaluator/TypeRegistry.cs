using System.Collections.Generic;

namespace ExpressionEvaluator
{
    public class TypeRegistry : Dictionary<string, object>
    {
        public TypeRegistry()
        {
            //Add default aliases
            this.Add("bool", typeof(System.Boolean));
            this.Add("byte", typeof(System.Byte));
            this.Add("char", typeof(System.Char));
            this.Add("int", typeof(System.Int32));
            this.Add("decimal", typeof(System.Decimal));
            this.Add("double", typeof(System.Double));
            this.Add("float", typeof(System.Single));
            this.Add("object", typeof(System.Object));
            this.Add("string", typeof(System.String));
        }

        public void RegisterDefaultTypes()
        {
            this.Add("DateTime", typeof(System.DateTime));
            this.Add("Convert", typeof(System.Convert));
            this.Add("Math", typeof(System.Math));
        }
    }

}
