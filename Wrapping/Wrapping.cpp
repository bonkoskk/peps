// Il s'agit du fichier DLL principal.

#include "stdafx.h"
#include "Wrapping.hpp"
#include "pricer.hpp"


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

}