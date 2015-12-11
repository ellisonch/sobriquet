using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sobriquet {
	/// <summary>
	/// Represents a name and associated weight (how many times that name counts).
	/// </summary>
	public struct WeightedName {
		public string Name;
		public int Weight;
		public WeightedName(string name, int weight) {
			Name = name;
			Weight = weight;
		}
	}
}
