#include "simulations.hpp"
#include "payoff.hpp"
#include <cmath>

extern double payoff_call_barrier_down_out(struct Params data, int J, double L, gsl_vector* simulations)
{

	double S_T = gsl_vector_get(simulations, J);
	double S_temp;

	double prod = 1;

	double proba;


	if (gsl_vector_get(simulations, 0) < L)
	{
		return 0;
	}

	for (int i = 1; i <= J; i++)
	{
		if (gsl_vector_get(simulations, i-1) < L || gsl_vector_get(simulations, i) < L)
		{
			return 0;
		}
		proba = 1 - exp((-2 * J * log(L / gsl_vector_get(simulations, i - 1)) * log(L / gsl_vector_get(simulations, i))) / (data.v*data.v * data.T));
		prod *= proba;
	}

	if (S_T - data.K > 0)
	{
		return (S_T - data.K)*prod;
	}

	return 0;
}

