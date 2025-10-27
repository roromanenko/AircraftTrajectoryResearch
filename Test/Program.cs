using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AircraftTrajectoryResearch
{
	internal class Program
	{
		static void Main(string[] args)
		{
			var parameters = new Params { };
			var coeff = new CoefficientsCalculator();
			var controlLaw = new CalculateControlLaw();

			var sim = new Simulation(coeff, parameters, controlLaw);

			double dt = 0.01;
			double w = 0;
			double NV = 0;
			int controlLawNumber = 3;
			double X0 = -50000;
			double Z0 = 2000;

			SimulationResult result = sim.Run(dt, w, NV, controlLawNumber, X0, Z0);

			Console.WriteLine("=== X ===");

			for (int i = 0; i < 20; i++)
			{
				Console.WriteLine(result.X[i]);
			}

			Console.WriteLine("=== Z ===");

			for (int i = 0; i < 20; i++)
			{
				Console.WriteLine(result.Z[i]);
			}

			Console.WriteLine("=== Gamma ===");

			for (int i = 0; i < 20; i++)
			{
				Console.WriteLine(result.Gamma[i]);
			}

			Console.WriteLine("=== Psi_g ===");

			for (int i = 0; i < 20; i++)
			{
				Console.WriteLine(result.Psi_g[i]);
			}

			Console.WriteLine("Sim is finish, enter any button");
			Console.ReadKey();
		}
	}
}
