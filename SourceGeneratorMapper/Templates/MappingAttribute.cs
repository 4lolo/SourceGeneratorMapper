using System;

namespace SourceGeneratorMapper.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class MappingAttribute<TSource, TDestination> : Attribute
{
    public bool TwoWay { get; init; }
}