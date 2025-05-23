using System;

namespace GhostPlugin.Attributes
{
    /// <summary>
    /// Specifies the default unsecure configuration for a plugin.
    /// This attribute can be applied to a plugin class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class UnsecureConfigurationAttribute : Attribute
    {
        /// <summary>
        /// Gets the unsecure configuration string.
        /// </summary>
        public string Configuration { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsecureConfigurationAttribute"/> class.
        /// </summary>
        /// <param name="configuration">The unsecure configuration string.</param>
        public UnsecureConfigurationAttribute(string configuration)
        {
            Configuration = configuration;
        }
    }
}
