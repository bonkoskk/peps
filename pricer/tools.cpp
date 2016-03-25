#include <iostream>
#include <cmath>
#include <stdexcept>
#include <gsl/gsl_blas.h>
#include <gsl/gsl_linalg.h>
#include <gsl/gsl_vector.h>
#include <gsl/gsl_matrix.h>
#include "tools.hpp"
#include <math.h>

extern int fact_cholesky(const gsl_matrix* correl, gsl_matrix* result)
{
	if (correl == NULL) {
		return 24;
	}
	if (result == NULL) {
		return 26;
	}
	int size = correl->size1;
	if (size != (int)correl->size2) {
		//std::cout << "bad size for correl matrix. The matrix is not square." << std::endl;
		return 1;
	}
	if (result->size1 != result->size2) {
		//std::cout << "bad size for result matrix. The matrix is not square." << std::endl;
		return 2;
	}
	if ((int)result->size1 != size) {
		//std::cout << "result should have same dimensions as correl." << std::endl;
		return 3;
	}
	gsl_matrix_memcpy(result, correl);
	gsl_error_handler_t *error_handler = gsl_set_error_handler_off();
	if (gsl_linalg_cholesky_decomp(result)) {
		return 25;
	}
	gsl_set_error_handler(error_handler);
	for (int i = 0; i < size; i++) {
		for (int j = i + 1; j < size; j++){
			gsl_matrix_set(result, i, j, 0);
		}
	}
	return 0;
}

extern int matrices_approx_equal(const gsl_matrix *mat1, const gsl_matrix *mat2, double precision, bool *result)
{
	if (mat1 == NULL || mat2 == NULL) {
		return 27;
	}
	if (result == NULL) {
		return 29;
	}
	if (precision <= 0) {
		return 28;
	}
	if ((mat1->size1 != mat2->size1) || (mat1->size2 != mat2->size2)) {
		//std::cout << "The matrices are not of same size." << std::endl;
		*result = false;
		return 0;
	}
	for (int i = 0; i < (int)mat1->size1; i++) {
		for (int j = 0; j < (int)mat1->size2; j++) {
			if (fabs(gsl_matrix_get(mat1, i, j) - gsl_matrix_get(mat2, i, j)) > precision)  {
				*result = false;
				return 0;
			}
		}
	}
	*result = true;
	return 0;
}

// Based on John Hull's "Options, Futures and other derivatives" V7
// chap.21 "Estimating volatilities and correlations"
extern int compounded_returns(const gsl_vector* prices, gsl_vector* returns)
{
	if (returns == NULL || prices == NULL) {
		return 30;
	}
	if (prices->size != returns->size + 1) {
		//std::cout << "bad size for returns vector. The vector should be of length returns->size + 1." << std::endl;
		return 4;
	}
	int nb_date = prices->size;
	for (int i = 0; i < nb_date - 1; i++)
	{
		gsl_vector_set(returns, i, (gsl_vector_get(prices, i + 1) - gsl_vector_get(prices, i)) / gsl_vector_get(prices, i));
	}
	return 0;
}

 // Based on John Hull's "Options, Futures and other derivatives" V7
// chap.21 "Estimating volatilities and correlations"

