// Wrapping.h

#pragma once

using namespace System;

namespace Wrapping {

	public ref class WrapperBarrier
	{
	public:
		double confidenceInterval;
		double price;
		WrapperBarrier() { confidenceInterval = price = 0; };
		void getPriceOptionBarrier(int sampleNb, double T, double S0, double K, double sigma, double r, double J, double L);
		void getPriceOptionCallQuanto(double S, double Q, double K, double R, double Rf, double sigma1, double sigma2, double rho, double tau);
		double getPrice() { return price; };
		double getIC() { return confidenceInterval; };

	};

}
