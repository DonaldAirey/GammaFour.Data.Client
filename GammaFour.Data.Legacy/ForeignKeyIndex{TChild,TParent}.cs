// <copyright file="ForeignKeyIndex{TChild,TParent}.cs" company="Donald Roy Airey">
//    Copyright © 2022 - Donald Roy Airey.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace GammaFour.Data.Legacy
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    /// <summary>
    /// An index.
    /// </summary>
    /// <typeparam name="TChild">The child value.</typeparam>
    /// <typeparam name="TParent">The parent value.</typeparam>
    public class ForeignKeyIndex<TChild, TParent>
        where TParent : class, IVersionable<TParent>
        where TChild : IVersionable<TChild>
    {
        /// <summary>
        /// The dictionary containing the index.
        /// </summary>
        private readonly Dictionary<object, HashSet<TChild>> dictionary = new Dictionary<object, HashSet<TChild>>();

        /// <summary>
        /// The unique parent index.
        /// </summary>
        private readonly UniqueKeyIndex<TParent> parentIndex;

        /// <summary>
        /// Used to filter items that appear in the index.
        /// </summary>
        private Func<TChild, bool> filterFunction = t => true;

        /// <summary>
        /// Used to get the key from the child record.
        /// </summary>
        private Func<TChild, object> keyFunction = t => throw new NotImplementedException();

        /// <summary>
        /// Initializes a new instance of the <see cref="ForeignKeyIndex{TChild, TParent}"/> class.
        /// </summary>
        /// <param name="name">The name of the index.</param>
        /// <param name="parentIndex">The parent index.</param>
        public ForeignKeyIndex(string name, UniqueKeyIndex<TParent> parentIndex)
        {
            // Initialize the object.
            this.Name = name;
            this.parentIndex = parentIndex;

            // This instructs the parent key to inform this object about any changes.
            this.parentIndex.IndexChangedHandler += this.HandleUniqueIndexChange;
        }

        /// <summary>
        /// Gets the name of the index.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Adds a key to the index.
        /// </summary>
        /// <param name="value">The referenced record.</param>
        public void Add(TChild value)
        {
            // For those values that qualify as keys, extract the key from the record and add it to the dictionary making sure we can undo the action.
            if (this.filterFunction(value))
            {
                // Don't attempt to add a record with a null key.
                object key = this.keyFunction(value);

                // Make sure the new key exist in the parent.
                if (!this.parentIndex.ContainsKey(key))
                {
                    throw new KeyNotFoundException($"{this.Name}: {key}");
                }

                // Find or create a bucket of child records for the new key.
                if (!this.dictionary.TryGetValue(key, out HashSet<TChild> hashSet))
                {
                    hashSet = new HashSet<TChild>();
                    this.dictionary.Add(key, hashSet);
                }

                // Add the key to the index and make sure it's unique.
                if (!hashSet.Add(value))
                {
                    throw new DuplicateKeyException($"{this.Name}: {key}");
                }
            }
        }

        /// <summary>
        /// Specifies the key for organizing the collection.
        /// </summary>
        /// <param name="filter">Used to filter items that appear in the index.</param>
        /// <returns>A reference to this object for Fluent construction.</returns>
        public ForeignKeyIndex<TChild, TParent> HasFilter(Expression<Func<TChild, bool>> filter)
        {
            this.filterFunction = filter.Compile();
            return this;
        }

        /// <summary>
        /// Finds the value indexed by the given key.
        /// </summary>
        /// <param name="parent">The parent record.</param>
        /// <returns>The record indexed by the given key, or null if it doesn't exist.</returns>
        public IEnumerable<TChild> GetChildren(TParent parent)
        {
            // Return the list of children for the given parent record, or an empty list if there are no children.
            return this.dictionary.TryGetValue(this.parentIndex.GetKey(parent), out HashSet<TChild> value) ? value : (IEnumerable<TChild>)new List<TChild>();
        }

        /// <summary>
        /// Gets the parent recordd of the given child.
        /// </summary>
        /// <param name="child">The child record.</param>
        /// <returns>The parent record of the given child.</returns>
        public TParent GetParent(TChild child)
        {
            // Find the parent record.
            return this.parentIndex.Find(this.keyFunction(child));
        }

        /// <summary>
        /// Specifies the key for organizing the collection.
        /// </summary>
        /// <param name="key">Used to extract the key from the record.</param>
        /// <returns>A reference to this object for Fluent construction.</returns>
        public ForeignKeyIndex<TChild, TParent> HasIndex(Expression<Func<TChild, object>> key)
        {
            this.keyFunction = key.Compile();
            return this;
        }

        /// <summary>
        /// Gets an indication of whether the child record has a parent.
        /// </summary>
        /// <param name="child">The child record.</param>
        /// <returns>The parent record of the given child.</returns>
        public bool HasParent(TChild child)
        {
            // Return the parent record.
            return this.parentIndex.Find(this.keyFunction(child)) != null;
        }

        /// <summary>
        /// Removes a key from the index.
        /// </summary>
        /// <param name="value">The the value.</param>
        public void Remove(TChild value)
        {
            // Only attempt to remove a child if the child has a valid key referencing the parent.
            if (this.filterFunction(value))
            {
                // Get the current property from the child that references a unique key on the parent.
                object key = this.keyFunction(value);

                // Find the set of child records belonging to the given parent that has the key extracted from the child.
                if (this.dictionary.TryGetValue(key, out HashSet<TChild> hashSet))
                {
                    // Remove the existing child record from the hash and remove the hash if it's empty.
                    hashSet.Remove(value);
                    if (hashSet.Count == 0)
                    {
                        this.dictionary.Remove(key);
                    }
                }
            }
        }

        /// <summary>
        /// Changes a key value.
        /// </summary>
        /// <param name="value">The new record.</param>
        public void Update(TChild value)
        {
            // Get the previous version of this record.
            TChild previousValue = value.GetVersion(RecordVersion.Previous);

            // If the key to this index hasn't changed from the previous value, then there's nothing to do here.
            object currentKey = this.keyFunction(value);
            object previousKey = this.keyFunction(previousValue);
            if (!object.Equals(currentKey, previousKey))
            {
                // Only remove the previous record from the index if it has a valid key referencing the parent table.
                if (this.filterFunction(previousValue))
                {
                    // Make sure the previous exist in the index before removing the child.
                    if (!this.dictionary.TryGetValue(previousKey, out var hashSet))
                    {
                        throw new KeyNotFoundException($"{this.Name}: {previousKey}");
                    }

                    // Remove the existing child record from the hash and remove the hash from the dictionary if it's empty.
                    hashSet.Remove(value);
                    if (hashSet.Count == 0)
                    {
                        this.dictionary.Remove(previousKey);
                    }
                }

                // Only add the current record to the index if it has a valid key referencing the parent table.
                if (this.filterFunction(value))
                {
                    // Don't attempt to add a record with a null key.
                    object newKey = this.keyFunction(value);

                    // Make sure the new key exist in the parent.
                    if (!this.parentIndex.ContainsKey(newKey))
                    {
                        throw new KeyNotFoundException($"{this.Name}: {newKey}");
                    }

                    // Find or create a bucket of child records for the new newKey.
                    if (!this.dictionary.TryGetValue(newKey, out var hashSet))
                    {
                        hashSet = new HashSet<TChild>();
                        this.dictionary.Add(newKey, hashSet);
                    }

                    // Add the newKey to the index and make sure it's unique.
                    if (!hashSet.Add(value))
                    {
                        throw new DuplicateKeyException($"{this.Name}: {newKey}");
                    }
                }
            }
        }

        /// <summary>
        /// Handles a change in the parent unique index.
        /// </summary>
        /// <param name="sender">The originator of the event.</param>
        /// <param name="recordChangeEventArgs">The event arguments.</param>
        private void HandleUniqueIndexChange(object sender, RecordChangeEventArgs<object> recordChangeEventArgs)
        {
            // When deleting a parent record, or updating a parent record, enforce the constraint that the child records cannot be orphaned.
            if ((recordChangeEventArgs.DataAction == DataAction.Delete || recordChangeEventArgs.DataAction == DataAction.Update)
                && recordChangeEventArgs.Previous != null && this.dictionary.ContainsKey(recordChangeEventArgs.Previous))
            {
                throw new ConstraintException($"Attempt to delete {recordChangeEventArgs.Previous}.", this.Name);
            }
        }
    }
}