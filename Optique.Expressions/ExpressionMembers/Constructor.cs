using System;
using System.Text;

namespace Optique.Expressions
{
    public class Constructor : IValueGetter
    {
        private readonly Type _constructionType;
        private readonly IValueGetter[] _parameters;

        
        internal Constructor(Type constructionType, params IValueGetter[] parameters)
        {
            _constructionType = constructionType;
            _parameters = parameters;
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder($"new {_constructionType.Name}");
            result.Append('(');

            for (int i = 0; i < _parameters.Length; ++i)
            {
                result.Append(_parameters[i]);

                if (i < _parameters.Length - 1)
                {
                    result.Append(", ");
                }
            }

            result.Append(')');

            return result.ToString();
        }

        public dynamic GetValue()
        {
            object[] parameters = new object[_parameters.Length];

            for (int i = 0; i < _parameters.Length; ++i)
            {
                parameters[i] = _parameters[i].GetValue();
            }
                
            return Activator.CreateInstance(_constructionType, parameters);
        }
    }
}