using System;

namespace Optique.Expressions
{
    internal class ReadOnlyVariable : IReadOnlyValueField
    {
        private readonly Func<dynamic> _variableGetter;
        private readonly Func<string> _nameGetter;

        public string Name => _nameGetter();


        internal ReadOnlyVariable(Func<dynamic> variableGetter, Func<string> nameGetter)
        {
            _variableGetter = variableGetter;
            _nameGetter = nameGetter;
        }

        public override string ToString()
        {
            return _nameGetter();
        }

        public dynamic GetValue()
        {
            return _variableGetter.Invoke();
        }
    }
}