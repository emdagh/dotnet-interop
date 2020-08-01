cmake_minimum_required(VERSION 3.8)
project(libnative)
find_package(SWIG REQUIRED)
include(${SWIG_USE_FILE})

message(STATUS "CMAKE_USE_FILE=${CMAKE_USE_FILE}")

file(GLOB SRC *.i ${CMAKE_SOURCE_DIR}/src/*.cpp)

set(SWIG_INCLUDE_DIRS 
    ${CMAKE_SOURCE_DIR}/include
    ${CMAKE_CURRENT_SOURCE_DIR}/include 
)

set_source_files_properties(${SRC} PROPERTIES
    CPLUSPLUS ON 
    SWIG_FLAGS "-debug-typemap;-Wall"
    INCLUDE_DIRECTORIES "${SWIG_INCLUDE_DIRS}"
    GENERATED_INCLUDE_DIRECTORIES "${SWIG_INCLUDE_DIRS}"
)

swig_add_library(${PROJECT_NAME}
    TYPE MODULE 
    LANGUAGE csharp
    OUTPUT_DIR ${CMAKE_SOURCE_DIR}/gen 
    OUTFILE_DIR ${CMAKE_SOURCE_DIR}/src/gen
    SOURCES ${SRC}
)
swig_link_libraries(${PROJECT_NAME})