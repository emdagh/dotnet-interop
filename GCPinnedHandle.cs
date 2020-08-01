using System;
using System.Runtime.InteropServices;
namespace joaBasics 
{
        class GCPinnedHandle : IDisposable 
        {
            private GCHandle _handle;
            public GCPinnedHandle(object obj) 
            {
                if(obj == null)
                {
                    throw new ArgumentNullException("obj"); 
                }
                _handle = GCHandle.Alloc(obj, GCHandleType.Pinned);
            }

            public void Dispose() => Dispose(true);
            protected void Dispose(bool disposing) 
            {
                if(_handle.IsAllocated)
                {
                    _handle.Free();
                }
                GC.SuppressFinalize(this);
            }

            public static implicit operator IntPtr(GCPinnedHandle self) 
            {
                if(!self._handle.IsAllocated) 
                {
                    throw new InvalidCastException("Attempting to get address of pinned object that wasn't previously allocated.");
                }
                return self._handle.AddrOfPinnedObject();
            }
        }
    }