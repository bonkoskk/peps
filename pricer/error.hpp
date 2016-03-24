/*
* File:   error.hpp
* Author : Théophile, Théo, Mouhamadou Bamba
* Created on 17 mars 2016, 16 : 08
*/
#ifndef ERROR_HPP
#define ERROR_HPP

#include "dll_define.hpp"
#include <string> 
using namespace std;

DLLEXP extern string get_error_message(int error_num);

#endif /* ERROR_HPP */
