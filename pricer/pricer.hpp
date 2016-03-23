#pragma once
#include "dll_define.hpp"


namespace Pricer{
	
	DLLEXP void call_vanilla(double &prix, double T,
		double S0, double K, double sigma, double r, double q);

	DLLEXP void call_vanilla_mc(double &ic, double &prix, double &delta, double &delta_mc, int nb_samples, double T,
		double S0, double K, double sigma, double r);

	DLLEXP void call_vanilla_delta(double &delta, double T,
		double S0, double K, double sigma, double r, double q);
	
	DLLEXP void put_vanilla(double &prix, double T,
		double S0, double K, double sigma, double r, double q);

	DLLEXP void call_quanto(double &prix, double S, double Q, double K, double R, double Rf, double sigma1, double sigma2, double rho, double tau);

	DLLEXP void put_quanto(double &prix, double S, double Q, double K, double R, double Rf, double sigma1, double sigma2, double rho, double tau);

	DLLEXP void call_barrier_down_out(double &ic, double &prix, int nb_samples, double T,
	double S0, double K, double sigma, double r, double J, double L);
	/*
	DLLEXP void put_barrier(double &ic, double &prix, int nb_samples, double T,
		double S0, double K, double sigma, double r, double J, double L);
		*/
	DLLEXP void put_american(double &price, double S0, double K, double T, double R, double vol, int N);
	
	DLLEXP void option_asian(double &ic, double &prix, int nb_samples, double T,
		double S0, double K, double sigma, double r, double J);
}
