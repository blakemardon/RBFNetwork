using System;
using System.Collections.Generic;

namespace RBF_Network_Project
{
	class Program
	{
		static void Main(string[] args)
		{
			RBFNetwork rBF = new RBFNetwork(16, 1, 4, "TrainingDataNorm.csv");
			List<double> test = new List<double>();
			Console.WriteLine(rBF.getAccuracy("evaluationData.csv"));
			rBF.train(1000, 0.007, 0.005);
			Console.WriteLine(rBF.getAccuracy("evaluationData.csv"));
		}
		
	}
}
