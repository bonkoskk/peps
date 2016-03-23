// Il s'agit du fichier DLL principal.

#include "Wrapping.hpp"
#include "pricer.hpp"
#include "everglades.hpp"
#include <iostream>
#include <stdexcept>
#include "error.hpp"


using namespace Pricer;

namespace Wrapping {
	void WrapperAmerican::getPricePutAmerican(double S0, double K, double T, double R, double vol, int N) {
		double px;
		int err;
		try{
			err = put_american(px, S0, K, T, R, vol, N);
			if (err != 0){
				throw std::logic_error("La fonction put_american a rencontré une erreur :" + get_error_message(err));
			}
		}
		catch (std::exception const& e) {
			throw std::logic_error("la fonction put_american a rencontré une erreur inconnue");
		}
		this->price = px;
	}

	void WrapperBarrier::getPriceCallBarrierDownOut(int sampleNb, double T, double S0, double K, double sigma, double r, double J, double L) {
		double ic, px;
		int err;
		try{
			err = call_barrier_down_out(ic, px, sampleNb, T, S0, K, sigma, r, J, L);
			if (err != 0){
				throw std::logic_error("La fonction call_barrier_down_out a rencontré une erreur :" + get_error_message(err));
			}
		}
		catch (std::exception const& e) {
			throw std::logic_error("la fonction call_barrier_down_out a rencontré une erreur inconnue");
		}
		this->confidenceInterval = ic;
		this->price = px;
	}

	void WrapperQuanto::getPriceCallQuanto(double S, double Q, double K, double R, double Rf, double sigma1, double sigma2, double rho, double tau) {
		double px;
		int err;
		try{
			err = call_quanto(px, S, Q, K, R, Rf, sigma1, sigma2, rho, tau);
			if (err != 0){
				throw std::logic_error("La fonction call_quanto a rencontré une erreur :" + get_error_message(err));
			}
		}
		catch (std::exception const& e) {
			throw std::logic_error("la fonction call_quanto a rencontré une erreur inconnue");
		}
		this->confidenceInterval = 0;
		this->price = px;
	}

	void WrapperQuanto::getPricePutQuanto(double S, double Q, double K, double R, double Rf, double sigma1, double sigma2, double rho, double tau) {
		double px = 1;
		int err;
		try{
			err = put_quanto(px, S, Q, K, R, Rf, sigma1, sigma2, rho, tau);
			if (err != 0){
				throw std::logic_error("La fonction put_quanto a rencontré une erreur :" + get_error_message(err));
			}
		}
		catch (std::exception const& e) {
			throw std::logic_error("la fonction put_quanto a rencontré une erreur inconnue");
		}
		this->confidenceInterval = 0;
	}

	void WrapperVanilla::getPriceOptionEuropeanCall(double T, double S0, double K, double sigma, double r, double q) {
		double px, delta;
		int err;
		try{
			err = call_vanilla(px, T, S0, K, sigma, r, q);
			if (err != 0){
				throw std::logic_error("La fonction call_vanilla a rencontré une erreur :" + get_error_message(err));
			}
		}
		catch (std::exception const& e) {
			throw std::logic_error("la fonction call_vanilla a rencontré une erreur inconnue");
		}
		this->price = px;
		try{
			err = call_vanilla_delta(delta, T, S0, K, sigma, r, q);
			if (err != 0){
				throw std::logic_error("La fonction call_vanilla_delta a rencontré une erreur :" + get_error_message(err));
			}
		}
		catch (std::exception const& e) {
			throw std::logic_error("la fonction call_vanilla_delta a rencontré une erreur inconnue");
		}
		this->delta = delta;
	}

	void WrapperVanilla::getPriceOptionEuropeanPut(double T, double S0, double K, double sigma, double r, double q) {
		double px;
		int err;
		try{
			err = put_vanilla(px, T, S0, K, sigma, r, q);
			if (err != 0){
				throw std::logic_error("La fonction put_vanilla a rencontré une erreur :" + get_error_message(err));
			}
		}
		catch (std::exception const& e) {
			throw std::logic_error("la fonction put_vanilla a rencontré une erreur inconnue");
		}
		this->price = px;
	}

	void WrapperVanilla::getPriceOptionEuropeanCallMC(int M, double T, double S0, double K, double sigma, double r, double q){

		double px, ic, d_mc, d;
		int err;
		try{
			err = call_vanilla_mc(ic, px, d, d_mc, M, T, S0, K, sigma, r);
			if (err != 0){
				throw std::logic_error("La fonction call_vanilla_mc a rencontré une erreur :" + get_error_message(err));
			}
		}
		catch (std::exception const& e) {
			throw std::logic_error("la fonction call_vanilla_mc a rencontré une erreur inconnue");
		}
		this->price = px;
		this->confidenceInterval = ic;
		this->delta = d;
		this->delta_mc = d_mc;

	}

