#pragma once
#include "dll_define.hpp"


namespace Pricer{
	
	DLLEXP int call_vanilla(double &prix, double T,
		double S0, double K, double sigma, double r, double q);

	DLLEXP int call_vanilla_mc(double &ic, double &prix, double &delta, double &delta_mc, int nb_samples, double T,
		double S0, double K, double sigma, double r);

	DLLEXP int call_vanilla_delta(double &delta, double T,
		double S0, double K, double sigma, double r, double q);

	DLLEXP int put_vanilla(double &prix, double T,
		double S0, double K, double sigma, double r, double q);

	DLLEXP int call_quanto(double &prix, double S, double Q, double K, double R, double Rf, double sigma1, double sigma2, double rho, double tau);

	DLLEXP int put_quanto(double &prix, double S, double Q, double K, double R, double Rf, double sigma1, double sigma2, double rho, double tau);

	DLLEXP int call_barrier_down_out(double &ic, double &prix, int nb_samples, double T,
		double S0, double K, double sigma, double r, double J, double L);
	/*
	DLLEXP void put_barrier(double &ic, double &prix, int nb_samples, double T,
		double S0, double K, double sigma, double r, double J, double L);
		*/
	DLLEXP int put_american(double &price, double S0, double K, double T, double R, double vol, int N);
	
	DLLEXP int option_asian(double &ic, double &prix, int nb_samples, double T,
		double S0, double K, double sigma, double r, double J);
}
