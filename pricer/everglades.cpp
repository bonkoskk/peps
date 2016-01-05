#include "Everglades.hpp"
#include <time.h>

using namespace std;

Everglades::Everglades(const double VLR)
{
	mVLR = VLR;
}


Everglades::~Everglades()
{
}

double Everglades::payoff(const gsl_matrix path) const {
	gsl_vector *performances = gsl_vector_calloc(path.size1);
	int nb_timesteps = path.size1;
	int nb_underlyings = path.size2;
	double perf;
	double sum_perf = 0;
	for (int i = 0; i < nb_timesteps; i++) {
		perf = 0;
		for (int j = 1; i < nb_underlyings; i++) {
			perf += (gsl_matrix_get(&path, i, j) / gsl_matrix_get(&path, 0, j) - 1) / ((double)nb_underlyings);
			gsl_vector_set(performances, i, perf);
			sum_perf += perf;
		}
		if (i == 7 && sum_perf / 8 >= 0.12) {
			return (mVLR * 1.09);
		}
	}
	return __max(mVLR * (1 + 0.75*sum_perf / nb_timesteps), mVLR);
	return 0.0;
}

double get_price(double& price, double& ic, const gsl_matrix& data, const time_t& date, const int nbSimu){
	gsl_matrix path;
	for (int i = 0; i < nbSimu; i++){

	}
	return 1.0;
}