	void WrapperEverglades::getPriceEverglades(int nb_dates, int nb_asset, array<double, 2>^ historic, array<double>^ expected_returns, array<double>^ vol, array<double, 2>^ correl, int nb_day_after, double r, int sampleNb) {
		h_gsl_matrix historic_matrix(nb_asset, nb_dates, historic);
		// h_gsl_vector expected_returns_vector(nb_asset, expected_returns);
		h_gsl_vector vol_vector(nb_asset, vol);
		h_gsl_matrix correl_matrix(nb_asset, nb_asset, correl);

		double price, ic;
		bool anticipated;
		int err;

		gsl_vector* deltas_temp;

		try{
			err = Everglades::get_price(price, ic, anticipated, *deltas_temp, *historic_matrix._matrix, nb_day_after, r,
				*vol_vector._vector, *correl_matrix._matrix, sampleNb);
			if (err != 0){
				throw std::logic_error("La fonction get_price a rencontré une erreur :" + get_error_message(err));
			}
		}
		catch (std::exception const& e) {
			throw std::logic_error("la fonction get_price a rencontré une erreur inconnue");
		}
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
		bool anticipated;
		gsl_vector* deltas_temp;
		int err;
		try{
			err = Everglades::get_price_with_forex(price, ic, anticipated, *deltas_temp, *historic_matrix._matrix, nb_day_after, r,
				*foreign_rates_vector._vector, *currency_corres_vector._vector, *vol_vector._vector, *correl_matrix._matrix, sampleNb);
			if (err != 0){
				throw std::logic_error("La fonction get_price_with_forex a rencontré une erreur :" + get_error_message(err));
			}
		}
		catch (std::exception const& e) {
			throw std::logic_error("la fonction get_price_with_forex a rencontré une erreur inconnue");
		}
		this->price = price;
		this->confidenceInterval = ic;
		this->payoffIsAnticipated;

		delta = gcnew array<double>(historic_matrix._matrix->size1);
		for (int i = 0; i < deltas_temp->size; i++)
		{
			this->delta[i] = gsl_vector_get(deltas_temp, i);
		}
	}


	void WrapperEverglades::getPayoffEverglades(int nb_dates, int nb_asset, array<double, 2>^ historic, double vlr) {
		h_gsl_matrix historic_matrix(nb_asset, nb_dates, historic);
		bool anticipated;
		int err;
		double res;
		try {
			err = Everglades::get_payoff(res, *historic_matrix._matrix, vlr, anticipated);
			if (err != 0){
				throw std::logic_error("La fonction get_payoff a rencontré une erreur :" + get_error_message(err));
			}
		}
		catch (std::exception const& e) {
			throw std::logic_error("la fonction get_payoff a rencontré une erreur inconnue");
		}
		this->payoff = res;
		this->payoffIsAnticipated = anticipated;
	}

	void WrapperDebugVanilla::getPriceVanilla(int nb_dates, int nb_asset, double S0, array<double>^ expected_returns, array<double>^ vol, array<double, 2>^ correl, double tau, double r, int sampleNb, double Strike) {

		h_gsl_vector expected_returns_vector(nb_asset, expected_returns);
		h_gsl_vector vol_vector(nb_asset, vol);
		h_gsl_matrix correl_matrix(nb_asset, nb_asset, correl);

		double price, ic, delta_temp, delta_mc;
		int err;
		try{
			err = Pricer::call_vanilla_mc(ic, price, delta_temp, delta_mc, sampleNb, tau, S0, Strike, 0.2, 0.3);
			if (err != 0){
				throw std::logic_error("La fonction call_vanilla_mc a rencontré une erreur :" + get_error_message(err));
			}
		}
		catch (std::exception const& e) {
			throw std::logic_error("la fonction call_vanilla_mc a rencontré une erreur inconnue");
		}
		this->price = price;
		this->confidenceInterval = ic;

		delta = gcnew array<double>(1);

		for (int i = 0; i < 1; i++){
			this->delta[i] = delta_temp;
		}
	}

	void WrapperAsian::getPriceCallAsian(int nb_samples, double T,
		double S0, double K, double sigma, double r, double J){
		double ic, px;
		int err;
		try{
			err = Pricer::option_asian(ic, px, nb_samples, T, S0, K, sigma, r, J);
			if (err != 0){
				throw std::logic_error("La fonction option_asian a rencontré une erreur :" + get_error_message(err));
			}
		}
		catch (std::exception const& e) {
			throw std::logic_error("la fonction option_asian a rencontré une erreur inconnue");
		}
		this->price = px;
		this->ic = ic;
	}

