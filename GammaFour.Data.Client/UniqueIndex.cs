﻿// <copyright file="UniqueIndex.cs" company="Donald Roy Airey">
//    Copyright © 2022 - Donald Roy Airey.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace GammaFour.Data.Client
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
        /// The dictionary mapping the keys to the rows.
        /// </summary>
        private readonly Dictionary<object, IRow> dictionary = new ();

        /// <summary>
        /// Gets or sets a function used to filter items that should not appear in the index.
        /// </summary>
        private Func<IRow, bool> filterFunction = t => true;

        /// <summary>
        /// Gets or sets the function used to get the primary key from the record.
        /// </summary>
        private Func<IRow, object> keyFunction = t => throw new NotImplementedException();

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
        public EventHandler<RecordChangeEventArgs<IRow>>? IndexChangedHandler { get; set; } = null;

        /// <inheritdoc/>
        public string? Name { get; } = null;

        /// <inheritdoc/>
        public ITable? Table { get; set; } = null;

        /// <inheritdoc/>
        public void Add(IRow row)
        {
            // For those rows that qualify as keys, extract the key from the record and add it to the dictionary while making sure we can undo the
            // action.
            if (this.Filter(row))
            {
                object key = this.GetKey(row);
                this.dictionary.Add(key, row);

                // This is used to notify a foreign key that this row has changed.  The parent will have the opportunity to reject the change if it
                // violates referential integrity.
                this.OnIndexChanging(DataAction.Add, null, row);
            }
        }

        /// <inheritdoc/>
        public bool ContainsKey(object key)
        {
            // Determine if the index holds the given key.
            return this.dictionary.ContainsKey(key);
        }

        /// <inheritdoc/>
        public virtual bool Filter(IRow row)
        {
            // This will typically be a test for null.
            return this.filterFunction(row);
        }

        /// <inheritdoc/>
        public IRow? Find(object? key)
        {
            // Validate the arguments.
            ArgumentNullException.ThrowIfNull(key);

            // Return the row from the dictionary, or null if it doesn't exist.
            return this.dictionary.TryGetValue(key, out IRow? row) ? row : default;
        }

        /// <inheritdoc/>
        public virtual object GetKey(IRow row)
        {
            return this.keyFunction(row);
        }

        /// <summary>
        /// Specifies the key for organizing the collection.
        /// </summary>
        /// <param name="filterFunction">Used to filter items that appear in the index.</param>
        /// <returns>A reference to this object for Fluent construction.</returns>
        public IUniqueIndex HasFilter(Func<IRow, bool> filterFunction)
        {
            this.filterFunction = filterFunction;
            return this;
        }

        /// <summary>
        /// Specifies the key for organizing the collection.
        /// </summary>
        /// <param name="keyFunction">Used to extract the key from the record.</param>
        /// <returns>A reference to this object for Fluent construction.</returns>
        public IUniqueIndex HasIndex(Func<IRow, object> keyFunction)
        {
            this.keyFunction = keyFunction;
            return this;
        }

        /// <inheritdoc/>
        public void Remove(IRow row)
        {
            // Make sure the key was properly removed before we push an undo operation on the stack.  Removing an item that isn't part of the index
            // is not considered an exception.
            if (this.Filter(row))
            {
                object key = this.GetKey(row);
                if (this.dictionary.Remove(key))
                {
                    this.OnIndexChanging(DataAction.Delete, row, null);
                }
            }
        }

        /// <inheritdoc/>
        public void Update(IRow row)
        {
            // Get the previous version of this record.
            IRow previousRow = row.GetVersion(RecordVersion.Previous);
            object previousKey = this.GetKey(previousRow);
            object currentKey = this.GetKey(row);

            // Update should only perform work when the rows are different.
            if (!object.Equals(previousKey, currentKey))
            {
                // Make sure the key was properly removed before we push an undo operation on the stack.  Removing an item that isn't part of the
                // index is not considered an exception.
                if (this.Filter(previousRow))
                {
                    this.dictionary.Remove(previousKey);
                }

                // Extract the new key from the row and add it to the dictionary making sure we can undo the action.
                if (this.Filter(row))
                {
                    this.dictionary.Add(currentKey, row);
                }

                // Notify when the index has changed.
                this.OnIndexChanging(DataAction.Update, previousRow, row);
            }
        }

        /// <summary>
        /// Handles the changing of the key values.
        /// </summary>
        /// <param name="dataAction">The action performed (Add, Update, Delete).</param>
        /// <param name="previousRow">The previous version of the row.</param>
        /// <param name="currentRow">The current version of the row.</param>
        private void OnIndexChanging(DataAction dataAction, IRow? previousRow, IRow? currentRow)
        {
            this.IndexChangedHandler?.Invoke(this, new RecordChangeEventArgs<IRow>(dataAction, previousRow, currentRow));
        }
    }
}