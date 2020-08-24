%module libnative
%include "typemaps.i"
%include "arrays_csharp.i"
%include "stl.i"
%include "std_string.i"
%include "include/std_wstring.i"
%include "include/netcore.i"
%include "include/debug_typemap.i"


%pragma(csharp) imclassimports=%{
using System;
using System.Runtime.InteropServices;
using joaBasics;
%}

%pragma(csharp) imclasscode=%{
  protected class SWIGWStringHelper2 {

    [return: global::System.Runtime.InteropServices.MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef=typeof(WStringMarshaler))]
    public delegate string SWIGWStringDelegate(global::System.IntPtr message);
    static SWIGWStringDelegate wstringDelegate = new SWIGWStringDelegate(CreateWString);

    [global::System.Runtime.InteropServices.DllImport("libnative", EntryPoint="SWIGRegisterWStringCallback2")]
    public static extern void SWIGRegisterWStringCallback2(SWIGWStringDelegate wstringDelegate);

    static string CreateWString(
        //[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef=typeof(WStringMarshaler))]
        IntPtr cString
    ) {
      return (string)WStringMarshaler.GetInstance("").MarshalNativeToManaged(cString);
    }

    static SWIGWStringHelper2() {
      SWIGRegisterWStringCallback2(wstringDelegate);
    }
  }
  static protected SWIGWStringHelper2 swigWStringHelper2 = new SWIGWStringHelper2();
%}

%define CSHARP_ARRAY_AND_COUNT(CTYPE, CSTYPE)
%typemap(ctype, out="/** ctype.out */ ") CTYPE& "/** ctype */ CTYPE::value_type* jarg0, size_t"
%typemap(cstype) CTYPE& "/** cstype */ CSTYPE[]"
%typemap(imtype) CTYPE& "/** imtype */ CSTYPE[] jarg0, uint"
%typemap(csin,
    pre="unsafe { /** csin.pre */
        fixed(CSTYPE* ptr_$csinput = $csinput) {",
    post="/** csin.post */ }",
    terminator="} /** csin.terminator */") CTYPE& "$csinput, (uint)$csinput.Length"
%typemap(in, numinputs=1) CTYPE& (CTYPE tmp) %{
    tmp = CTYPE(jarg0, jarg0 + $input);
    $1 = &tmp;
%}
%typemap(arginit) CTYPE& "/** arginit */"
%typemap(argout) CTYPE& "memcpy(jarg0, &tmp1[0], jarg1 * sizeof(CTYPE::value_type)); /** argout */"
%typemap(varin) CTYPE& "/** varin */"
%typemap(varout) CTYPE& "/** varout */"
%typemap(memberin) CTYPE& %{
    /** memberin */
%}
%typemap(freearg) CTYPE& %{
    /** freearg */
    //delete $1;
%}
%enddef

CSHARP_ARRAY_AND_COUNT(std::vector<int>, int);
//DEBUG_TYPEMAP(std::vector<int>&, int);

%template(vector_wstring) std::vector<std::wstring>;

//%apply unsigned int& INOUT { unsigned int& };
DEBUG_TYPEMAP(unsigned int*, global::System.IntPtr)
DEBUG_TYPEMAP(wchar_t**, global::System.IntPtr)
DEBUG_TYPEMAP(wchar_t*, global::System.IntPtr)

%{
#include "libnative.h"


/* Callback for returning strings to C# without leaking memory */
static SWIG_CSharpWStringHelperCallback SWIG_csharp_wstring_callback2 = NULL;

extern "C" SWIGEXPORT void SWIGSTDCALL SWIGRegisterWStringCallback2(SWIG_CSharpWStringHelperCallback callback) {
    SWIG_csharp_wstring_callback2 = callback;
}
%}
%include "libnative.h"