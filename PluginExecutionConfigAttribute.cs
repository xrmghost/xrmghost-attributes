using System;

namespace GhostPlugin.Attributes
{
    /// <summary>
    /// Specifies default execution context parameters for a plugin.
    /// This attribute can be applied to a plugin class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class PluginExecutionConfigAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the default Message Name (e.g., "Create", "Update").
        /// </summary>
        public string? MessageName { get; set; }

        /// <summary>
        /// Gets or sets the default Primary Entity Logical Name.
        /// </summary>
        public string? PrimaryEntityName { get; set; }

        /// <summary>
        /// Gets or sets the default Plugin Stage.
        /// 10: PreValidation
        /// 20: PreOperation
        /// 40: PostOperation
        /// </summary>
        public int Stage { get; set; } = -1; // Default to unassigned

        /// <summary>
        /// Gets or sets the default Execution Mode.
        /// 0: Synchronous
        /// 1: Asynchronous
        /// </summary>
        public int Mode { get; set; } = -1; // Default to unassigned

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginExecutionConfigAttribute"/> class.
        /// </summary>
        public PluginExecutionConfigAttribute()
        {
        }
    }
}
