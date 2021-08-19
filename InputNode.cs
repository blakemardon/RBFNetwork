using System;
using System.Collections.Generic;
using System.Text;

namespace RBF_Network_Project
{
	class InputNode : Node
	{
		public override double getDerivitiveValue()
		{
			return 1;
		}

		public override double getValue()
		{
			return (double)value;
		}

		public void setValue(double? val)
		{
			value = val;
		}
	}
}
