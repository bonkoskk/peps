#include "Everglades.hpp"
#include "simulations.hpp"
#include <time.h>
#include "iostream"

using namespace std;

double Everglades::get_payoff(const gsl_matrix &path, double vlr, bool &anticipated) {
	anticipated = false;
	int nb_timesteps = path.size2-1;
	int nb_underlyings = path.size1;
	double perf;
	double sum_perf = 0.0;
	for (int i = 1; i < nb_timesteps; i++) {
		perf = 0.0;
		for (int j = 0; j < nb_underlyings; j++) {
			perf += (gsl_matrix_get(&path, j, i) / gsl_matrix_get(&path, j, 0) - 1.0);
			sum_perf += perf / ((double)nb_underlyings);
		}
		if (i == 7 && sum_perf / 8 >= 0.12) {
			anticipated = true;
			return (vlr * 1.09);
		}
	}
	double res = __max(vlr * (1.0 + 0.75*sum_perf / nb_timesteps), vlr);
	return __max(vlr * (1.0 + 0.75*sum_perf / nb_timesteps), vlr);
}

void Everglades::get_price(double& price, double& ic, gsl_vector** delta, const gsl_matrix& historic, int nb_day_after, double r,
	const gsl_vector& expected_returns, const gsl_vector& vol, const gsl_matrix& correl, int nbSimu){
	
	double epsilon = 1;

	int nb_sj = historic.size1;
	*delta = gsl_vector_calloc(nb_sj);

	int last_index = historic.size2;
	gsl_matrix* path = gsl_matrix_calloc(historic.size1, 25);
	gsl_vector *temp = gsl_vector_calloc(historic.size1);
	gsl_rng *rng = gsl_rng_alloc(gsl_rng_default);
	gsl_rng_set(rng, (unsigned long int)time(NULL));

	//Copie d'historic dans le début du path
	for (int i = 0; i < last_index; i++) {
		gsl_matrix_get_col(temp, &historic, i);
		gsl_matrix_set_col(path, i, temp);
	}

	//Initialisation valeurs intermédiaires du calcul du prix et de l'intervalle de confiance
	double sumPayoff_anticipated = 0.0;
	double sumIc_anticipated = 0.0;
	double sumPayoff_final = 0.0;
	double sumIc_final = 0.0;
	double phi;
	bool anticipated;

	double s_temp;
	double payoff_up;
	double payoff_down;
	double delta_temp;

	//Simulations de Monte-Carlo
	for (int i = 0; i < nbSimu; i++){
		gsl_matrix_set_col(path, last_index - 1, temp); //Remise à sa valeur initiale de la valeur actuelle des sous-jacents

		simulations::simulate_n_sj(*path, last_index, nb_day_after, expected_returns, vol, correl, rng);


		for (int sj = 0; sj < nb_sj; sj++)
		{
			gsl_matrix* path_up = gsl_matrix_alloc(path->size1, path->size2);
			gsl_matrix_memcpy(path_up, path);

			//gsl_matrix* path_down = new gsl_matrix(*path);


			
			for (int t = last_index; t < path->size2; t++)
			{
				s_temp = gsl_matrix_get(path, sj, t);
				gsl_matrix_set(path_up, sj, t, (1+1000)*s_temp);
			//	gsl_matrix_set(path_down, sj, t, s_temp - epsilon);
			}

			payoff_up = Everglades::get_payoff(*path_up, VLR, anticipated);

			if (anticipated) {
				payoff_up *= exp(-r*((9 - last_index) * PERIODE - nb_day_after) / DAY);
			} else {
				payoff_up *= exp(-r*((25 - last_index) * PERIODE - nb_day_after) / DAY);
			}
			payoff_up /= ((double)nbSimu);

			/*payoff_down = Everglades::get_payoff(*path_down, VLR, anticipated);

			if (anticipated) {
				payoff_down *= exp(-r*(25 - last_index + 1) * (PERIODE - nb_day_after) / DAY);
			}
			else {
				payoff_down *= exp(-r*(25 - last_index + 1) * (PERIODE - nb_day_after) / DAY);
			}
			payoff_down /= ((double)nbSimu);*/

			delta_temp = gsl_vector_get(*delta, sj);
			gsl_vector_set(*delta, sj, delta_temp + (payoff_up));
			//gsl_vector_set(*delta, sj, delta_temp + (payoff_up - payoff_down) / ( (double)nbSimu * 2.0 * espsilon));

			//gsl_matrix_free(path_up);
		}

		phi = Everglades::get_payoff(*path, VLR, anticipated);
		
		
		if (anticipated) {
			sumPayoff_anticipated += phi;
			sumIc_anticipated += phi*phi;
		}
		else {
			sumPayoff_final += phi;
			sumIc_final += phi*phi;
		}
	}

	//calcul du prix
	price = 0;
	price += sumPayoff_anticipated*exp(-r*((9 - last_index) * PERIODE - nb_day_after) / DAY);
	price += sumPayoff_final*exp(-r*((25 - last_index) * PERIODE - nb_day_after) / DAY);
	price /= ((double)nbSimu);

	//calcul de l'intervalle de confiance
	double sumIc = sumIc_anticipated*exp(-2 * r*(25 - last_index) * (PERIODE - nb_day_after) / DAY);
	sumIc += sumIc_final*exp(-2*r*(25 - last_index + 1) * (PERIODE - nb_day_after) / DAY);
	double xi2 = sumIc / ((double) nbSimu) - (price)*(price);
	ic = 1.96*sqrt(xi2) / sqrt((double) nbSimu);

	//libération mémoire
	gsl_vector_free(temp);
	gsl_matrix_free(path);
}

