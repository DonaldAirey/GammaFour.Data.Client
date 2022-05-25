// <copyright file="IUniqueIndex.cs" company="Donald Roy Airey">
//    Copyright © 2022 - Donald Roy Airey.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace GammaFour.Data.Legacy
{
    using System;
    using System.Linq.Expressions;

    /// <summary>
    /// An interface for a unique index.
    /// </summary>
    public interface IUniqueIndex
    {
        /// <summary>
        /// Gets or sets the handler for when the index is changed.
        /// </summary>
        EventHandler<RecordChangeEventArgs<object>> IndexChangedHandler { get; set; }

        /// <summary>
        /// Gets the name of the index.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Adds a key to the index.
        /// </summary>
        /// <param name="value">The referenced record.</param>
        void Add(object value);

        /// <summary>
        /// Gets a value that indicates if the index contains the given key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>True if the index contains the given key, false otherwise.</returns>
        bool ContainsKey(object key);

        /// <summary>
        /// Finds the value indexed by the given key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The record indexed by the given key, or null if it doesn't exist.</returns>
        object Find(object key);

        /// <summary>
        /// Gets the key of the given record.
        /// </summary>
        /// <param name="value">The record.</param>
        /// <returns>The key values.</returns>
        object GetKey(object value);

        /// <summary>
        /// Removes a key from the index.
        /// </summary>
        /// <param name="value">The record to be removed.</param>
        void Remove(object value);

        /// <summary>
        /// Updates the key of a record in the index.
        /// </summary>
        /// <param name="value">The record that has changed.</param>
        void Update(object value);
    }
}