// <copyright file="ITable.cs" company="Donald Roy Airey">
//    Copyright © 2022 - Donald Roy Airey.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace GammaFour.Data.Legacy
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Used by a template selector to connect a view model to the template used to display it.
    /// </summary>
    public interface ITable : IEnumerable<object>
    {
        /// <summary>
        /// Gets the unique indices.
        /// </summary>
        Dictionary<string, IUniqueIndex> UniqueIndex { get; }

        /// <summary>
        /// Gets the set of records from the shared data model.
        /// </summary>
        /// <returns>The set of records from the source.</returns>
        Task<IEnumerable<object>> GetAsync();

        /// <summary>
        /// A method to merge a record into a set.
        /// </summary>
        /// <param name="source">A set of records.</param>
        /// <returns>The records that couldn't be merged.</returns>
        IEnumerable<object> Merge(IEnumerable<object> source);

        /// <summary>
        /// Patches a set of records in the shared data model.
        /// </summary>
        /// <param name="records">A set of records.</param>
        /// <returns>The set of accounts.</returns>
        Task<IEnumerable<object>> PatchAsync(IEnumerable<object> records);

        /// <summary>
        /// Posts a set of records into the shared data model.
        /// </summary>
        /// <param name="records">A set of records.</param>
        /// <returns>The set of accounts.</returns>
        Task<IEnumerable<object>> PostAsync(IEnumerable<object> records);

        /// <summary>
        /// A method to purge a record from a set.
        /// </summary>
        /// <param name="source">A set of records.</param>
        /// <returns>The records that couldn't be purged.</returns>
        IEnumerable<object> Purge(IEnumerable<object> source);

        /// <summary>
        /// Puts a set of records into the shared data model.
        /// </summary>
        /// <param name="records">A set of records.</param>
        /// <returns>The set of accounts.</returns>
        Task<IEnumerable<object>> PutAsync(IEnumerable<object> records);
    }
}