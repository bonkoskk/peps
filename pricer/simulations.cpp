#include "simulations.hpp"
#include "iostream"
#include "ctime"
#include "gsl\gsl_randist.h"


extern gsl_vector* simulate_brownian(struct Params data, int J, double L, gsl_rng* rng)
{
	double W_temp = 0;

	gsl_vector* brownian = gsl_vector_calloc(J + 1);

	for (int i = 1; i <= J; i++)
	{
		W_temp += sqrt((data.T / J)) * gsl_ran_gaussian(rng, 1);
		gsl_vector_set(brownian, i, W_temp);
	}

	return brownian;
}

extern gsl_vector* simulate_sj(struct Params data, int J, double L, gsl_rng* rng)
{
	gsl_vector* simulations = gsl_vector_alloc(J + 1);
	gsl_vector* brownian = simulate_brownian(data, J, L, rng);

	double St = data.S;
	gsl_vector_set(simulations, 0, St);

	for (int i = 1; i <= J; i++)
	{
		St = data.S * exp((data.r - data.v*data.v / 2) * (i* data.T / J) + data.v * gsl_vector_get(brownian, i));
		gsl_vector_set(simulations, i, St);
	}

	return simulations;
}
