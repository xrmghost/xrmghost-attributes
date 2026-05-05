using System;

namespace XrmGhost.Attributes
{
    /// <summary>
    /// Specifies a message that the plugin handles.
    /// This attribute can be applied multiple times to a plugin class
    /// to indicate support for multiple messages.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class HandlesMessageAttribute : Attribute
    {
        /// <summary>
        /// Gets the name of the message the plugin handles (e.g., "Create", "Update").
        /// </summary>
        public string MessageName { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlesMessageAttribute"/> class.
        /// </summary>
        /// <param name="messageName">The name of the message.</param>
        public HandlesMessageAttribute(string messageName)
        {
            if (string.IsNullOrWhiteSpace(messageName))
            {
                throw new ArgumentException("Message name cannot be null or whitespace.", nameof(messageName));
            }
            MessageName = messageName;
        }
    }
}
