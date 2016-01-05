#include "gsl/gsl_matrix.h"

#define DLLEXP   __declspec( dllexport )

class Everglades
{
private:
	double mVLR;
public:
	DLLEXP double payoff(gsl_matrix path) const;
	DLLEXP Everglades(const double VLR);
	DLLEXP ~Everglades();
	DLLEXP double get_price(double& price, double& ic, gsl_matrix data, int nbSimu);
};