extern int historic_volatility(double& hist_vol, gsl_vector* prices)
{

	if (prices == NULL) {
		return 30;
	}
	int nb_date = prices->size;
	gsl_vector* returns = gsl_vector_alloc(nb_date - 1);
	compounded_returns(prices, returns);
	double sum2 = 0;
	// returns average is assumed to be zero, so we dont have to compute it
	double temp;
	for (int i = 0; i < nb_date - 1; i++)
	{
		temp = gsl_vector_get(returns, i);
		sum2 += temp * temp;
	}
	double var = sum2 / (nb_date - 2);
	gsl_vector_free(returns);
	hist_vol = sqrt(var);
	return 0;
}
//calcul de la volatilité de Yang Zhang : utilise la volatilité(overnight) , la volatilité(open to close)
//ainsi que la volatilité de Rogers-Stachell
extern int historic_Yang_Zhang_Volatility(double& yz_vol, gsl_vector* open_prices, gsl_vector* close_prices, gsl_vector* high_prices, gsl_vector* low_prices, double frequency)
{
	// N
	if (open_prices == NULL || close_prices == NULL || high_prices == NULL || low_prices == NULL) {
		return 30;
	}
	if (frequency == 0.0){
		return 31;
	}
	if (open_prices->size != close_prices->size || close_prices->size != high_prices->size || open_prices->size != low_prices->size || high_prices->size != low_prices->size) {
		//std::cout << "vectors of prices have not the same size." << std::endl;
		return 5;
	}
	/*
	int nb_dates = close_prices->size;
	gsl_vector* open_returns = gsl_vector_alloc(nb_dates - 1);
	compounded_returns(open_prices, open_returns);
	gsl_vector* close_returns = gsl_vector_alloc(nb_dates - 1);
	compounded_returns(close_prices, close_returns);
	double mean_co=mean_close_open(open_prices,close_prices);
	double mean_oc=mean_open_close(open_prices,close_prices);
	double overnight_vol=0;
	for (int i =1;i<nb_dates;i++){
	overnight_vol+=(log(gsl_vector_get(open_returns,i)/gsl_vector_get(close_returns,i-1))-mean_co)*(log(gsl_vector_get(open_returns,i)/gsl_vector_get(close_returns,i-1))-mean_co);
	}
	overnight_vol= (1/(nb_dates-1))*overnight_vol;
	double open_to_close_vol=0;
	for (int i =1;i<nb_dates;i++){
	open_to_close_vol+=(log(gsl_vector_get(close_returns,i)/gsl_vector_get(open_returns,i))-mean_oc)*(log(gsl_vector_get(close_returns,i)/gsl_vector_get(open_returns,i))-mean_oc);
	}
	open_to_close_vol= (1/(nb_dates-1))*open_to_close_vol;
	double& rs_vol=yz_vol;
	historic_Rogers_Stachell_volatility(rs_vol,open_prices,close_prices,high_prices,low_prices,frequency);
	double k = 0.34/(1.34+(nb_dates+1)/(nb_dates-1));

	yz_vol= sqrt(frequency)*sqrt(overnight_vol+k*open_to_close_vol+(1-k)*rs_vol);
	*/
	int nb_dates = close_prices->size;
	double mean_co = mean_close_open(open_prices, close_prices);
	double mean_oc = mean_open_close(open_prices, close_prices);
	double overnight_vol = 0;
	for (int i = 1; i < nb_dates; i++){
		overnight_vol += (log(gsl_vector_get(open_prices, i) / gsl_vector_get(close_prices, i - 1)) - mean_co)*(log(gsl_vector_get(open_prices, i) / gsl_vector_get(close_prices, i - 1)) - mean_co);
	}
	overnight_vol = (1 / (nb_dates - 1))*overnight_vol;
	double open_to_close_vol = 0;
	for (int i = 1; i < nb_dates; i++){
		open_to_close_vol += (log(gsl_vector_get(close_prices, i) / gsl_vector_get(open_prices, i)) - mean_oc)*(log(gsl_vector_get(close_prices, i) / gsl_vector_get(open_prices, i)) - mean_oc);
	}
	open_to_close_vol = (1 / (nb_dates - 1))*open_to_close_vol;
	double& rs_vol = yz_vol;
	historic_Rogers_Stachell_volatility(rs_vol, open_prices, close_prices, high_prices, low_prices, frequency);
	double k = 0.34 / (1.34 + (nb_dates + 1) / (nb_dates - 1));

	yz_vol = sqrt(frequency)*sqrt(overnight_vol + k*open_to_close_vol + (1 - k)*rs_vol);
	return 0;

}
// calcul de la volatilité de Rogers-Stachell
extern int historic_Rogers_Stachell_volatility(double& rs_vol, gsl_vector* open_prices, gsl_vector* close_prices, gsl_vector* high_prices, gsl_vector* low_prices, double frequency){
	if (open_prices == NULL || close_prices == NULL || high_prices == NULL || low_prices == NULL) {
		return 30;
	}
	if (open_prices->size != close_prices->size || close_prices->size != high_prices->size || open_prices->size != low_prices->size || high_prices->size != low_prices->size) {
		//std::cout << "vectors of prices have not the same size." << std::endl;
		return 5;
	}

	if (frequency == 0.0){
		return 31;
	}
	int nb_dates = close_prices->size;
	/*
	gsl_vector* open_returns = gsl_vector_alloc(nb_dates - 1);
	compounded_returns(open_prices, open_returns);
	gsl_vector* close_returns = gsl_vector_alloc(nb_dates - 1);
	compounded_returns(close_prices, close_returns);
	gsl_vector* high_returns = gsl_vector_alloc(nb_dates - 1);
	compounded_returns(high_prices, high_returns);
	gsl_vector* low_returns = gsl_vector_alloc(nb_dates - 1);
	compounded_returns(low_prices, low_returns);
	double sum=0;
	for(int i=1; i<=nb_dates ;i++){
	sum+= log(gsl_vector_get(high_returns,i)/gsl_vector_get(open_returns,i))*log(gsl_vector_get(high_returns,i)/gsl_vector_get(close_returns,i))+log(gsl_vector_get(low_returns,i)/gsl_vector_get(open_returns,i))*log(gsl_vector_get(low_returns,i)/gsl_vector_get(close_returns,i));
	}
	rs_vol = sqrt(frequency/nb_dates)*sqrt(sum);
	*/

	double sum = 0;
	for (int i = 0; i < nb_dates; i++){
		sum += log(gsl_vector_get(high_prices, i) / gsl_vector_get(open_prices, i))*log(gsl_vector_get(high_prices, i) / gsl_vector_get(close_prices, i)) + log(gsl_vector_get(low_prices, i) / gsl_vector_get(open_prices, i))*log(gsl_vector_get(low_prices, i) / gsl_vector_get(close_prices, i));
	}
	rs_vol = sqrt(frequency / nb_dates)*sqrt(sum);
	return 0;
}
// Moyenne des log(open_prices(i)/close_prices(i-1))
double mean_close_open(gsl_vector* open_prices, gsl_vector* close_prices){

	if (open_prices == NULL || close_prices == NULL) {
		return 30;
	}
	int nb_dates = close_prices->size;
	/*
	gsl_vector* open_returns = gsl_vector_alloc(nb_dates - 1);
	compounded_returns(open_prices, open_returns);
	gsl_vector* close_returns = gsl_vector_alloc(nb_dates - 1);
	compounded_returns(close_prices, close_returns);
	double sum=0;
	for (int i =1;i<nb_dates;i++){
	sum+=log(gsl_vector_get(open_returns,i)/gsl_vector_get(close_returns,i-1));

	}
	double res = 1/(nb_dates-1)*sum;
	*/
	double sum = 0;
	for (int i = 1; i < nb_dates; i++){
		sum += log(gsl_vector_get(open_prices, i) / gsl_vector_get(close_prices, i - 1));

	}
	double res = 1 / (nb_dates - 1)*sum;
	return res;
}
// Moyenne des log(close_prices/open_prices)
double mean_open_close(gsl_vector* open_prices, gsl_vector* close_prices){
	int nb_dates = close_prices->size;
	if (open_prices == NULL || close_prices == NULL) {
		return 30;
	}
	/*
	gsl_vector* open_returns = gsl_vector_alloc(nb_dates - 1);
	compounded_returns(open_prices, open_returns);
	gsl_vector* close_returns = gsl_vector_alloc(nb_dates - 1);
	compounded_returns(close_prices, close_returns);
	double sum=0;
	for (int i =1;i<nb_dates;i++){
	sum+=log(gsl_vector_get(close_returns,i)/gsl_vector_get(open_returns,i));

	}
	double res = 1/(nb_dates-1)*sum;
	*/
	double sum = 1;
	for (int i = 0; i < nb_dates; i++){
		sum += log(gsl_vector_get(close_prices, i) / gsl_vector_get(open_prices, i));

	}
	double res = 1 / (nb_dates - 1)*sum;
	return res;
}

