using System;
using System.Collections.Generic;
using System.Text;

namespace RBF_Network_Project
{
	abstract class Node
	{
		protected double? value = null;
		protected double? input = null;
		public List<double> Weights { get; set; }
		public List<Node> previousNodes;
		public List<double> PreviousWeightDeltas { get; set; }

		public Node()
		{
			PreviousWeightDeltas = new List<double>();
		}
		public void SetPreviousNodes(List<Node> prevNodes)
		{
			previousNodes = prevNodes;
		}

		public void SetWeights(List<double> weightsIn)
		{
			Weights = weightsIn;
			for(int i = 0; i < weightsIn.Count; i++)
			{
				PreviousWeightDeltas.Add(0);
			}
		}

		public void Clear()
		{
			value = null;
			input = null;
		}
		public abstract double getValue();
		public abstract double getDerivitiveValue();
	}
}
