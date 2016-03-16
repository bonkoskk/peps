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


	double eps = 0.1;

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
        //gsl_vector *epsilon = gsl_vector_alloc(nb_sj);
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

	//gsl_matrix_get_col(epsilon, &path, last_index-1);
    //gsl_vector_scale(epsilon, 0.1); 
                
	//Initialisation valeurs interm�diaires du calcul du prix et de l'intervalle de confiance
	double sumPayoff_anticipated = 0.0;
	double sumIc_anticipated = 0.0;
	double sumPayoff_final = 0.0;
	double sumIc_final = 0.0;
	double phi;
	bool anticipated;

        //Initialisation valeurs interm�diaires du calcul du delta
	double s_temp;
	       
        //Simulations de Monte-Carlo
	for (int i = 0; i < nbSimu; i++){
		gsl_matrix_set_col(path, last_index - 1, temp); //Remise � sa valeur initiale de la valeur actuelle des sous-jacents
		simulations::simulate_n_sj(*path, last_index, nb_day_after, expected_returns, vol, correl, rng);
        
		for (int sj = 0; sj < nb_sj; sj++)
		{
					gsl_matrix_memcpy(path_up, path);
					gsl_matrix_memcpy(path_down, path);

                    for (int t = last_index - 1; t < (int)path->size2; t++)
                    {
                    	if (t == 0) {
                    		continue;
                    	}
                            s_temp = gsl_matrix_get(path, sj, t);
                            gsl_matrix_set(path_up, sj, t, (s_temp*(1+eps)));
                            gsl_matrix_set(path_down, sj, t, (s_temp*(1-eps)));
                    }

                    //Valeurs pour le delta_up
                    phi = Everglades::get_payoff(*path_up, VLR, anticipated);
                    if (anticipated) {
                        s_temp = gsl_vector_get(payoff_up_anticipated, sj);
                        gsl_vector_set(payoff_up_anticipated, sj, s_temp+phi);
                    }
                    else {
                        s_temp = gsl_vector_get(payoff_up_final, sj);
                        gsl_vector_set(payoff_up_final, sj, s_temp+phi);
                    }


                    //Valeurs pour le delta_down
                    phi = Everglades::get_payoff(*path_down, VLR, anticipated);
                    if (anticipated) {
                        s_temp = gsl_vector_get(payoff_down_anticipated, sj);
                        gsl_vector_set(payoff_down_anticipated, sj, s_temp+phi);
                    }
                    else {
                        s_temp = gsl_vector_get(payoff_down_final, sj);
                        gsl_vector_set(payoff_down_final, sj, s_temp+phi);
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
        gsl_vector_scale(*delta, 1/(2*nbSimu*eps));
        gsl_vector_div(*delta, temp);

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
        //gsl_vector_free(epsilon);
        gsl_vector_free(payoff_up_anticipated);
        gsl_vector_free(payoff_up_final);
	gsl_vector_free(payoff_down_anticipated);
        gsl_vector_free(payoff_down_final);
	return 0;
}

extern int Everglades::get_price_with_forex(double& price, double& ic, gsl_vector** delta, const gsl_matrix& historic, int nb_day_after, double r,
	const gsl_vector& foreign_rates, const gsl_vector& currency, const gsl_vector& vol, const gsl_matrix& correl, int nbSimu){

	double eps = pow(nbSimu, -0.2);
	//Récupération des tailles
	int nb_assets = historic.size1; //nombre d'action du produit + nombre de monnaie étrangère
	int nb_sj = currency.size; //nombre d'action du produit
	int last_index = historic.size2; //indice du dernier prix
	int nb_dates = 25; //nombre de dates de constatations


	//Allocation des vecteurs itermédiaires de calculs
	*delta = gsl_vector_calloc(nb_assets);
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

	//Initialisation du générateur aléatoire
	gsl_rng *rng = gsl_rng_alloc(gsl_rng_default);
	gsl_rng_set(rng, (unsigned long int)time(NULL));

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

	//Simulations de Monte-Carlo
	for (int i = 0; i < nbSimu; i++){
		gsl_matrix_set_col(path, last_index - 1, temp); //Remise � sa valeur initiale des prix courants des sous-jacents
		simulations::simulate_n_sj(*path, last_index, nb_day_after, *expected_returns, vol, correl, rng);
		for (int sj = 0; sj < nb_assets; sj++)
		{
			gsl_matrix_memcpy(path_up, path);
			gsl_matrix_memcpy(path_down, path);

			//On bump la courbe
			for (int t = last_index - 1; t < nb_dates; t++)
			{
				if (t == 0) {
					// si on est à la date d'émission on ne bump pas le prix d'aujourd'hui
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
			phi = Everglades::get_payoff(*path_for_payoff, VLR, anticipated);
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
			phi = Everglades::get_payoff(*path_for_payoff, VLR, anticipated);
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
		phi = Everglades::get_payoff(*path_for_payoff, VLR, anticipated);
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
	gsl_vector_scale(*delta, 1 / (2 * nbSimu*eps));
	gsl_vector_div(*delta, temp);

	/*
	for (int action = 0; action < nb_sj; action++){
		if (gsl_vector_get(&currency, action) == 0) {
			continue;
		}
		else {
			s_temp = gsl_vector_get(temp, nb_sj + gsl_vector_get(&currency, action) - 1);
			s_temp2 = gsl_vector_get(*delta, action);
			gsl_vector_set(*delta, action, s_temp*s_temp2);
		}
	}
	*/
	//calcul de l'intervalle de confiance
	double sumIc = sumIc_anticipated*exp(-2 * r*(9 - last_index) * (PERIODE - nb_day_after) / DAY);
	sumIc += sumIc_final*exp(-2 * r*(25 - last_index + 1) * (PERIODE - nb_day_after) / DAY);
	double xi2 = sumIc / ((double)nbSimu) - (price)*(price);
	ic = 1.96*sqrt(xi2) / sqrt((double)nbSimu);

	//lib�ration m�moire
	gsl_vector_free(expected_returns);
	gsl_matrix_free(path_up);
	gsl_matrix_free(path_down);
	gsl_vector_free(temp);
	gsl_vector_free(temp2);
	gsl_matrix_free(path);
	gsl_rng_free(rng);
	gsl_vector_free(payoff_up_anticipated);
	gsl_vector_free(payoff_up_final);
	gsl_vector_free(payoff_down_anticipated);
	gsl_vector_free(payoff_down_final);
	gsl_matrix_free(path_for_payoff);
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