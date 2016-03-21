#ifndef PAYOFF_HPP
#define PAYOFF_HPP
#pragma once

#include <gsl/gsl_rng.h>
#include "simulations.hpp"

extern double payoff_call_barrier_down_out(struct simulations::Params data, int J, double L, gsl_vector* simulations);

extern double payoff_call_vanilla(struct simulations::Params data, double st);

extern double payoff_call_asian(gsl_vector* simulations, struct simulations::Params data, int J);

#endif // PAYOFF_HPP