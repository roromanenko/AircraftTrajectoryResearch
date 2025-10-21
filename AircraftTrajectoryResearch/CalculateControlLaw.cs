using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace AircraftTrajectoryResearch
{
	public class CalculateControlLaw
	{
		public double CalculateFirstLaw
			(
			Params parameters,
			double gamma,
			double omega_x,
			double Vsh,
			double psi_g,
			double Z,
			double X
			)
		{
			double Pzt = 57.3 * Math.Atan(Z / X);
			double KKzt = Pzt - psi_g;
			double gamma_zad_star = parameters.k_gamma_set * Vsh * Math.Sin(KKzt);
			double gamma_zad = Math.Clamp(gamma_zad_star, -20, 20);
			double de = parameters.k_gamma * (gamma - gamma_zad) + parameters.k_omega_x * omega_x;

			return de;
		}

		public double CalculateSecondLaw
			(
			Params parameters, 
			double gamma,
			double omega_x,
			double Vsh,
			double psi_g,
			double Z,
			double X,
			double Z_dot,
			double X_dot
			)
		{
			double Pzt = 57.3 * Math.Atan(Z / X);
			double SHK = 57.3 * Math.Atan(Z_dot / X_dot);
			double delta_SHK = Pzt - SHK;
			double gamma_zad_star = parameters.k_gamma_set * Vsh * Math.Sin(delta_SHK);
			double gamma_zad = Math.Clamp(gamma_zad_star, -20, 20);
			double de = parameters.k_gamma * (gamma - gamma_zad) + parameters.k_omega_x * omega_x;

			return de;
		}

		public double CalculateThirdLaw
			(
			Params parameters,
			double Z,
			double Z_dot,
			double omega_x,
			double gamma
			)
		{
			double gamma_zad_star = -(parameters.k_z * Z + parameters.k_zDot * Z_dot);
			double gamma_zad = Math.Clamp(gamma_zad_star, -20, 20);
			double de = parameters.k_gamma * (gamma - gamma_zad) + parameters.k_omega_x * omega_x;

			return de;
		}
	}
}
