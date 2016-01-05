#pragma once
#include "Computations.hpp"
using namespace System;

namespace wrapper {

	public ref class WrapperClassEverglades
	{
	private:
		double confidenceInterval;
		double price;
	public:
		WrapperClassEverglades() { confidenceInterval = price = 0; };


		void getPriceOptionBarrier(int sampleNb, double T, double S0, double K, double sigma, double r, double J, double L);


		double getPrice() { return price; };
		double getIC() { return confidenceInterval; };

	};
}