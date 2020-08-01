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
[StructLayout(LayoutKind.Sequential)]
public struct datablock
{
    public IntPtr _array;
    public uint _size;
}
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
%}
%include "libnative.h"