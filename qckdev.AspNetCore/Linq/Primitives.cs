using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace qckdev.AspNetCore.Linq
{
    /// <summary>
    /// LINQ extension methods for primitive types.
    /// </summary>
    public static class Primitives
    {

        /// <summary>
        /// Creates a string array from the current <see cref="StringValues"/> object. Includes values split by commas.
        /// </summary>
        /// <returns>A string array represented by this instance.</returns>
        /// <remarks>
        /// If the <see cref="StringValues"/> contains a single string internally, it is copied to a new array.
        /// If the <see cref="StringValues"/> contains an array internally it returns that array instance.
        /// If the <see cref="StringValues"/> contains a single string internally with several values split by comma, it returns a new array with all values split.
        /// </remarks>
        public static string[]? ToArraySplit([AllowNull] this StringValues values)
            => ((string[]?)values)?
                .SelectMany(x => x.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                .Select(x => x.Trim())
                .ToArray();

    }
}
