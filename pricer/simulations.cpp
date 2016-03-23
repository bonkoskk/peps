#include <iostream>
#include <cmath>
#include <stdexcept>
#include <gsl/gsl_randist.h>
#include <gsl/gsl_blas.h>
#include <gsl/gsl_linalg.h>
#include "simulations.hpp"
#include "everglades.hpp"


int simulations::simulate_brownian(double T, int J, gsl_rng* rng, gsl_vector** brownian)
{
	if (T < 0) {
		return 16;
	}
	if (J <= 0) {
		return 17;
	}
	if (rng == NULL) {
		return 18;
	}
	if (brownian == NULL) {
		return 19;
	}
	double W_temp = 0;
	*brownian = gsl_vector_calloc(J + 1);
	for (int i = 1; i <= J; i++)
	{
		W_temp += sqrt((T / J)) * gsl_ran_ugaussian(rng);
		gsl_vector_set(*brownian, i, W_temp);
	}
	return 0;
}

int simulations::simulate_n_brownian(int nb, gsl_rng* rng, gsl_vector** brownian)
{
	if (rng == NULL) {
		return 18;
	}
	if (nb < 1) {
		return 20;
	}
	if (brownian == NULL) {
		return 19;
	}
	double W_temp = 0;
	*brownian = gsl_vector_calloc(nb);
	for (int i = 0; i < nb; i++)
	{
		W_temp = gsl_ran_ugaussian(rng);
		gsl_vector_set(*brownian, i, W_temp);
	}
	return 0;
}

int simulations::simulate_sj(struct simulations::Params data, int J, gsl_rng* rng, gsl_vector** simulations)
{
	int err;
	if (data.r < 0 || data.v < 0 || data.T < 0 || data.S < 0 || data.K < 0) {
		return 10;
	}
	if (J <= 0) {
		return 17;
	}
	if (rng == NULL) {
		return 18;
	}
	if (simulations == NULL) {
		return 19;
	}
	*simulations = gsl_vector_alloc(J + 1);
	gsl_vector* brownian;
	err = simulate_brownian(data.T, J, rng, &brownian);
	if (err) {
		return err;
	}
	double St = data.S;
	gsl_vector_set(*simulations, 0, St);
	for (int i = 1; i <= J; i++)
	{
		St = data.S * exp((data.r - data.v*data.v / 2) * (i* data.T / J) + data.v * gsl_vector_get(brownian, i));
		gsl_vector_set(*simulations, i, St);
	}
	gsl_vector_free(brownian);
	return 0;
}

int simulations::simulate_ST(struct simulations::Params data, gsl_rng* rng, double* result)
{
	if (data.r < 0 || data.v < 0 || data.T < 0 || data.S < 0 || data.K < 0) {
		return 10;
	}
	if (rng == NULL) {
		return 18;
	}
	if (result == NULL) {
		return 19;
	}
	*result = data.S * exp((data.r - data.v*data.v / 2) * (data.T) + data.v *  sqrt(data.T) * gsl_ran_ugaussian(rng));
	return 0;
}

int simulations::simulate_n_sj(gsl_matrix &path, int last_index, int nb_day_after, const gsl_vector &expected_returns, const gsl_vector &vol, const gsl_matrix &cholesky, gsl_rng* rng)
{
	int err;
	if (last_index < 0) {
		return 22;
	}
	if (nb_day_after < 0 || nb_day_after > PERIODE) {
		return 12;
	}
	if (cholesky.size1 != cholesky.size2) {
		return 1;
	}
	for (int i = 0; i<cholesky.size1; i++) {
		for (int j = 0; j<cholesky.size2; j++) {
			if (j > i && std::abs(gsl_matrix_get(&cholesky, i, j)) > 0.00001) {
				return 23;
			}
		}
	}
	if (rng == NULL) {
		return 18;
	}


	int size = path.size2;
	int nb_stocks = path.size1;
	gsl_vector *Ld = gsl_vector_calloc(nb_stocks);
	gsl_vector *Brownien;
	err = simulate_n_brownian(nb_stocks, rng, &Brownien);
	if (err) {
		return err;
	}

	double prev_val;
	double curr_val;
	double sigmad;
	double lg;
	if (nb_day_after != 0) {
		for (int j = 0; j <nb_stocks; j++){
			prev_val = gsl_matrix_get(&path, j, last_index - 1);
			gsl_matrix_get_row(Ld, &cholesky, j);
			sigmad = gsl_vector_get(&vol, j);

			gsl_blas_ddot(Ld, Brownien, &lg);
			curr_val = prev_val * exp((gsl_vector_get(&expected_returns, j) - sigmad*sigmad / 2)*(PERIODE - nb_day_after) / DAY + sigmad*sqrt((PERIODE - nb_day_after) / DAY)*lg);
			gsl_matrix_set(&path, j, last_index - 1, curr_val);
		}
	}
	gsl_vector_free(Brownien);

	for (int i = last_index; i < size; i++) {
		err = simulate_n_brownian(nb_stocks, rng, &Brownien);
		if (err) {
			return err;
		}
		for (int j = 0; j < nb_stocks; j++){
			prev_val = gsl_matrix_get(&path, j, i - 1);
			gsl_matrix_get_row(Ld, &cholesky, j);
			sigmad = gsl_vector_get(&vol, j);
			gsl_blas_ddot(Ld, Brownien, &lg);
			curr_val = prev_val * exp((gsl_vector_get(&expected_returns, j) - sigmad*sigmad / 2)*(PERIODE) / DAY + sigmad*sqrt(PERIODE / DAY)*lg);
			gsl_matrix_set(&path, j, i, curr_val);
		}
		gsl_vector_free(Brownien);
	}
	gsl_vector_free(Ld);
	return 0;
}





int simulations::simulate_sj_integral(struct Params data, int J, gsl_rng* rng,gsl_vector** simulations){
	int err;
	if (data.r < 0 || data.v < 0 || data.T < 0 || data.S < 0 || data.K < 0) {
		return 10;
	}
	if (J <= 0) {
		return 17;
	}
	if (rng == NULL) {
		return 18;
	}
	if (simulations == NULL) {
		return 19;
	}
	*simulations = gsl_vector_alloc(J + 1);
	gsl_vector* brownian;
	err = simulate_brownian(data.T, J, rng, &brownian);

	double St = data.S;
	gsl_vector_set(*simulations, 0, St);

	for (int i = 1; i <= J; i++)
	{
		St = data.S * exp((data.r - data.v*data.v / 2) * (i* data.T / J) + data.v * gsl_vector_get(brownian, i));
		gsl_vector_set(*simulations, i, St);
	}

	return 0;
}
