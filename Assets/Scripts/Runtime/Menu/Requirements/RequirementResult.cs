namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// Holds the result of evaluating a single scene requirement, including
    /// whether it is satisfied and, if not, remediation text explaining why.
    /// </summary>
    public readonly struct RequirementResult
    {
        /// <summary>
        /// Whether this requirement is satisfied.
        /// </summary>
        public bool isSupported { get; }

        /// <summary>
        /// Human-readable name of the requirement.
        /// </summary>
        public string requirementName { get; }

        /// <summary>
        /// Optional text describing how to satisfy the requirement.
        /// Empty when the requirement cannot be remediated.
        /// </summary>
        public string remediationText { get; }

        /// <summary>
        /// Whether remediation text is available.
        /// </summary>
        public bool hasRemedy => !string.IsNullOrEmpty(remediationText);

        /// <summary>
        /// Constructs a <see cref="RequirementResult"/>.
        /// </summary>
        /// <param name="isSupported">Whether the requirement is satisfied.</param>
        /// <param name="requirementName">Human-readable name of the requirement.</param>
        /// <param name="remediationText">Optional text describing how to satisfy the requirement.</param>
        public RequirementResult(bool isSupported, string requirementName, string remediationText = "")
        {
            this.isSupported = isSupported;
            this.requirementName = requirementName;
            this.remediationText = remediationText ?? string.Empty;
        }
    }
}
