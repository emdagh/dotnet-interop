#include "libnative.h"
#include <numeric>
#include <cstdint>
#include <iostream>
#include <cstring>

namespace ns {
    std::vector<unsigned int> vec;
    
    extern "C" void do_real_wstring(const std::wstring& s) {
        std::wcout << s << std::endl;
    }
    
    extern "C" void do_wstring_vector(std::vector<std::wstring>& v) {

        for(auto it=vec.begin(); it != vec.end(); ++it) {
            std::wcout << "native: " << *it << std::endl;
        }
    }
    extern "C" void do_vector(std::vector<int>& vec) 
    {
        std::iota(vec.begin(), vec.end(), 1);
        for(auto it=vec.begin(); it != vec.end(); ++it) {
            std::cout << "native: " << *it << std::endl;
        }
    }
    size_t get_array(unsigned int* res, size_t len) {
        if(res != nullptr) {
            std::copy(vec.begin(), std::next(vec.begin(), std::min(len, vec.size())), res);
        }
        return vec.size();
    }
    extern "C" void do_string(char* arg1) {
        std::cout << arg1 << std::endl;
    }
    extern "C" void do_wstring(wchar_t* arg1) {
        size_t len = wcslen(arg1);
        arg1[len - 1] = L'x';
        //std::wcout << arg1 << std::endl;
    }
    extern "C" void do_wstring_array(wchar_t** ary, int size) {
        for(int i=0; i < size; i++) {
            do_wstring(ary[i]);
        }
    }
    extern "C" void do_string_array(char** ary, int size) {
        for(int i=0; i < size; i++) {
            do_string(ary[i]);
        }

    }
}