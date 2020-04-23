/*

Copyright (c) 2019, Gustave Monce - gus33000.me - @gus33000
Copyright (c) 2018, Proto Beta Test - protobetatest.com - @ProtoBetaTest

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

*/

using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Deployer.FileSystem.Gpt
{
    internal class DeviceStream : Stream
    {
        private const uint GenericRead = 0x80000000;
        private const uint GenericWrite = 0x40000000;

        private const uint OpenExisting = 3;
        private const uint FileAttributeDevice = 0x40;
        private const uint FileFlagNoBuffering = 0x20000000;
        private const uint FileFlagWriteThrough = 0x80000000;
        private const uint DiskBase = 7;

        private const uint FileAnyAccess = 0;
        private const uint FileShareRead = 1;
        private const uint FileShareWrite = 2;

        private const uint FileDeviceFileSystem = 9;
        private const uint MethodBuffered = 0;

        private static readonly uint DiskGetDriveGeometryEx = CTL_CODE(DiskBase, 0x0028, MethodBuffered, FileAnyAccess);
        private static readonly uint FsctlLockVolume = CTL_CODE(FileDeviceFileSystem, 6, MethodBuffered, FileAnyAccess);
        private static readonly uint FsctlUnlockVolume = CTL_CODE(FileDeviceFileSystem, 7, MethodBuffered, FileAnyAccess);

        private enum MediaType : int
        {
            Unknown = 0,
            F51Pt2512 = 1,
            F31Pt44512 = 2,
            F32Pt88512 = 3,
            F320Pt8512 = 4,
            F3720512 = 5,
            F5360512 = 6,
            F5320512 = 7,
            F53201024 = 8,
            F5180512 = 9,
            F5160512 = 10,
            RemovableMedia = 11,
            FixedMedia = 12,
            F3120M512 = 13,
            F3640512 = 14,
            F5640512 = 15,
            F5720512 = 16,
            F31Pt2512 = 17,
            F31Pt231024 = 18,
            F51Pt231024 = 19,
            F3128Mb512 = 20,
            F3230Mb512 = 21,
            F8256128 = 22,
            F3200Mb512 = 23,
            F3240M512 = 24,
            F332M512 = 25
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DiskGeometry
        {
            internal readonly long Cylinders;
            internal readonly MediaType MediaType;
            internal readonly uint TracksPerCylinder;
            internal readonly uint SectorsPerTrack;
            internal readonly uint BytesPerSector;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DiskGeometryEx
        {
            internal readonly DiskGeometry Geometry;
            internal readonly long DiskSize;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
            internal readonly byte[] Data;
        }

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadFile(IntPtr hFile, byte[] lpBuffer, int nNumberOfBytesToRead, ref int lpNumberOfBytesRead, IntPtr lpOverlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteFile(IntPtr hFile, byte[] lpBuffer, int nNumberOfBytesToWrite, ref int lpNumberOfBytesWritten, IntPtr lpOverlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern uint DeviceIoControl(SafeFileHandle hDevice, uint dwIoControlCode, IntPtr lpInBuffer, uint nInBufferSize, IntPtr lpOutBuffer, int nOutBufferSize, ref uint lpBytesReturned, IntPtr lpOverlapped);

        [DllImport("kernel32.dll")]
        private static extern bool SetFilePointerEx(SafeFileHandle hFile, long liDistanceToMove, out long lpNewFilePointer, uint dwMoveMethod);

        private SafeFileHandle handleValue = null;
        private long position = 0;
        private readonly long length = 0;
        private readonly uint sectorsize = 0;
        private readonly bool canWrite = false;
        private readonly bool canRead = false;
        private bool disposed = false;

        private static uint CTL_CODE(uint deviceType, uint function, uint method, uint access)
        {
            return (((deviceType) << 16) | ((access) << 14) | ((function) << 2) | (method));
        }

        public DeviceStream(uint diskId, FileAccess access) : this(@"\\.\PhysicalDrive" + diskId, access)
        {            
        }

        public DeviceStream(string devicePath, FileAccess access)
        {
            uint fileAccess = 0;
            switch (access)
            {
                case FileAccess.Read:
                    fileAccess = GenericRead;
                    canRead = true;
                    break;
                case FileAccess.ReadWrite:
                    fileAccess = GenericRead | GenericWrite;
                    canRead = true;
                    canWrite = true;
                    break;
                case FileAccess.Write:
                    fileAccess = GenericWrite;
                    canWrite = true;
                    break;
            }

            (length, sectorsize) = GetDiskProperties(devicePath);

            IntPtr ptr = CreateFile(devicePath, fileAccess, 0, IntPtr.Zero, OpenExisting, FileAttributeDevice | FileFlagNoBuffering | FileFlagWriteThrough, IntPtr.Zero);
            handleValue = new SafeFileHandle(ptr, true);

            if (handleValue.IsInvalid)
            {
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            }

            uint lpBytesReturned = 0;
            uint result = DeviceIoControl(handleValue, FsctlLockVolume, IntPtr.Zero, 0, IntPtr.Zero, 0, ref lpBytesReturned, IntPtr.Zero);

            if (result == 0)
            {
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            }
        }

        public override bool CanRead
        {
            get { return canRead; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return canWrite; }
        }

        public override void Flush()
        {
            return;
        }

        public override long Length
        {
            get
            {
                return length;
            }
        }

        public override long Position
        {
            get
            {
                return position;
            }
            set
            {
                Seek(value, SeekOrigin.Begin);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between offset and 
        /// (offset + count - 1) replaced by the bytes read from the current source. </param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin storing the data read from the current stream. </param>
        /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
        /// <returns></returns>
        private int InternalRead(byte[] buffer, int offset, int count)
        {
            int bytesRead = 0;
            var bufBytes = new byte[count];
            if (!ReadFile(handleValue.DangerousGetHandle(), bufBytes, count, ref bytesRead, IntPtr.Zero))
            {
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            }
            for (int i = 0; i < bytesRead; i++)
            {
                buffer[offset + i] = bufBytes[i];
            }

            position += count;

            return bytesRead;
        }

        /// <summary>
        /// Some devices cannot read portions that are not modulo a sector, this aims to fix that issue.
        /// </summary>
        /// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between offset and 
        /// (offset + count - 1) replaced by the bytes read from the current source. </param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin storing the data read from the current stream. </param>
        /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
        /// <returns></returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (count % sectorsize != 0)
            {
                long extrastart = Position % sectorsize;
                if (extrastart != 0)
                {
                    Seek(-extrastart, SeekOrigin.Current);
                }

                var addedcount = sectorsize - count % sectorsize;
                var ncount = count + addedcount;
                byte[] tmpbuffer = new byte[extrastart + buffer.Length + addedcount];
                buffer.CopyTo(tmpbuffer, extrastart);
                InternalRead(tmpbuffer, offset, (int)ncount);
                tmpbuffer.ToList().Skip((int)extrastart).Take(count + offset).ToArray().CopyTo(buffer, 0);
                return count;
            }

            return InternalRead(buffer, offset, count);
        }

        public override int ReadByte()
        {
            int bytesRead = 0;
            var lpBuffer = new byte[1];
            if (!ReadFile(
            handleValue.DangerousGetHandle(),                        // handle to file
            lpBuffer,                                                // data buffer
            1,                                                       // number of bytes to read
            ref bytesRead,                                           // number of bytes read
            IntPtr.Zero
            ))
            { Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error()); ; }

            position += 1;

            return lpBuffer[0];
        }

        public override void WriteByte(byte Byte)
        {
            int bytesWritten = 0;
            var lpBuffer = new byte[1];
            lpBuffer[0] = Byte;
            if (!WriteFile(
            handleValue.DangerousGetHandle(),                        // handle to file
            lpBuffer,                                                // data buffer
            1,                                                       // number of bytes to write
            ref bytesWritten,                                        // number of bytes written
            IntPtr.Zero
            ))
            { Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error()); ; }

            position += 1;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            long off = offset;

            switch (origin)
            {
                case SeekOrigin.Current:
                    off += position;
                    break;
                case SeekOrigin.End:
                    off += length;
                    break;
            }

            long ret;
            if (!SetFilePointerEx(handleValue, off, out ret, 0))
                return position;
            position = ret;

            return ret;
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            int bytesWritten = 0;
            var bufBytes = new byte[count];
            for (int i = 0; i < count; i++)
            {
                bufBytes[offset + i] = buffer[i];
            }

            if (!WriteFile(handleValue.DangerousGetHandle(), bufBytes, count, ref bytesWritten, IntPtr.Zero))
            {
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            }

            position += count;
        }

        public override void Close()
        {
            uint lpBytesReturned = 0;
            var result = DeviceIoControl(handleValue, FsctlUnlockVolume, IntPtr.Zero, 0, IntPtr.Zero, 0, ref lpBytesReturned, IntPtr.Zero);

            if (0 == result)
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());

            handleValue.Close();
            handleValue.Dispose();
            handleValue = null;
            base.Close();
        }

        new void Dispose()
        {
            Dispose(true);
            base.Dispose();
            GC.SuppressFinalize(this);
        }

        private new void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (handleValue != null)
                    {
                        uint lpBytesReturned = 0;
                        var result = DeviceIoControl(handleValue, FsctlUnlockVolume, IntPtr.Zero, 0, IntPtr.Zero, 0, ref lpBytesReturned, IntPtr.Zero);

                        if (0 == result)
                            Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());

                        handleValue.Close();
                        handleValue.Dispose();
                        handleValue = null;
                    }
                }
                // Note disposing has been done.
                disposed = true;
            }
        }

        private static (long,uint) GetDiskProperties(string deviceName)
        {
            var x = new DiskGeometryEx();
            Execute(ref x, DiskGetDriveGeometryEx, deviceName);
            return (x.DiskSize, x.Geometry.BytesPerSector);
        }

        private static void Execute<T>(ref T x, uint dwIoControlCode, string lpFileName, uint dwDesiredAccess = GenericRead, uint dwShareMode = FileShareWrite | FileShareRead, IntPtr lpSecurityAttributes = default(IntPtr), uint dwCreationDisposition = OpenExisting, uint dwFlagsAndAttributes = 0, IntPtr hTemplateFile = default(IntPtr))
        {
            var hDevice = CreateFile(lpFileName, dwDesiredAccess, dwShareMode, lpSecurityAttributes, dwCreationDisposition, dwFlagsAndAttributes, hTemplateFile);
            
            var handleValue = new SafeFileHandle(hDevice, true);

            if (null == hDevice || handleValue.IsInvalid)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            var nOutBufferSize = Marshal.SizeOf(typeof(T));
            var lpOutBuffer = Marshal.AllocHGlobal(nOutBufferSize);
            var lpBytesReturned = default(uint);

            var result = DeviceIoControl(handleValue, dwIoControlCode, IntPtr.Zero, 0, lpOutBuffer, nOutBufferSize, ref lpBytesReturned, IntPtr.Zero);

            if (result == 0)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            x = (T)Marshal.PtrToStructure(lpOutBuffer, typeof(T));
            Marshal.FreeHGlobal(lpOutBuffer);

            handleValue.Close();
            handleValue.Dispose();
        }
    }
}