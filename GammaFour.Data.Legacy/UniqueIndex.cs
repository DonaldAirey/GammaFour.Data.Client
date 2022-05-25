// <copyright file="UniqueIndex.cs" company="Donald Roy Airey">
//    Copyright © 2022 - Donald Roy Airey.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace GammaFour.Data.Legacy
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    /// <summary>
    /// A unique index.
    /// </summary>
    public class UniqueIndex : IUniqueIndex
    {
        /// <summary>
        /// The dictionary mapping the keys to the values.
        /// </summary>
        private readonly Dictionary<object, object> dictionary = new Dictionary<object, object>();

        /// <summary>
        /// Gets or sets a filter for items that appear in the index.
        /// </summary>
        private Func<object, bool> filterFunction = t => true;

        /// <summary>
        /// Gets or sets the function used to get the primary key from the record.
        /// </summary>
        private Func<object, object> keyFunction = t => throw new NotImplementedException();

        /// <summary>
        /// Initializes a new instance of the <see cref="UniqueIndex"/> class.
        /// </summary>
        /// <param name="name">The name of the index.</param>
        public UniqueIndex(string name)
        {
            // Initialize the object.
            this.Name = name;
        }

        /// <summary>
        /// Gets or sets the handler for when the index is changed.
        /// </summary>
        public EventHandler<RecordChangeEventArgs<object>> IndexChangedHandler { get; set; }

        /// <summary>
        /// Gets the name of the index.
        /// </summary>
        public string Name { get; }

        /// <inheritdoc/>
        public void Add(object value)
        {
            // For those values that qualify as keys, extract the key from the record and add it to the dictionary while making sure we can undo the
            // action.
            if (this.Filter(value))
            {
                object key = this.GetKey(value);
                this.dictionary.Add(key, value);

                // This is used to notify a foreign key that this value has changed.  The parent will have the opportunity to reject the change if it
                // violates referential integrity.
                this.OnIndexChanging(DataAction.Add, key, null);
            }
        }

        /// <inheritdoc/>
        public bool ContainsKey(object key)
        {
            // Determine if the index holds the given key.
            return this.dictionary.ContainsKey(key);
        }

        /// <inheritdoc/>
        public object Find(object key)
        {
            // Return the value from the dictionary, or null if it doesn't exist.
            return this.dictionary.TryGetValue(key, out object value) ? value : default;
        }

        /// <inheritdoc/>
        public virtual object GetKey(object value)
        {
            return this.keyFunction(value);
        }

        /// <summary>
        /// Specifies the key for organizing the collection.
        /// </summary>
        /// <param name="filter">Used to filter items that appear in the index.</param>
        /// <returns>A reference to this object for Fluent construction.</returns>
        public IUniqueIndex HasFilter(Expression<Func<object, bool>> filter)
        {
            this.filterFunction = filter.Compile();
            return this;
        }

        /// <summary>
        /// Specifies the key for organizing the collection.
        /// </summary>
        /// <param name="key">Used to extract the key from the record.</param>
        /// <returns>A reference to this object for Fluent construction.</returns>
        public IUniqueIndex HasIndex(Expression<Func<object, object>> key)
        {
            this.keyFunction = key.Compile();
            return this;
        }

        /// <inheritdoc/>
        public void Remove(object value)
        {
            // Make sure the key was properly removed before we push an undo operation on the stack.  Removing an item that isn't part of the index
            // is not considered an exception.
            if (this.Filter(value))
            {
                object key = this.GetKey(value);
                if (this.dictionary.Remove(key))
                {
                    this.OnIndexChanging(DataAction.Delete, null, key);
                }
            }
        }

        /// <inheritdoc/>
        public void Update(object value)
        {
            // Get the previous version of this record.
            IVersionable versionable = value as IVersionable;
            if (versionable != null)
            {
                object previousValue = versionable.GetVersion(RecordVersion.Previous);
                object previousKey = this.GetKey(previousValue);
                object currentKey = this.GetKey(value);

                // Update should only perform work when the values are different.
                if (!object.Equals(previousKey, currentKey))
                {
                    // Make sure the key was properly removed before we push an undo operation on the stack.  Removing an item that isn't part of the
                    // index is not considered an exception.
                    if (this.Filter(previousValue))
                    {
                        this.dictionary.Remove(previousKey);
                    }

                    // Extract the new key from the value and add it to the dictionary making sure we can undo the action.
                    if (this.Filter(value))
                    {
                        this.dictionary.Add(currentKey, value);
                    }

                    // Notify when the index has changed.
                    this.OnIndexChanging(DataAction.Update, previousKey, currentKey);
                }
            }
        }

        /// <summary>
        /// Determines if the row should be filtered from the index.
        /// </summary>
        /// <param name="value">the row.</param>
        /// <returns>true if the row can be indexed, false if not.</returns>
        protected virtual bool Filter(object value)
        {
            // This will typically be a test for null.
            return this.filterFunction(value);
        }

        /// <summary>
        /// Handles the changing of the index.
        /// </summary>
        /// <param name="dataAction">The action performed (Add, Update, Delete).</param>
        /// <param name="previous">The previous value of the key.</param>
        /// <param name="current">The current value of the key.</param>
        private void OnIndexChanging(DataAction dataAction, object previous, object current)
        {
            this.IndexChangedHandler?.Invoke(this, new RecordChangeEventArgs<object>(dataAction, previous, current));
        }
    }
}