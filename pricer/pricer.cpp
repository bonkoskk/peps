#include "simulations.hpp"
#include "pricer.hpp"
#include "payoff.hpp"
#include <cmath>
#include <ctime>
#include <gsl/gsl_cdf.h>

void Pricer::call_vanilla(double &prix, double T,
	double S0, double K, double sigma, double r, double q)
{
	const double sigma2sur2 = sigma*sigma / 2;
	const double logs0surK = log(S0 / K);
	const double sigmasqrtT = sigma*sqrt(T);
	const double d1 = (logs0surK + (r - q + sigma2sur2)*T) / sigmasqrtT;
	const double d2 = d1 - sigmasqrtT;
	prix = exp(-q*T)*S0*gsl_cdf_ugaussian_P(d1) - exp(-r*T)*K*gsl_cdf_ugaussian_P(d2);
}

void Pricer::put_vanilla(double &prix, double T,
	double S0, double K, double sigma, double r, double q)
{
	const double sigma2sur2 = sigma*sigma / 2;
	const double logs0surK = log(S0 / K);
	const double sigmasqrtT = sigma*sqrt(T);
	const double d1 = (logs0surK + (r - q + sigma2sur2)*T) / sigmasqrtT;
	const double d2 = d1 - sigmasqrtT;
	prix = exp(-r*T)*K*gsl_cdf_ugaussian_P(-d2) - exp(-q*T)*S0*gsl_cdf_ugaussian_P(-d1);
}

void Pricer::call_barrier_down_out(double &ic, double &prix, int nb_samples, double T,
	double S0, double K, double sigma, double r, double J, double L)
{
	struct simulations::Params data;
	data.M = nb_samples;
	data.S = S0;
	data.K = K;
	data.r = r;
	data.v = sigma;
	data.T = T;

	double sum = 0;
	double var = 0;

	double payoff;

	gsl_rng *rng = gsl_rng_alloc(gsl_rng_default);
	gsl_rng_set(rng, (unsigned long int)time(NULL));

	gsl_vector* simulations;

	for (int i = 0; i < nb_samples; i++)
	{
		simulations = simulate_sj(data, J, rng);

		payoff = payoff_call_barrier_down_out(data, J, L, simulations);
		sum += exp(-r*T) * payoff;
		var += exp(-2.*r*T) * payoff * payoff;
	}


	prix = sum / nb_samples;
	var = var / nb_samples - prix * prix;
	ic = 1.96 * sqrt(var / nb_samples);
	gsl_rng_free(rng);
}

/// <summary>
/// Calculate the price of a quanto option
/// </summary>
/// <param name="S">Asset price at t in local money</param>
/// <param name="Q">Exchange rates at t : price of one unit of foreign money in local money</param>
/// <param name="K">Strike in local money</param>
/// <param name="R">Local interest rates</param>
/// <param name="Rf">Foreign interest rates</param>
/// <param name="sigma1">volatility of asset price</param>
/// <param name="sigma2">volatility of exchange rate processus</param>
/// <param name="rho">correlation between S and Q 's brownians</param>
/// <param name="tau">time before maturity</param>
void Pricer::call_quanto(double &prix, double S, double Q, double K, double R, double Rf, double sigma1, double sigma2, double rho, double tau)
{
	double sigma4 = sqrt(sigma1*sigma1 - 2 * rho*sigma1*sigma2 + sigma2*sigma2);
	double a = R - Rf + rho*sigma1*sigma2 - sigma2*sigma2;
	double x = S / Q;
	double d1 = (log(x / K) + (R - a + sigma4*sigma4 / 2)*tau) / (sigma4*sqrt(tau));
	double d2 = (log(x / K) + (R - a - sigma4*sigma4 / 2)*tau) / (sigma4*sqrt(tau));
	prix = x*exp(-a*tau)*gsl_cdf_ugaussian_P(d1) - exp(-R*tau)*K*gsl_cdf_ugaussian_P(d2);
}

void Pricer::put_quanto(double &prix, double S, double Q, double K, double R, double Rf, double sigma1, double sigma2, double rho, double tau)
{
	double sigma4 = sqrt(sigma1*sigma1 - 2 * rho*sigma1*sigma2 + sigma2*sigma2);
	double a = R - Rf + rho*sigma1*sigma2 - sigma2*sigma2;
	double x = S / Q;
	double d1 = (log(x / K) + (R - a + sigma4*sigma4 / 2)*tau) / (sigma4*sqrt(tau));
	double d2 = (log(x / K) + (R - a - sigma4*sigma4 / 2)*tau) / (sigma4*sqrt(tau));
	prix = -x*exp(-a*tau)*gsl_cdf_ugaussian_P(-d1) + exp(-R*tau)*K*gsl_cdf_ugaussian_P(-d2);
}