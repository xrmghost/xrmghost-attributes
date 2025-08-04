using System;

namespace XrmGhost.Attributes
{
    /// <summary>
    /// Specifies execution context parameters for a plugin.
    /// This attribute can be applied multiple times to a plugin class
    /// to support different entity and message combinations.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class PluginExecutionConfigAttribute : Attribute
    {
        /// <summary>
        /// Gets the Primary Entity Logical Name.
        /// </summary>
        public string PrimaryEntityName { get; }

        /// <summary>
        /// Gets the array of Message Names that this configuration handles (e.g., "Create", "Update").
        /// If null or empty, the configuration applies to all messages for the entity.
        /// </summary>
        public string[]? Messages { get; }

        /// <summary>
        /// Gets or sets the Plugin Stage.
        /// 10: PreValidation
        /// 20: PreOperation
        /// 40: PostOperation
        /// </summary>
        public int Stage { get; set; } = -1; // Default to unassigned

        /// <summary>
        /// Gets or sets the Execution Mode.
        /// 0: Synchronous
        /// 1: Asynchronous
        /// </summary>
        public int Mode { get; set; } = -1; // Default to unassigned

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginExecutionConfigAttribute"/> class.
        /// </summary>
        /// <param name="primaryEntityName">The logical name of the primary entity.</param>
        /// <param name="messages">Optional message names that this configuration handles. If not specified, applies to all messages for the entity.</param>
        public PluginExecutionConfigAttribute(string primaryEntityName, params string[]? messages)
        {
            if (string.IsNullOrWhiteSpace(primaryEntityName))
            {
                throw new ArgumentException("Primary entity name cannot be null or whitespace.", nameof(primaryEntityName));
            }

            // Validate messages if provided
            if (messages != null && messages.Length > 0)
            {
                for (int i = 0; i < messages.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(messages[i]))
                    {
                        throw new ArgumentException($"Message at index {i} cannot be null or whitespace.", nameof(messages));
                    }
                }
            }

            PrimaryEntityName = primaryEntityName;
            Messages = messages;
        }
    }
}
