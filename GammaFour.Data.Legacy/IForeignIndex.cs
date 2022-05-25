// <copyright file="IForeignIndex.cs" company="Donald Roy Airey">
//    Copyright © 2022 - Donald Roy Airey.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace GammaFour.Data.Legacy
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    /// <summary>
    /// A foreign index.
    /// </summary>
    public interface IForeignIndex
    {
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
        /// Specifies the key for organizing the collection.
        /// </summary>
        /// <param name="filter">Used to filter items that appear in the index.</param>
        /// <returns>A reference to this object for Fluent construction.</returns>
        IForeignIndex HasFilter(Expression<Func<object, bool>> filter);

        /// <summary>
        /// Finds the value indexed by the given key.
        /// </summary>
        /// <param name="parent">The parent record.</param>
        /// <returns>The record indexed by the given key, or null if it doesn't exist.</returns>
        IEnumerable<object> GetChildren(object parent);

        /// <summary>
        /// Gets the parent recordd of the given child.
        /// </summary>
        /// <param name="child">The child record.</param>
        /// <returns>The parent record of the given child.</returns>
        object GetParent(object child);

        /// <summary>
        /// Specifies the key for organizing the collection.
        /// </summary>
        /// <param name="key">Used to extract the key from the record.</param>
        /// <returns>A reference to this object for Fluent construction.</returns>
        IForeignIndex HasIndex(Expression<Func<object, object>> key);

        /// <summary>
        /// Gets an indication of whether the child record has a parent.
        /// </summary>
        /// <param name="child">The child record.</param>
        /// <returns>The parent record of the given child.</returns>
        bool HasParent(object child);

        /// <summary>
        /// Removes a key from the index.
        /// </summary>
        /// <param name="value">The the value.</param>
        void Remove(object value);

        /// <summary>
        /// Changes a key value.
        /// </summary>
        /// <param name="value">The new record.</param>
        void Update(object value);
    }
}