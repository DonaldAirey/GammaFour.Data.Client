// <copyright file="ITable.cs" company="Donald Roy Airey">
//    Copyright © 2022 - Donald Roy Airey.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace GammaFour.Data.Client
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Used by a template selector to connect a view model to the template used to display it.
    /// </summary>
    public interface ITable : IEnumerable
    {
        /// <summary>
        /// Gets the name of the index.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the foreign indices.
        /// </summary>
        Dictionary<string, IForeignIndex> ForeignIndex { get; }

        /// <summary>
        /// Gets the unique indices.
        /// </summary>
        Dictionary<string, IUniqueIndex> UniqueIndex { get; }

        /// <summary>
        /// Deserializes JSON data.
        /// </summary>
        /// <param name="source">The source text.</param>
        /// <returns>A set of rows from deserialized from the stream.</returns>
        IEnumerable<IRow> Deserialize(string source);

        /// <summary>
        /// Gets the set of rows from the shared data model.
        /// </summary>
        /// <returns>The set of rows from the source.</returns>
        Task<IEnumerable<IRow>> GetAsync();

        /// <summary>
        /// A method to merge a record into a set.
        /// </summary>
        /// <param name="source">A set of rows.</param>
        /// <returns>The rows that couldn't be merged.</returns>
        IEnumerable<IRow> Merge(IEnumerable<IRow> source);

        /// <summary>
        /// Patches a set of rows in the shared data model.
        /// </summary>
        /// <param name="rows">A set of rows.</param>
        /// <returns>The set of accounts.</returns>
        Task<IEnumerable<IRow>> PatchAsync(IEnumerable<IRow> rows);

        /// <summary>
        /// Posts a set of rows into the shared data model.
        /// </summary>
        /// <param name="rows">A set of rows.</param>
        /// <returns>The set of accounts.</returns>
        Task<IEnumerable<IRow>> PostAsync(IEnumerable<IRow> rows);

        /// <summary>
        /// A method to purge a record from a set.
        /// </summary>
        /// <param name="source">A set of rows.</param>
        /// <returns>The rows that couldn't be purged.</returns>
        IEnumerable<IRow> Purge(IEnumerable<IRow> source);

        /// <summary>
        /// Puts a set of rows into the shared data model.
        /// </summary>
        /// <param name="rows">A set of rows.</param>
        /// <returns>The set of accounts.</returns>
        Task<IEnumerable<IRow>> PutAsync(IEnumerable<IRow> rows);
    }
}