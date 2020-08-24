namespace joaBasics 
{
    using System;
    using System.Text;
    using System.Runtime.InteropServices;

    class StringHelper 
    {
        public static unsafe int StringLength(sbyte* b, Encoding encoding) 
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

        public object MarshalNativeToManaged( IntPtr pNativeData ) 
        {
            if(pNativeData == IntPtr.Zero)
                return null;
            
            unsafe {
                sbyte* ptr = (sbyte*)pNativeData;
                int sizeof_Char = _encoding.GetBytes("\0").Length;
                int len = StringHelper.StringLength(ptr, this._encoding);
                return new string(ptr, 0, len * sizeof_Char, _encoding);
            }
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
                IntPtr res = Marshal.AllocHGlobal(strbuf.Length);
                Marshal.Copy(strbuf, 0, res, strbuf.Length);
                return res;    
            } 
            throw new MarshalDirectiveException("Argument must be a string");
        }
        public void CleanUpNativeData( IntPtr pNativeData )
        {
            Marshal.FreeHGlobal(pNativeData);
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