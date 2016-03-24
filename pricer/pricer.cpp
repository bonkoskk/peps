#include "simulations.hpp"
#include "pricer.hpp"
#include "payoff.hpp"
#include <cmath>
#include <ctime>
#include <gsl/gsl_cdf.h>
#include <iostream>
#include <gsl/gsl_randist.h>
#include <gsl/gsl_blas.h>

int Pricer::call_vanilla_mc(double &ic, double &prix, double &delta, double &delta_mc, int nb_samples, double T,
	double S0, double K, double sigma, double r)
{
	int err;
	struct simulations::Params data;
	data.S = S0;
	data.K = K;
	data.r = r;
	data.v = sigma;
	data.T = T;

	if (S0 <= 0 || K <= 0 || sigma <= 0 || T <= 0 || nb_samples <= 0)  {
		//std::cout << "the asset price, the exchange rate, the maturity, the strike and the volatilities must be positive " << std::endl;
		return 10;
	}
	double sum = 0;
	double sum_delta_mc = 0;
	double var = 0;

	double payoff = 0;

	gsl_rng *rng = gsl_rng_alloc(gsl_rng_default);
	gsl_rng_set(rng, (unsigned long int)time(NULL));

	double St;
	double epsilon = 0.1;
	double payoff_up, payoff_down;

	for (int i = 0; i < nb_samples; i++)
	{
		//simulations = simulate_sj(data, 2, rng);

		err = simulate_ST(data, rng, &St);
		payoff = payoff_call_vanilla(data, St);

		payoff_up = payoff_call_vanilla(data, St * (1 + epsilon));
		payoff_down = payoff_call_vanilla(data, St * (1 - epsilon));



		sum += exp(-r*T) * payoff;
		var += exp(-2.*r*T) * payoff * payoff;
		sum_delta_mc += (payoff_up - payoff_down) / (2 * epsilon * St);
	}

	prix = sum / nb_samples;
	delta_mc = exp(-r*T) * sum_delta_mc / nb_samples;
	var = var / nb_samples - prix * prix;
	ic = 1.96 * sqrt(var / nb_samples);
	gsl_rng_free(rng);
	return 0;
}

int Pricer::call_vanilla(double &prix, double T,
	double S0, double K, double sigma, double r, double q)
{
	if (S0 <= 0 || K <= 0 || sigma <= 0 || T <= 0)  {
		//std::cout << "the asset price, the exchange rate, the maturity, the strike and the volatilities must be positive " << std::endl;
		return 10;
	}

	const double sigma2sur2 = sigma*sigma / 2;
	const double logs0surK = log(S0 / K);
	const double sigmasqrtT = sigma*sqrt(T);
	const double d1 = (logs0surK + (r - q + sigma2sur2)*T) / sigmasqrtT;
	const double d2 = d1 - sigmasqrtT;
	prix = exp(-q*T)*S0*gsl_cdf_ugaussian_P(d1) - exp(-r*T)*K*gsl_cdf_ugaussian_P(d2);
	return 0;
}

int Pricer::call_vanilla_delta(double &delta, double T,
	double S0, double K, double sigma, double r, double q)
{

	if (S0 <= 0 || K <= 0 || sigma <= 0 || T <= 0)  {
		//std::cout << "the asset price, the exchange rate, the maturity, the strike and the volatilities must be positive " << std::endl;
		return 10;
	}
	const double sigma2sur2 = sigma*sigma / 2;
	const double logs0surK = log(S0 / K);
	const double sigmasqrtT = sigma*sqrt(T);
	const double d1 = (logs0surK + (r - q + sigma2sur2)*T) / sigmasqrtT;
	delta = exp(-q*T)*gsl_cdf_ugaussian_P(d1);
	return 0;
}

int Pricer::put_vanilla(double &prix, double T,
	double S0, double K, double sigma, double r, double q)
{

	if (S0 <= 0 || K <= 0 || sigma <= 0 || T <= 0)  {
		//std::cout << "the asset price, the exchange rate, the maturity, the strike and the volatilities must be positive " << std::endl;
		return 10;
	}
	const double sigma2sur2 = sigma*sigma / 2;
	const double logs0surK = log(S0 / K);
	const double sigmasqrtT = sigma*sqrt(T);
	const double d1 = (logs0surK + (r - q + sigma2sur2)*T) / sigmasqrtT;
	const double d2 = d1 - sigmasqrtT;
	prix = exp(-r*T)*K*gsl_cdf_ugaussian_P(-d2) - exp(-q*T)*S0*gsl_cdf_ugaussian_P(-d1);
	return 0;
}

