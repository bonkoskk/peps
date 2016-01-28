#ifndef EVERGLADES_HPP
#define EVERGLADES_HPP
#pragma once

#include <gsl/gsl_matrix.h>
#include <gsl/gsl_rng.h>
#define DAY 252.0
#define VLR 200.0
#define PERIODE 63.0

#include "dll_define.hpp"

namespace Everglades
{
	/**
	* Permet de calculer le Payoff du produit Everglades
	*
	* @param[out] anticipated est vrai si l'exercice anticipé a eu lieu.
	*
	* @param[in] path contient une trajectoire avec les prix aux 24 dates de constatation et à la date d'émission.
	* C'est une matrice de taille nb_ss_Jacents x 25.
	*/
	DLLEXP extern double get_payoff(const gsl_matrix &path, double vlr, bool &anticipated);

	/**
	* Permet de calculer le prix du produit everglades
	*
	* @param[out] price : contient le prix du produit.
	* @param[out] ic : contient la largeur du demi intervalle de confiance centré sur le prix.
	* @param[in] historic : contient une trajectoire avec les prix constatés passés ainsi que le prix du jour.
	* C'est une matrice de taille nb_sous_jacents x (nb_dates_constatations_passées + 1 ou + 2).
	* @param[in] nb_day_after : nombre de jour ouvré depuis la dernière date de constatations.
	* @param[in] r1 : taux d'intérêt (en cas d'exercice anticipé) jusqu'à la date d'exercice anticipée.
	* @param[in] r2 : taux d'intérêt (en cas d'exercice NON anticipé) jusqu'à la date de fin du produit.
	* @param[in] expected_returns : performances attendues des sous-jacents.
	* @param[in] vol : vecteur de volatilité des actifs sous jacents.
	* @param[in] correl : matrice de corrélation des actifs sous jacents.
	* @param[in] nbSimu : nombre de simulation.
	*/
	DLLEXP extern int get_price(double& price, double& ic, gsl_vector** delta, const gsl_matrix& historic, int nb_day_after, double r,
		const gsl_vector& expected_returns, const gsl_vector& vol, const gsl_matrix& correl, int nbSimu);
};

#endif // EVERGLADES_HPP