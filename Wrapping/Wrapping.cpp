// Il s'agit du fichier DLL principal.

#include "Wrapping.hpp"
#include "pricer.hpp"
#include "everglades.hpp"
#include <iostream>



using namespace Pricer;

namespace Wrapping {
	void WrapperAmerican::getPricePutAmerican(double S0, double K, double T, double R, double vol, int N) {
		double px;
		put_american(px, S0, K, T, R, vol, N);
		this->price = px;
	}

	void WrapperBarrier::getPriceCallBarrierDownOut(int sampleNb, double T, double S0, double K, double sigma, double r, double J, double L) {
		double ic, px;
		call_barrier_down_out(ic, px, sampleNb, T, S0, K, sigma, r, J, L);
		this->confidenceInterval = ic;
		this->price = px;
	}

	void WrapperQuanto::getPriceCallQuanto(double S, double Q, double K, double R, double Rf, double sigma1, double sigma2, double rho, double tau) {
		double px;
		call_quanto(px, S, Q, K, R, Rf, sigma1, sigma2, rho, tau);
		this->confidenceInterval = 0;
		this->price = px;
	}

	void WrapperQuanto::getPricePutQuanto(double S, double Q, double K, double R, double Rf, double sigma1, double sigma2, double rho, double tau) {
		double px = 1;
		put_quanto(px, S, Q, K, R, Rf, sigma1, sigma2, rho, tau);
		this->confidenceInterval = 0;
	}

	void WrapperVanilla::getPriceOptionEuropeanCall(double T, double S0, double K, double sigma, double r, double q) {
		double px, delta;
		call_vanilla(px, T, S0, K, sigma, r, q);
		this->price = px;
		call_vanilla_delta(delta, T, S0, K, sigma, r, q);
		this->delta = delta;
	}

	void WrapperVanilla::getPriceOptionEuropeanPut(double T, double S0, double K, double sigma, double r, double q) {
		double px;
		put_vanilla(px, T, S0, K, sigma, r, q);
		this->price = px;
	}

	void WrapperVanilla::getPriceOptionEuropeanCallMC(int M, double T, double S0, double K, double sigma, double r, double q){
		
		double px, ic, d_mc, d;
		call_vanilla_mc(ic, px, d, d_mc, M, T, S0, K, sigma, r);
 		this->price = px;
		this->confidenceInterval = ic;
		this->delta = d;
		this->delta_mc = d_mc;

	}


	void WrapperEverglades::getPriceEverglades(int nb_dates, int nb_asset, array<double, 2>^ historic, array<double>^ expected_returns, array<double>^ vol, array<double, 2>^ correl, int nb_day_after, double r, int sampleNb) {
		h_gsl_matrix historic_matrix(nb_asset, nb_dates, historic);
		h_gsl_vector expected_returns_vector(nb_asset, expected_returns);
		h_gsl_vector vol_vector(nb_asset, vol);
		h_gsl_matrix correl_matrix(nb_asset, nb_asset, correl);
	
		double price, ic;


		gsl_vector* deltas_temp;
				

		Everglades::get_price(price, ic, &deltas_temp , *historic_matrix._matrix, nb_day_after, r, *expected_returns_vector._vector,
								*vol_vector._vector, *correl_matrix._matrix , sampleNb);
		this->price = price;
		this->confidenceInterval = ic;

		delta = gcnew array<double>(historic_matrix._matrix->size1);


		for (int i = 0; i < deltas_temp->size; i++)
		{
			this->delta[i] = gsl_vector_get(deltas_temp, i);
		}
	}

	void WrapperEverglades::getPriceEvergladesWithForex(int nb_dates, int nb_asset, int nb_currencies, array<double>^ foreign_rates,
														array<int>^ currency_corres, array<double, 2>^ historic, array<double>^ vol,
														array<double, 2>^ correl, int nb_day_after, double r, int sampleNb) {
		h_gsl_vector foreign_rates_vector(nb_currencies, foreign_rates);
		h_gsl_vector currency_corres_vector(nb_asset, currency_corres);
		h_gsl_matrix historic_matrix(nb_asset + nb_currencies, nb_dates, historic);
		h_gsl_vector vol_vector(nb_asset + nb_currencies, vol);
		h_gsl_matrix correl_matrix(nb_asset + nb_currencies, nb_asset + nb_currencies, correl);

		double price, ic;

		gsl_vector* deltas_temp;


		Everglades::get_price_with_forex(price, ic, &deltas_temp, *historic_matrix._matrix, nb_day_after, r,
			*foreign_rates_vector._vector, *currency_corres_vector._vector, *vol_vector._vector, *correl_matrix._matrix, sampleNb);
		this->price = price;
		this->confidenceInterval = ic;

		delta = gcnew array<double>(historic_matrix._matrix->size1);
		for (int i = 0; i < deltas_temp->size; i++)
		{
			this->delta[i] = gsl_vector_get(deltas_temp, i);
		}
	}


	void WrapperEverglades::getPayoffEverglades(int nb_dates, int nb_asset, array<double, 2>^ historic, double vlr) {
		h_gsl_matrix historic_matrix(nb_asset, nb_dates, historic);
		bool anticipated;
		this->payoff = Everglades::get_payoff(*historic_matrix._matrix, vlr, anticipated);
		this->payoffIsAnticipated = anticipated;
	}

	array<double, 2>^ WrapperEverglades::factCholesky(array<double, 2>^ correl, int nb_asset) {
		array<double, 2>^ result = gcnew array<double, 2>(nb_asset, nb_asset);
		h_gsl_matrix correl_gsl(nb_asset, nb_asset, correl);
		h_gsl_matrix result_gsl(nb_asset, nb_asset);
		fact_cholesky(correl_gsl._matrix, result_gsl._matrix);
		for (int i = 0; i < nb_asset; i++)
		{
			for (int j = 0; j < nb_asset; j++)
			{
				result[i, j] = gsl_matrix_get(result_gsl._matrix, i, j);
			}
		}
		return result;
	}

	void WrapperDebugVanilla::getPriceVanilla(int nb_dates, int nb_asset, double S0, array<double>^ expected_returns, array<double>^ vol, array<double, 2>^ correl, double tau, double r, int sampleNb, double Strike) {
		
		h_gsl_vector expected_returns_vector(nb_asset, expected_returns);
		h_gsl_vector vol_vector(nb_asset, vol);
		h_gsl_matrix correl_matrix(nb_asset, nb_asset, correl);

		double price, ic, delta_temp, delta_mc;

		Pricer::call_vanilla_mc(ic, price, delta_temp, delta_mc, sampleNb, tau, S0, Strike, 0.2, 0.3);
		this->price = price;
		this->confidenceInterval = ic;

		delta = gcnew array<double>(1);

		for (int i = 0; i < 1; i++){
			this->delta[i] = delta_temp;
		}
	}

	void WrapperAsian::getPriceCallAsian( int nb_samples, double T,
		double S0, double K, double sigma, double r, double J){
		double ic, px;
		Pricer::option_asian(ic, px, nb_samples, 1, 100, 100, 0.2, 0.095, J);

		//Pricer::option_asian(ic, px, nb_samples, T, S0, K, sigma, r, J);
		this->price = px;
		this->ic = ic;
	}
}