double compute_covariance_for2(const gsl_vector* returns1, const gsl_vector* returns2)
{

	if (returns1->size != returns2->size)  {
		//std::cout << "the 2 vectors must have the same size " << std::endl;
	}

	int nb_returns = returns1->size;
	if (returns1->size == 1)  {
		//std::cout << "the size of the return vector can not be 1 " << std::endl;
	}
	double sum = 0;
	double sum_returns1 = 0;
	double sum_returns2 = 0;
	for (int i = 0; i < nb_returns; i++){
		sum_returns1 += gsl_vector_get(returns1, i);
		sum_returns2 += gsl_vector_get(returns2, i);
	}
	sum_returns1 = sum_returns1 / nb_returns;
	sum_returns2 = sum_returns2 / nb_returns;

	for (int i = 0; i < nb_returns; i++)
	{
		sum += (gsl_vector_get(returns1, i) - sum_returns1) * (gsl_vector_get(returns2, i) - sum_returns2);
	}
	return sum / (nb_returns - 1);
}
double compute_covariance_for2_Yang_Zhang(gsl_vector* open_prices1, gsl_vector* close_prices1, gsl_vector* high_prices1, gsl_vector* low_prices1, gsl_vector* open_prices2, gsl_vector* close_prices2, gsl_vector* high_prices2, gsl_vector* low_prices2){
	int nb_dates = open_prices1->size;
	double overnight_vol = 0;
	double mean_co1 = mean_close_open(open_prices1, close_prices1);
	double mean_co2 = mean_close_open(open_prices2, close_prices2);
	double mean_oc1 = mean_open_close(open_prices1, close_prices1);
	double mean_oc2 = mean_open_close(open_prices2, close_prices2);

	for (int i = 1; i < nb_dates; i++){
		overnight_vol += (log(gsl_vector_get(open_prices1, i) / gsl_vector_get(close_prices1, i - 1)) - mean_co1)*(log(gsl_vector_get(open_prices2, i) / gsl_vector_get(close_prices2, i - 1)) - mean_co2);
	}
	overnight_vol = (1 / (nb_dates - 1))*overnight_vol;
	double open_to_close_vol = 0;
	for (int i = 1; i < nb_dates; i++){
		open_to_close_vol += (log(gsl_vector_get(close_prices1, i) / gsl_vector_get(open_prices1, i)) - mean_oc1)*(log(gsl_vector_get(close_prices2, i) / gsl_vector_get(open_prices2, i)) - mean_oc2);
	}
	open_to_close_vol = (1 / (nb_dates - 1))*open_to_close_vol;
	return sqrt(365)*sqrt(open_to_close_vol*open_to_close_vol + overnight_vol*overnight_vol);

}

