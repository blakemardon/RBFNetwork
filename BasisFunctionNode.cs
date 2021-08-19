using System;
using System.Collections.Generic;
using System.Text;

namespace RBF_Network_Project
{
	class BasisFunctionNode : Node
	{
		List<double> center;
		double radius;

		public BasisFunctionNode(List<double> centerIn, double radiusIn)
		{
			center = centerIn;
			radius = radiusIn;
		}

		public override double getDerivitiveValue()
		{
			if (input == null)
			{
				getValue();
			}
			return (double)(2 * input * value);
		}

		public override double getValue()
		{
			if(value != null)
			{
				return (double)value;
			}
			double distanceToCenter = 0;
			for(int i = 0; i < previousNodes.Count; i++)
			{
				double temp = previousNodes[i].getValue() * Weights[i] - center[i];
				temp = Math.Abs(temp);
				distanceToCenter += temp * temp;
			}
			distanceToCenter = Math.Sqrt(distanceToCenter);
			input = distanceToCenter;
			value = Math.Pow(Math.E, (-1 * Math.Pow(distanceToCenter, 2)) / Math.Pow(radius, 2));
			if (double.IsNaN((double)value))
			{
				value = 0;
			}
			return (double)value;
		}
	}
}
