#ifndef LIBNATIVE_H_
#define LIBNATIVE_H_

#include <cstdint>
#include <string>
#include <vector>

namespace ns {

    extern "C" size_t get_array(unsigned int* res, size_t len);
    extern "C" void do_wstring_array(wchar_t**, int);
    extern "C" void do_wstring(wchar_t*);
    extern "C" void do_vector(std::vector<int>&);
    extern "C" void do_real_wstring(const std::wstring&);
    extern "C" void do_wstring_vector(std::vector<std::wstring>&);

}

#endif