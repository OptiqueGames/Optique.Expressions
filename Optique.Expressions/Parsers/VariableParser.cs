using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Optique.Reflection;

namespace Optique.Expressions
{
    internal class VariableParser : IParser<IReadOnlyValueField>
    {
        private readonly VariableParserSettings _settings;


        public VariableParser(VariableParserSettings settings)
        {
            _settings = settings;
        }

        public bool Validate(string unparsedValue)
        {
            if (_settings.IsActive == false)
            {
                return false;
            }
            
            foreach (char c in unparsedValue)
            {
                if (char.IsLetterOrDigit(c) == false && c != '_' && c != '.')
                {
                    return false;
                }
            }

            string rootName = GetRootName(unparsedValue);

            return _settings.ContainsVariable(rootName);
        }

        public IReadOnlyValueField Parse(string unparsedValue)
        {
            if (Validate(unparsedValue) == false)
            {
                throw new Exception($"Invalid variable '{unparsedValue}'");
            }

            string rootName = GetRootName(unparsedValue);

            if (_settings.ContainsVariable(rootName) == false)
            {
                throw new Exception($"Invalid variable '{rootName}'");
            }

            IReadOnlyValueField field = _settings.GetVariable(rootName);


            string[] names = unparsedValue.Split('.');
            if (names.Length > 1)
            {
                List<MemberInfo> membersChain = new List<MemberInfo>(names.Length - 1);
                Type type = field.GetValue().GetType();

                for (int i = 1; i < names.Length; ++i)
                {
                    membersChain.Add(type.GetMember(names[i]).First(m => m.IsField() || m.IsProperty()));
                    if (membersChain[i - 1] == null)
                    {
                        throw new Exception();
                    }

                    type = membersChain[i - 1].GetUnderlyingType();
                }

                dynamic VariableGetter() =>
                        ReflectionUtility.GetAccessor(field.GetValue(), membersChain).Invoke();

                void VariableSetter(dynamic value) =>
                        ReflectionUtility.GetMutator(field.GetValue(), membersChain).Invoke(value);

                string unchangingName = new string(unparsedValue.ToCharArray(rootName.Length,
                        unparsedValue.Length - rootName.Length));
                
                string NameGetter() => $"{field.Name}{unchangingName}";

                return new Variable(VariableGetter, VariableSetter, NameGetter);
            }

            if (field is IValueField valueField)
            {
                return new Variable(() => valueField.GetValue(), value => valueField.SetValue(value),
                        () => valueField.Name);
            }
            else
            {
                return new ReadOnlyVariable(() => field.GetValue(), () => field.Name);
            }
        }

        private string GetRootName(string unparsedValue)
        {
            int dotIndex = unparsedValue.IndexOf('.');

            if (dotIndex > 0)
            {
                return new string(unparsedValue.ToCharArray(0, dotIndex));
            }

            return unparsedValue;
        }
    }
}