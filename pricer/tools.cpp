#include <iostream>
#include <cmath>
#include <stdexcept>
#include <gsl/gsl_blas.h>
#include <gsl/gsl_linalg.h>
#include <gsl/gsl_vector.h>
#include <gsl/gsl_matrix.h>
#include "tools.hpp"
#include <math.h>

extern int fact_cholesky(const gsl_matrix* correl, gsl_matrix* result)
{
    int size = result->size1;
    if (size != (int)correl->size2) {
        std::cout << "bad size for correl matrix. The matrix is not square." << std::endl;
        return 1;
    }
    if (result->size1 != result->size2) {
        std::cout << "bad size for result matrix. The matrix is not square." << std::endl;
        return 2;
    }
    if ((int)result->size1 != size) {
        std::cout << "result should have same dimensions as correl." << std::endl;
        return 3;
    }
    gsl_matrix_memcpy(result, correl);
    gsl_linalg_cholesky_decomp(result);
    for (int i = 1; i < size; i++) {
        for (int j = 0; j < i; j++){
            gsl_matrix_set(result, i, j, 0);
        }
    }
    return 0;
}

extern bool matrices_approx_equal(const gsl_matrix *mat1, const gsl_matrix *mat2, double precision)
{
    if ((mat1->size1 != mat2->size1) || (mat1->size2 != mat2->size2)) {
        std::cout << "The matrices are not of same size." << std::endl;
        return false;
    }
    
    for (int i = 0; i < (int)mat1->size1; i++) {
        for (int j = 0; j < (int)mat1->size2; j++) {
            if(fabs(gsl_matrix_get(mat1, i, j) - gsl_matrix_get(mat2, i, j)) > precision) 
                return false;
        }
    }
    return true;
}

// Based on John Hull's "Options, Futures and other derivatives" V7
// chap.21 "Estimating volatilities and correlations"
extern int compounded_returns(const gsl_vector* prices, gsl_vector* returns)
{
    if (prices->size != returns->size + 1) {
        std::cout << "bad size for returns vector. The vector should be of length prices->size + 1." << std::endl;
        return 1;
    }
    int nb_date = prices->size;
    for (int i = 0; i < nb_date - 1; i++)
    {
        gsl_vector_set(returns, i, (gsl_vector_get(prices,i+1)-gsl_vector_get(prices,i))/gsl_vector_get(prices,i));
    }
    return 0;
}


 // Based on John Hull's "Options, Futures and other derivatives" V7
// chap.21 "Estimating volatilities and correlations"
extern double historic_volatility(gsl_vector* prices)
{
    int nb_date = prices->size;
    gsl_vector* returns = gsl_vector_alloc(nb_date - 1);
    compounded_returns(prices, returns);
    double sum2 = 0;
    // returns average is assumed to be zero, so we dont have to compute it
    double temp;
    for (int i = 0; i < nb_date - 1; i++)
    {
        temp = gsl_vector_get(returns, i);
        sum2 += temp * temp;
    }
    double var = sum2 / (nb_date - 2);
    gsl_vector_free(returns);
    return sqrt(var);
}

double compute_covariance_for2(const gsl_vector* returns1, const gsl_vector* returns2)
{
    int nb_returns = returns1->size;
    double sum = 0;
    for (int i = 0; i < nb_returns; i++)
    {
        sum += gsl_vector_get(returns1,i) * gsl_vector_get(returns2,i);
    }
    return sum / (nb_returns - 1);
}

extern int compute_covariance(const gsl_matrix* prices, gsl_matrix* covariance)
{
    if (covariance->size1 != covariance->size2) {
        std::cout << "bad size for covariance matrix. The matrix is not square." << std::endl;
        return 1;
    }
    if (covariance->size1 != prices->size1) {
        std::cout << "error :  The covariance matrix should be of size prices->size1." << std::endl;
        return 2;
    }
    int nb_assets = prices->size1;
    int nb_dates = prices->size2;
    gsl_matrix* returns = gsl_matrix_alloc(nb_assets, nb_dates - 1);
    gsl_vector* temp_price=gsl_vector_alloc(nb_dates);
    gsl_vector* temp_returns=gsl_vector_alloc(nb_dates - 1);
    
    // Computation and storage of the compounded returns of all assets 
    for (int i = 0; i < nb_assets; i++)
    {
        gsl_matrix_get_row(temp_price,prices,i);
        compounded_returns(temp_price, temp_returns);
        gsl_matrix_set_row(returns, i, temp_returns);
    }
    
    // Computation of the correlations
    gsl_vector* temp_returns2 = gsl_vector_alloc(nb_dates - 1);
    double temp;
    for (int i = 0; i < nb_assets; i++)
    {
        gsl_matrix_get_row(temp_returns, returns, i);
        for (int j = 0; j <= i; j++)
        {
            gsl_matrix_get_row(temp_returns2, returns, j);
            temp = compute_covariance_for2(temp_returns, temp_returns2);

            //covariance[i,j] = computeCovarianceFor2(date_nb, returns[i], returns[j]);
            gsl_matrix_set(covariance,i,j,temp);
            gsl_matrix_set(covariance,j,i,temp);
        }
    }
    gsl_matrix_free(returns);
    gsl_vector_free(temp_price);
    gsl_vector_free(temp_returns);
    gsl_vector_free(temp_returns2);
    return 0;
}

extern int get_correlation_and_volatility(const gsl_matrix* covariance, gsl_matrix* correl, gsl_vector* vol)
{
    int size = covariance->size1;
    if (size != (int)correl->size2) {
        std::cout << "bad size for correl matrix. The matrix is not square." << std::endl;
        return 1;
    }
    if (size != (int)covariance->size2) {
        std::cout << "bad size for covariance matrix. The matrix is not square." << std::endl;
        return 2;
    }
    if (size != (int)correl->size1) {
        std::cout << "error :  The correl matrix should have the same dimensions as the covariance." << std::endl;
        return 3;
    }
    if (size != (int)vol->size) {
        std::cout << "error :  The volatility vector should have the same length as the covariance matrix." << std::endl;
        return 4;
    }
    double tmp;
    // Computation of the volatility vector
    for (int i = 0; i < size; i++)
    {
        tmp = sqrt(gsl_matrix_get(covariance, i, i));
        gsl_vector_set(vol, i, tmp);
    }
    // Computation of the correlation matrix
    gsl_matrix_set_identity(correl);
    for (int i = 0; i < size; i++)
    {
        for (int j = 0; j < i; j++)
        {
            tmp = gsl_matrix_get(covariance, i,j)/(gsl_vector_get(vol, i)*gsl_vector_get(vol, j));
            gsl_matrix_set(correl, i, j, tmp);
            gsl_matrix_set(correl, j, i, tmp);
        }
    }
    return 0;
}