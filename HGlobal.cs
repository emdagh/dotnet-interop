using System;
using System.Runtime.InteropServices;

namespace joaBasics 
{
        class HGlobal : IDisposable
        {
            IntPtr _handle;
            public HGlobal(int cb)
            {
                _handle = Marshal.AllocHGlobal(cb);
            }

            public void Dispose() => Dispose(true);

            protected void Dispose(bool disposing) 
            {
                Marshal.FreeHGlobal(_handle);
                GC.SuppressFinalize(this);
            }

            public static implicit operator IntPtr(HGlobal self)
            {
                return self._handle;
            }
        }
    }