int Pricer::call_barrier_down_out(double &ic, double &prix, int nb_samples, double T,
	double S0, double K, double sigma, double r, double J, double L)
{
	struct simulations::Params data;
	data.S = S0;
	data.K = K;
	data.r = r;
	data.v = sigma;
	data.T = T;

	if (S0 <= 0 || K <= 0 || sigma <= 0 || T <= 0 || nb_samples <= 0 || J <= 0 || L <= 0)  {
		//std::cout << "the asset price, the exchange rate, the maturity, the strike and the volatilities must be positive " << std::endl;
		return 10;
	}
	double sum = 0;
	double var = 0;

	double payoff;

	gsl_rng *rng = gsl_rng_alloc(gsl_rng_default);
	gsl_rng_set(rng, (unsigned long int)time(NULL));

	gsl_vector* simulations;

	for (int i = 0; i < nb_samples; i++)
	{
		simulate_sj(data, J, rng, &simulations);

		payoff = payoff_call_barrier_down_out(data, J, L, simulations);
		sum += exp(-r*T) * payoff;
		var += exp(-2.*r*T) * payoff * payoff;
	}


	prix = sum / nb_samples;

	var = var / nb_samples - prix * prix;
	ic = 1.96 * sqrt(var / nb_samples);
	gsl_rng_free(rng);
	return 0;
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
/// <param name="T">time before maturity</param>
int Pricer::call_quanto(double &prix, double S, double Q, double K, double R, double Rf, double sigma1, double sigma2, double rho, double T)
{
	double sigma4 = sqrt(sigma1*sigma1 - 2 * rho*sigma1*sigma2 + sigma2*sigma2);
	double a = R - Rf + rho*sigma1*sigma2 - sigma2*sigma2;
	double x = S / Q;
	if (S <= 0 || Q <= 0 || K <= 0 || sigma1 <= 0 || sigma2 <= 0 || T<0)  {
		//std::cout << "the asset price, the exchange rate, the maturity, the strike and the volatilities must be positive " << std::endl;
		return 10;
	}
	double d1 = (log(x / K) + (R - a + sigma4*sigma4 / 2)*T) / (sigma4*sqrt(T));
	double d2 = (log(x / K) + (R - a - sigma4*sigma4 / 2)*T) / (sigma4*sqrt(T));
	prix = x*exp(-a*T)*gsl_cdf_ugaussian_P(d1) - exp(-R*T)*K*gsl_cdf_ugaussian_P(d2);


	return 0;
}

int Pricer::put_quanto(double &prix, double S, double Q, double K, double R, double Rf, double sigma1, double sigma2, double rho, double T)
{
	double sigma4 = sqrt(sigma1*sigma1 - 2 * rho*sigma1*sigma2 + sigma2*sigma2);
	double a = R - Rf + rho*sigma1*sigma2 - sigma2*sigma2;
	double x = S / Q;
	if (S <= 0 || Q <= 0 || K <= 0 || sigma1 <= 0 || sigma2 <= 0 || T<0)  {
		//std::cout << "the asset price, the exchange rate, the maturity, the strike and the volatilities must be positive " << std::endl;
		return 10;
	}
	double d1 = (log(x / K) + (R - a + sigma4*sigma4 / 2)*T) / (sigma4*sqrt(T));
	double d2 = (log(x / K) + (R - a - sigma4*sigma4 / 2)*T) / (sigma4*sqrt(T));
	prix = -x*exp(-a*T)*gsl_cdf_ugaussian_P(-d1) + exp(-R*T)*K*gsl_cdf_ugaussian_P(-d2);

	return 0;
}

typedef struct {
	float K;			//Strike price
	float S0;			//Spot price
	float T;			//Time to maturity
	float R;			//Risk-free rate
	float V;			//volatility
	int N;			//Steps
} TOptionData;


static double getU(double T, double vol, int N)
{
	return exp(vol * sqrt(T / (double)N));
}

static double getD(double T, double vol, int N)
{
	return exp(-vol * sqrt(T / (float)N));
}

static double* expiryPutValues(double S0, double K, int N, double u, double d)
{
	double* PutPayoffs = (double*)malloc((N + 1) * sizeof(double));
	double ST;
	double x;
	int i;
	for (i = 0; i <= N; i++) {
		ST = S0 * pow(u, N - i) * pow(d, i);
		x = K - ST;
		PutPayoffs[i] = (x > 0) ? x : 0;
	}
	return PutPayoffs;
}

int Pricer::put_american(double &price, double S0, double K, double T, double R, double vol, int N)
{
	if (S0 <= 0 || K <= 0 || vol <= 0 || T <= 0)  {
		//std::cout << "the asset price, the exchange rate, the maturity, the strike and the volatilities must be positive " << std::endl;
		return 10;
	}
	double *PutPayoffs;

	//calcul des valeurs que l'on aura besoin plus tard pour le pricing
	//pas de temps
	const double      h = T / (float)N;
	const double     rh = R * h;
	//facteurs d'actualisation et de capitalisation
	const double      If = exp(rh);
	const double      Df = exp(-rh);
	//pseudo-probabilites
	const double       u = getU(T, vol, N);
	const double       d = getD(T, vol, N);
	const double      pu = (If - d) / (u - d);
	const double      pd = 1.0 - pu;
	const double  puByDf = pu * Df;
	const double  pdByDf = pd * Df;

	double x;
	double s;
	double payoff;
	int i, j;

	//calcul des payoffs a maturite
	PutPayoffs = expiryPutValues(S0, K, N, u, d);

	//descente dans l'arbre
	for (i = N - 1; i >= 0; i--){
		for (j = 0; j <= i; j++){
			x = puByDf * PutPayoffs[j] + pdByDf * PutPayoffs[j + 1];
			s = S0 * pow(u, i - j) * pow(d, j);
			payoff = K - s;
			payoff = (payoff>0) ? payoff : 0;
			PutPayoffs[j] = (payoff>x) ? payoff : x;
		}
	}

	//le prix en 0 est stocke a la premiere position du tableau
	price = (double)PutPayoffs[0];
	free(PutPayoffs);
	return 0;
}

static double calculate_controle(gsl_vector* simulations, struct simulations::Params data, int J)
{
	double sum = log(gsl_vector_get(simulations, 0)) / 2.0;
	double S_temp;

	for (int i = 1; i < J; i++)
	{
		S_temp = gsl_vector_get(simulations, i);
		sum += log(S_temp);
	}

	sum += log(gsl_vector_get(simulations, J)) / 2.0;

	sum /=J;
	double result = exp(sum);
	result -= data.K;

	if (result > 0)
	{
		return exp(-data.r*data.T)*result;
	}
	return 0;
}

int Pricer::option_asian(double &ic, double &prix, int nb_samples, double T,
	double S0, double K, double sigma, double r, double J)
{
	if (S0 <= 0 || K <= 0 || sigma <= 0 || T <= 0 || nb_samples <= 0 || J <= 0 )  {
		//std::cout << "the asset price, the exchange rate, the maturity, the strike and the volatilities must be positive " << std::endl;
		return 10;
	}

	struct simulations::Params data;
	data.M = nb_samples;
	data.S = S0;
	data.K = K;
	data.r = r;
	data.v = sigma;
	data.T = T;
	//data.J = J;

	double sum = 0;
	double var = 0;

	double payoff;
	gsl_rng *rng = gsl_rng_alloc(gsl_rng_default);
	gsl_rng_set(rng, (unsigned long int)time(NULL));

	gsl_vector* simulations;


	double temp;
	int err = 0;
	double d = sqrt(3.0 / data.T) * (log(data.K / data.S) - (data.r - data.v * data.v / 2.0)* (data.T / 2.0)) / data.v;
	double esperance_controle2 = exp(-data.r*data.T) * (-data.K * gsl_cdf_ugaussian_P(-d) + data.S*exp((data.r - data.v * data.v / 6)*T / 2.0)*gsl_cdf_ugaussian_P(-d + data.v*sqrt(data.T / 3.0)));

	for (int i = 0; i < nb_samples; i++)
	{
		err = simulate_sj_integral(data, J, rng, &simulations);
		payoff = payoff_call_asian(simulations, data, J);
		temp = exp(-r*T) * payoff - calculate_controle(simulations, data, J) + esperance_controle2;
		sum += temp;
		var += temp * temp;
		gsl_vector_free(simulations);
	}



	prix = sum / nb_samples;
	var = var / nb_samples - prix * prix;
	ic = 1.96 * sqrt(var / nb_samples);
	gsl_rng_free(rng);
	return err;
}

int Pricer::option_put_asian(double &ic, double &prix, int nb_samples, double T,
	double S0, double K, double sigma, double r, double J)
{
	if (S0 <= 0 || K <= 0 || sigma <= 0 || T <= 0 || nb_samples <= 0 || J <= 0)  {
		//std::cout << "the asset price, the exchange rate, the maturity, the strike and the volatilities must be positive " << std::endl;
		return 10;
	}

	struct simulations::Params data;
	data.M = nb_samples;
	data.S = S0;
	data.K = K;
	data.r = r;
	data.v = sigma;
	data.T = T;
	//data.J = J;

	double sum = 0;
	double var = 0;

	double payoff;
	gsl_rng *rng = gsl_rng_alloc(gsl_rng_default);
	gsl_rng_set(rng, (unsigned long int)time(NULL));

	gsl_vector* simulations;


	double temp;
	int err = 0;
	double d = sqrt(3.0 / data.T) * (log(data.K / data.S) - (data.r - data.v * data.v / 2.0)* (data.T / 2.0)) / data.v;
	double esperance_controle2 = exp(-data.r*data.T) * (-data.K * gsl_cdf_ugaussian_P(-d) + data.S*exp((data.r - data.v * data.v / 6)*T / 2.0)*gsl_cdf_ugaussian_P(-d + data.v*sqrt(data.T / 3.0)));

	for (int i = 0; i < nb_samples; i++)
	{
		err = simulate_sj_integral(data, J, rng, &simulations);
		payoff = payoff_put_asian(simulations, data, J);
		temp = exp(-r*T) * payoff - calculate_controle(simulations, data, J) + esperance_controle2;
		sum += temp;
		var += temp * temp;
		gsl_vector_free(simulations);
	}



	prix = sum / nb_samples;
	var = var / nb_samples - prix * prix;
	ic = 1.96 * sqrt(var / nb_samples);
	gsl_rng_free(rng);
	return err;
}