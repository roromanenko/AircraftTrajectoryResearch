using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AircraftTrajectoryResearch
{
	public sealed class SimulationResult
	{
		public List<double> Time { get; }
		public List<double> X { get; }
		public List<double> Z { get; }
		public List<double> Gamma { get; }
		public List<double> Psi_g { get; }
		public List<double> G_p { get; }

		public SimulationResult(List<double> time, List<double> x, List<double> z, List<double> gamma, List<double> psi_g, List<double> g_p) 
		{
			Time = time;
			X = x;
			Z = z;
			Gamma = gamma;
			Psi_g = psi_g;
			G_p = g_p;
		}
	}

	public class Simulation
	{
		private readonly CoefficientsCalculator _coeff;
		private readonly Params _parameters;
		private readonly CalculateControlLaw _controlLaw;

		public Simulation(CoefficientsCalculator coeff, Params parameters, CalculateControlLaw controlLaw)
		{
			_coeff = coeff;
			_parameters = parameters;
			_controlLaw = controlLaw;
		}

		public SimulationResult Run(double tEnd, double dt, double w, double NV, int controlLawNumber, double X0, double Z0)
		{
			//Result lists
			var time = new List<double>();
			var X = new List<double>();
			var Z = new List<double>();
			var gamma = new List<double>();
			var psi_g = new List<double>();
			var g_p = new List<double>();

			//Var arrays
			double[] x = new double[15];
			double[] y = new double[15];

			//Var additional
			double mass = _parameters.G0 + _parameters.Gp0;
			double DN, DE;

			y[13] = _parameters.Gp0;
			y[8] = X0;
			y[9] = Z0;
			y[0] = 57.3 * Math.Atan(Z0 / X0);

			//Main loop
			for (double t = 0; t < tEnd;)
			{
				var coeffs = _coeff.ComputeCoefficients(_parameters, mass);
				var a = coeffs.A;
				var b = coeffs.B;

				DN = _parameters.k_omega_y * x[0];
				DE = controlLawNumber switch
				{
					1 => _controlLaw.CalculateFirstLaw(_parameters, y[2], x[2], x[10], x[7], y[9], y[8]),
					2 => _controlLaw.CalculateSecondLaw(_parameters, y[2], x[2], x[10], x[7], y[9], y[8], x[9], x[8]),
					3 => _controlLaw.CalculateThirdLaw(_parameters, y[9], x[9], x[2], y[2]),
					_ => _controlLaw.CalculateFirstLaw(_parameters, y[2], x[2], x[10], x[7], y[9], y[8])
				};

				x[0] = x[1];																	//Ψ` Psi`
				x[1] = -a[1] * x[0] - b[6] * x[2] - a[2] * x[5] - a[3] * DN - b[5] * DE;		//Ψ`` Psi``
				x[2] = x[3];																	//gamma`
				x[3] = -a[6] * x[0] - b[1] * x[2] - b[2] * x[5] - a[5] * DN - b[3] * DE;		//gamma``
				x[4] = x[0] + b[7] * x[2] + b[4] * y[2] - a[4] * x[5] - a[7] * DN;				//beta`
				x[5] = y[4] + x[6];																//beta_v
				x[6] = -57.3 * (x[12] / _parameters.V);											//beta_w
				x[7] = -y[0];																	//psi_g
				x[8] = x[10] * Math.Cos((x[7] + y[4]));											//X`
				x[9] = x[10] * Math.Sin((x[7] + y[4]));											//Z`
				x[10] = _parameters.V + x[11];													//V_sh
				x[11] = w * Math.Cos(NV - x[7]);												//Wx
				x[12] = w * Math.Sin(NV - x[7]);												//Wz
				x[13] = -3 * _parameters.q_dv;													//G`p

				y = EulerIntegration(x, y, dt);

				mass = _parameters.G0 + y[13];

				//Results
				time.Add(t);
				X.Add(y[8]);
				Z.Add(y[9]);
				gamma.Add(y[2]);
				psi_g.Add(x[7]);
				g_p.Add(y[13]);

				//Integration step
				t += dt;
			}

			return new SimulationResult(time, X, Z, gamma, psi_g, g_p);
		}

		public static double[] EulerIntegration(double[] x, double[] y, double dt)
		{
			for (int k = 0; k < x.Length; k++)
			{
				y[k] += x[k] * dt;
			}

			return y;
		}
	}
}
