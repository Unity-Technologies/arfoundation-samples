using System.Collections.Generic;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public interface IBooleanExpression
    {
        bool Evaluate();
    }

    public static class BooleanExpressionListExtensions
    {
        /// <summary>
        /// Returns <see langword="true"/> if all expressions in the list evaluate to true.
        /// Otherwise, returns <see langword="false"/>.
        /// </summary>
        /// <param name="expressions">The expressions to evaluate.</param>
        public static bool EvaluateAll(this IList<IBooleanExpression> expressions)
        {
            foreach (var ex in expressions)
            {
                if (!ex.Evaluate())
                    return false;
            }

            return true;
        }
    }
}
