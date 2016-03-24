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
		return "La matrice de corr�lation n'est pas carr�.";
	}
	case 2:
	{
		return "La matrice de r�sultat n'est pas carr�.";
	}
	case 3:
	{
		return "La matrice de r�sultat n'a pas la m�me taille que la matrice de corr�lation.";
	}
	case 4:
	{
		return "Le vecteur return n'a pas la bonne taille.";
	}
	case 5:
	{
		return "Les vecteurs des prix ont des tailles diff�rentes.";
	}
	case 6:{
		return "La matrice de covariance n'est pas carr�.";
	}
	case 7: {
		return "La taille de la matrice de covariance est diff�rente du nombre d'actifs.";
	}
	case 8:
	{
		return "La matrice de correlation n'a pas la m�me taille que la matrice de covariance.";
	}
	case 9: {
		return "Les tailles des arguments de types matrices et vecteurs ne correspondent pas.";
	}
	case 10: {
		return "Les param�tres prix spot, taux de change, maturit�, strike et volatilit�s doivent �tre positifs.";
	}
	case 12:
	{
		return "nb_day_after doit �tre compris entre 0 et " TOSTRING(PERIODE);
	}
	case 13:
	{
		return "Un des actifs a une valeur n�gative ou nulle en z�ro.";
	}
	case 14:
	{
		return "La valeur liquidative de r�f�rence doit �tre positive!";
	}
	case 15:
	{
		return "Un des prix du path est n�gatif!";
	}
	case 16:
	{
		return "Le temps ne peut pas �tre n�gatif";
	}
	case 17:
	{
		return "Le nombre de pas de temps doit �tre positif";
	}
	case 18: {
		return "Le g�n�rateur al�atoire est null";
	}
	case 19: {
		return "Le pointeur o� stocker l'adresse du r�sultat est null";
	}
	case 20: {
		return "Le nombre de sous-jacents doit �tre positif";
	}
	case 22: {
		return "last_index doit �tre positif";
	}
	case 23: {
		return "La matrice de cholesky n'est pas triangulaire inf�rieure";
	}
	case 24: {
		return "La matrice de corr�lation est null";
	}
	case 25: {
		return "La matrice de corr�lation n'est pas sym�trique d�finie positive";
	}
	case 26: {
		return "La matrice de cholesky o� stocker les r�sultats n'est pas allou�e";
	}
	case 27: {
		return "Une des matrices pass�es en entr�e est NULL";
	}
	case 28: {
		return "La pr�cision doit �tre un r�el strictement positif";
	}
	case 29: {
		return "Le boolean de r�sultat doit �tre allou�";
	}
	case 30: {
		return "Un des vecteurs pass�s en entr�e est NULL";
	}
	case 31: {
		return "la fr�quence est �gale � 0";
	}
	case 32: {
		return "La volatilit� doit �tre positive";
	}
	case 33: {
		return "Le nombre de simulation doit �tre positif";
	}
	default:
	{
		return "Erreur inconnue.";
	}
	}
}