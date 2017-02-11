﻿using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Project2GameManager : MonoBehaviour {

	// Car
	[Serializable]
	public class CarSetUp {
		[Tooltip("How many cars are going to spawn")]
		public int toSpawn = 2;
		[Tooltip("The Car prefab")]
		public GameObject prefab;
		[Tooltip("GameObject to organize generated Car(s)")]
		public GameObject holder;
		[Tooltip("The Delivery Car tag")]
		public string tag = "DeliveryCar";

		[HideInInspector]
		public List<GameObject> cars;

		public void Spawn(int needed, GameObject[] pos) {
			for (int i = 0; i < needed; i++) {
				cars.Add(Instantiate(prefab,
					pos[(int)UnityEngine.Random.Range(0, pos.Length)].transform.position,
					Quaternion.identity));
				cars[i].name = "car " + i + " (clone)";
				cars[i].GetComponent<CarBehavior>().index = i;
				cars [i].GetComponent<CarBehavior> ().color = new Color(((int)UnityEngine.Random.Range(0, 256)) / 255.0f,
					((int)UnityEngine.Random.Range(0, 256)) / 255.0f,
					((int)UnityEngine.Random.Range(0, 256)) / 255.0f,
					1);
				cars[i].transform.parent = holder.transform;
			}
		}
	}

	// PickUp
	[Serializable]
	public class PickUpSetUp {
		[Tooltip("The Pick Up prefab")]
		public GameObject prefab;
		[Tooltip("The location to spawn the pick up")]
		public Transform pickPoint;
		[Tooltip("GameObject to organize generated Pick Ups")]
		public GameObject holder;

		public void Spawn(int index) {
			GameObject pickUp = Instantiate(prefab,
				                    pickPoint.position,
				                    pickPoint.localRotation);
			pickUp.name = "PickUp " + index + " (clone)";
			pickUp.GetComponent<PickUpBehavior>().index = index;
			pickUp.transform.parent = holder.transform;
		}
	}

	// Delivery
	[Serializable]
	public class DeliverySetUp {
		[Tooltip("The Delivery Point tag")]
		public string pointTag = "DeliveryPoint";

		[HideInInspector]
		public GameObject[] go;
		[HideInInspector]
		public float totalNeeded;
		[HideInInspector]
		public List<int> indexes;

		public void GetGameObjects(string tag) {
			go = GameObject.FindGameObjectsWithTag(tag);
		}

		public void ClearPoints() {
			for (int i = 0; i < go.Length; i++) {
				go[i].SetActive(false);
			}
		}

		public Vector3 SelectPoint() {
			int index = (int)UnityEngine.Random.Range(0, go.Length);

			if (indexes.Count > 0) {
				while (indexes.Contains(index)) {
					index = (int)UnityEngine.Random.Range(0, go.Length);
				}
			}

			if (indexes.Count >= totalNeeded) {
				indexes.RemoveAt(0);
			}

			indexes.Add(index);
			go[index].SetActive(true);
			return go[index].transform.position;
		}
	}

	// Pedestrians
	[Serializable]
	public class PedestrianSetUp {
		[Tooltip("How many Pedestrian Leaders are going to spawn")]
		public int toSpawn = 10;
		[Tooltip("Pedestrian Leader prefab")]
		public GameObject prefab;
		[Tooltip("GameObject to organize generated Pedestrian Leaders")]
		public GameObject holder;
		[Tooltip("The Pedestrian tag")]
		public string tag = "Pedestrian";
		[Tooltip("The Pedestrin Location Point tag")]
		public string pointTag = "VisitPoint";

		[HideInInspector]
		public List<GameObject> pedestrians;
		[HideInInspector]
		public Transform[] loc;

		public void GetLocatorPoints(string tag) {
			GameObject[] locObject = GameObject.FindGameObjectsWithTag(tag).OrderBy(go => go.name).ToArray();

			loc = new Transform[locObject.Length];
			for (int i = 0; i < loc.Length; i++) {
				loc[i] = locObject[i].transform;
			}
		}

		public void Spawn(int needed) {
			for (int i = 0; i < needed; i++) {
				pedestrians.Add(new GameObject());
				pedestrians[i].name = "PedestrianGroup " + i;
				pedestrians[i].transform.parent = holder.transform;

				GameObject newPedLeader = Instantiate(prefab,
					                          loc[(int)UnityEngine.Random.Range(0, loc.Length)].position,
					                          loc[(int)UnityEngine.Random.Range(0, loc.Length)].localRotation);
				newPedLeader.GetComponent<PedestrianLeaderBehavior>().color = new Color(((int)UnityEngine.Random.Range(0, 256)) / 255.0f,
					((int)UnityEngine.Random.Range(0, 256)) / 255.0f,
					((int)UnityEngine.Random.Range(0, 256)) / 255.0f,
					1);
				newPedLeader.GetComponent<PedestrianLeaderBehavior>().index = i;
				newPedLeader.name = "PedestrianLeader (clone)";
				newPedLeader.transform.parent = pedestrians[i].transform;
			}
		}

		public Vector3 SelectPoint() {
			return loc[(int)UnityEngine.Random.Range(0, loc.Length)].position;
		}

		public void CheckPedestrians(int count) {
			for (int i = 0; i < count; i++) {
				if (pedestrians[i] == null) {
					pedestrians[i] = (new GameObject());
					pedestrians[i].name = "PedestrianGroup " + i;
					pedestrians[i].transform.parent = holder.transform;

					GameObject newPedLeader = Instantiate(prefab,
						                          loc[(int)UnityEngine.Random.Range(0, loc.Length)].position,
						                          loc[(int)UnityEngine.Random.Range(0, loc.Length)].localRotation);
					newPedLeader.GetComponent<PedestrianLeaderBehavior>().color = new Color(((int)UnityEngine.Random.Range(0, 256)) / 255.0f,
						((int)UnityEngine.Random.Range(0, 256)) / 255.0f,
						((int)UnityEngine.Random.Range(0, 256)) / 255.0f,
						1);
					newPedLeader.GetComponent<PedestrianLeaderBehavior>().index = i;
					newPedLeader.name = "PedestrianLeader (clone)";
					newPedLeader.transform.parent = pedestrians[i].transform;
				}
			}
		}
	}

	// Cross Walk
	[Serializable]
	public class CrossWalkSetUp {
		[Tooltip("Cross Walk prefab")]
		public GameObject prefab;
		[Tooltip("GameObject to organize generated Cross Walks")]
		public GameObject holder;
		[Tooltip("The Cross Walk Point tag")]
		public string pointTag = "CrossWalkPoint";

		[HideInInspector]
		public GameObject[] go;
		[HideInInspector]
		public Transform[] loc;

		public void GetLocatorPoints(string tag) {
			GameObject[] locObject = GameObject.FindGameObjectsWithTag(tag).OrderBy(go => go.name).ToArray();

			loc = new Transform[locObject.Length];
			for (int i = 0; i < loc.Length; i++) {
				loc[i] = locObject[i].transform;
			}
		}

		public void Place(int needed) {
			go = new GameObject[needed];

			for (int i = 0; i < needed; i++) {
				go[i] = Instantiate(prefab,
					loc[i].position,
					loc[i].localRotation);
				go[i].name = "CrossWalk " + i + " (clone)";
				go[i].transform.parent = holder.transform;
			}
		}
	}

	// Cop
	[Serializable]
	public class CopSetUp {
		[Tooltip("The Cop prefab")]
		public GameObject prefab;

		[HideInInspector]
		public GameObject copCar;

		//public void Spawn(GameObject car) {
		public void Spawn(int carID) {
			if (copCar == null) {
				Instantiate (prefab, new Vector3 (43, 0, 0), Quaternion.Euler (0, -90f, 0));
				//copCar.name = "Cop";
			}
			if (copCar.GetComponent<CopBehavior> () != null) {
				//copCar.GetComponent<CopBehavior>().ChaseCar(car);
				copCar.GetComponent<CopBehavior> ().ChaseCar (carID);
			}
		}
	}


	public static float delivered = 0;

	// Cars
	public CarSetUp car = new CarSetUp();
	// Pick Up
	public PickUpSetUp pickUp = new PickUpSetUp();
	// Delivery
	public DeliverySetUp del = new DeliverySetUp();
	// Pedestrians
	public PedestrianSetUp ped = new PedestrianSetUp();
	// Cross Walk
	public CrossWalkSetUp cross = new CrossWalkSetUp();
	// Cop
	public CopSetUp cop = new CopSetUp();

	private RaycastHit carHit;


	// Use this for initialization
	void Start() {
		// Get locator points
		ped.GetLocatorPoints(ped.pointTag);
		cross.GetLocatorPoints(cross.pointTag);

		// Initialize delivery points
		del.GetGameObjects(del.pointTag);
		del.totalNeeded = car.toSpawn;
		del.ClearPoints();

		// Spawn characters/Generate objects
		ped.Spawn(ped.toSpawn);
		car.Spawn(car.toSpawn,del.go);
		cross.Place(cross.loc.Length);
	}
	
	// Update is called once per frame
	void Update() {

		ped.CheckPedestrians(ped.toSpawn);

		// handle roadblock situation

		// handle police chase situation
		if (Input.GetMouseButtonDown(0)) {
			Ray tempRay = Camera.main.ScreenPointToRay (Input.mousePosition);
			if (Physics.Raycast (tempRay, out carHit, 100000)) {
				Debug.DrawLine (tempRay.origin, carHit.point, Color.cyan);
				if (carHit.collider.gameObject.GetComponent<CarBehavior>().didCrime == true) {
					//cop.Spawn (carHit.collider.gameObject);
					cop.Spawn(carHit.transform.gameObject.GetComponent<CarBehavior>().index);
				}
			}
		}

		// Quit game
		if (Input.GetKey(KeyCode.Space)) {
			//SceneManager.LoadScene(SceneManager.GetActiveScene().name);
			Application.Quit();
		}
	}
}