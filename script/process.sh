#!/bin/bash
#BUILDDIR=$2


while getopts ":o:" opt; do
  case $opt in
    o) IMDIR="$OPTARG"
    ;;
    \?) echo "Invalid option -$OPTARG" >&2
    ;;
  esac
done

shift $((OPTIND - 1))

[ -d $IMDIR ] || mkdir -p $IMDIR
#[ -d $BUILDDIR ] || mkdir -p $BUILDDIR

DISCLAIMER="HIC SVNT LEONES - This file was automatically generated on $(date +'%Y-%m-%d %H:%M:%S')"

do_foo() {
    DIRNAME=$(echo "${1%/*}" | sed -e 's#\.\.\/##g')
    BASENAME=${1##*/}
    OUTPATH=$IMDIR/$DIRNAME
    echo "processing $1"
    echo "$(cat $1 | sed -e 's#\/\/.*$##g' -e "s/JOA.*_API//g" | ./remove_function_bodies.sh | ./filter_access.sh  | sed -e '/^\s*$/d')" > $OUTPATH/include/SWIG_$BASENAME
}

create_output_directories() {
    L=($(echo ${1} | tr ' ' '\n' | sort -u))
    for i in ${L[@]} 
    do
        BASEDIR=$IMDIR/$(echo "$i" | sed -e 's#\.\.\/##g')
        mkdir -p $BASEDIR/include 
        mkdir -p $BASEDIR/src 
        echo "** Created $BASEDIR/include" 
        echo "** Created $BASEDIR/src"
        NAME=${BASEDIR##*/} 
        CMAKELISTS=$(cat template.cmake | sed -e "s/\${MODULENAME}/${NAME}/g" -e "s/\${DISCLAIMER}/${DISCLAIMER}/g")
        echo "${CMAKELISTS}" > $BASEDIR/CMakeLists.txt
    done 
}

create_interfaces() {
    L=($(echo ${1} | tr ' ' '\n' | sort -u))
    for i in ${L[@]}
    do 
        DIRNAME=$(echo "${i%/*}" | sed -e 's#\.\.\/##g')
        BASENAME=${i##*/}
        MODULENAME=$(echo $BASENAME | cut -f 1 -d '.')
        OUTPATH=$IMDIR/$DIRNAME/$MODULENAME
        INCLUDEPATH=$(pwd)/$DIRNAME/include
        OUTFILE_INTERFACE="$OUTPATH/$MODULENAME.i"
        #echo $OUTFILE_INTERFACE
        #touch $OUTFILE_INTERFACE
        echo "//$DISCLAIMER" > $OUTFILE_INTERFACE
        echo "%module (directors=1) ${MODULENAME}" >> $OUTFILE_INTERFACE
        #echo "%ignore GENERATEADAPTOR;" >> $OUTFILE_INTERFACE
        echo '%include "typemaps.i"' >> $OUTFILE_INTERFACE
        echo "%include \"lib/${MODULENAME}.i\"" >> $OUTFILE_INTERFACE
        echo "%{" >> $OUTFILE_INTERFACE
        echo "    #include \"joaBasics/AdaptorGenerator.h\"" >> $OUTFILE_INTERFACE
        HEADERS=($(ls $OUTPATH/include))
        for header in ${HEADERS[@]}
        do
            echo "    #include <include/$header>" >> $OUTFILE_INTERFACE
        done
        echo "%}" >> $OUTFILE_INTERFACE
        echo "%include \"joaBasics/AdaptorGenerator.h\"" >> $OUTFILE_INTERFACE
        for header in ${HEADERS[@]}
        do
            echo "%include <include/$header>" >> $OUTFILE_INTERFACE
        done
    done
}

lines=()
dir=()
while read line
do
    lines+=("$line")
    dir+=(${line%/*})
done < "${1:-/dev/stdin}"

L=($(echo ${dir[@]} | tr ' ' '\n' | sort -u))

create_output_directories ${L[@]}

for line in ${lines[@]} 
do
    do_foo $line &
done
wait

create_interfaces ${L[@]}

#mkdir -p ${BUILD_DIR}
CMAKE_MAIN="${IMDIR}/CMakeLists.txt"

echo "## ${DISCLAIMER}" > "$CMAKE_MAIN"
echo "cmake_minimum_required(VERSION 3.8)" >> "$CMAKE_MAIN"
echo "project(swig_interfaces)" >> "$CMAKE_MAIN"
for i in "${L[@]}"
do 
    DIRNAME=$(echo "${i%/*}" | sed -e 's#\.\.\/##g')
    BASENAME=${i##*/}
    echo "add_subdirectory(\"$DIRNAME/$BASENAME\")" >> "$CMAKE_MAIN"
    echo $i
done

