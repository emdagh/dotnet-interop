using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using Microsoft.Win32.SafeHandles;
namespace arrays
{
    class GCPinnedHandle : IDisposable 
    {
        private GCHandle _handle;
        public GCPinnedHandle(object obj)
        {
            if(obj == null) 
            {
                throw new ArgumentException("invalid object");
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

        public static implicit operator IntPtr(GCPinnedHandle hnd) 
        { 
            if(!hnd._handle.IsAllocated)
            {
                throw new InvalidCastException("Can't obtain an unallocated pointer from GCHandle");
            }
            return hnd._handle.AddrOfPinnedObject();
        }
    }
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

        public static implicit operator IntPtr(HGlobal h) 
        {
            return h._handle;
        }
    };
    class WStringHelper 
    {
        public static byte[] WStringGetBytes(string str, Encoding encoding) 
        {
            return encoding.GetBytes(str + '\0');
        }
        public static unsafe int WStringLength(byte* bytes, Encoding encoding)
        {
            byte[] zero = encoding.GetBytes(new char[] { '\0' });
            int sizeof_Char = zero.Length;
            int i = 0;
            while(true)
            {
                for(int j=0; j < zero.Length; j++) 
                {
                    if(bytes[i * sizeof_Char + j] != 0) 
                    {
                        break;
                    }
                    return i;
                }
                i++;
            }
        }
    }
    class ArrayMarshaller<T> : IDisposable
    where T : struct
    {
        T[] _array;
        IntPtr _ptr;

        ArrayMarshaller(IntPtr ptr1, int size)
        {
            _array = new T[size];
        }

        public void Dispose() => Dispose(true);

        protected void Dispose(bool disposing) 
        {
        }
    }
    class WStringMarshaller : IDisposable
    {
        //HGlobal _handle;
        IntPtr[] _handles = null; 
        public IntPtr[] Handles 
        {
            get 
            {
                return _handles;
            }
        }
        static Encoding _encoding = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? Encoding.UTF32 : Encoding.Default;
        static int sizeof_Char = _encoding.GetByteCount("\0");
        public WStringMarshaller(string[] strs) 
        {
            _handles = MarshalManagedToNative(strs);
        }

        public string[] MarshalNativeToManaged() {
            return MarshalNativeToManaged(_handles);
        }

        public static IntPtr[] MarshalManagedToNative(string[] obj) {
            IntPtr[] res = new IntPtr[obj.Length];
            for(int i=0; i < obj.Length; i++) 
            {
                byte[] bytes = WStringHelper.WStringGetBytes(obj[i], _encoding);
                res[i] = Marshal.AllocHGlobal(bytes.Length);
                Marshal.Copy(bytes, 0, res[i], bytes.Length);
            }
            return res;
        }


        public static unsafe string[] MarshalNativeToManaged(IntPtr[] input)
        {
            string[] res = new string[input.Length];
            for(int i=0; i < input.Length; i++) 
            {
                res[i] = new string((sbyte*)input[i], 0, WStringHelper.WStringLength((byte*)input[i], _encoding) * sizeof_Char, _encoding);
            }
            return res;
        }

        public void Dispose()
        {

            foreach(IntPtr ptr in _handles) 
            {
                Marshal.FreeHGlobal(ptr);
            }            
        }

        public IntPtr ToIntPtr() 
        {
            IntPtr res = Marshal.AllocHGlobal(_handles.Length * IntPtr.Size);
            Marshal.Copy(_handles, 0, res, _handles.Length);
            return res;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            /*string[] strings = new string[5];
            for(int i=0; i < strings.Length; i++) {
                strings[i] = new string("string");
            }
            int[] x = new int[5];
            libnative.do_vector(x);
            foreach(int i in x) 
            {
                Console.WriteLine(i);
            }
            libnative.do_wstring_vector(new vector_wstring(strings));*/
        }
    }
}
