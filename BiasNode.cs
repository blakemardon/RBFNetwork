using System;
using System.Collections.Generic;
using System.Text;

namespace RBF_Network_Project
{
	class BiasNode : Node
	{
		public override double getDerivitiveValue()
		{
			return 1;
		}

		public override double getValue()
		{
			return 1;
		}
	}
}
