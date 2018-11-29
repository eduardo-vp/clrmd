﻿using System.Runtime.InteropServices;

namespace Microsoft.Diagnostics.Runtime.DacInterface
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct StackRefData
    {
        public readonly uint HasRegisterInformation;
        public readonly int Register;
        public readonly int Offset;
        public readonly ulong Address;
        public readonly ulong Object;
        public readonly uint Flags;

        public readonly uint SourceType;
        public readonly ulong Source;
        public readonly ulong StackPointer;
    }
}