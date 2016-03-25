#include <time.h>
#include <iostream>
#include <cmath>

#include "everglades.hpp"
#include "simulations.hpp"


#ifndef __max
	#define __max(a,b) \
		({ __typeof__ (a) _a = (a); \
			__typeof__ (b) _b = (b); \
			_a > _b ? _a : _b; })
#endif

using namespace std;

extern int Everglades::get_payoff(double &res, const gsl_matrix &path, double vlr, bool &anticipated) {
	if (vlr < 0) {
		return 14;
	}
	if (path.size1 == 0 || path.size1 == 0){
		return 30;
	}
	for (int i = 0; i < (int)path.size1; i++)
	{
		if (gsl_matrix_get(&path, i, 0) <= 0)
		{
			return 13;
		}
	}
	for (int j = 0; j < (int)path.size2; j++)
	{
		for (int i = 0; i < (int)path.size1; i++)
		{
			if (gsl_matrix_get(&path, i, j) < 0)
			{
				return 15;
			}
		}
	}
	res = Everglades::get_payoff_fast(path, vlr, anticipated);
	return 0;
}

double Everglades::get_payoff_fast(const gsl_matrix &path, double vlr, bool &anticipated)
{
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

/*
gsl_vector* simulate_brownian_integral(struct Params data, gsl_rng* rng)
{
double W_temp = 0;

gsl_vector* brownian = gsl_vector_alloc(data.J + 1);
gsl_vector_set(brownian, 0, 0);

for (int i = 1; i <= data.J; i++)
{
W_temp += sqrt((data.T / data.J)) * gsl_rng_normal(rng);
gsl_vector_set(brownian, i, W_temp);
}

return brownian;
}

double compute_integral(struct Params data, gsl_rng* rng)
{
gsl_vector* simulations = gsl_vector_alloc(data.J + 1);
gsl_vector* brownian = simulate_brownian_integral(data, rng);

double St = data.S;
gsl_vector_set(simulations, 0, St);

for (int i = 1; i <= data.J; i++)
{
St = data.S * exp((data.r - data.v*data.v / 2) * (i* data.T / data.J) + data.v * gsl_vector_get(brownian, i));
gsl_vector_set(simulations, i, St);
}

return simulations;
}


double calculate_integral(gsl_vector* simulations, struct Params data)
{
double sum = gsl_vector_get(simulations, 0) / 2.0;
double S_temp;

for (int i = 1; i < data.J; i++)
{
S_temp = gsl_vector_get(simulations, i);
sum += S_temp;
}

sum += gsl_vector_get(simulations, data.J) / 2.0;

sum = sum/data.J - data.K;

if (sum > 0)
{
return sum;
}

return 0;
}

double calculate_controle(const gsl_matrix &simulations, struct Params data)
{
double result;
double sum = log(gsl_vector_get(simulations, 0)) / 2.0;
double S_temp;

for (int i = 1; i < data.J; i++)
{
S_temp = gsl_vector_get(simulations, i);
sum += log(S_temp);
}

sum += log(gsl_vector_get(simulations, data.J)) / 2.0;
sum /= data.J;
result = exp(sum) - data.K;

if (result > 0)
{
return exp(-data.r*data.T)*result;
}
return 0;
}

void Computations::option_asian_controle(double &ic, double &prix, int nb_samples, double T,
double S0, double K, double sigma, double r, double J)
{
struct Params data;
data.M = nb_samples;
data.S = S0;
data.K = K;
data.r = r;
data.v = sigma;
data.T = T;
data.J = J;

double sum = 0;
double var = 0;

double payoff;

PnlRng *rng = pnl_rng_create(PNL_RNG_MERSENNE);
pnl_rng_sseed(rng, 0);

gsl_vector* simulations;

double temp;

double d = sqrt(3.0 / data.T) * (log(data.K / data.S) - (data.r - data.v * data.v / 2.0)* (data.T /2.0)) / data.v;
double esperance_controle2 = exp(-data.r*data.T) * (-data.K * cdf_nor(-d) + data.S*exp((data.r - data.v * data.v / 6)*T/2.0)*cdf_nor(-d + data.v*sqrt(data.T / 3.0)));

for (int i = 0; i < nb_samples; i++)
{
simulations = simulate_sj_integral(data, rng);

payoff = calculate_integral(simulations, data);
temp = exp(-r*T) * payoff - calculate_controle_2(simulations, data) + esperance_controle2;
sum += temp;
var += temp * temp;
}



prix = sum / nb_samples;
var = var / nb_samples - prix * prix;
ic = 1.96 * sqrt(var / nb_samples);
pnl_rng_free(&rng);
}
*/

extern int Everglades::get_price(double& price, double& ic, bool& is_anticipated, gsl_vector& delta, const gsl_matrix& historic,
	int nb_day_after, double r, const gsl_vector& vol, const gsl_matrix& correl, int nbSimu)
{
	if ((nb_day_after < 0) || (nb_day_after > PERIODE)){
		return 12;
	}
	if (nbSimu < 0) {
		return 30;
	}
	if (correl.size1 != correl.size2) {
		return 1;
	}
	if (vol.size != correl.size1 || delta.size != correl.size1 || historic.size1 != correl.size1) {
		return 9;
	}
	for (int i = 0; i < (int)historic.size1; i++)
	{
		if (gsl_matrix_get(&historic, i, 0) <= 0)
		{
			return 13;
		}
	}
	for (int j = 0; j < (int)historic.size2; j++)
	{
		for (int i = 0; i < (int)historic.size1; i++)
		{
			if (gsl_matrix_get(&historic, i, j) < 0)
			{
				return 15;
			}
		}
	}
	for (int i = 0; i < (int)vol.size; i++)
	{
		if (gsl_vector_get(&vol, i) <= 0)
		{
			return 31;
		}
	}
	double eps = pow(nbSimu, -0.2);
	//Récupération des tailles
	int nb_sj = historic.size1;
	int last_index = historic.size2;
	int nb_dates = 25;

	//Allocation des vecteurs itermédiaires de calculs
	gsl_vector *expected_returns = gsl_vector_alloc(nb_sj);
	gsl_matrix *path = gsl_matrix_alloc(nb_sj, nb_dates);
	gsl_matrix *path_up = gsl_matrix_alloc(nb_sj, nb_dates);
	gsl_matrix *path_down = gsl_matrix_alloc(nb_sj, nb_dates);
	gsl_vector *temp = gsl_vector_alloc(nb_sj);
	//gsl_vector *epsilon = gsl_vector_alloc(nb_sj);
	gsl_vector *payoff_up_anticipated = gsl_vector_calloc(nb_sj);
	gsl_vector *payoff_up_final = gsl_vector_calloc(nb_sj);
	gsl_vector *payoff_down_anticipated = gsl_vector_calloc(nb_sj);
	gsl_vector *payoff_down_final = gsl_vector_calloc(nb_sj);

	//Initialisation du générateur aléatoire
	gsl_rng *rng = gsl_rng_alloc(gsl_rng_default);
	gsl_rng_set(rng, (unsigned long int)time(NULL));

	//Initialisation des expected returns
	gsl_vector_set_all(expected_returns, r);


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

	//Si on est à la maturité
	if (last_index == 25)
	{
		price = Everglades::get_payoff_fast(*path, VLR, is_anticipated);
		ic = 0;
		return 0;
	}

	//Si la date d'exercice anticipé est passé, on vérifie si il a eu lieu ou non
	if (last_index > 8 || (last_index == 8 && nb_day_after == 0))
	{
		Everglades::get_payoff_fast(*path, VLR, is_anticipated);
	}
	//Initialisation valeurs interm�diaires du calcul du delta
	double s_temp;

	//Simulations de Monte-Carlo
	for (int i = 0; i < nbSimu; i++){
		if (nb_day_after >0) {
			gsl_matrix_set_col(path, last_index - 1, temp); //Remise � sa valeur initiale de la valeur actuelle des sous-jacents
		}

		simulations::simulate_n_sj(*path, last_index, nb_day_after, *expected_returns, vol, correl, rng);

		for (int sj = 0; sj < nb_sj; sj++)
		{
			gsl_matrix_memcpy(path_up, path);
			gsl_matrix_memcpy(path_down, path);

			for (int t = last_index - 1; t < (int)path->size2; t++)
			{
				if ((t == last_index - 1) && nb_day_after == 0) {
					// si on est à une date de constatition, on ne bump pas le prix d'aujourd'hui
					continue;
				}
				s_temp = gsl_matrix_get(path, sj, t);
				gsl_matrix_set(path_up, sj, t, (s_temp*(1 + eps)));
				gsl_matrix_set(path_down, sj, t, (s_temp*(1 - eps)));
			}

			//Valeurs pour le delta_up
			phi = Everglades::get_payoff_fast(*path_up, VLR, anticipated);
			if (anticipated) {
				s_temp = gsl_vector_get(payoff_up_anticipated, sj);
				gsl_vector_set(payoff_up_anticipated, sj, s_temp + phi);
			}
			else {
				s_temp = gsl_vector_get(payoff_up_final, sj);
				gsl_vector_set(payoff_up_final, sj, s_temp + phi);
			}


			//Valeurs pour le delta_down
			phi = Everglades::get_payoff_fast(*path_down, VLR, anticipated);
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
		phi = Everglades::get_payoff_fast(*path, VLR, anticipated);
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
	double is_constatation_date = 0.0;
	if (nb_day_after == 0) {
		is_constatation_date = 1;
	}
	price = 0;
	price += sumPayoff_anticipated*exp(-r*((10 - last_index - is_constatation_date) * PERIODE - nb_day_after) / DAY);
	price += sumPayoff_final*exp(-r*((26 - last_index - is_constatation_date) * PERIODE - nb_day_after) / DAY);
	price /= ((double)nbSimu);

	//Calcul des deltas
	gsl_vector_set_all(&delta, 0); //remise à zéro
	gsl_vector_scale(payoff_up_anticipated, exp(-r*((10 - last_index - is_constatation_date) * PERIODE - nb_day_after) / DAY));
	gsl_vector_scale(payoff_up_final, exp(-r*((26 - last_index - is_constatation_date) * PERIODE - nb_day_after) / DAY));
	gsl_vector_scale(payoff_down_anticipated, exp(-r*((10 - last_index - is_constatation_date) * PERIODE - nb_day_after) / DAY));
	gsl_vector_scale(payoff_down_final, exp(-r*((26 - last_index - is_constatation_date) * PERIODE - nb_day_after) / DAY));
	gsl_vector_add(&delta, payoff_up_anticipated);
	gsl_vector_add(&delta, payoff_up_final);
	gsl_vector_sub(&delta, payoff_down_anticipated);
	gsl_vector_sub(&delta, payoff_down_final);
	gsl_vector_scale(&delta, 1 / (2 * nbSimu*eps));
	gsl_vector_div(&delta, temp);

	//calcul de l'intervalle de confiance
	double sumIc = sumIc_anticipated*exp(-2 * r * ((10 - last_index - is_constatation_date) * PERIODE - nb_day_after) / DAY);
	sumIc += sumIc_final*exp(-2 * r * ((26 - last_index - is_constatation_date) * PERIODE - nb_day_after) / DAY);
	double xi2 = sumIc / ((double)nbSimu) - (price)*(price);
	ic = 1.96*sqrt(xi2) / sqrt((double)nbSimu);

	//lib�ration m�moire
	gsl_vector_free(expected_returns);
	gsl_matrix_free(path_up);
	gsl_matrix_free(path_down);
	gsl_vector_free(temp);
	gsl_matrix_free(path);
	gsl_rng_free(rng);
	gsl_vector_free(payoff_up_anticipated);
	gsl_vector_free(payoff_up_final);
	gsl_vector_free(payoff_down_anticipated);
	gsl_vector_free(payoff_down_final);
	return 0;
}

extern int Everglades::get_price_with_forex(double& price, double& ic, bool& is_anticipated, gsl_vector& delta, const gsl_matrix& historic, int nb_day_after, double r,
	const gsl_vector& foreign_rates, const gsl_vector& currency, const gsl_vector& vol, const gsl_matrix& correl, int nbSimu, gsl_rng *rng)
{
	if ((nb_day_after < 0) || (nb_day_after > PERIODE)){
		return 12;
	}
	if (nbSimu < 0) {
		return 30;
	}
	if (correl.size1 != correl.size2) {
		return 1;
	}
	if (vol.size != correl.size1 || delta.size != correl.size1 || historic.size1 != correl.size1 || historic.size1 != foreign_rates.size + currency.size) {
		return 9;
	}
	for (int i = 0; i < (int)historic.size1; i++)
	{
		if (gsl_matrix_get(&historic, i, 0) <= 0)
		{
			return 13;
		}
	}
	for (int j = 0; j < (int)historic.size2; j++)
	{
		for (int i = 0; i < (int)historic.size1; i++)
		{
			if (gsl_matrix_get(&historic, i, j) < 0)
			{
				return 15;
			}
		}
	}
	for (int i = 0; i < (int)vol.size; i++)
	{
		if (gsl_vector_get(&vol, i) <= 0)
		{
			return 31;
		}
	}


	double eps = pow(nbSimu, -0.2);

	//Récupération des tailles
	int nb_assets = historic.size1; //nombre d'action du produit + nombre de monnaie étrangère
	int nb_sj = currency.size; //nombre d'action du produit
	int last_index = historic.size2; //indice du dernier prix
	int nb_dates = 25; //nombre de dates de constatations
	is_anticipated = false;

	//Allocation des vecteurs itermédiaires de calculs
	gsl_vector *expected_returns = gsl_vector_alloc(nb_assets);
	gsl_matrix *path = gsl_matrix_alloc(nb_assets, nb_dates);
	gsl_matrix *path_up = gsl_matrix_alloc(nb_assets, nb_dates);
	gsl_matrix *path_down = gsl_matrix_alloc(nb_assets, nb_dates);
	gsl_vector *temp = gsl_vector_alloc(nb_assets);
	gsl_vector *temp2 = gsl_vector_alloc(nb_dates);
	gsl_vector *payoff_up_anticipated = gsl_vector_calloc(nb_assets);
	gsl_vector *payoff_up_final = gsl_vector_calloc(nb_assets);
	gsl_vector *payoff_down_anticipated = gsl_vector_calloc(nb_assets);
	gsl_vector *payoff_down_final = gsl_vector_calloc(nb_assets);
	gsl_matrix *path_for_payoff = gsl_matrix_alloc(nb_sj, nb_dates);

	//Initialisation valeurs interm�diaires du calcul du prix et de l'intervalle de confiance
	double sumPayoff_anticipated = 0.0;
	double sumIc_anticipated = 0.0;
	double sumPayoff_final = 0.0;
	double sumIc_final = 0.0;
	double phi;
	bool anticipated; //sera mis à vrai quand l'exercice anticipé a lieu dans le tour de boucle
	double s_temp;
	double s_temp2;

	//Initialisation du générateur aléatoire si NULL
	bool rngIsMine;
	if (rng == NULL) {
		rngIsMine = true;
		rng = gsl_rng_alloc(gsl_rng_default);
		gsl_rng_set(rng, (unsigned long int)time(NULL));
	}
	else {
		rngIsMine = false;
	}

	//Copie d'historic dans le d�but du path
	for (int i = 0; i < last_index; i++) {
		gsl_matrix_get_col(temp, &historic, i);
		gsl_matrix_set_col(path, i, temp);
	}

	//Calcul des expected returns
	for (int asset = 0; asset < nb_sj; asset++) {
		//Pour les actions (prix converti en euros)
		gsl_vector_set(expected_returns, asset, r);
	}
	for (int asset = nb_sj; asset < nb_assets; asset++) {
		//Pour les taux de change
		gsl_vector_set(expected_returns, asset, r - gsl_vector_get(&foreign_rates, asset - nb_sj));
	}

	//Création du path_for_payoff qui contient le prix des actions dans leurs monnaies d'émission
	for (int action = 0; action < nb_sj; action++){
		if (gsl_vector_get(&currency, action) == 0) {
			//si l'action est cotée en euro : pas de modifications des prix
			gsl_matrix_get_row(temp2, path, action);
			gsl_matrix_set_row(path_for_payoff, action, temp2);
		}
		else {
			for (int t = 0; t < last_index; t++) {
				//On divise les prix par le taux de change approprié
				s_temp = gsl_matrix_get(path, action, t);
				s_temp2 = gsl_matrix_get(path, nb_sj + gsl_vector_get(&currency, action) - 1, t);
				gsl_matrix_set(path_for_payoff, action, t, s_temp / s_temp2);
			}
		}
	}

	//Si on est à la maturité
	if (last_index == 25)
	{
		price = Everglades::get_payoff_fast(*path_for_payoff, VLR, is_anticipated);
		ic = 0;
		return 0;
	}

	//Si la date d'exercice anticipé est passé, on vérifie si il a eu lieu ou non
	if (last_index > 8 || (last_index == 8 && nb_day_after == 0))
	{
		Everglades::get_payoff_fast(*path_for_payoff, VLR, is_anticipated);
	}

	//Simulations de Monte-Carlo
	for (int i = 0; i < nbSimu; i++){
		if (nb_day_after >0) {
			gsl_matrix_set_col(path, last_index - 1, temp); //Remise � sa valeur initiale de la valeur actuelle des sous-jacents
		}
		simulations::simulate_n_sj(*path, last_index, nb_day_after, *expected_returns, vol, correl, rng);
		for (int sj = 0; sj < nb_assets; sj++)
		{
			gsl_matrix_memcpy(path_up, path);
			gsl_matrix_memcpy(path_down, path);

			//On bump la courbe
			for (int t = last_index - 1; t < nb_dates; t++)
			{
				if ((t == last_index - 1) && nb_day_after == 0) {
					// si on est à une date de constatition, on ne bump pas le prix d'aujourd'hui
					continue;
				}
				s_temp = gsl_matrix_get(path, sj, t);
				gsl_matrix_set(path_up, sj, t, (s_temp*(1 + eps)));
				gsl_matrix_set(path_down, sj, t, (s_temp*(1 - eps)));
			}

			//Remplissage du path_for_payoff pour le delta_up
			for (int action = 0; action < nb_sj; action++){
				if (gsl_vector_get(&currency, action) == 0) {
					gsl_matrix_get_row(temp2, path_up, action);
					gsl_matrix_set_row(path_for_payoff, action, temp2);
				}
				else {
					for (int t = last_index - 1; t < nb_dates; t++) {
						s_temp = gsl_matrix_get(path_up, action, t);
						s_temp2 = gsl_matrix_get(path_up, nb_sj + gsl_vector_get(&currency, action) - 1, t);
						gsl_matrix_set(path_for_payoff, action, t, s_temp / s_temp2);
					}
				}
			}

			//valeur pour le delta_up
			phi = Everglades::get_payoff_fast(*path_for_payoff, VLR, anticipated);
			if (anticipated) {
				s_temp = gsl_vector_get(payoff_up_anticipated, sj);
				gsl_vector_set(payoff_up_anticipated, sj, s_temp + phi);
			}
			else {
				s_temp = gsl_vector_get(payoff_up_final, sj);
				gsl_vector_set(payoff_up_final, sj, s_temp + phi);
			}

			//Remplissage du path_for_payoff pour le delta_down
			for (int action = 0; action < nb_sj; action++){
				if (gsl_vector_get(&currency, action) == 0) {
					gsl_matrix_get_row(temp2, path_down, action);
					gsl_matrix_set_row(path_for_payoff, action, temp2);
				}
				else {
					for (int t = last_index - 1; t < nb_dates; t++) {
						s_temp = gsl_matrix_get(path_down, action, t);
						s_temp2 = gsl_matrix_get(path_down, nb_sj + gsl_vector_get(&currency, action) - 1, t);
						gsl_matrix_set(path_for_payoff, action, t, s_temp / s_temp2);
					}
				}
			}

			//Valeurs pour le delta_down
			phi = Everglades::get_payoff_fast(*path_for_payoff, VLR, anticipated);
			if (anticipated) {
				s_temp = gsl_vector_get(payoff_down_anticipated, sj);
				gsl_vector_set(payoff_down_anticipated, sj, s_temp + phi);
			}
			else {
				s_temp = gsl_vector_get(payoff_down_final, sj);
				gsl_vector_set(payoff_down_final, sj, s_temp + phi);
			}

		}

		//Remplissage du path_for_payoff pour le prix
		for (int action = 0; action < nb_sj; action++){
			if (gsl_vector_get(&currency, action) == 0) {
				gsl_matrix_get_row(temp2, path, action);
				gsl_matrix_set_row(path_for_payoff, action, temp2);
			}
			else {
				for (int t = last_index - 1; t < nb_dates; t++) {
					s_temp = gsl_matrix_get(path, action, t);
					s_temp2 = gsl_matrix_get(path, nb_sj + gsl_vector_get(&currency, action) - 1, t);
					gsl_matrix_set(path_for_payoff, action, t, s_temp / s_temp2);
				}
			}
		}

		//Valeurs pour le prix
		phi = Everglades::get_payoff_fast(*path_for_payoff, VLR, anticipated);
		if (anticipated) {
			sumPayoff_anticipated += phi;
			sumIc_anticipated += phi*phi;
		}
		else {
			sumPayoff_final += phi;
			sumIc_final += phi*phi;
		}
	}
	double is_constatation_date = 0.0;
	if (nb_day_after == 0) {
		is_constatation_date = 1;
	}
	//calcul du prix
	price = 0;
	price += sumPayoff_anticipated*exp(-r*((10 - last_index - is_constatation_date) * PERIODE - nb_day_after) / DAY);
	price += sumPayoff_final*exp(-r*((26 - last_index - is_constatation_date) * PERIODE - nb_day_after) / DAY);
	price /= ((double)nbSimu);

	//Calcul des deltas
	gsl_vector_set_all(&delta, 0); //remise à zéro
	gsl_vector_scale(payoff_up_anticipated, exp(-r*((10 - last_index - is_constatation_date) * PERIODE - nb_day_after) / DAY));
	gsl_vector_scale(payoff_up_final, exp(-r*((26 - last_index - is_constatation_date) * PERIODE - nb_day_after) / DAY));
	gsl_vector_scale(payoff_down_anticipated, exp(-r*((10 - last_index - is_constatation_date) * PERIODE - nb_day_after) / DAY));
	gsl_vector_scale(payoff_down_final, exp(-r*((26 - last_index - is_constatation_date) * PERIODE - nb_day_after) / DAY));
	gsl_vector_add(&delta, payoff_up_anticipated);
	gsl_vector_add(&delta, payoff_up_final);
	gsl_vector_sub(&delta, payoff_down_anticipated);
	gsl_vector_sub(&delta, payoff_down_final);
	gsl_vector_scale(&delta, 1 / (2 * nbSimu*eps));
	gsl_vector_div(&delta, temp);

	//calcul de l'intervalle de confiance
	double sumIc = sumIc_anticipated*exp(-2 * r *((10 - last_index - is_constatation_date) * PERIODE - nb_day_after) / DAY);
	sumIc += sumIc_final*exp(-2 * r *((26 - last_index - is_constatation_date) * PERIODE - nb_day_after) / DAY);
	double xi2 = sumIc / ((double)nbSimu) - (price)*(price);
	ic = 1.96*sqrt(xi2) / sqrt((double)nbSimu);

	//lib�ration m�moire
	gsl_vector_free(expected_returns);
	gsl_matrix_free(path_up);
	gsl_matrix_free(path_down);
	gsl_vector_free(temp);
	gsl_vector_free(temp2);
	gsl_matrix_free(path);
	if (rngIsMine) {
		gsl_rng_free(rng);
	}
	gsl_vector_free(payoff_up_anticipated);
	gsl_vector_free(payoff_up_final);
	gsl_vector_free(payoff_down_anticipated);
	gsl_vector_free(payoff_down_final);
	gsl_matrix_free(path_for_payoff);
	return 0;
}