#pragma once
#define DLLEXP   __declspec( dllexport )


namespace Pricer{
	
	DLLEXP void call_vanilla(double &prix, double T,
		double S0, double K, double sigma, double r, double q);
	
	DLLEXP void put_vanilla(double &prix, double T,
		double S0, double K, double sigma, double r, double q);

	DLLEXP void call_quanto(double &prix, double S, double Q, double K, double R, double Rf, double sigma1, double sigma2, double rho, double tau);

	//DLLEXP void put_quanto(double &prix, double S, double Q, double K, double R, double Rf, double sigma1, double sigma2, double rho, double tau);

	DLLEXP void call_barrier(double &ic, double &prix, int nb_samples, double T, double S0, double K, double sigma, double r, double J, double L);
	/*
	DLLEXP void put_barrier(double &ic, double &prix, int nb_samples, double T,
		double S0, double K, double sigma, double r, double J, double L);
		*/
}