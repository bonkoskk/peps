/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

/* 
 * File:   tools.h
 * Author: Theophile & Theo
 *
 * Created on 23 janvier 2016, 11:49
 */

#ifndef TOOLS_HPP
#define TOOLS_HPP
#pragma once
#include <gsl/gsl_vector_double.h>
#include <gsl/gsl_matrix_double.h>
#include "dll_define.hpp"

DLLEXP extern int fact_cholesky(const gsl_matrix* correl, gsl_matrix* result);

extern bool matrices_approx_equal(const gsl_matrix *mat1, const gsl_matrix *mat2, double precision);

extern int compounded_returns(const gsl_vector* prices, gsl_vector* returns);

extern double historic_volatility(gsl_vector* prices);

DLLEXP extern int compute_covariance(const gsl_matrix* prices, gsl_matrix* covariance);

DLLEXP extern int get_correlation_and_volatility(const gsl_matrix* covariance, gsl_matrix* correl, gsl_vector* vol);

#endif /* TOOLS_HPP */

