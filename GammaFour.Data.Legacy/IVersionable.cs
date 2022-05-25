// <copyright file="IVersionable.cs" company="Donald Roy Airey">
//    Copyright © 2022 - Donald Roy Airey.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace GammaFour.Data.Legacy
{
    /// <summary>
    /// Allows for the cloning of different versions of a record (original, previous, current).
    /// </summary>
    public interface IVersionable
    {
        /// <summary>
        /// Gets the requested version of a record.
        /// </summary>
        /// <param name="recordVersion">The record version (original, previous, current).</param>
        /// <returns>A clone of the requested version of the record.</returns>
        object GetVersion(RecordVersion recordVersion);
    }
}