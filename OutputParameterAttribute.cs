using System;

namespace XrmGhost.Attributes
{
    /// <summary>
    /// Specifies an expected output parameter for a plugin's execution context.
    /// This attribute can be applied to a plugin class multiple times for multiple parameters.
    /// The value is stored as a JSON string to allow for complex objects and define expectations.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class OutputParameterAttribute : Attribute
    {
        /// <summary>
        /// Gets the name of the output parameter.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the expected value of the output parameter, represented as a JSON string.
        /// This can be used by a scenario generator to pre-fill expected outcomes.
        /// For simple types: "true", "123", "\"expected string\"".
        /// For complex types: "{ \"someProperty\": \"expectedValue\" }".
        /// </summary>
        public string ExpectedValueJson { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OutputParameterAttribute"/> class.
        /// </summary>
        /// <param name="name">The name of the output parameter.</param>
        /// <param name="expectedValueJson">The expected value of the output parameter, as a JSON string.</param>
        public OutputParameterAttribute(string name, string expectedValueJson)
        {
            Name = name;
            ExpectedValueJson = expectedValueJson;
        }
    }
}
