#pragma once
#define DLLEXP   __declspec( dllexport )


namespace Pricer{

	DLLEXP void option_barrier(double &ic, double &prix, int nb_samples, double T,
		double S0, double K, double sigma, double r, double J, double L);

}