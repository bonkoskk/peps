#ifndef EVERGLADES_HPP
#define EVERGLADES_HPP
#pragma once

#include <gsl/gsl_matrix.h>
#include <gsl/gsl_rng.h>
#define DAY 365.0
#define VLR 200.0
#define PERIODE 92.0

#include "dll_define.hpp"


namespace Everglades
{
	/**
	* Permet de calculer le Payoff du produit Everglades
	*
	* @param[out] anticipated est vrai si l'exercice anticip� a eu lieu.
	*
	* @param[in] path contient une trajectoire avec les prix aux 24 dates de constatation et � la date d'�mission.
	* C'est une matrice de taille nb_ss_Jacents x 25.
	*/
	DLLEXP extern double get_payoff(const gsl_matrix &path, double vlr, bool &anticipated);
	DLLEXP extern void printtoto();

	/**
	* Permet de calculer le prix du produit everglades
	*
	* @param[out] price : contient le prix du produit.
	* @param[out] ic : contient la largeur du demi intervalle de confiance centr� sur le prix.
	* @param[in] historic : contient une trajectoire avec les prix constat�s pass�s ainsi que le prix du jour.
	* C'est une matrice de taille nb_sous_jacents x (nb_dates_constatations_pass�es + 1 ou + 2).
	* @param[in] nb_day_after : nombre de jour ouvr� depuis la derni�re date de constatations.
	* @param[in] r1 : taux d'int�r�t (en cas d'exercice anticip�) jusqu'� la date d'exercice anticip�e.
	* @param[in] r2 : taux d'int�r�t (en cas d'exercice NON anticip�) jusqu'� la date de fin du produit.
	* @param[in] expected_returns : performances attendues des sous-jacents.
	* @param[in] vol : vecteur de volatilit� des actifs sous jacents.
	* @param[in] correl : matrice de corr�lation des actifs sous jacents.
	* @param[in] nbSimu : nombre de simulation.
	*/
	DLLEXP extern int get_price(double& price, double& ic, gsl_vector** delta, const gsl_matrix& historic, int nb_day_after, double r,
		const gsl_vector& expected_returns, const gsl_vector& vol, const gsl_matrix& correl, int nbSimu);
};

#endif // EVERGLADES_HPP