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
        /// Gets or sets the User ID under which the plugin should be registered.
        /// If null or not specified, the plugin will run under the user who triggers the operation.
        /// This property is used by automated registration tools to configure the impersonating user context.
        /// </summary>
        /// <example>
        /// <code>
        /// [PluginExecutionConfig("account", "Create", ImpersonatingUserId = "12345678-1234-1234-1234-123456789012")]
        /// </code>
        /// </example>
        public string? ImpersonatingUserId { get; set; }

        /// <summary>
        /// Gets or sets the filtering attributes (columns) that restrict when the registered step fires.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>Null and empty are equivalent.</b> Both <see langword="null"/> and an empty array mean
        /// "no filtering attributes": the step fires for any changed column. Downstream registrars must
        /// map both cases to a Dataverse <c>filteringattributes</c> value of <see langword="null"/>
        /// (not an empty string), so that upgrading from an earlier package version is a no-op.
        /// </para>
        /// <para>
        /// <b>Update-only applicability.</b> Filtering attributes are meaningful only for the
        /// <c>Update</c> message. If this configuration declares other messages (for example
        /// <c>Create</c> or <c>Delete</c>), downstream registrars must ignore this property for those
        /// messages and register them without filtering attributes. The attribute itself performs no
        /// message-dependent validation: it stores exactly what the author declared.
        /// </para>
        /// <para>
        /// <b>Canonical downstream serialization.</b> This attribute preserves the source order and the
        /// source casing of the declared values verbatim. Consumers that read this metadata (via
        /// reflection or Mono.Cecil) and compare it against, or write it to, Dataverse must first
        /// canonicalize the array as follows, in this exact order:
        /// </para>
        /// <list type="number">
        /// <item><description>trim each entry and drop entries that are null, empty, or whitespace;</description></item>
        /// <item><description>lowercase each entry using the invariant culture;</description></item>
        /// <item><description>remove duplicates;</description></item>
        /// <item><description>sort ordinally (<see cref="System.StringComparer.Ordinal"/>);</description></item>
        /// <item><description>join with a single comma and no spaces (<c>"name,revenue"</c>);</description></item>
        /// <item><description>if the resulting sequence is empty, emit <see langword="null"/> rather than an empty string.</description></item>
        /// </list>
        /// <para>
        /// Applying the same canonical form on both the read and the write side is what makes
        /// "is the registered step already up to date?" a stable, order-insensitive comparison.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code>
        /// [PluginExecutionConfig("account", "Update", FilteringAttributes = new[] { "name", "revenue" })]
        /// </code>
        /// </example>
        public string[]? FilteringAttributes { get; set; }

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
