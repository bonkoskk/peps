#include <gsl\gsl_matrix.h>
#include <gsl\gsl_vector.h>
#include <gsl\gsl_blas.h>

namespace managed_gsl {
	// handled gsl_matrix class
	ref class h_gsl_matrix
	{
	public:
		gsl_matrix* _matrix;
		h_gsl_matrix(int n1, int n2);
		h_gsl_matrix(int n1, int n2, array<double, 2>^ values);
		~h_gsl_matrix();
		void set_value(int i, int j, double x);
		double get_value(int i, int j);
		h_gsl_matrix^ cholesky_factorization();
	};

	// handled gsl_vector class
	ref class h_gsl_vector
	{
	public:
		gsl_vector* _vector;
		h_gsl_vector(int n);
		h_gsl_vector(int n, array<double>^ values);
		h_gsl_vector(int n, array<int>^ values);
		~h_gsl_vector();
		void set_value(int i, double x);
		double get_value(int i);
	};
}