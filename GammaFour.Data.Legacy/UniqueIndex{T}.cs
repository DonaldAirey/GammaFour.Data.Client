// <copyright file="UniqueIndex{T}.cs" company="Donald Roy Airey">
//    Copyright © 2022 - Donald Roy Airey.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace GammaFour.Data.Legacy
{
    using System;
    using System.Linq.Expressions;

    /// <summary>
    /// A unique index.
    /// </summary>
    /// <typeparam name="T">The type of object managed by the index.</typeparam>
    public class UniqueIndex<T> : UniqueIndex
        where T : class
    {
        /// <summary>
        /// Gets or sets a filter for items that appear in the index.
        /// </summary>
        private Func<T, bool> filterFunction = t => true;

        /// <summary>
        /// Gets or sets the function used to get the primary key from the record.
        /// </summary>
        private Func<T, object> keyFunction = t => throw new NotImplementedException();

        /// <summary>
        /// Initializes a new instance of the <see cref="UniqueIndex{T}"/> class.
        /// </summary>
        /// <param name="name">The name of the index.</param>
        public UniqueIndex(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Finds the value indexed by the given key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The record indexed by the given key, or null if it doesn't exist.</returns>
        public new T Find(object key)
        {
            // Return the value from the dictionary, or null if it doesn't exist.
            return base.Find(key) as T;
        }

        /// <inheritdoc/>
        public override object GetKey(object value)
        {
            return this.keyFunction(value as T);
        }

        /// <summary>
        /// Specifies the key for organizing the collection.
        /// </summary>
        /// <param name="filter">Used to filter items that appear in the index.</param>
        /// <returns>A reference to this object for Fluent construction.</returns>
        public UniqueIndex<T> HasFilter(Expression<Func<T, bool>> filter)
        {
            this.filterFunction = filter.Compile();
            return this;
        }

        /// <summary>
        /// Specifies the key for organizing the collection.
        /// </summary>
        /// <param name="key">Used to extract the key from the record.</param>
        /// <returns>A reference to this object for Fluent construction.</returns>
        public UniqueIndex<T> HasIndex(Expression<Func<T, object>> key)
        {
            this.keyFunction = key.Compile();
            return this;
        }

        /// <inheritdoc/>
        protected override bool Filter(object value)
        {
            // This will typically be a test for null.
            return this.filterFunction(value as T);
        }
    }
}