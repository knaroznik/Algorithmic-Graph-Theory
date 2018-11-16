﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Graph{

	public OList<Vertex> vertexes;

	protected PaintModule brush;
	protected InfoModule info;
	protected ConstructModule construct;
	protected LocationModule locationModule;

	protected NucleusModule nucleus = new NucleusModule();
	protected ConsistencyModule consistency = new ConsistencyModule();

	public Graph(GameObject _vertexPrefab, GameObject _edgePrefab, Material _originalMaterial, Material _markedMaterial){
		vertexes = new OList<Vertex> ();
		locationModule = new LocationModule ();
		info = new InfoModule (this);
		brush = new PaintModule (_originalMaterial, _markedMaterial);
		construct = new ConstructModule (_vertexPrefab, _edgePrefab,brush);
	}

	public int Count{
		get{
			return vertexes.Count;
		}
	}

	public int LowestValue(){
		return info.LowestValue ();
	}

	#region Construct Module 

	public void AddVertex(string _newVertexName){
		construct.AddVertex (_newVertexName, ref vertexes);
	}

	public void RemoveVertex(string _vertexName){
		construct.RemoveVertex (_vertexName, ref vertexes);
	}

	public void AddEdge(string one, string two, string edgeCost){
		construct.AddEdge (one, two, edgeCost, ref vertexes);
	}

	public void AddEdge(string one, string two, int edgeCost){
		construct.AddEdge (one, two, edgeCost, ref vertexes);
	}

	public void RemoveEdge(string one, string two){
		construct.RemoveEdge (one, two, ref vertexes);
	}

	#endregion

	public string Print(){
		string output = "";
		output += " Macierz sąsiedztwa : \n";
		for (int i = 0; i < vertexes.Count; i++)
		{
			output += "\t" + vertexes[i].VertexName;
		}

		for (int i = 0; i < vertexes.Count; i++)
		{
			output += "\n";
			for (int j = 0; j < vertexes.Get(i).Count; j++)
			{
				if(j == 0)
				{
					output += vertexes[i].VertexName;
				}

				output += "\t" + vertexes[i][j];                  
			}
		}
		output += info.PrintInfo ();
		return output;
	}

	public string CheckCycles(){
		string output = Print ();
		output += naiveCycles ();
		output += findCycleMultiplication ();
		return output;
	}

	protected string naiveCycles(){
		int cyclesFound = 0;

		for (int i = 0; i < vertexes.Count; i++) {
			findCycleLength(i, new OList<Vertex>(), i, 3,  ref cyclesFound);
		}


		if(cyclesFound > 0)
		{
			return "\n\nNaive C3 : YES";
		}
		else
		{
			return "\n\nNaive C3 : NO";
		}
	}

	protected string findCycleMultiplication()
	{
		var matrix = VertexesToArray ();
		var matrixN = matrix;
		for (int i = 0; i < 2; i++)
		{
			matrixN = Utilities.MultiplyMatrix(matrixN, matrix);
		}

		var trace = Utilities.MatrixTrace(matrixN);

		if (trace/6 > 0)
		{
			return "\nMULTIPLE : YES";
		}
		else
		{
			return "\nMULTIPLE : NO";
		}
	}

	protected int[][] VertexesToArray(){
		int[][] output = new int[vertexes.Count][];
		for (int i = 0; i < vertexes.Count; i++) {
			output [i] = new int[vertexes.Count];
			for (int j = 0; j < vertexes.Count; j++) {
				output [i] [j] = vertexes [i] [j];
			}
		}
		return output;
	}

	protected bool findCycle(int current, List<int> visited, int parent, ref int cyclesFound)
	{
		visited.Add(current);
		for (int i = 0; i < vertexes[current].Count; i++)
		{
			if (vertexes[current][i] == 1)
			{
				if (!visited.Contains(i))
				{
					findCycle(i, visited, current, ref cyclesFound);
				}
				else
				{
					if(parent < 0)
						continue;

					if (i != parent && vertexes[i][parent] == 1)
					{
						cyclesFound++;
						return true;
					}
				}
			}
		}

		return false;
	}

	protected bool findCycleLength(int current, OList<Vertex> visited, int original, int lenght, ref int cyclesFound)
	{
		if (visited.Count == lenght-1) {
			if (vertexes [original] [current] == 1) {
				cyclesFound++;
				return true;
			} else {
				return false;
			}
		} else {
			visited.Add (vertexes[current]);
			for (int i = 0; i < vertexes.Count; i++) {
				if (vertexes [current] [i] == 1) {
					if (!visited.ToList().Contains (vertexes [i])) {
						OList<Vertex> x = new OList<Vertex> (visited.ToList ());
						findCycleLength (i, x, original, lenght, ref cyclesFound);
					}
				}
			}
		}

		return false;
	}
	#region Zadanie 2
	public string CheckCyclesOfLength(int x){
		string output = "";
		output += Print ();
		output += "\n\n FINDING CYCLE OF MIN(deg(matrix) + 1 \n\n ";
		output += "\nStarting from C" + x;
		for (int i = x; i <= vertexes.Count; i++) {
			OList<Vertex> cycle = naiveCycles (i);
			if (cycle != null) {
				string result = String.Join("->", cycle.ToList().Select(item => item.ToString()).ToArray());
				output += "\nFound cycle C"+ i +" :\n";
				output += result;
				brush.Paint (cycle);
				break;
			}
		}
		return output;
	}

	protected OList<Vertex> naiveCycles(int x){
		for (int i = 0; i < vertexes.Count; i++) {
			OList<Vertex> q = findCycleLength(i, new OList<Vertex>(), i, x);
			if (q != null) {
				return q;
			}
		}
		return null;
	}

	protected OList<Vertex> findCycleLength(int current, OList<Vertex> visited, int original, int lenght)
	{
		if (visited.Count == lenght-1) {
			if (vertexes [original] [current] == 1) {
				visited.Add (vertexes[current]);
				return visited;
			} else {
				return null;
			}
		} else {
			visited.Add (vertexes[current]);
			for (int i = 0; i < vertexes.Count; i++) {
				if (vertexes [current] [i] == 1) {
					if (!visited.ToList().Contains (vertexes [i])) {
						OList<Vertex> x = new OList<Vertex> (visited.ToList ());
						OList<Vertex> q = findCycleLength (i, x, original, lenght);
						if (q != null) {
							return q;
						}

					}
				}
			}
		}

		return null;
	}
	#endregion

	#region Jordan Algorithm

	public bool IsConsistent(){
		return consistency.IsConsistent (vertexes);
	}
		

	public string WriteVertexes(){
		string result = String.Join(" ", vertexes.ToList().Select(item => item.ToString()).ToArray());
		return result;
	}

	public bool HasCycle(){
		OList<Vertex> cycle = null;
		for (int cycleLength = 3; cycleLength <= vertexes.Count; cycleLength++) {
			for (int i = 0; i < vertexes.Count; i++) {
				OList<Vertex> q = findCycleLength (i, new OList<Vertex> (), i, cycleLength);
				if (q != null) {
					cycle = q;
					break;
				}
			}
		}

		if (cycle != null) {
			return true;
		}

		return false;
	}



	#endregion

	public void ResetEdges(){
		construct.ResetEdges (this);
	}

	public void InsertEdges(OList<EdgeStruct> _edges){
		construct.InsertEdges (_edges, this);
	}

	public void PaintConsistency(){
		consistency.PaintConsistency (vertexes, brush);
	}

	public void PaintConsistency(OList<OList<Vertex>> consistencyParts){
		brush.Paint (consistencyParts);
	}

	public OList<EdgeObject> GetEdges(){
		return locationModule.GetEdges (vertexes);
	}

	public string FindNucleus(){
		return nucleus.FindNucleus (this);
	}

	public OList<EdgeStruct> DFSAlgorithm(){
		return locationModule.DFS (this);
	}
}