	array<double, 2>^ WrapperEverglades::factCholesky(array<double, 2>^ correl, int nb_asset) {
		array<double, 2>^ result = gcnew array<double, 2>(nb_asset, nb_asset);
		h_gsl_matrix correl_gsl(nb_asset, nb_asset, correl);
		h_gsl_matrix result_gsl(nb_asset, nb_asset);
		int err;
		try{
			err = fact_cholesky(correl_gsl._matrix, result_gsl._matrix);
			if (err != 0){
				throw std::logic_error("La fonction fact_cholesky a rencontré une erreur :" + get_error_message(err));
			}
		}
		catch (std::exception const& e) {
			throw std::logic_error("la fonction fact_cholesky a rencontré une erreur inconnue");
		}
		for (int i = 0; i < nb_asset; i++)
		{
			for (int j = 0; j < nb_asset; j++)
			{
				result[i, j] = gsl_matrix_get(result_gsl._matrix, i, j);
			}
		}
		return result;
	}

	void Tools::getCorrelAndVol(int nb_dates, int nb_asset, array<double, 2>^ prices, array<double, 2>^ correl, array<double>^ vol) {
		h_gsl_matrix prices_gsl(nb_asset, nb_dates, prices);
		h_gsl_matrix covariance_gsl(nb_asset, nb_asset);
		h_gsl_matrix correl_gsl(nb_asset, nb_asset);
		h_gsl_vector vol_gsl(nb_asset);
		int err1;
		int err2;
		try{
			err1 = compute_covariance(prices_gsl._matrix, covariance_gsl._matrix);
			err2 = get_correlation_and_volatility(covariance_gsl._matrix, correl_gsl._matrix, vol_gsl._vector);
			if (err1 != 0){
				throw std::logic_error("La fonction compute_covariance a rencontré une erreur :" + get_error_message(err1));
			}
			if (err1 != 0){
				throw std::logic_error("La fonction get_correlation_and_volatility a rencontré une erreur :" + get_error_message(err2));
			}
		}
		catch (std::exception const& e) {
			throw std::logic_error("la fonction a rencontré une erreur inconnue");
		}
		for (int i = 0; i < correl_gsl._matrix->size1; i++)
		{
			for (int j = 0; j < correl_gsl._matrix->size2; j++)
			{
				correl[i, j] = sqrt(365)*gsl_matrix_get(correl_gsl._matrix, i, j);
			}
		}

		for (int i = 0; i < vol_gsl._vector->size; i++)
		{
			vol[i] = gsl_vector_get(vol_gsl._vector, i);
		}
	}
	void Tools::getCorrelAndVol_Yang_Zhang(int nb_dates, int nb_asset, array<double, 2>^ open_prices, array<double, 2>^ close_prices, array<double, 2>^ high_prices, array<double, 2>^ low_prices, array<double, 2>^ correl, array<double>^ vol) {
		h_gsl_matrix open_prices_gsl(nb_asset, nb_dates, open_prices);
		h_gsl_matrix close_prices_gsl(nb_asset, nb_dates, close_prices);
		h_gsl_matrix high_prices_gsl(nb_asset, nb_dates, high_prices);
		h_gsl_matrix low_prices_gsl(nb_asset, nb_dates, low_prices);
		h_gsl_matrix covariance_gsl(nb_asset, nb_asset);
		h_gsl_matrix correl_gsl(nb_asset, nb_asset);
		h_gsl_vector vol_gsl(nb_asset);
		int err1;
		int err2;
		try{
			err1 = compute_covariance_Yang_Zhang(open_prices_gsl._matrix, close_prices_gsl._matrix, high_prices_gsl._matrix, low_prices_gsl._matrix, covariance_gsl._matrix);
			err2 = get_correlation_and_volatility(covariance_gsl._matrix, correl_gsl._matrix, vol_gsl._vector);
			if (err1 != 0){
				throw std::logic_error("La fonction compute_covariance_Yang_Zhang a rencontré une erreur :" + get_error_message(err1));
			}
			if (err1 != 0){
				throw std::logic_error("La fonction get_correlation_and_volatility a rencontré une erreur :" + get_error_message(err2));
			}
		}
		catch (std::exception const& e) {
			throw std::logic_error("la fonction a rencontré une erreur inconnue");
		}
		for (int i = 0; i < correl_gsl._matrix->size1; i++)
		{
			for (int j = 0; j < correl_gsl._matrix->size2; j++)
			{
				correl[i, j] = gsl_matrix_get(correl_gsl._matrix, i, j);
			}
		}

		for (int i = 0; i < vol_gsl._vector->size; i++)
		{
			vol[i] = gsl_vector_get(vol_gsl._vector, i);
		}
	}

}