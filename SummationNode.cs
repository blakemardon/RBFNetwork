using System;
using System.Collections.Generic;
using System.Text;

namespace RBF_Network_Project
{
	class SummationNode : Node
	{
		public override double getDerivitiveValue()
		{
			return 1;
		}

		public override double getValue()
		{
			double sum = 0;
			for(int i = 0; i < previousNodes.Count; i++)
			{
				sum += (double)(previousNodes[i].getValue() * Weights[i]);
			}

			value = sum;
			return sum;
		}
	}
}
