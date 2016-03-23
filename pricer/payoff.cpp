#include "payoff.hpp"
#include <cmath>

extern double payoff_call_barrier_down_out(struct simulations::Params data, int J, double L, gsl_vector* simulations)
{

	double S_T = gsl_vector_get(simulations, J);
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

extern double payoff_call_vanilla(struct simulations::Params data, double st)
{
	if (st - data.K > 0)
	{
		return st - data.K;

	}
	return 0;

}

extern double payoff_call_asian(gsl_vector* simulations, struct simulations::Params data, int J)
{
	double sum = gsl_vector_get(simulations, 0) / 2.0;
	double S_temp;

	for (int i = 1; i < J; i++)
	{
		S_temp = gsl_vector_get(simulations, i);
		sum += S_temp;
	}

	sum += gsl_vector_get(simulations, J) / 2.0;

	sum = sum / J - data.K;

	if (sum > 0)
	{
		return sum;
	}

	return 0;
}
