#include "simulations.hpp"
#include "iostream"
#include "gsl\gsl_randist.h"
#include "gsl\gsl_blas.h"
#include "everglades.hpp"


extern gsl_vector* simulations::simulate_brownian(double T, int J, gsl_rng* rng)
{
	double W_temp = 0;

	gsl_vector* brownian = gsl_vector_calloc(J + 1);

	for (int i = 1; i <= J; i++)
	{
		W_temp += sqrt((T / J)) * gsl_ran_ugaussian(rng);
		gsl_vector_set(brownian, i, W_temp);
	}

	return brownian;
}

extern gsl_vector* simulations::simulate_n_brownian(int nb, gsl_rng* rng)
{
	double W_temp = 0;

	gsl_vector* brownian = gsl_vector_calloc(nb);

	for (int i = 0; i < nb; i++)
	{
		W_temp = gsl_ran_ugaussian(rng);
		gsl_vector_set(brownian, i, W_temp);
	}
	return brownian;
}

extern gsl_matrix* simulations::fact_cholesky(gsl_matrix &cov) {
	if (cov.size1 != cov.size2) throw std::invalid_argument("matrix must be square for cholesky factorization!");
	int n = cov.size1;
	double sum1 = 0.0;
	double sum2 = 0.0;
	double sum3 = 0.0;
	double tmp;
	gsl_matrix *l = gsl_matrix_calloc(n,n);
	gsl_matrix_set(l, 0, 0, sqrt(gsl_matrix_get(&cov, 0, 0)));
	for (int j = 1; j <= n - 1; j++)
		gsl_matrix_set(l, j, 0, gsl_matrix_get(&cov, j, 0)/gsl_matrix_get(l, 0, 0));
	for (int i = 1; i <= (n - 2); i++)
	{
		sum1 = 0.0;
		for (int k = 0; k <= (i - 1); k++)
			sum1 += gsl_matrix_get(l, i, k)*gsl_matrix_get(l, i, k);
		gsl_matrix_set(l, i, i, sqrt(gsl_matrix_get(&cov, i, i) - sum1));
		for (int j = (i + 1); j <= (n - 1); j++)
		{
			sum2 = 0.0;
			for (int k = 0; k <= (i - 1); k++)
				sum2 += gsl_matrix_get(l, j, k)*gsl_matrix_get(l, i, k);
			gsl_matrix_set(l, j, i, (gsl_matrix_get(&cov, j, i) - sum2) / gsl_matrix_get(l, i, i));
		}
	}
	for (int k = 0; k <= (n - 2); k++)
		sum3 += gsl_matrix_get(l, n - 1, k)* gsl_matrix_get(l, n - 1, k);
	gsl_matrix_set(l, n - 1, n - 1, sqrt(gsl_matrix_get(&cov, n - 1, n - 1) - sum3));
	return l;
}

extern gsl_vector* simulations::simulate_sj(struct simulations::Params data, int J, gsl_rng* rng)
{
	gsl_vector* simulations = gsl_vector_alloc(J + 1);
	gsl_vector* brownian = simulate_brownian(data.T, J, rng);

	double St = data.S;
	gsl_vector_set(simulations, 0, St);

	for (int i = 1; i <= J; i++)
	{
		St = data.S * exp((data.r - data.v*data.v / 2) * (i* data.T / J) + data.v * gsl_vector_get(brownian, i));
		gsl_vector_set(simulations, i, St);
	}
	gsl_vector_free(brownian);
	return simulations;
}

extern void simulations::simulate_n_sj(gsl_matrix &path, int last_index, int nb_day_after, const gsl_vector &expected_returns, const gsl_vector &vol, const gsl_matrix &cholesky, gsl_rng* rng)
{
	int size = path.size2;
	int nb_stocks = path.size1;

	gsl_vector *Ld = gsl_vector_calloc(nb_stocks);
	gsl_vector *Brownien = simulate_n_brownian(nb_stocks, rng);

	double prev_val;
	double curr_val;
	double sigmad;
	double lg;
	for (int j = 0; j <nb_stocks; j++){
		prev_val = gsl_matrix_get(&path, j, last_index-1);
		gsl_matrix_get_col(Ld, &cholesky, j);
		sigmad = gsl_vector_get(&vol, j);

		gsl_blas_ddot(Ld, Brownien, &lg);
		curr_val = prev_val * exp((gsl_vector_get(&expected_returns, j) - sigmad*sigmad / 2)*(PERIODE - nb_day_after) / DAY + sigmad*sqrt((PERIODE - nb_day_after) / DAY)*lg);
		gsl_matrix_set(&path, j, last_index-1, curr_val);
	}

	for (int i = last_index; i < size; i++) {
		Brownien = simulate_n_brownian(nb_stocks, rng);
		for (int j = 0; j < nb_stocks; j++){
			prev_val = gsl_matrix_get(&path, j, i - 1);
			gsl_matrix_get_col(Ld, &cholesky, j);
			sigmad = gsl_vector_get(&vol, j);
			gsl_blas_ddot(Ld, Brownien, &lg);
			curr_val = prev_val * exp((gsl_vector_get(&expected_returns, j) - sigmad*sigmad / 2)*(PERIODE) / DAY + sigmad*sqrt(PERIODE / DAY)*lg);
			gsl_matrix_set(&path, j, i, curr_val);
		}
	}
	gsl_vector_free(Ld);
	gsl_vector_free(Brownien);
}
