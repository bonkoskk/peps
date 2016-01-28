#ifndef PAYOFF_HPP
#define PAYOFF_HPP
#pragma once

#include <gsl/gsl_rng.h>
#include "simulations.hpp"

extern double payoff_call_barrier_down_out(struct simulations::Params data, int J, double L, gsl_vector* simulations);

extern double payoff_call_vanilla(struct simulations::Params data, double st);

#endif // PAYOFF_HPP