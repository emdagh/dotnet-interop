%define CSHARP_ARRAYS_AS_REF( CTYPE, CSTYPE )
%typemap(ctype)   CTYPE AS_REF[] "CTYPE*"
%typemap(imtype)  CTYPE AS_REF[] "ref CSTYPE /** global::System.IntPtr */"
%typemap(cstype)  CTYPE AS_REF[] "global::System.Span<CSTYPE> /** CSTYPE[] */"
%typemap(csin,
           pre=       "    fixed(CSTYPE* ptr_to_$csinput = &global::System.Runtime.InteropServices.MemoryMarshal.GetReference($csinput)) {",
           terminator="    }") 
                  CTYPE AS_REF[] "ptr_to_$csinput /** (global::System.IntPtr)ptr_to_$csinput */"
%typemap(in)      CTYPE AS_REF[] "/** typemap(in) */ $1 = $input;"
%typemap(freearg) CTYPE AS_REF[] "/** freearg */"
%typemap(argout)  CTYPE AS_REF[] "/** argout */"
%enddef // CSHARP_ARRAYS_AS_REF

CSHARP_ARRAYS_AS_REF(signed char, sbyte)
CSHARP_ARRAYS_AS_REF(unsigned char, byte)
CSHARP_ARRAYS_AS_REF(short, short)
CSHARP_ARRAYS_AS_REF(unsigned short, ushort)
CSHARP_ARRAYS_AS_REF(int, int)
CSHARP_ARRAYS_AS_REF(unsigned int, uint)
CSHARP_ARRAYS_AS_REF(long, int)
CSHARP_ARRAYS_AS_REF(unsigned long, uint)
CSHARP_ARRAYS_AS_REF(long long, long)
CSHARP_ARRAYS_AS_REF(unsigned long long, ulong)
CSHARP_ARRAYS_AS_REF(float, float)
CSHARP_ARRAYS_AS_REF(double, double)
CSHARP_ARRAYS_AS_REF(bool, bool)
