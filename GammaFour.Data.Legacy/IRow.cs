// <copyright file="IRow.cs" company="Donald Roy Airey">
//    Copyright © 2022 - Donald Roy Airey.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace GammaFour.Data.Legacy
{
    /// <summary>
    /// Used by a template selector to connect a view model to the template used to display it.
    /// </summary>
    public interface IRow : IVersionable
    {
        /// <summary>
        /// Gets the element from the given column index.
        /// </summary>
        /// <param name="index">The column index.</param>
        /// <returns>The object in the row at the given index.</returns>
        object this[string index] { get; set; }
    }
}