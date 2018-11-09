﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class MatrixBehaviour : MonoBehaviour {

	public GameObject VertexPrefab;
	public GameObject EdgePrefab;
	public Text infoText;

	public NeighborhoodMatrix matrix;

	public bool DebugMode;

	private string newVertexName;
	private string newEdgeVertexNameOne;
	private string newEdgeVertexNameTwo;
	private string graphicList;

	[Header("Paint materials")]
	public Material OriginalMaterial;
	public Material MarkedMaterial;

	void Awake () {
		matrix = new NeighborhoodMatrix (VertexPrefab, EdgePrefab, OriginalMaterial, MarkedMaterial);
	}

	public void VertexNameChanged(string _name){
		newVertexName = _name;
	}

	public void EdgeVertexOneNameChanged(string _name){
		newEdgeVertexNameOne = _name;
	}

	public void EdgeVertexTwoNameChanged(string _name){
		newEdgeVertexNameTwo = _name;
	}

	public void GraphicGraphChanged(string _name){
		graphicList = _name;
	}

	public void AddVertex(){
		if (newVertexName != "" && newVertexName != null) {
			matrix.AddVertex (newVertexName);
			if(DebugMode)
				infoText.text = matrix.Print ();
		}
	}

	public void RemoveVertex(){
		if (newVertexName != "" && newVertexName != null) {
			matrix.RemoveVertex (newVertexName);
			if(DebugMode)
				infoText.text = matrix.Print ();
		}
	}

	public void AddEdge(){
		if (newEdgeVertexNameOne != "" && newEdgeVertexNameOne != null && newEdgeVertexNameTwo != "" && newEdgeVertexNameTwo != null) {
			matrix.AddEdge (newEdgeVertexNameOne, newEdgeVertexNameTwo);
			if(DebugMode)
				infoText.text = matrix.Print ();
		}
	}

	public void RemoveEdge(){
		if (newEdgeVertexNameOne != "" && newEdgeVertexNameOne != null && newEdgeVertexNameTwo != "" && newEdgeVertexNameTwo != null) {
			matrix.RemoveEdge (newEdgeVertexNameOne, newEdgeVertexNameTwo);
			if(DebugMode)
				infoText.text = matrix.Print ();
		}
	}

	public void CheckCycles(){
		infoText.text = matrix.CheckCycles ();
	}

	public void CheckGraphic(){
		if (graphicList == "" || graphicList == null) {

			return;
		}
		List<int> graph = graphicList.Split(',').Select(Int32.Parse).ToList();
		string output = "";
		if (sequenceIsGraphic (graph)) {
			output += "Is Graphic\n";
			MathNeighborhoodMatrix mat = new MathNeighborhoodMatrix ();
			mat.Construct (graph);
			output += mat.Print ();
		} else {
			output += "Not graphic\n";
		}
		infoText.text = output;
	}

	private bool sequenceIsGraphic(List<int> sequence)
	{
		var temp = new List<int>(sequence);
		int count = temp[0];
		if(count >= sequence.Count)
		{
			return false;
		}
		temp.RemoveAt(0);
		temp = temp.OrderByDescending(i => i).ToList();
		for (int i = 0; i < count; i++)
		{
			temp[i] -= 1;
		}

		if (temp.TrueForAll(s => s == 0))
		{
			return true;
		}

		if (temp.Exists(s => s < 0))
		{
			return false;
		}
		return sequenceIsGraphic(temp);
	}

	public void CheckMinCycle(){
		int minValue = matrix.LowestValue ();
		if (minValue < 2) {
			infoText.text = matrix.Print () + " \n\n Za mały minimalny stopień aby znaleźć cykl! ";
			return;
		}
		minValue++;
		infoText.text = matrix.CheckCyclesOfLength (minValue);
	}
}