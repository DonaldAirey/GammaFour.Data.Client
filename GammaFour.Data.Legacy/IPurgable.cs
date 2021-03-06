// <copyright file="IPurgable.cs" company="Donald Roy Airey">
//    Copyright ? 2020 - Donald Roy Airey.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace GammaFour.Data.Legacy
{
    using System.Collections.Generic;

    /// <summary>
    /// Used by a template selector to connect a view model to the template used to display it.
    /// </summary>
    public interface IPurgable
    {
        /// <summary>
        /// A method to purge a record from a set.
        /// </summary>
        /// <param name="source">A set of records.</param>
        /// <returns>The records that couldn't be purged.</returns>
        IEnumerable<object> Purge(IEnumerable<object> source);
    }
}