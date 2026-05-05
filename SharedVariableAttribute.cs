using System;

namespace XrmGhost.Attributes
{
    /// <summary>
    /// Specifies a default shared variable for a plugin's execution context.
    /// This attribute can be applied to a plugin class multiple times for multiple shared variables.
    /// The value is stored as a JSON string to allow for complex objects.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class SharedVariableAttribute : Attribute
    {
        /// <summary>
        /// Gets the name of the shared variable.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the value of the shared variable, represented as a JSON string.
        /// For simple types, this can be the direct string representation (e.g., "true", "123", "\"my string\"").
        /// For complex types, this should be a JSON object string.
        /// </summary>
        public string ValueJson { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SharedVariableAttribute"/> class.
        /// </summary>
        /// <param name="name">The name of the shared variable.</param>
        /// <param name="valueJson">The value of the shared variable, as a JSON string.</param>
        public SharedVariableAttribute(string name, string valueJson)
        {
            Name = name;
            ValueJson = valueJson;
        }
    }
}
