#include "simulations.hpp"
#include "computations.hpp"
#include "payoff.hpp"
#include <cmath>
#include <ctime>

void computations::option_barrier(double &ic, double &prix, int nb_samples, double T,
	double S0, double K, double sigma, double r, double J, double L)
{
	struct Params data;
	data.M = nb_samples;
	data.S = S0;
	data.K = K;
	data.r = r;
	data.v = sigma;
	data.T = T;

	double sum = 0;
	double var = 0;

	double payoff;

	gsl_rng *rng = gsl_rng_alloc(gsl_rng_default);
	gsl_rng_set(rng, (unsigned long int)time(NULL));

	gsl_vector* simulations;

	for (int i = 0; i < nb_samples; i++)
	{
		simulations = simulate_sj(data, J, L, rng);

		payoff = payoff_barrier(data, J, L, simulations);
		sum += exp(-r*T) * payoff;
		var += exp(-2.*r*T) * payoff * payoff;
	}


	prix = sum / nb_samples;
	var = var / nb_samples - prix * prix;
	ic = 1.96 * sqrt(var / nb_samples);
	gsl_rng_free(rng);
}