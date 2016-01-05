#include "simulations.hpp"
#include "payoff.hpp"

extern double payoff_barrier(struct Params data, int J, double L, gsl_vector* simulations)
{

	double S_T = gsl_vector_get(simulations, J);
	double S_temp;


	for (int i = 0; i <= J; i++)
	{
		S_temp = gsl_vector_get(simulations, i);
		if (S_temp < L)
		{
			return 0;
		}
	}

	if (S_T - data.K > 0)
	{
		return S_T - data.K;
	}

	return 0;
}

