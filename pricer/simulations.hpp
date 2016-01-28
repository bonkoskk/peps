#ifndef SIMULATIONS_HPP
#define SIMULATIONS_HPP
#pragma once

#include <gsl/gsl_vector_double.h>
#include <gsl/gsl_rng.h>
#include <gsl/gsl_matrix_double.h>
#include "dll_define.hpp"

namespace simulations {
	struct Params
	{
		int M;
		double S;
		double K;
		double r;
		double v;
		double T;
	};

	extern gsl_vector* simulate_brownian(double T, int J, gsl_rng* rng);

	extern gsl_vector* simulate_n_brownian(int nb, gsl_rng* rng);

	extern gsl_vector* simulate_sj(struct Params data, int J, gsl_rng* rng);

	extern void simulate_n_sj(gsl_matrix &path, int last_index, int nb_day_after, const gsl_vector &expected_returns, const gsl_vector &vol, const gsl_matrix &cholesky, gsl_rng* rng);

	extern double simulate_ST(struct simulations::Params data, gsl_rng* rng);

}

#endif // SIMULATIONS_HPP