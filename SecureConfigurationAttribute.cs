using System;

namespace XrmGhost.Attributes
{
    /// <summary>
    /// Specifies the default secure configuration for a plugin.
    /// This attribute can be applied to a plugin class.
    /// Note: In a real Dynamics 365 environment, secure configuration is managed by the platform
    /// and is not typically embedded directly in code. This attribute is for simulation purposes
    /// within XrmGhost to help generate scenarios or provide default values during testing.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class SecureConfigurationAttribute : Attribute
    {
        /// <summary>
        /// Gets the secure configuration string.
        /// </summary>
        public string Configuration { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SecureConfigurationAttribute"/> class.
        /// </summary>
        /// <param name="configuration">The secure configuration string.</param>
        public SecureConfigurationAttribute(string configuration)
        {
            Configuration = configuration;
        }
    }
}
