// Il s'agit du fichier DLL principal.

#include "stdafx.h"
#include "Wrapping.hpp"
#include "pricer.hpp"


using namespace Pricer;

namespace Wrapping {

	void WrapperBarrier::getPriceOptionBarrier(int sampleNb, double T, double S0, double K, double sigma, double r, double J, double L){
		double ic, px;
		option_barrier(ic, px, sampleNb, T, S0, K, sigma, r, J, L);
		this->confidenceInterval = ic;
		this->price = px;
	}

}