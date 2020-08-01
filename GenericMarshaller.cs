namespace arrays{
    using System;
    using System.Globalization;
    using System.Text;
    using System.Runtime.InteropServices;
    class GenericMarshaller
    {
        public static IntPtr GetPointer<T>(T[] ary) {
            int sizeof_T = Marshal.SizeOf(typeof(T));
            int size = sizeof_T * ary.Length;
            IntPtr res = Marshal.AllocHGlobal(size);
            int start = 0;
            byte[] data = new byte[size];
            foreach(T item in ary) {

                IntPtr p = Marshal.AllocHGlobal(sizeof_T);
                //Marshal.StructureToPtr(item, p, false);
                Marshal.Copy(p, data, start, sizeof_T);
                Marshal.FreeHGlobal(p);
                start += sizeof_T;
            }
            Marshal.Copy(data, 0, res, size);
            return res;
        }
        public static IntPtr IntPtrFromStructArray<T>(T[] InputArray) where T : new()
        {
            int sizeof_T = Marshal.SizeOf(typeof(T));
            int len = InputArray.Length;
            IntPtr ret = Marshal.AllocHGlobal(sizeof_T * len);
            for (int i = 0; i < len; i++)
            {
                Marshal.StructureToPtr(InputArray[i], (IntPtr)(ret + i * sizeof_T), false);
            }
            return ret;
        }
        public static T[] StructArrayFromIntPtr<T>(IntPtr outArray, int size) where T : new()
        {
            int sizeof_T = Marshal.SizeOf(typeof(T));
            T[] resArray = new T[size];
            IntPtr current = outArray;
            for (int i = 0; i < size; i++)
            {
                resArray[i] = new T();
                Marshal.PtrToStructure(current, resArray[i]);
                Marshal.DestroyStructure(current, typeof(T));
                //int structsize = Marshal.SizeOf(resArray[i]);
                current = (IntPtr)(current + sizeof_T);//structsize);
            }
            Marshal.FreeHGlobal(outArray);
            return resArray;
        }


       /* 
        public static IntPtr StringArrayToIntPtr(string[] ary, Encoding encoding) {
            IntPtr[] ptrs = new IntPtr[ary.Length];
            IntPtr res = Marshal.AllocHGlobal(ary.Length * IntPtr.Size);

            for(int i=0; i < ary.Length; i++) {
                byte[] bytes = encoding.GetBytes(ary[i] + '\0');
                ptrs[i] = Marshal.AllocHGlobal(bytes.Length);
                Marshal.Copy(bytes, 0, ptrs[i], bytes.Length);
            }
            Marshal.Copy(ptrs, 0, res, ary.Length);
            return res;
        }
        public static string[] IntPtrToStringArray(IntPtr ptr, int len, Encoding encoding, bool release=true) {
            int clen = encoding.GetByteCount("x");
            IntPtr[] outputs = new IntPtr[len];
            Marshal.Copy(ptr, outputs, 0, len);
            string[] res = new string[len]; 
            for(int i=0; i < len; i++) {
                unsafe {
                    res[i] = new string((sbyte*)outputs[i], 0, _strlen((byte*)outputs[i], encoding) * clen, encoding);
                }
                if(release) Marshal.FreeHGlobal(outputs[i]);
            }
            if(release) Marshal.FreeHGlobal(ptr);

            return res;   
        }*/
    }
}