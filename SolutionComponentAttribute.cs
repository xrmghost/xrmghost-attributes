using System;

namespace XrmGhost.Attributes
{
    /// <summary>
    /// Specifies which solution(s) a plugin component should be added to during automated registration.
    /// This attribute can be applied multiple times to a plugin class to support registration in multiple solutions.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class SolutionComponentAttribute : Attribute
    {
        /// <summary>
        /// Gets the unique name of the solution where the component should be added.
        /// </summary>
        public string SolutionUniqueName { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this is the default (primary) solution for the component.
        /// When multiple solutions are specified, the default solution is used for primary registration
        /// and processed first by automated registration tools.
        /// </summary>
        /// <example>
        /// <code>
        /// [SolutionComponent("CoreBusinessLogic", IsDefault = true)]
        /// [SolutionComponent("ExtendedFeatures")]
        /// </code>
        /// </example>
        public bool IsDefault { get; set; } = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionComponentAttribute"/> class.
        /// </summary>
        /// <param name="solutionUniqueName">The unique name of the solution where the component should be added.</param>
        /// <exception cref="ArgumentException">Thrown when solution unique name is null or whitespace.</exception>
        public SolutionComponentAttribute(string solutionUniqueName)
        {
            if (string.IsNullOrWhiteSpace(solutionUniqueName))
            {
                throw new ArgumentException("Solution unique name cannot be null or whitespace.", nameof(solutionUniqueName));
            }

            SolutionUniqueName = solutionUniqueName;
        }
    }
}
