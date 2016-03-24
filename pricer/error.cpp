#include "error.hpp"
#include "everglades.hpp"

#define STRINGIFY(x) #x
#define TOSTRING(x) STRINGIFY(x)

extern string get_error_message(int error_num)
{
	switch (error_num)
	{
	case 0:
	{
		return "Ce n'est pas une erreur.";
	}
	case 1:
	{
		return "La matrice de corrélation n'est pas carré.";
	}
	case 2:
	{
		return "La matrice de résultat n'est pas carré.";
	}
	case 3:
	{
		return "La matrice de résultat n'a pas la même taille que la matrice de corrélation.";
	}
	case 4:
	{
		return "Le vecteur return n'a pas la bonne taille.";
	}
	case 5:
	{
		return "Les vecteurs des prix ont des tailles différentes.";
	}
	case 6:{
		return "La matrice de covariance n'est pas carré.";
	}
	case 7: {
		return "La taille de la matrice de covariance est différente du nombre d'actifs.";
	}
	case 8:
	{
		return "La matrice de correlation n'a pas la même taille que la matrice de covariance.";
	}
	case 9: {
		return "Les tailles des arguments de types matrices et vecteurs ne correspondent pas.";
	}
	case 10: {
		return "Les paramètres prix spot, taux de change, maturité, strike et volatilités doivent être positifs.";
	}
	case 12:
	{
		return "nb_day_after doit être compris entre 0 et " TOSTRING(PERIODE);
	}
	case 13:
	{
		return "Un des actifs a une valeur négative ou nulle en zéro.";
	}
	case 14:
	{
		return "La valeur liquidative de référence doit être positive!";
	}
	case 15:
	{
		return "Un des prix du path est négatif!";
	}
	case 16:
	{
		return "Le temps ne peut pas être négatif";
	}
	case 17:
	{
		return "Le nombre de pas de temps doit être positif";
	}
	case 18: {
		return "Le générateur aléatoire est null";
	}
	case 19: {
		return "Le pointeur où stocker l'adresse du résultat est null";
	}
	case 20: {
		return "Le nombre de sous-jacents doit être positif";
	}
	case 22: {
		return "last_index doit être positif";
	}
	case 23: {
		return "La matrice de cholesky n'est pas triangulaire inférieure";
	}
	case 24: {
		return "La matrice de corrélation est null";
	}
	case 25: {
		return "La matrice de corrélation n'est pas symétrique définie positive";
	}
	case 26: {
		return "La matrice de cholesky où stocker les résultats n'est pas allouée";
	}
	case 27: {
		return "Une des matrices passées en entrée est NULL";
	}
	case 28: {
		return "La précision doit être un réel strictement positif";
	}
	case 29: {
		return "Le boolean de résultat doit être alloué";
	}
	case 30: {
		return "Un des vecteurs passés en entrée est NULL";
	}
	case 31: {
		return "la fréquence est égale à 0";
	}
	case 32: {
		return "La volatilité doit être positive";
	}
	case 33: {
		return "Le nombre de simulation doit être positif";
	}
	default:
	{
		return "Erreur inconnue.";
	}
	}
}