extern int compute_covariance(const gsl_matrix* prices, gsl_matrix* covariance)
{
	if (prices == NULL || covariance == NULL) {
		return 27;
	}
	if (covariance->size1 != covariance->size2) {
		//std::cout << "bad size for covariance matrix. The matrix is not square." << std::endl;
		return 6;
	}
	if (covariance->size1 != prices->size1) {
		//std::cout << "error :  The covariance matrix should be of size prices->size1." << std::endl;
		return 7;
	}

	int nb_assets = prices->size1;
	int nb_dates = prices->size2;
	gsl_matrix* returns = gsl_matrix_alloc(nb_assets, nb_dates - 1);
	gsl_vector* temp_price = gsl_vector_alloc(nb_dates);
	gsl_vector* temp_returns = gsl_vector_alloc(nb_dates - 1);

	// Computation and storage of the compounded returns of all assets 
	for (int i = 0; i < nb_assets; i++)
	{
		gsl_matrix_get_row(temp_price, prices, i);
		compounded_returns(temp_price, temp_returns);
		gsl_matrix_set_row(returns, i, temp_returns);
	}

	// Computation of the correlations
	gsl_vector* temp_returns2 = gsl_vector_alloc(nb_dates - 1);
	double temp;
	for (int i = 0; i < nb_assets; i++)
	{
		gsl_matrix_get_row(temp_returns, returns, i);
		for (int j = 0; j <= i; j++)
		{
			gsl_matrix_get_row(temp_returns2, returns, j);
			temp = compute_covariance_for2(temp_returns, temp_returns2);

			//covariance[i,j] = computeCovarianceFor2(date_nb, returns[i], returns[j]);
			gsl_matrix_set(covariance, i, j, temp);
			gsl_matrix_set(covariance, j, i, temp);
		}
	}
	gsl_matrix_free(returns);
	gsl_vector_free(temp_price);
	gsl_vector_free(temp_returns);
	gsl_vector_free(temp_returns2);
	return 0;
}

