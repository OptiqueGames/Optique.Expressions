using System;
using System.Collections.Generic;
using System.Reflection;

namespace Optique.Expressions
{
    public class FunctionParserSettings : ParserSettings
    {
        public bool IgnoreCase = false;

        private readonly Dictionary<string, List<MethodInfo>> _functionsMap;


        internal FunctionParserSettings()
        {
            _functionsMap = new Dictionary<string, List<MethodInfo>>();
        }

        internal MethodInfo[] GetFunctions(string functionName)
        {
            if (_functionsMap.ContainsKey(functionName))
            {
                return _functionsMap[functionName].ToArray();
            }
            else if(IgnoreCase)
            {
                string name = functionName.ToLower();
                foreach (string key in _functionsMap.Keys)
                {
                    if (name.Equals(key.ToLower()))
                    {
                        return _functionsMap[key].ToArray();
                    }
                }
            }
            
            return Array.Empty<MethodInfo>();
        }

        internal void AddFunction(MethodInfo function)
        {
            string name = function.Name;

            if (_functionsMap.ContainsKey(name))
            {
                if (_functionsMap[name].Contains(function))
                {
                    throw new Exception($"Trying to add a duplicate of {name}()");
                }
            }
            else
            {
                _functionsMap[name] = new List<MethodInfo>();
            }

            _functionsMap[name].Add(function);
        }

        internal void RemoveFunction(MethodInfo function)
        {
            if (ContainsFunction(function))
            {
                _functionsMap[function.Name].Remove(function);
            }
        }

        internal void RemoveFunctions(string functionName)
        {
            if (_functionsMap.ContainsKey(functionName))
            {
                _functionsMap[functionName].Clear();
                _functionsMap.Remove(functionName);
            }
        }

        internal bool ContainsFunction(MethodInfo function)
        {
            string name = function.Name;
            return ContainsFunctions(name) && _functionsMap[name].Contains(function);
        }

        internal bool ContainsFunctions(string functionName)
        {
            if (IgnoreCase)
            {
                functionName = functionName.ToLower();
                
                foreach (string key in _functionsMap.Keys)
                {
                    if (key.ToLower() == functionName)
                    {
                        return true;
                    }
                }
            }
            else
            {
                return _functionsMap.ContainsKey(functionName);
            }

            return false;
        }

        internal void ClearFunctionsList()
        {
            _functionsMap.Clear();
        }
    }
}