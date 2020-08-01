%define %cs_class(TYPE, CSTYPE)
	%ignore TYPE;
	%typemap(ctype)    TYPE*, TYPE&               %{ TYPE* %}
	%typemap(in)       TYPE*, TYPE&               %{ $1 = $input; %}
	%typemap(varin)    TYPE*, TYPE&               %{ $1 = $input; %}
	%typemap(memberin) TYPE*, TYPE&               %{ $1 = $input; %}
	%typemap(out, null="NULL") TYPE*, TYPE&       %{ $result = $1; %}
	%typemap(imtype, out="IntPtr") TYPE*, TYPE&   %{ CSTYPE %}
	%typemap(cstype)   TYPE*, TYPE&               %{ CSTYPE %}
	%typemap(csin)     TYPE*, TYPE&               %{ $csinput %}
	%typemap(csout, excode=SWIGEXCODE) TYPE*, TYPE& {
		IntPtr ptr = $imcall;$excode
		CSTYPE ret = (CSTYPE)Marshal.PtrToStructure(ptr, typeof(CSTYPE));
		return ret;
	}
	%typemap(csvarin, excode=SWIGEXCODE2) TYPE*
	%{
		set { $imcall;$excode } 
	%}
	%typemap(csvarout, excode=SWIGEXCODE2) TYPE*
	%{
		get { 
			IntPtr ptr = $imcall;$excode
			CSTYPE ret = (CSTYPE)Marshal.PtrToStructure(ptr, typeof(CSTYPE));
			return ret;
		}
	%}
%enddef