// Il s'agit du fichier DLL principal.

#include "Wrapping.hpp"
#include "pricer.hpp"
#include "everglades.hpp"
#include <iostream>



using namespace Pricer;

namespace Wrapping {

	void WrapperBarrier::getPriceOptionBarrier(int sampleNb, double T, double S0, double K, double sigma, double r, double J, double L) {
		double ic, px;
		call_barrier(ic, px, sampleNb, T, S0, K, sigma, r, J, L);
		this->confidenceInterval = ic;
		this->price = px;
	}

	void WrapperQuanto::getPriceOptionCallQuanto(double S, double Q, double K, double R, double Rf, double sigma1, double sigma2, double rho, double tau) {
		double px;
		call_quanto(px, S, Q, K, R, Rf, sigma1, sigma2, rho, tau);
		this->confidenceInterval = 0;
		this->price = px;
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
	void WrapperEverglades::getPriceEverglades(h_gsl_matrix historic, h_gsl_vector expected_returns, h_gsl_vector vol, h_gsl_matrix correl, int nb_day_after, double r1, double r2, int sampleNb) {
		double price, ic;
		Everglades::get_price(price, ic, *historic._matrix, nb_day_after, r1, r2, *expected_returns._vector, *vol._vector, *correl._matrix , sampleNb);
		this->price = price;
		this->confidenceInterval = ic;
	}
}