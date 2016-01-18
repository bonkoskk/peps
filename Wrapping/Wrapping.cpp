// Il s'agit du fichier DLL principal.

#include "Wrapping.hpp"
#include "pricer.hpp"
#include "everglades.hpp"
#include <iostream>



using namespace Pricer;

namespace Wrapping {

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
		double px;
		call_vanilla(px, T, S0, K, sigma, r, q);
		this->price = px;
	}

	void WrapperVanilla::getPriceOptionEuropeanPut(double T, double S0, double K, double sigma, double r, double q) {
		double px;
		put_vanilla(px, T, S0, K, sigma, r, q);
		this->price = px;
	}

	//void WrapperEverglades::getPriceEverglades(h_gsl_matrix historic, h_gsl_vector expected_returns, h_gsl_vector vol, h_gsl_matrix correl, int nb_day_after, double r1, double r2, int sampleNb) {
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
}