using System;
using System.Collections.Generic;
using System.Linq;

namespace Optique.Expressions
{
    public class ConstructorParserSettings : ParserSettings
    {
        public readonly IReadOnlyList<Type> Types;

        private readonly List<Type> _list;


        internal ConstructorParserSettings()
        {
            Types = _list = new List<Type>();
        }

        internal void AddType(Type type)
        {
            if (_list.Contains(type))
            {
                throw new Exception($"Trying to add a duplicate of '{type.Name}' variable");
            }
            else if (_list.Any(valueField => valueField.Name.Equals(type.Name)))
            {
                throw new Exception(
                        $"Trying to add a variable with the same name of already contained '{type.Name}'");
            }

            _list.Add(type);
        }

        internal Type GetType(string name)
        {
            return _list.FirstOrDefault(type => type.Name.Equals(name));
        }

        internal bool ContainsType(Type type)
        {
            return _list.Contains(type) || ContainsType(type.Name);

        }

        internal bool ContainsType(string name)
        {
            return _list.Any(valueField => valueField.Name.Equals(name));
        }

        internal void RemoveType(Type type)
        {
            if (_list.Contains(type))
            {
                _list.Remove(type);
            }
        }

        internal void RemoveType(string name)
        {
            if (ContainsType(name))
            {
                _list.Remove(_list.First(valueField => valueField.Name.Equals(name)));
            }
        }

        internal void ClearTypesList()
        {
            _list.Clear();
        }
    }
}