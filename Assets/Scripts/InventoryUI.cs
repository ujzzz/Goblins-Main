using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour {

	public static int arrowsLeft;
	Text arrowInventoryUI;

	// Use this for initialization
	void Start () {
		arrowsLeft = 30;
		arrowInventoryUI = GetComponent<Text> ();
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		arrowInventoryUI.text = "Arrows Left: " + arrowsLeft;
	
	}
}
