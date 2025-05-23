using System;

namespace GhostPlugin.Attributes
{
    /// <summary>
    /// Specifies a default Pre-Entity Image for a plugin's execution context.
    /// This attribute can be applied to a plugin class multiple times for multiple images.
    /// The entity image is stored as a JSON string.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class PreImageAttribute : Attribute
    {
        /// <summary>
        /// Gets the name of the pre-entity image (alias).
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the pre-entity image, represented as a JSON string.
        /// This should be a JSON object string representing an Entity.
        /// Example: "{ \"__entityName\": \"account\", \"accountid\": \"...guid...\", \"name\": \"Old Account Name\" }"
        /// </summary>
        public string EntityJson { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PreImageAttribute"/> class.
        /// </summary>
        /// <param name="name">The name (alias) of the pre-entity image.</param>
        /// <param name="entityJson">The pre-entity image, as a JSON string.</param>
        public PreImageAttribute(string name, string entityJson)
        {
            Name = name;
            EntityJson = entityJson;
        }
    }
}
