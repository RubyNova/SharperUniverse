using System;

namespace SharperUniverse.Core
{
    /// <summary>
    /// Marks the method for injecting the <see cref="IIOHandler"/> and related <see cref="ISharperSystem{T}"/> dependencies into either an <see cref="IUniverseCommandBinding"/> or an <see cref="ISharperSystem{T}"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class SharperInjectAttribute : Attribute
    {
    }
}
