﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.Diagnostics.Runtime.AbstractDac;
using Microsoft.Diagnostics.Runtime.Interfaces;

namespace Microsoft.Diagnostics.Runtime
{
    /// <summary>
    /// Represents an AppDomain in the target runtime.
    /// </summary>
    public sealed class ClrAppDomain : IClrAppDomain
    {
        private readonly IClrNativeHeapHelpers? _nativeHeapHelpers;

        internal AppDomainInfo AppDomainInfo { get; }

        internal ClrAppDomain(ClrRuntime runtime, AppDomainInfo info, IClrNativeHeapHelpers? nativeHeapHelpers)
        {
            _nativeHeapHelpers = nativeHeapHelpers;
            AppDomainInfo = info;
            Runtime = runtime;
        }

        public bool Equals(IClrAppDomain? other) => other is not null && other.Address == Address;
        public bool Equals(ClrAppDomain? other)
        {
            if (other is IClrAppDomain domain)
                return domain.Equals(this);

            return false;
        }

        public override bool Equals(object? obj)
        {
            if (obj is IClrAppDomain domain)
                return domain.Equals(this);

            return false;
        }

        public override int GetHashCode() => Address.GetHashCode();

        /// <summary>
        /// Gets the runtime associated with this ClrAppDomain.
        /// </summary>
        public ClrRuntime Runtime { get; }

        IClrRuntime IClrAppDomain.Runtime => Runtime;

        /// <summary>
        /// Gets address of the AppDomain.
        /// </summary>
        public ulong Address => AppDomainInfo.Address;

        /// <summary>
        /// Gets the AppDomain's ID.
        /// </summary>
        public int Id => AppDomainInfo.Id;

        /// <summary>
        /// Gets the name of the AppDomain, as specified when the domain was created.
        /// </summary>
        public string? Name => AppDomainInfo.Name;

        /// <summary>
        /// Gets a list of modules loaded into this AppDomain.
        /// </summary>
        public ImmutableArray<ClrModule> Modules { get; internal set; }

        /// <summary>
        /// Gets the config file used for the AppDomain.  This may be <see langword="null"/> if there was no config file
        /// loaded, or if the targeted runtime does not support enumerating that data.
        /// </summary>
        public string? ConfigurationFile => AppDomainInfo.ConfigFile;

        /// <summary>
        /// Gets the base directory for this AppDomain.  This may return <see langword="null"/> if the targeted runtime does
        /// not support enumerating this information.
        /// </summary>
        public string? ApplicationBase => AppDomainInfo.ApplicationBase;

        /// <summary>
        /// Returns the LoaderAllocator for this AppDomain.  This is used to debug some CLR internal state
        /// and isn't generally useful for most developers.  This field is only available when debugging
        /// .Net 8+ runtimes.
        /// </summary>
        public ulong LoaderAllocator =>  AppDomainInfo.Address;

        /// <summary>
        /// Enumerates the native heaps associated with this AppDomain.  Note that this may also enumerate
        /// the same heaps as other domains if they share the same LoaderAllocator (especially SystemDomain).
        /// </summary>
        /// <returns>An enumerable of native heaps associated with this AppDomain.</returns>
        public IEnumerable<ClrNativeHeapInfo> EnumerateLoaderAllocatorHeaps() => _nativeHeapHelpers?.EnumerateDomainHeaps(Address) ?? Enumerable.Empty<ClrNativeHeapInfo>();

        /// <summary>
        /// To string override.
        /// </summary>
        /// <returns>The name of this AppDomain.</returns>
        public override string? ToString() => Name;

        ImmutableArray<IClrModule> IClrAppDomain.Modules => Modules.CastArray<IClrModule>();
    }
}