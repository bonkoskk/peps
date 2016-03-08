#include <time.h>
#include <iostream>
#include <cmath>

#include "everglades.hpp"
#include "simulations.hpp"

#ifndef __max(a,b)
	#define __max(a,b) \
		({ __typeof__ (a) _a = (a); \
			__typeof__ (b) _b = (b); \
			_a > _b ? _a : _b; })
#endif

using namespace std;

extern double Everglades::get_payoff(const gsl_matrix &path, double vlr, bool &anticipated) {
	anticipated = false;
	int nb_timesteps = path.size2 - 1;
	int nb_underlyings = path.size1;
	double perf;
	double sum_perf = 0.0;
	for (int i = 1; i < nb_timesteps; i++) {
		perf = 0.0;
		for (int j = 0; j < nb_underlyings; j++) {
			perf += (gsl_matrix_get(&path, j, i) / gsl_matrix_get(&path, j, 0) - 1.0);
		}
		sum_perf += perf / ((double)nb_underlyings);
		if (i == 7 && sum_perf / 8 >= 0.12) {
			anticipated = true;
			return (vlr * 1.09);
		}
	}
	return __max(vlr * (1.0 + 0.75*sum_perf / nb_timesteps), vlr);
}

extern int Everglades::get_price(double& price, double& ic, gsl_vector** delta, const gsl_matrix& historic, int nb_day_after, double r,
	const gsl_vector& expected_returns, const gsl_vector& vol, const gsl_matrix& correl, int nbSimu){

	//Récupération des tailles
	int nb_sj = historic.size1;
	int last_index = historic.size2;
	int nb_dates = 25;

	//Allocation des vecteurs itermédiaires de calculs
	*delta = gsl_vector_calloc(nb_sj);
	gsl_matrix *path = gsl_matrix_alloc(nb_sj, nb_dates);
	gsl_matrix *path_up = gsl_matrix_alloc(nb_sj, nb_dates);
	gsl_matrix *path_down = gsl_matrix_alloc(nb_sj, nb_dates);
	gsl_vector *temp = gsl_vector_alloc(nb_sj);
	gsl_vector *epsilon = gsl_vector_alloc(nb_sj);
	gsl_vector *payoff_up_anticipated = gsl_vector_calloc(nb_sj);
	gsl_vector *payoff_up_final = gsl_vector_calloc(nb_sj);
	gsl_vector *payoff_down_anticipated = gsl_vector_calloc(nb_sj);
	gsl_vector *payoff_down_final = gsl_vector_calloc(nb_sj);

	//Initialisation du générateur aléatoire
	gsl_rng *rng = gsl_rng_alloc(gsl_rng_default);
	gsl_rng_set(rng, (unsigned long int)time(NULL));

	//Copie d'historic dans le d�but du path
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

	//Initialisation valeurs interm�diaires du calcul du delta
	double s_temp;
	gsl_matrix_get_col(epsilon, &historic, last_index - 1);
	gsl_vector_scale(epsilon, 0.1);

	//Simulations de Monte-Carlo
	for (int i = 0; i < nbSimu; i++){
		gsl_matrix_set_col(path, last_index - 1, temp); // WTFFFFFFFFFFFFF ?????????
		//Remise � sa valeur initiale de la valeur actuelle des sous-jacents
		simulations::simulate_n_sj(*path, last_index, nb_day_after, expected_returns, vol, correl, rng);
		gsl_matrix_memcpy(path_up, path);
		gsl_matrix_memcpy(path_down, path);

		for (int sj = 0; sj < nb_sj; sj++)
		{
			for (int t = last_index; t < (int)path->size2; t++)
			{
				s_temp = gsl_matrix_get(path, sj, t);
				gsl_matrix_set(path_up, sj, t, (s_temp + gsl_vector_get(epsilon, sj)));
				gsl_matrix_set(path_down, sj, t, (s_temp - gsl_vector_get(epsilon, sj)));
			}

			//Valeurs pour le delta_up
			phi = Everglades::get_payoff(*path_up, VLR, anticipated);
			if (anticipated) {
				s_temp = gsl_vector_get(payoff_up_anticipated, sj);
				gsl_vector_set(payoff_up_anticipated, sj, s_temp + phi);
			}
			else {
				s_temp = gsl_vector_get(payoff_up_final, sj);
				gsl_vector_set(payoff_up_final, sj, s_temp + phi);
			}

			//Valeurs pour le delta_down
			phi = Everglades::get_payoff(*path_down, VLR, anticipated);
			if (anticipated) {
				s_temp = gsl_vector_get(payoff_down_anticipated, sj);
				gsl_vector_set(payoff_down_anticipated, sj, s_temp + phi);
			}
			else {
				s_temp = gsl_vector_get(payoff_down_final, sj);
				gsl_vector_set(payoff_down_final, sj, s_temp + phi);
			}
		}

		//Valeurs pour le prix
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

	//Calcul des deltas
	gsl_vector_scale(payoff_up_anticipated, exp(-r*((9 - last_index) * PERIODE - nb_day_after) / DAY));
	gsl_vector_scale(payoff_up_final, exp(-r*((25 - last_index) * PERIODE - nb_day_after) / DAY));
	gsl_vector_scale(payoff_down_anticipated, exp(-r*((9 - last_index) * PERIODE - nb_day_after) / DAY));
	gsl_vector_scale(payoff_down_final, exp(-r*((25 - last_index) * PERIODE - nb_day_after) / DAY));
	gsl_vector_add(*delta, payoff_up_anticipated);
	gsl_vector_add(*delta, payoff_up_final);
	gsl_vector_sub(*delta, payoff_down_anticipated);
	gsl_vector_sub(*delta, payoff_down_final);
	gsl_vector_scale(*delta, 0.5 / (nbSimu));
	gsl_vector_div(*delta, epsilon);

	//calcul de l'intervalle de confiance
	double sumIc = sumIc_anticipated*exp(-2 * r*(9 - last_index) * (PERIODE - nb_day_after) / DAY);
	sumIc += sumIc_final*exp(-2 * r*(25 - last_index + 1) * (PERIODE - nb_day_after) / DAY);
	double xi2 = sumIc / ((double)nbSimu) - (price)*(price);
	ic = 1.96*sqrt(xi2) / sqrt((double)nbSimu);

	//lib�ration m�moire
	gsl_matrix_free(path_up);
	gsl_matrix_free(path_down);
	gsl_vector_free(temp);
	gsl_matrix_free(path);
	gsl_rng_free(rng);
	gsl_vector_free(epsilon);
	gsl_vector_free(payoff_up_anticipated);
	gsl_vector_free(payoff_up_final);
	gsl_vector_free(payoff_down_anticipated);
	gsl_vector_free(payoff_down_final);
	return 0;
}

/*extern int Everglades::get_price_multi_curency(double& price, double& ic, gsl_vector** delta, const gsl_matrix& historic,
int nb_day_after, const gsl_vector& rates, const gsl_vector& vol, const gsl_matrix& correl, int nbSimu){


int nb_sj = historic.size1;
*delta = gsl_vector_calloc(nb_sj);

int last_index = historic.size2;
gsl_matrix *path = gsl_matrix_calloc(historic.size1, 25);
gsl_vector *temp = gsl_vector_calloc(historic.size1);
gsl_rng *rng = gsl_rng_alloc(gsl_rng_default);
gsl_rng_set(rng, (unsigned long int)time(NULL));

//Copie d'historic dans le d�but du path
for (int i = 0; i < last_index; i++) {
gsl_matrix_get_col(temp, &historic, i);
gsl_matrix_set_col(path, i, temp);
}

//Initialisation valeurs interm�diaires du calcul du prix et de l'intervalle de confiance
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

double epsilon;

//Simulations de Monte-Carlo
for (int i = 0; i < nbSimu; i++){
gsl_matrix_set_col(path, last_index - 1, temp); //Remise � sa valeur initiale de la valeur actuelle des sous-jacents

simulations::simulate_n_sj(*path, last_index, nb_day_after, expected_returns, vol, correl, rng);

for (int sj = 0; sj < nb_sj; sj++)
{
gsl_matrix* path_up = gsl_matrix_alloc(path->size1, path->size2);
gsl_matrix_memcpy(path_up, path);

gsl_matrix* path_down = gsl_matrix_alloc(path->size1, path->size2);
gsl_matrix_memcpy(path_down, path);


epsilon = 0.1 * gsl_matrix_get(path, sj, last_index);

for (int t = last_index; t < (int)path->size2; t++)
{
s_temp = gsl_matrix_get(path, sj, t);
gsl_matrix_set(path_up, sj, t, (s_temp + epsilon));
gsl_matrix_set(path_down, sj, t, (s_temp - epsilon));
}

payoff_up = Everglades::get_payoff(*path_up, VLR, anticipated);

if (anticipated) {
payoff_up *= exp(-r*((9 - last_index) * PERIODE - nb_day_after) / DAY);
}
else {
payoff_up *= exp(-r*((25 - last_index) * PERIODE - nb_day_after) / DAY);
}
payoff_up /= ((double)nbSimu);

payoff_down = Everglades::get_payoff(*path_down, VLR, anticipated);

if (anticipated) {
payoff_down *= exp(-r*((9 - last_index) * PERIODE - nb_day_after) / DAY);
}
else {
payoff_down *= exp(-r*(25 - last_index + 1) * (PERIODE - nb_day_after) / DAY);
}
payoff_down /= ((double)nbSimu);

delta_temp = gsl_vector_get(*delta, sj);
gsl_vector_set(*delta, sj, delta_temp + (payoff_up - payoff_down) / (2.0 * epsilon));

gsl_matrix_free(path_up);
gsl_matrix_free(path_down);
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
sumIc += sumIc_final*exp(-2 * r*(25 - last_index + 1) * (PERIODE - nb_day_after) / DAY);
double xi2 = sumIc / ((double)nbSimu) - (price)*(price);
ic = 1.96*sqrt(xi2) / sqrt((double)nbSimu);

//lib�ration m�moire
gsl_vector_free(temp);
gsl_matrix_free(path);
gsl_rng_free(rng);
return 0;
}*/
