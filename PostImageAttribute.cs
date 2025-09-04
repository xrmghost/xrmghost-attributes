using System;

namespace XrmGhost.Attributes
{
    /// <summary>
    /// Specifies a default Post-Entity Image for a plugin's execution context.
    /// This attribute can be applied to a plugin class multiple times for multiple images.
    /// The entity image is stored as a JSON string.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class PostImageAttribute : Attribute
    {
        /// <summary>
        /// Gets the name of the post-entity image (alias).
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the post-entity image, represented as a JSON string.
        /// This should be a JSON object string representing an Entity.
        /// Example: "{ \"__entityName\": \"account\", \"accountid\": \"...guid...\", \"name\": \"New Account Name\" }"
        /// </summary>
        public string EntityJson { get; }

        /// <summary>
        /// Gets or sets the attributes (columns) to include in the post-entity image.
        /// If null or not specified, all attributes will be included in the image registration.
        /// This property is used by automated registration tools to configure column filtering.
        /// </summary>
        /// <example>
        /// <code>
        /// [PostImage("primary", entityJson, Attributes = new[] { "name", "accountid", "modifiedon" })]
        /// </code>
        /// </example>
        public string[]? Attributes { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PostImageAttribute"/> class.
        /// </summary>
        /// <param name="name">The name (alias) of the post-entity image.</param>
        /// <param name="entityJson">The post-entity image, as a JSON string.</param>
        public PostImageAttribute(string name, string entityJson)
        {
            Name = name;
            EntityJson = entityJson;
        }
    }
}
