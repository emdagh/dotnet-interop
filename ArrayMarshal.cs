
namespace arrays {
using System;
using System.Runtime.InteropServices;
    class ArrayMarshal<T> : IDisposable
    where T: struct
    {

        public int Count { get; set; }
        public IntPtr Ptr { get; set; }
        public T[] Array { get; set; }
        public static readonly int sizeof_T = System.Runtime.InteropServices.Marshal.SizeOf(typeof(T));

        bool _ownMemory= false;

        public void ArrayToPointer() {
            if(Array == null || Ptr == IntPtr.Zero) {
                return;
            }

            int len = (int)Math.Min(Count, Array.Length);
            int size = len * sizeof_T;
            GCHandle hnd = GCHandle.Alloc(Array, GCHandleType.Pinned);
            unsafe {
                for(int i=0; i < size; i++) {
                    *(((byte*)Ptr) + i) = *(((byte*)hnd.AddrOfPinnedObject()) + i);
                }
            }
            hnd.Free();

            if(Count > Array.Length) {
                unsafe {
                    int size2 = size + (sizeof_T * (Count - Array.Length));
                    for(int i=size; i < size2; i++) {
                        *(((byte*)Ptr) + i) = 0;
                    }
                }
            }
        }

        public void PointerToArray() {
            if(Ptr == IntPtr.Zero) {
                return;
            }
            Array = new T[Count];
            GCHandle hnd = GCHandle.Alloc(Array, GCHandleType.Pinned);
            int size = Array.Length * sizeof_T;
            unsafe {
                for(int i=0; i < size; i++) {
                    *(((byte*)hnd.AddrOfPinnedObject())) = *(((byte*)Ptr) + i);
                }
            }
            hnd.Free();
        }

        public ArrayMarshal(IntPtr ptr, int len)
        { 
            //sizeof_T = Marshal.SizeOf(typeof(T));
            Ptr = ptr;
            Count = len;
            PointerToArray();
        }

        public ArrayMarshal(T[] ary) : this(GenericMarshaller.GetPointer<T>(ary), ary.Length)
        {
            _ownMemory = true;
        }

        ~ArrayMarshal() {
        }

        public void Dispose() => Dispose(true);

        protected void Dispose(bool disposing) {
            if(_ownMemory) {
                Marshal.FreeHGlobal(Ptr);
            }
        }        
    }

}