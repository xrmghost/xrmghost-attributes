using System;

namespace GhostPlugin.Attributes
{
    /// <summary>
    /// Specifies a default input parameter for a plugin's execution context.
    /// This attribute can be applied to a plugin class multiple times for multiple parameters.
    /// The value is stored as a JSON string to allow for complex objects.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class InputParameterAttribute : Attribute
    {
        /// <summary>
        /// Gets the name of the input parameter.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the value of the input parameter, represented as a JSON string.
        /// For simple types, this can be the direct string representation (e.g., "true", "123", "\"my string\"").
        /// For complex types (like Entity or EntityReference), this should be a JSON object string.
        /// Example for an Entity: "{ \"__entityName\": \"account\", \"accountid\": \"...guid...\", \"name\": \"Test Account\" }"
        /// </summary>
        public string ValueJson { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InputParameterAttribute"/> class.
        /// </summary>
        /// <param name="name">The name of the input parameter.</param>
        /// <param name="valueJson">The value of the input parameter, as a JSON string.</param>
        public InputParameterAttribute(string name, string valueJson)
        {
            Name = name;
            ValueJson = valueJson;
        }
    }
}
