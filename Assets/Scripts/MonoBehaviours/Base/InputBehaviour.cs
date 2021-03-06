﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputBehaviour : MonoBehaviour {

    public static InputBehaviour instance;

    public List<Button> sceneButtons;

    public GameObject linePrefab;
    public Text inputModeName;

    private MatrixBehaviour matrixObject;
    public InputMode inputMode = InputMode.IDLE;

    private GameObject currentSelectedGameObject;
    public GameObject CurrentSelectedGameObject {
        get
        {
            return currentSelectedGameObject;
        }
        set
        {
            if(inputMode == InputMode.IDLE)
            {
                if (value == true)
                {
                    currentSelectedGameObject = value;
                }
            }
            else
            {
                currentSelectedGameObject = value;
            }
        }
    }


    private DrawEdgeObject CurrentDrawingLine;
    public Text EdgeCostTextComponent;
    private string edgeCostString;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        matrixObject = GetComponent<MatrixBehaviour>();
        edgeCostString = "";
    }

    // Update is called once per frame
    void Update () {

        HandleSwitch();
        HandleInput();

        OpenStream();
	}

    void OpenStream()
    {
        if(CurrentDrawingLine != null)
        {
            //Writing numbers.
            foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(vKey))
                {
                    int checkInt = GetKeyValue((int)vKey);
                    if(checkInt != -1)
                    {
                        edgeCostString += checkInt.ToString();
                        EdgeCostTextComponent.text = edgeCostString;
                    }

                }
            }

            if (Input.GetKeyDown(KeyCode.Slash))
            {
                edgeCostString += "/";
                EdgeCostTextComponent.text = edgeCostString;
            }

            //Less than zero numbers.
            if (Input.GetKeyDown(KeyCode.Minus))
            {
                int temp;
                int.TryParse(edgeCostString, out temp);
                temp = temp * -1;
                edgeCostString = temp.ToString();
                EdgeCostTextComponent.text = edgeCostString;
            }

            //Deleting last digit of edgeCost.
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                if (edgeCostString.Length > 0)
                {
                    edgeCostString = edgeCostString.Remove(edgeCostString.Length - 1);
                    EdgeCostTextComponent.text = edgeCostString;
                }
            }

            //Letting know that we dont want to draw line anymore.
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Destroy(CurrentDrawingLine.gameObject);
                CurrentDrawingLine = null;
                EdgeCostTextComponent.gameObject.SetActive(false);
                EdgeCostTextComponent.text = "";
                edgeCostString = "";
            }
        }
    }

    private int GetKeyValue(int keyValue)
    {
        if (keyValue >= 48 && keyValue <= 57)
        {
            return keyValue - 48;
        }
        else if (keyValue >= 96 && keyValue <= 105)
        {
            return keyValue - 96;
        }
        else
        {
            return -1; // Not a number... do whatever...
        }
    }

    void HandleSwitch()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if(inputMode == InputMode.IDLE)
            {
                inputMode = InputMode.EDIT;
            }else if (inputMode == InputMode.EDIT)
            {
                inputMode = InputMode.DELETE;
            }else if (inputMode == InputMode.DELETE)
            {
                inputMode = InputMode.IDLE;
            }
            inputModeName.text = inputMode.ToString();
            HandleObjects();
        }
    }

    void HandleObjects()
    {
        bool editMode = true;
        if(inputMode == InputMode.IDLE)
        {
            editMode = false;
        }

        for(int i=0; i<sceneButtons.Count; i++)
        {
            sceneButtons[i].interactable = !editMode;
        }
    }

    void HandleInput()
    {
        if(matrixObject == null)
        {
            return;
        }

        if (inputMode == InputMode.IDLE){
            HandleDrag();
        }
        else if(inputMode == InputMode.EDIT){
            HandleClick();
        }else if(inputMode == InputMode.DELETE){
            HandleDelete();
        }

        
    }

    void HandleClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            
            if (CurrentSelectedGameObject != null)
            {
                VertexObject obj = CurrentSelectedGameObject.GetComponent<VertexObject>();
                if (obj != null)
                {
                    if (CurrentDrawingLine != null)
                    {

                        EndLine(CurrentSelectedGameObject);
                    }
                    else
                    {
                        DrawLine(CurrentSelectedGameObject);
                    }
                }
            }
            else
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePosition = new Vector3(mousePosition.x, mousePosition.y, 0f);

                matrixObject.matrix.construct.AddNewVertex(mousePosition);
                matrixObject.Print();
            }
        }
    }

    void HandleDrag()
    {
        if(CurrentSelectedGameObject != null && CurrentSelectedGameObject.GetComponent<VertexObject>() && Input.GetMouseButton(1))
        {
            MoveObject(CurrentSelectedGameObject);
        }
    }

    void MoveObject(GameObject movingObject)
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition = new Vector3(mousePosition.x, mousePosition.y, 0f);

        movingObject.transform.position = mousePosition;
        movingObject.GetComponent<VertexObject>().UpdateEdges();
    }

    void HandleDelete()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (CurrentSelectedGameObject != null)
            {
                DestroyObject(CurrentSelectedGameObject);
            }
        }
    }

    void DestroyObject(GameObject destroyingObject)
    {
        if (destroyingObject.GetComponent<VertexObject>())
        {
            matrixObject.matrix.construct.RemoveVertex(destroyingObject.GetComponent<VertexObject>().vertexData.VertexName);
        }
        else
        {
            matrixObject.matrix.construct.RemoveEdge(destroyingObject.GetComponent<EdgeObject>().obj1.vertexData.VertexName,
                destroyingObject.GetComponent<EdgeObject>().obj2.vertexData.VertexName);
        }
        matrixObject.Print();
    }

    void DrawLine(GameObject startLine)
    {
        GameObject line = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
        line.GetComponent<DrawEdgeObject>().startLineOnbject = startLine;
        line.GetComponent<DrawEdgeObject>().StartVertexName = startLine.GetComponent<VertexObject>().vertexData.VertexName;
        CurrentDrawingLine = line.GetComponent<DrawEdgeObject>();

        EdgeCostTextComponent.gameObject.SetActive(true);
    }

    void EndLine(GameObject endLine)
    {
        
        string start = CurrentDrawingLine.StartVertexName;
        string end = endLine.GetComponent<VertexObject>().vertexData.VertexName;
        int edgeCost = 1;

        //TODO : Situations when inputed 13/11 etc.

        if (edgeCostString != "")
        {
            if(!int.TryParse(edgeCostString, out edgeCost) && CornerParser.IsGraphic(edgeCostString))
            {
                int edgeCostReturn = -1;
                CornerParser.Parse(edgeCostString, ref edgeCost, ref edgeCostReturn);
                if (edgeCost != 0)
                {
                    addEdge(start, end, edgeCost, Operator.MINUS);
                }
                if (edgeCostReturn != 0)
                {
                    addEdge(end, start, edgeCostReturn);
                }
            }
            else
            {
                addEdge(start, end, edgeCost, Operator.MINUS);
            }
        }else
        {
            addEdge(start, end, edgeCost, Operator.MINUS);
        }


        matrixObject.Print();
        Destroy(CurrentDrawingLine.gameObject);
        CurrentDrawingLine = null;
        EdgeCostTextComponent.gameObject.SetActive(false);
        EdgeCostTextComponent.text = "";
        edgeCostString = "";
    }

    private void addEdge(string start, string end, int edgeCost, Operator sign = Operator.PLUS)
    {
        if (start != end)
        {
            matrixObject.matrix.construct.AddEdge(start, end, edgeCost, sign);
            
        }
    }
}

public enum InputMode { IDLE, EDIT, DELETE}