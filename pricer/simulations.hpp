#include "gsl\gsl_vector_double.h"
#include "gsl\gsl_rng.h"

struct Params
{
	int M;
	double S;
	double K;
	double r;
	double v;
	double T;
};

extern gsl_vector* simulate_brownian(struct Params data, int J, double L, gsl_rng* rng);

extern gsl_vector* simulate_sj(struct Params data, int J, double L, gsl_rng* rng);
