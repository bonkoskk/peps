#include "gsl/gsl_matrix.h"

class Everglades
{
private:
	double mVLR;
public:
	double payoff(gsl_matrix path) const;
	double getPrice(gsl_matrix path, Model model);
	Everglades(const double VLR);
	~Everglades();
};
