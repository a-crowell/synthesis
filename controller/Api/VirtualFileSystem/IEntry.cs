﻿using SynthesisAPI.Utilities;
using System;

namespace SynthesisAPI.VirtualFileSystem
{
    /// <summary>
    /// Any type of resource managed by the virtual file system
    /// </summary>
    public interface IEntry
    {
        /// <summary>
        /// Name of the resource (used as its identifier in the virtual file system)
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Module that owns this resource
        /// </summary>
        public Guid Owner { get; internal set; }

        /// <summary>
        /// Access permissions of this resource
        /// </summary>
        public Permissions Permissions { get; internal set; }

        /// <summary>
        /// Parent directory of this resource in the virtual file system
        /// 
        /// (null if unset)
        /// </summary>
        public Directory Parent { get; internal set; }

        [ExposedApi]
        public void Delete();

        internal void DeleteInner();
    }
}
