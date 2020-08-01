#!/bin/bash


STATE_NONE=0
STATE_INCLASS=1
STATE_INCLASS_BODY=2
DEPTH=0
WRITESTATE=1
STATE=STATE_NONE
CLASSDEPTH=0
LINE_DONE=0
while read line
do
    ##[[ $WRITESTATE == 1 ]] && echo "$line"
    #if [[ $line == *"${CLASSNAME}("* ]]; then 
    #    echo "$line" 
    #    continue ## catch ctor/dtor
    #fi
    #if [[ $LINE_DONE == 0 ]]; then 
    #    [[ $line == *";"* ]] && LINE_DONE=1
    #    echo "line not done $line"
    #    continue
    #fi
    

    ##echo "-- $line"
    if [[ $line == *"class"* ]]; then  #&& $STATE != $STATE_INCLASS_BODY ]]; then
        #STATE=$STATE_INCLASS
        CLASSNAME=$(echo $line | perl -pe 's/class\s+([^:\s]+).*/\1/g')
    fi
    #if [[ $line == *"{"* ]]; then 
    #    DEPTH=$(($DEPTH + 1))
    #    if [[ $STATE == $STATE_INCLASS ]]; then
    #        echo "{" 
    #        echo "public:" 
    #        STATE=$STATE_INCLASS_BODY 
    #        CLASSDEPTH=$DEPTH
    #        continue
    #    fi

    #fi
    #if [[ $STATE == $STATE_INCLASS_BODY ]]; then 
    #    [[ $line == *"public"*":"* ]]    && WRITESTATE=0 
    #    [[ $line == *"private"*":"* ]]   && WRITESTATE=0 
    #    [[ $line == *"protected"*":"* ]] && WRITESTATE=0 
    #    [[ $line == *"publicapi"*":"* ]] && WRITESTATE=1 && continue
    #    
    #fi
    #if [[ $line == *"}"* ]]; then
    #    if [[ $line == *"};"* ]]; then
    #        STATE=$STATE_NONE
    #        #echo "CLASSDEPTH=$CLASSDEPTH, DEPTH=$DEPTH"
    #        [[ $CLASSDEPTH == $DEPTH ]] && WRITESTATE=1
    #    fi
    #    DEPTH=$(($DEPTH - 1))
    #fi

    #[[ $line == *";"* ]] && LINE_DONE=1 || LINE_DONE=0
    #[[ $line == "//"* ]] && WRITESTATE=0 && LINE_DONE=1 
    #[[ $line == *"public"*":"* ]]    && WRITESTATE=0 && LINE_DONE=1
    #[[ $line == *"private"*":"* ]]   && WRITESTATE=0 && LINE_DONE=1
    #[[ $line == *"protected"*":"* ]] && WRITESTATE=0 && LINE_DONE=1
    #[[ $line == *"publicapi"*":"* ]] && WRITESTATE=1 && LINE_DONE=1

    #echo $line

    #echo "CLASSNAME=$CLASSNAME"    
    echo "$WRITESTATE:$DEPTH:$LINE_DONE -> $line"

done < "${1:-/dev/stdin}"