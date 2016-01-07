// Il s'agit du fichier DLL principal.

#include "stdafx.h"
#include "Wrapping.hpp"
#include "pricer.hpp"


using namespace Pricer;

namespace Wrapping {

	void WrapperBarrier::getPriceOptionBarrier(int sampleNb, double T, double S0, double K, double sigma, double r, double J, double L) {
		double ic, px;
		call_barrier(ic, px, sampleNb, T, S0, K, sigma, r, J, L);
		this->confidenceInterval = ic;
		this->price = px;
	}

	void WrapperBarrier::getPriceOptionCallQuanto(double S, double Q, double K, double R, double Rf, double sigma1, double sigma2, double rho, double tau) {
		double px;
		call_quanto(px, S, Q, K, R, Rf, sigma1, sigma2, rho, tau);
		this->confidenceInterval = 0;
		this->price = px;
	}

}