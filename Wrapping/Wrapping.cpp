// Il s'agit du fichier DLL principal.

#include "stdafx.h"
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
	void WrapperEverglades::getPriceEverglades(array<double, 2>^ historic, array<double>^ expected_returns, array<double>^ vol, array<double, 2>^ correl, int nb_day_after, double r1, double r2, int sampleNb) {
		double price, ic;
		int historic_size1 = historic->GetLength(0);
		int historic_size2 = historic->GetLength(1);
		int correl_size1 = correl->GetLength(0);
		int correl_size2 = correl->GetLength(1);
		int vol_size = vol->GetLength(0);
		int returns_size = expected_returns->GetLength(0);
		if (correl_size1 != correl_size2) throw std::invalid_argument("correlation matrix must be square!");
		if (correl_size1 != historic_size1) throw std::invalid_argument("correlation matrix has different size from number of underlyings!");
		if (vol_size != historic_size1 || returns_size != historic_size1) throw std::invalid_argument("invalid size of argument vector!");

		gsl_matrix* historic_gsl = gsl_matrix_alloc(historic_size1, historic_size2);
		gsl_matrix* correl_gsl = gsl_matrix_alloc(correl_size1, correl_size1);
		gsl_vector* vol_gsl = gsl_vector_alloc(historic_size1);
		gsl_vector* returns_gsl = gsl_vector_alloc(historic_size1);

		// remplissage des gsl_matrix et gsl_vector à partir des array<double>
		for (int i = 0; i < historic_size1; i++) {
			for (int j = 0; j < historic_size2; j++) {
				gsl_matrix_set(historic_gsl, i, j, historic[i, j]);
			}
			for (int j = 0; j < correl_size2; j++) {
				gsl_matrix_set(correl_gsl, i, j, correl[i, j]);
			}
			gsl_vector_set(vol_gsl, i, vol[i]);
			gsl_vector_set(returns_gsl, i, expected_returns[i]);
		}
		// Calcul du prix du produit
		Everglades::get_price(price, ic, *historic_gsl, nb_day_after, r1, r2, *returns_gsl, *vol_gsl, *correl_gsl, sampleNb);
		this->price = price;
		this->confidenceInterval = ic;
	}
}