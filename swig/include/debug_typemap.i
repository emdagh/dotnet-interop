%define DEBUG_TYPEMAP(CTYPE, CSTYPE)
%typemap(in)                        CTYPE "/** in */ $1 = $input;"
%typemap(out,
    null="/** out.null */ NULL")    CTYPE "/** out i*/ $result = $1;"
%typemap(argout)                    CTYPE "/** argout */"
%typemap(cstype, 
    out="/** cstype.out */ CSTYPE") CTYPE "/** cstype */ CSTYPE"
%typemap(imtype,
    inattributes="/** imtype[inattributes] */",
    outattributes="/** imtype[outattributes] */",
    out="/** imtype.out */ CSTYPE") CTYPE "/** imtype */ CSTYPE"
%typemap(ctype,
    out="/** ctype.out */ CTYPE")   CTYPE "/** ctype */ CTYPE"
%typemap(csin, 
    pre="/** csin.pre */",
    post="/** csin.post */")      CTYPE "/** csin */ $csinput"
%typemap(csvarin, 
    excode=SWIGEXCODE2)             CTYPE
%{
    /** csvarin */
    set 
    { 
        $imcall;
        $excode;
    }
%}
%typemap(csout,
    excode=SWIGEXCODE)              CTYPE
%{
/** csout */
    CSTYPE ret = $imcall;$excode;
    return ret;
%}
%typemap(csvarout, 
    excode=SWIGEXCODE)              CTYPE
%{
    get 
    { /** csvarout */ 
        CSTYPE ret = $imcall;$excode;
        return ret;
    }
%}
%typemap(arginit)   CTYPE "/** arginit */"
%typemap(argout)    CTYPE "/** argout */"
%typemap(varin)     CTYPE "/** varin */"
%typemap(varout)    CTYPE "/** varout */"
%typemap(csclassmodifiers) SWIGTYPE "/** csclassmodifiers */ public class"
%enddef