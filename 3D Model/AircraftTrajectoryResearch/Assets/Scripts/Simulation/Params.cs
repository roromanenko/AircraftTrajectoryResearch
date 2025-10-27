using UnityEngine;

public class Params
{
	//Aircraft geometry and inertia
	public double S { get; set; } = 201.45;        //S, m^2
	public double l { get; set; } = 37.55;         //l, m
	public double G0 { get; set; } = 60000;         //G, kg
	public double Gp0 { get; set; } = 20000;         //G, kg
	public double Ix { get; set; } = 250000;       //Ix, kg m s^2
	public double Iy { get; set; } = 875000;       //Iy, kg m s^2
	public double q_dv { get; set; } = 0.585;       //Iz, kg m s^2

	// Flight conditions
	public double V { get; set; } = 236;          // V0, m/s
	public double H { get; set; } = 11050;           // H0, m
	public double Rho { get; set; } = 0.0372;      // ρ, кг·с^2/м^4
	public double g { get; set; } = 9.73;          // g, m/s^2
	public double Alpha_bal { get; set; } = 6.04;        // alpha, degrees
	public double Theta0 { get; set; } = 0;        // Theta, degrees

	// Pitching moment
	public double m_y_omega_y { get; set; } = -0.145;
	public double m_y_beta { get; set; } = -0.1719;
	public double m_y_delta_n { get; set; } = -0.0716;
	public double m_x_delta_n { get; set; } = -0.01719;
	public double m_x_omega_y { get; set; } = -0.11;
	public double m_x_omega_x { get; set; } = -0.66;
	public double m_x_beta { get; set; } = -0.1146;
	public double m_x_delta_e { get; set; } = -0.043;
	public double m_y_delta_e { get; set; } = 0.0;
	public double m_y_omega_x { get; set; } = -0.006;

	// Aerodynamic coefficients
	public double C_z_beta { get; set; } = -0.8595;
	public double C_z_delta_n { get; set; } = -0.1759;

	// Control law params general
	public double k_gamma { get; set; } = 2;
	public double k_omega_x { get; set; } = 1.5;
	public double k_omega_y { get; set; } = 2.5;

	// Control law params 1
	public double k_gamma_set { get; set; } = 0.7;

	// Control law params 3
	public double k_z { get; set; } = 0.02;
	public double k_zDot { get; set; } = 0.7;
}


