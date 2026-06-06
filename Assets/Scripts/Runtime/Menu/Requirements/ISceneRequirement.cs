using System.Collections.Generic;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// Defines a requirement that must be met for a sample scene to be supported on the current device.
    /// </summary>
    public interface ISceneRequirement
    {
        /// <summary>
        /// Evaluates whether this requirement is met and appends results to <paramref name="results"/>.
        /// </summary>
        /// <param name="results">
        /// Caller-owned list to which this requirement appends one or more <see cref="RequirementResult"/>
        /// entries describing each sub-requirement and whether it is satisfied.
        /// </param>
        /// <remarks>
        /// Implementors must only call <c>results.Add(...)</c> — never <c>results.Clear()</c>.
        /// Classes that inherit from a concrete <see cref="ISceneRequirement"/> implementation
        /// (rather than implementing the interface directly) should call <c>base.Evaluate(results)</c>
        /// before appending their own entries.
        /// </remarks>
        void Evaluate(List<RequirementResult> results);
    }
}
