namespace AircraftTrajectoryResearch
{
	public class Params
	{
		//Aircraft geometry and inertia
		public double S { get; init; } = 201.45;        //S, m^2
		public double l { get; init; } = 37.55;         //l, m
		public double G0 { get; init; } = 60000;         //G, kg
		public double Gp0 { get; init; } = 20000;         //G, kg
		public double Ix { get; init; } = 250000;       //Ix, kg m s^2
		public double Iy { get; init; } = 875000;       //Iy, kg m s^2
		public double q_dv { get; init; } = 0.585;       //Iz, kg m s^2

		// Flight conditions
		public double V { get; init; } = 236;          // V0, m/s
		public double H { get; init; } = 11050;           // H0, m
		public double Rho { get; init; } = 0.0372;      // ρ, кг·с^2/м^4
		public double g { get; init; } = 9.73;          // g, m/s^2
		public double Alpha_bal { get; init; } = 6.04;        // alpha, degrees
		public double Theta0 { get; init; } = 0;        // Theta, degrees

		// Pitching moment
		public double m_y_omega_y { get; init; } = -0.145;
		public double m_y_beta { get; init; } = -0.1719;
		public double m_y_delta_n { get; init; } = -0.0716;
		public double m_x_delta_n { get; init; } = -0.01719;
		public double m_x_omega_y { get; init; } = -0.11;
		public double m_x_omega_x { get; init; } = -0.66;
		public double m_x_beta { get; init; } = -0.1146;
		public double m_x_delta_e { get; init; } = -0.043;
		public double m_y_delta_e { get; init; } = 0.0;
		public double m_y_omega_x { get; init; } = -0.006;

		// Aerodynamic coefficients
		public double C_z_beta { get; init; } = -0.8595;
		public double C_z_delta_n { get; init; } = -0.1759;

		// Control law params general
		public double k_gamma { get; init; } = 2;
		public double k_omega_x { get; init; } = 1.5;
		public double k_omega_y { get; init; } = 2.5;

		// Control law params 1
		public double k_gamma_set { get; init; } = 0.7;

		// Control law params 3
		public double k_z { get; init; } = 0.02;
		public double k_zDot { get; init; } = 0.7;
	}
}
