#include "managed_gsl.hpp"
#include "simulations.hpp"

namespace managed_gsl{
	// gsl_matrix functions
	h_gsl_matrix::h_gsl_matrix(int n1, int n2)
	{
		_matrix = gsl_matrix_calloc(n1, n2);
	}
	h_gsl_matrix::~h_gsl_matrix()
	{
		gsl_matrix_free(_matrix);
	}
	void h_gsl_matrix::set_value(int i, int j, double x){
		gsl_matrix_set(_matrix, i, j, x);
	}
	double h_gsl_matrix::get_value(int i, int j){
		return gsl_matrix_get(_matrix, i, j);
	}
	h_gsl_matrix^ h_gsl_matrix::cholesky_factorization(){
		h_gsl_matrix^ res = gcnew h_gsl_matrix(_matrix->size1, _matrix->size1);
		res->_matrix = simulations::fact_cholesky(*_matrix);
		return res;
	}

	//gsl_vector functions
	h_gsl_vector::h_gsl_vector(int n)
	{
		_vector = gsl_vector_alloc(n);
	}
	h_gsl_vector::~h_gsl_vector()
	{
		gsl_vector_free(_vector);
	}
	void h_gsl_vector::set_value(int i, double x){
		gsl_vector_set(_vector, i, x);
	}
	double h_gsl_vector::get_value(int i){
		return gsl_vector_get(_vector, i);
	}
}