using System;

namespace Optique.Expressions
{
    internal class Variable : IValueField
    {
        private readonly Func<dynamic> _variableGetter;
        private readonly Action<dynamic> _variableSetter;
        private readonly Func<string> _nameGetter;

        public string Name => _nameGetter();


        internal Variable(Func<dynamic> variableGetter, Action<dynamic> variableSetter, Func<string> nameGetter)
        {
            _variableGetter = variableGetter;
            _variableSetter = variableSetter;
            _nameGetter = nameGetter;
        }

        public dynamic GetValue()
        {
            return _variableGetter.Invoke();
        }

        public void SetValue(dynamic value)
        {
            _variableSetter(value);
        }

        public override string ToString()
        {
            return _nameGetter();
        }
    }
}