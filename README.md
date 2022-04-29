# Optique.Expressions

Optique.Reflection is an expressions evaluating engine

## Features

- Support most of operators: 
  - Unary numeric and logical negation (`-`, `!`)
  - Binary arithmetic (`+`, `-`, `*`, `/`, `%`)
  - Comparison and equality (`>`, `<`, `>=`, `<=`, `==`, `!=`)
  - Logical (`&`, `|`, `&&`, `||`)
- Support most of literals: `int`, `float`, `bool`, `string`
- Functions registration:
  - Custom static methods by its MethodInfo
  - Extracting all public static methods from a target type to use it in expressions
- Variables registration:
  - By name and its getter and optional setter
  - By IReadOnlyValueField interface
  - By extracting all public constants from a target type
- Access variables via dot (`.`) operator
- Using overloaded operators of user types

## Requirements

.NET 4.0 or higher because of using dynamic types.

## Dependencies

- [Optique.Reflection](https://github.com/OptiqueGames/Optique.Reflection)

## Usage examples

Entry point:
```csharp
ExpressionEvaluator evaluator = new ExpressionEvaluator();

evaluator.RegisterFunctionsFromType(typeof(System.Math));
evaluator.RegisterConstantsFromType(typeof(System.Math));

ExpressionParser parser = evaluator.GetExpressionParser();
```

Calculating of complex expression:
```csharp
IntegerContainer container = new IntegerContainer(2);

evaluator.RegisterVariable("x", () => container);
evaluator.RegisterVariable("factor", () => 10);
Expression expression = parser.Parse("Pow(x.Value, 5) > PI * factor");

Console.WriteLine(result.Calculate()); // true

...

private class IntegerContainer
{
  public int Value { get; }
  public IntegerContainer(int value) => Value = value;
}
```

Working with third party math library and using custom types with its operators overloads:
```csharp
using Unity.Mathematics;

...

evaluator.RegisterFunctionsFromType(typeof(math));
evaluator.FunctionParserSettings.IgnoreCase = true;

float2 a = new float2(1f, 0.1f);
float2 b = new float2(0f, 1f);
float2 c = new float2(-0.4f, 0.1f);

evaluator.RegisterVariable(nameof(a), () => a);
evaluator.RegisterVariable(nameof(b), () => b);
evaluator.RegisterVariable(nameof(c), () => c);

Expression expression = parser.Parse("DOT(a * 2, b + c)");

Debug.Log(expression.Calculate() == math.dot(a * 2, b + c)); // true
```
