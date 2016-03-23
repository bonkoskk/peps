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

extern int matrices_approx_equal(const gsl_matrix *mat1, const gsl_matrix *mat2, double precision, bool *result);

extern int compounded_returns(const gsl_vector* prices, gsl_vector* returns);

extern int historic_volatility(double& hist_vol, gsl_vector* prices);

extern int historic_Yang_Zhang_Volatility(double& yz_vol, gsl_vector* open_prices, gsl_vector* close_prices, gsl_vector* high_prices, gsl_vector* low_prices, double frequency);

extern int historic_Rogers_Stachell_volatility(double& rs_vol, gsl_vector* open_prices, gsl_vector* close_prices, gsl_vector* high_prices, gsl_vector* low_prices, double frequency);

double mean_close_open(gsl_vector* open_prices, gsl_vector* close_prices);

double mean_open_close(gsl_vector* open_prices, gsl_vector* close_prices);

DLLEXP extern int compute_covariance(const gsl_matrix* prices, gsl_matrix* covariance);

DLLEXP extern int compute_covariance_Yang_Zhang(gsl_matrix* open_prices, gsl_matrix* close_prices, gsl_matrix* high_prices, gsl_matrix* low_prices, gsl_matrix* covariance);

DLLEXP extern int get_correlation_and_volatility(const gsl_matrix* covariance, gsl_matrix* correl, gsl_vector* vol);

#endif /* TOOLS_HPP */


