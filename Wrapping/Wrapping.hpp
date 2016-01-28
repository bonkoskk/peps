// Wrapping.h
#include "managed_gsl.hpp"

#pragma once

using namespace System;

namespace Wrapping {

	public ref class WrapperVanilla
	{
	protected:
		double confidenceInterval;
		double price;
		double delta;
		double delta_mc;
	public:
		WrapperVanilla() { confidenceInterval = price = 0; };
		void getPriceOptionEuropeanCall(double T, double S0, double K, double sigma, double r, double q);
		void getPriceOptionEuropeanCallMC(int M, double T, double S0, double K, double sigma, double r, double q);
		void getPriceOptionEuropeanPut(double T, double S0, double K, double sigma, double r, double q);
		double getPrice() { return price; };
		double getIC() { return confidenceInterval; };
		double getDelta() { return delta; };
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
		array<double>^ delta;
		bool payoffIsAnticipated;
		double payoff;


	public:
		/**
		 * compute price and delta of everglades and put results in variables accessible
		 * through getPrice() and getDelta()
		 *
		 * param[in] nb_dates number of dates sent in historic
		 * param[in] nb_asset number of assetss in the underlying portfolio
		 * param[in] historic[asset, date] matrix of prices of each asset dates must 
		 *			correspond to all constattion dates until now plus now
		 * param[in] expected_returns vector of expected returns rates for each asset
		 * param[in] vol vector of volatility for each asset
		 * param[in] correl[asset, asset] matrix of correlations
		 * param[in] nb_day_after number of days between now and last constation date
		 * param[in] r return rate of the product everglades
		 * param[in] sampleNb number of monte carlo simulations to run
		 */
		void getPriceEverglades(int nb_dates, int nb_asset, array<double, 2>^ historic, array<double>^ expected_returns, array<double>^ vol, array<double,2>^ correl, int nb_day_after, double r, int sampleNb);
		void getPayoffEverglades(int nb_dates, int nb_asset, array<double, 2>^ historic, double vlr);
		WrapperEverglades() { 
			confidenceInterval = price = 0;
		};
		double getPrice() { return price; };
		double getIC() { return confidenceInterval; };
		array<double>^ getDelta() { return delta; };
		bool getPayoffIsAnticipated() { return payoffIsAnticipated; };
		double getPayoff() { return payoff; };

	};
}
