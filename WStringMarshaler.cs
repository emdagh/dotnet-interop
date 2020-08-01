namespace joaBasics 
{
    using System;
    using System.Text;
    using System.Runtime.InteropServices;

        class StringHelper 
        {
            public static unsafe int StringLength(byte* b, Encoding encoding) 
            {
                byte[] zero = encoding.GetBytes(new char[] { '\0' });
                int sizeof_Char = zero.Length;
                int i=0;
                while(true) 
                {
                    for(int j=0; j < zero.Length; j++) 
                    {
                        if(b[i * sizeof_Char + j] != 0) 
                            break;
                        return i;
                    }
                    i++;
                }
            }
        }
        class WStringMarshaler : ICustomMarshaler 
        {
            private static WStringMarshaler _instance; 
            private readonly Encoding _encoding = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? Encoding.UTF32 : Encoding.Default;

            private unsafe object NativeToManaged(IntPtr pNativeData) 
            {
                byte* ptr = (byte*)pNativeData;
                int len = StringHelper.StringLength(ptr, this._encoding);
                byte[] strbuf = new byte[len];
                Marshal.Copy(pNativeData, strbuf, 0, len);
                return _encoding.GetString(strbuf);
            }

            private IntPtr GetNativePointer(IntPtr pNativeData) 
            {
                return pNativeData - sizeof(Int32);
            }

            private int GetNativeLength(IntPtr pNativeData) {
                IntPtr ptr = this.GetNativePointer(pNativeData);//pNativeData - sizeof(Int32);
                return Marshal.ReadInt32(ptr);
            }

            public object MarshalNativeToManaged( IntPtr pNativeData ) 
            {
                int len = this.GetNativeLength(pNativeData);
                if(len > 1) 
                {
                    IntPtr[] ary = new IntPtr[len];
                    string[] res = new string[len];
                    for(int i=0; i < len; i++)
                    {
                        res[i] = (string)NativeToManaged(ary[i]);
                    }
                } 
                return NativeToManaged(pNativeData + sizeof(Int32));//this.GetNativePointer(pNativeData));
            }

            public IntPtr MarshalManagedToNative( object ManagedObj )
            {
                if(ManagedObj == null) 
                {
                    return IntPtr.Zero;
                }
                if(ManagedObj is string)
                {
                    byte[] strbuf = _encoding.GetBytes((string)ManagedObj + '\0');
                    IntPtr res = Marshal.AllocHGlobal(sizeof(Int32) + strbuf.Length);
                    Marshal.WriteInt32(res, 1);
                    Marshal.Copy(strbuf, 0, res + sizeof(Int32), strbuf.Length);
                    return res + sizeof(Int32);    
                } 
                else if(ManagedObj is string[]) 
                {
                    IntPtr[] ary = new IntPtr[((string[])ManagedObj).Length];
                    IntPtr res = Marshal.AllocHGlobal(sizeof(Int32) + ary.Length * IntPtr.Size);
                    Marshal.WriteInt32(res, ary.Length);
                    for(int i=0; i < ary.Length; i++) 
                    {
                        byte[] strbuf = _encoding.GetBytes(((string[])ManagedObj)[i] + '\0');
                        ary[i] = Marshal.AllocHGlobal(strbuf.Length);//ManagedToNative(((string[])ManagedObj)[i]);
                        Marshal.Copy(strbuf, 0, ary[i], strbuf.Length);
                    }
                    Marshal.Copy(ary, 0, res + sizeof(Int32), ary.Length);
                    return res + sizeof(Int32);
                }
                throw new MarshalDirectiveException("Argument must be a string");
            }
            public void CleanUpNativeData( IntPtr pNativeData )
            {
                int len = this.GetNativeLength(pNativeData);
                if(len > 1)
                {
                    IntPtr[] ary = new IntPtr[len];
                    for(int i=0; i < len; i++)
                    {
                        Marshal.FreeHGlobal(ary[i]);
                    } 
                }
                Marshal.FreeHGlobal(this.GetNativePointer(pNativeData));
            }
            public void CleanUpManagedData( object ManagedObj )
            {

            }
            public int GetNativeDataSize()
            {
                return -1;
            }

            public static ICustomMarshaler GetInstance(string cookie) 
            {
                if(_instance == null) 
                {
                    _instance = new WStringMarshaler();
                }
                return _instance;
            }
        }

}