// Wrapping.h
#include "managed_gsl.hpp"

#pragma once

using namespace System;

namespace Wrapping {

	public ref class WrapperVanilla
	{
	public:
		double confidenceInterval;
		double price;
		WrapperVanilla() { confidenceInterval = price = 0; };
		void getPriceOptionEuropeanCall(double T, double S0, double K, double sigma, double r, double q);
		void getPriceOptionEuropeanPut(double T, double S0, double K, double sigma, double r, double q);
		double getPrice() { return price; };
		double getIC() { return confidenceInterval; };
	};

	public ref class WrapperBarrier
	{
	public:
		double confidenceInterval;
		double price;
		WrapperBarrier() { confidenceInterval = price = 0; };
		void getPriceCallBarrierDownOut(int sampleNb, double T, double S0, double K, double sigma, double r, double J, double L);
		double getPrice() { return price; };
		double getIC() { return confidenceInterval; };

	};

	public ref class WrapperQuanto
	{
	public:
		double confidenceInterval;
		double price;
		WrapperQuanto() { confidenceInterval = price = 0; };
		void getPriceCallQuanto(double S, double Q, double K, double R, double Rf, double sigma1, double sigma2, double rho, double tau);
		void getPricePutQuanto(double S, double Q, double K, double R, double Rf, double sigma1, double sigma2, double rho, double tau);
		double getPrice() { return price; };
		double getIC() { return confidenceInterval; };
	};

	using namespace managed_gsl;
	
	public ref class WrapperEverglades
	{
	private:
		double confidenceInterval;
		double price;
	public:
		WrapperEverglades() { confidenceInterval = price = 0; };
		void getPriceEverglades(int nb_dates, int nb_asset, array<double, 2>^ historic, array<double>^ expected_returns, array<double>^ vol, array<double,2>^ correl, int nb_day_after, double r1, double r2, int sampleNb);
		double getPrice() { return price; };
		double getIC() { return confidenceInterval; };
	};
}