extern int compute_covariance_Yang_Zhang(gsl_matrix* open_prices, gsl_matrix* close_prices, gsl_matrix* high_prices, gsl_matrix* low_prices, gsl_matrix* covariance)
{
	if (open_prices == NULL || close_prices == NULL || high_prices == NULL || low_prices == NULL || covariance == NULL) {
		return 27;
	}
	if (covariance->size1 != covariance->size2) {
		//std::cout << "bad size for covariance matrix. The matrix is not square." << std::endl;
		return 6;
	}
	if (covariance->size1 != open_prices->size1) {
		//std::cout << "error :  The covariance matrix should be of size prices->size1." << std::endl;
		return 7;
	}

	int nb_assets = open_prices->size1;
	int nb_dates = open_prices->size2;
	gsl_vector* temp_open_prices1 = gsl_vector_alloc(nb_dates);
	gsl_vector* temp_close_prices1 = gsl_vector_alloc(nb_dates);
	gsl_vector* temp_high_prices1 = gsl_vector_alloc(nb_dates);
	gsl_vector* temp_low_prices1 = gsl_vector_alloc(nb_dates);
	gsl_vector* temp_open_prices2 = gsl_vector_alloc(nb_dates);
	gsl_vector* temp_close_prices2 = gsl_vector_alloc(nb_dates);
	gsl_vector* temp_high_prices2 = gsl_vector_alloc(nb_dates);
	gsl_vector* temp_low_prices2 = gsl_vector_alloc(nb_dates);

	double temp;
	for (int i = 0; i < nb_assets; i++)
	{
		gsl_matrix_get_row(temp_open_prices1, open_prices, i);
		gsl_matrix_get_row(temp_close_prices1, close_prices, i);
		gsl_matrix_get_row(temp_high_prices1, high_prices, i);
		gsl_matrix_get_row(temp_low_prices1, low_prices, i);

		for (int j = 0; j <= i; j++)
		{
			gsl_matrix_get_row(temp_open_prices2, open_prices, i);
			gsl_matrix_get_row(temp_close_prices2, close_prices, i);
			gsl_matrix_get_row(temp_high_prices2, high_prices, i);
			gsl_matrix_get_row(temp_low_prices2, low_prices, i);
			if (i == j){
				temp = compute_covariance_for2_Yang_Zhang(temp_open_prices1, temp_close_prices1, temp_high_prices1, temp_low_prices1, temp_open_prices2, temp_close_prices2, temp_high_prices2, temp_low_prices2);
			}
			else{
				historic_Yang_Zhang_Volatility(temp, temp_open_prices1, temp_close_prices1, temp_high_prices1, temp_low_prices1, 365);
			}
			//covariance[i,j] = computeCovarianceFor2(date_nb, returns[i], returns[j]);
			gsl_matrix_set(covariance, i, j, temp);
			gsl_matrix_set(covariance, j, i, temp);
		}
	}
	gsl_matrix_free(open_prices);
	gsl_matrix_free(close_prices);
	gsl_matrix_free(high_prices);
	gsl_matrix_free(low_prices);
	gsl_vector_free(temp_open_prices1);
	gsl_vector_free(temp_close_prices1);
	gsl_vector_free(temp_high_prices1);
	gsl_vector_free(temp_low_prices1);
	gsl_vector_free(temp_open_prices2);
	gsl_vector_free(temp_close_prices2);
	gsl_vector_free(temp_high_prices2);
	gsl_vector_free(temp_low_prices2);

	return 0;
}

extern int get_correlation_and_volatility(const gsl_matrix* covariance, gsl_matrix* correl, gsl_vector* vol)
{

	if (covariance == NULL || correl == NULL || vol == NULL) {
		return 27;
	}
	int size = covariance->size1;
	if (size != (int)correl->size2) {
		//std::cout << "bad size for correl matrix. The matrix is not square." << std::endl;
		return 1;
	}
	if (size != (int)covariance->size2) {
		//std::cout << "bad size for covariance matrix. The matrix is not square." << std::endl;
		return 6;
	}
	if (size != (int)correl->size1) {
		//std::cout << "error :  The correl matrix should have the same dimensions as the covariance." << std::endl;
		return 8;
	}
	if (size != (int)vol->size) {
		//std::cout << "error :  The volatility vector should have the same length as the covariance matrix." << std::endl;
		return 9;
	}
	double tmp;
	// Computation of the volatility vector
	for (int i = 0; i < size; i++)
	{
		tmp = sqrt(gsl_matrix_get(covariance, i, i));
		gsl_vector_set(vol, i, tmp);
	}
	// Computation of the correlation matrix
	gsl_matrix_set_identity(correl);
	for (int i = 0; i < size; i++)
	{
		for (int j = 0; j < i; j++)
		{
			tmp = gsl_matrix_get(covariance, i, j) / (gsl_vector_get(vol, i)*gsl_vector_get(vol, j));
			gsl_matrix_set(correl, i, j, tmp);
			gsl_matrix_set(correl, j, i, tmp);
		}
	}
	return 0;
}