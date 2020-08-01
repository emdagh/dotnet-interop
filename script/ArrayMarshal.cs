using System;
using System.Runtime.InteropServices;

namespace joaBasics {
    public class ArrayMarshal<T> : IDisposable 
    where T : struct 
    {
        public IntPtr Ptr { get; set; }
        public int Count { get; set; }
        public T[] Array { get; set; }
        private int sizeof_T;
        private bool _ownPointer = false;

        public void PointerToArray() {
            if(Ptr == IntPtr.Zero) return;
            Array = new T[Count];
            GCHandle handle = GCHandle.Alloc(Array, GCHandleType.Pinned);
            int size = Array.Length * sizeof_T;
            unsafe {
                for(int i=0; i < size; i++) {
                    *(((byte*)handle.AddrOfPinnedObject()) + i) = *(((byte*)Ptr) + i);
                }
            }
            handle.Free();
        }

        public void PointerFromArray() {
            if(Array == null || Ptr == IntPtr.Zero) return;
            int len = (int)Math.Min(Count, Array.Length);
            int size = len * sizeof_T;
            GCHandle handle = GCHandle.Alloc(Array, GCHandleType.Pinned);
            unsafe {
                for(int i=0; i < size; i++) {
                    *(((byte*)Ptr) + i) = *(((byte*)handle.AddrOfPinnedObject()) + i);
                }
            }
            handle.Free();

            if(Count > Array.Length) {
                unsafe {
                    int len2 = len + (sizeof_T * (Count - Array.Length));
                    for(int i=len; i < len2; i++) {
                        *(((byte*)Ptr) + i) = 0;
                    }
                }
            }
        }

        public ArrayMarshal() {
            sizeof_T = Marshal.SizeOf(typeof(T));
        }

        public ArrayMarshal(T[] ary) : this(GetPointer(ary), ary.Length) {
        }

        public ArrayMarshal(IntPtr ptr, int len) {
            sizeof_T = Marshal.SizeOf(typeof(T));
            Ptr = ptr;
            Count = len;
            PointerToArray();
        }

        ~ArrayMarshal() {
            Dispose();
        }
        public void Dispose() => Dispose(true);

        protected void Dispose(bool disposing) {
            if(_ownPointer) {
                Marshal.FreeHGlobal(Ptr);
            }
        }

        //little helper to marshal an array to a single pointer
        private static IntPtr GetPointer(T[] items)
        {
            int sizeof_T = Marshal.SizeOf(typeof(T));
            int len = items.Length * sizeof_T;
            IntPtr result = Marshal.AllocHGlobal(len);
            int start = 0;
            byte[] data = new byte[len];   //prepare to get the whole items array to this bytes array
            foreach (T item in items)
            {
                IntPtr ptr = Marshal.AllocHGlobal(sizeof_T);
                Marshal.StructureToPtr(item, ptr, false);
                Marshal.Copy(ptr, data, start, sizeof_T);
                start += sizeof_T;
                Marshal.FreeHGlobal(ptr);
            }
            Marshal.Copy(data, 0, result, len);
            return result;
        }
    }
}