#include "stdafx.h"
#include "computations.hpp"

#include "wrapper_pricer.hpp"

using namespace computations;

namespace wrapper {

	void WrapperClassEverglades::getPriceOptionBarrier(int sampleNb, double T, double S0, double K, double sigma, double r, double J, double L){
		double ic, px;
		option_barrier(ic, px, sampleNb, T, S0, K, sigma, r, J, L);
		this->confidenceInterval = ic;
		this->price = px;
	}

	
}