using System;
using System.Collections.Generic;
using System.Linq;

namespace Optique.Expressions
{
    public class VariableParserSettings : ParserSettings
    {
        public readonly IReadOnlyList<IReadOnlyValueField> Variables;

        private readonly List<IReadOnlyValueField> _list;


        internal VariableParserSettings()
        {
            Variables = _list = new List<IReadOnlyValueField>();
        }

        internal void AddVariable(IReadOnlyValueField variable)
        {
            if (_list.Contains(variable))
            {
                throw new Exception($"Trying to add a duplicate of '{variable.Name}' variable");
            }
            else if (_list.Any(valueField => valueField.Name.Equals(variable.Name)))
            {
                throw new Exception(
                    $"Trying to add a variable with the same name of already contained '{variable.Name}'");
            }

            _list.Add(variable);
        }

        internal void RemoveVariable(IReadOnlyValueField variable)
        {
            if (_list.Contains(variable))
            {
                _list.Remove(variable);
            }
        }

        internal void RemoveVariable(string variableName)
        {
            if (ContainsVariable(variableName))
            {
                _list.Remove(_list.First(valueField => valueField.Name.Equals(variableName)));
            }
        }

        internal bool ContainsVariable(IReadOnlyValueField variable)
        {
            return _list.Contains(variable) || ContainsVariable(variable.Name);
        }

        internal bool ContainsVariable(string variableName)
        {
            return _list.Any(valueField => valueField.Name.Equals(variableName));
        }

        internal void ClearVariablesList()
        {
            _list.Clear();
        }
    }
}