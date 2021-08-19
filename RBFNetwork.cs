using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace RBF_Network_Project
{
	class RBFNetwork
	{
		List<double> expectedResults;
		List<List<double>> testData;
		List<InputNode> inputNodes;
		List<Node> hiddenNodes;
		List<SummationNode> outputNodes;
		public RBFNetwork(int functionConut, int outputCount, double radius, string trainingData)
		{
			Random rand = new Random();
			// import test data from file
			using (StreamReader reader = new StreamReader(trainingData))
			{
				string line;
				string[] values;
				testData = new List<List<double>>();
				expectedResults = new List<double>();
				while (!reader.EndOfStream)
				{
					line = reader.ReadLine();
					values = line.Split(",");
					expectedResults.Add(double.Parse(values[0]));
					List<double> temp = new List<double>();
					for (int i = 1; i < values.Length; i++)
					{
						temp.Add(double.Parse(values[i]));
					}
					testData.Add(temp);
				}
			}
			//create input nodes
			inputNodes = new List<InputNode>();
			for(int i = 0; i < testData[0].Count; i++)
			{
				inputNodes.Add(new InputNode());
				inputNodes[i].setValue(0);
			}
			//create hidden layer
			List<List<double>> centers = clusterCenters(functionConut);
			hiddenNodes = new List<Node>();
			for (int i = 0; i < centers.Count; i++)
			{
				hiddenNodes.Add(new BasisFunctionNode(centers[i], radius));
			}
			hiddenNodes.Add(new BiasNode());
			foreach (Node n in hiddenNodes)
			{
				List<double> hiddenWeights = new List<double>();
				for (int i = 0; i < inputNodes.Count; i++)
				{
					hiddenWeights.Add(rand.NextDouble());
				}
				n.SetWeights(hiddenWeights);
			}
			foreach (Node n in hiddenNodes)
			{
				n.SetPreviousNodes(inputNodes.Cast<Node>().ToList());
			}
			//create output nodes
			outputNodes = new List<SummationNode>();
			for (int i = 0; i < outputCount; i++)
			{
				SummationNode temp = new SummationNode();
				temp.SetPreviousNodes(hiddenNodes.Cast<Node>().ToList());
				outputNodes.Add(temp);
			}
			foreach (Node n in outputNodes)
			{
				List<double> outputWeights = new List<double>();
				for (int i = 0; i < hiddenNodes.Count; i++)
				{
					outputWeights.Add(rand.NextDouble());
				}
				n.SetWeights(outputWeights);
			}
		}

		public void train(int iterations, double trainingRate, double momentum)
		{
			Random rand = new Random();
			for (int itr = 0; itr < iterations; itr++)
			{
				int currentTestValues = rand.Next(0, testData.Count);
				// calculate output node deltas
				List<double> outputNodeDeltas = new List<double>(); //this will not work for multiple outputs needs fixing
				outputNodeDeltas.Add(expectedResults[currentTestValues] - evaluate(testData[currentTestValues])[0]);

				// calculate hidden node deltas
				List<double> hiddenNodeDeltas = new List<double>();
				for (int i = 0; i < hiddenNodes.Count; i++)
				{
					hiddenNodeDeltas.Add(hiddenNodes[i].getDerivitiveValue() * outputNodeDeltas[0] * outputNodes[0].Weights[i]);
				}

				//update weights
				//update output node weights
				for(int i = 0; i < outputNodeDeltas.Count; i++)
				{
					for (int j = 0; j < outputNodes[i].Weights.Count; j++)
					{
						outputNodes[i].PreviousWeightDeltas[j] = trainingRate * (outputNodeDeltas[i] - outputNodes[i].previousNodes[j].getValue()) + momentum * outputNodes[i].PreviousWeightDeltas[j];
						outputNodes[i].Weights[j] += outputNodes[i].PreviousWeightDeltas[j];
					}
				}
				//update hidden node weights

				for (int i = 0; i < hiddenNodeDeltas.Count; i++)
				{
					for (int j = 0; j < hiddenNodes[i].Weights.Count; j++)
					{
						hiddenNodes[i].PreviousWeightDeltas[j] = trainingRate * (hiddenNodeDeltas[i] - hiddenNodes[i].previousNodes[j].getValue()) + momentum * hiddenNodes[i].PreviousWeightDeltas[j];
						hiddenNodes[i].Weights[j] += hiddenNodes[i].PreviousWeightDeltas[j];
					}
				}
			}
			
		}

		public List<double> evaluate(List<double> parameters)
		{
			List<double> returnList = new List<double>();
			for (int i = 0; i < parameters.Count; i++)
			{
				inputNodes[i].setValue(parameters[i]);
			}
			foreach(Node n in hiddenNodes)
			{
				n.Clear();
			}
			foreach(SummationNode n in outputNodes)
			{
				n.Clear();
			}
			for (int i = 0; i < outputNodes.Count; i++)
			{
				returnList.Add(outputNodes[i].getValue());
			}
			return returnList;
		}

		public double getAccuracy(string testData)
		{
			List<List<double>> observations;
			List<double> measuredResults;
			using (StreamReader reader = new StreamReader(testData))
			{
				string line;
				string[] values;
				observations = new List<List<double>>();
				measuredResults = new List<double>();
				while (!reader.EndOfStream)
				{
					line = reader.ReadLine();
					values = line.Split(",");
					measuredResults.Add(double.Parse(values[0]));
					List<double> temp = new List<double>();
					for (int i = 1; i < values.Length; i++)
					{
						temp.Add(double.Parse(values[i]));
					}
					observations.Add(temp);
				}
			}
			int correctCounter = 0;
			for(int i = 0; i < observations.Count; i++)
			{
				double evalValue = evaluate(observations[i])[0] - 0.5;
				if ( evalValue  > 0 && measuredResults[i] == 1 || evalValue < 0 && measuredResults[i] == 0)
				{
					correctCounter++;
				}
			}
			return correctCounter/(double)observations.Count;
		}

		List<List<double>> clusterCenters(int numberOfCenters)
		{
			Random rand = new Random();
			//initialize centers
			List<List<double>> centers = new List<List<double>>();
			for (int i = 0; i < numberOfCenters; i++)
			{
				centers.Add(testData[i]);
			}
			int[] closestCenter = new int[testData.Count]; // the closest center for each piece of test data
			for (int i = 0; i < closestCenter.Length; i++)
			{
				closestCenter[i] = 0;
			}
			bool centerChange = true;
			while (centerChange)
			{
				//add each observation to a center
				centerChange = false;
				for (int i = 0; i < testData.Count; i++)
				{
					int currentClosest = closestCenter[i];
					double smallestDistance = 0;
					//get the distance to the current closest center
					for (int k = 0; k < testData[0].Count; k++)
					{
						smallestDistance += Math.Pow(testData[i][k] - centers[currentClosest][k], 2);
					}
					//get the distance of other centers and if that distance is smaller then update
					for (int j = 0; j < centers.Count; j++)
					{
						double currentDistance = 0;
						for(int k = 0; k < testData[0].Count; k++)
						{
							currentDistance += Math.Pow(testData[i][k] - centers[j][k], 2);
						}
						if(currentDistance < smallestDistance && currentClosest != j)
						{
							closestCenter[i] = j;
							currentClosest = j;
							smallestDistance = currentDistance;
							centerChange = true;
						}
					}
				}

				// update centers
				List<List<double>>[] centerBuckets = new List<List<double>>[centers.Count];
				for (int i = 0; i < centerBuckets.Length; i++)
				{
					centerBuckets[i] = new List<List<double>>();
				}
				for (int i = 0; i < testData.Count; i++)
				{
					centerBuckets[closestCenter[i]].Add(testData[i]); //add each piece of test data to a bucket
				}
				for (int i = 0; i < centerBuckets.Length; i++)
				{
					List<double>[] valuesToAverage = new List<double>[testData[0].Count];
					for (int j = 0; j < valuesToAverage.Length; j++)
					{
						valuesToAverage[j] = new List<double>();
					}
					for (int j = 0; j < centerBuckets[i].Count; j++)
					{
						for (int k = 0; k < centerBuckets[i][j].Count; k++)
						{
							valuesToAverage[k].Add(centerBuckets[i][j][k]);
						}
					}
					for (int j = 0; j < valuesToAverage.Length; j++)
					{
						double sum = 0;
						for (int k = 0; k < valuesToAverage[j].Count; k++)
						{
							sum += valuesToAverage[j][k];
						}
						centers[i][j] = sum / valuesToAverage[j].Count;
					}
				}
			}
			return centers;
		}
	}
}
