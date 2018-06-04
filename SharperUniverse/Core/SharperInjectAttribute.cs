using System;

namespace SharperUniverse.Core
{
    /// <summary>
    /// Marks the method for injecting the <see cref="IIOHandler"/> and related <see cref="ISharperSystem{T}"/> dependencies into either an <see cref="IUniverseCommandInfo"/> or an <see cref="ISharperSystem{T}"/>
    /// </summary>
    [Obsolete]
    [AttributeUsage(AttributeTargets.Method)]
    public class SharperInjectAttribute : Attribute
    {
    }
}
