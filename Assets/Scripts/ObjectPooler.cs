using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

/* The idea of this component is to create an array of Objects, and to rotate through
 * using them - based on disabling and enabling them using setActive. This saves on 
 * creating a potentially infinite amount of objects in memory, allowing reuse. 
 * The Object Pooler takes a list of Game Objects that it will pool (1 or more)*/
public class ObjectPooler : MonoBehaviour {
	public List<GameObject> prefabPool;		//we pass in a list of Game Objects to be pooled within this pool (1 or more)
	public int pooledAmount;					//this defines how many of each Game Object type to create within our pool
	private List<GameObject> pooledList;		//this will store the pooled objects, so will be number of game object types x pooled amount in length

	void Start () {
		pooledList = new List<GameObject> ();				//we make it of type game object, because everything is a game object and we don't know what type will be passed into this method explicitly.
		foreach (GameObject prefab in prefabPool) {		//we go through each of the game objects provided that need to be poooled (1 or more)
			for (int i = 0; i < pooledAmount; i++) {		//for each different object type, create pooled amount of them in our pooledList
				//try{
				//Debug.Log(prefab.name);
				AddToPoolList(prefab);						//dont have to assign the returned object to anything.
				//}catch(Exception e){
					//Debug.Log (pooledList [i]);
				//}
			}
		}
	}

	//function for adding an object to the pool list. It will return the object added, though doesn't have to be assigned to variable in C.
	private GameObject AddToPoolList(GameObject prefab){				
		GameObject theGameObject = Instantiate (prefab); 				//so, has to be gameobject cause thats the type of our list. however - we will actually instantiate object type that was passed in.
		theGameObject.SetActive (false);								//we want all to be inactive to begin, and activate them as we need
		pooledList.Add(theGameObject);									//then we add the object we've made to the list
		return theGameObject;	//!!
	}

	/* This method will be give the caller a pooled object that matches the "search criteria". It will check all inactive
	 * game objects in the list, and see if any of those match search criteria. If they do, it returns it. If they don't,
	 * it ensures that objects that match the search criteria actually are meant to be pooled in the poolList, and if so, 
	 * it will add a new object to the pool of the type that matched the search criteria. */
	public GameObject Get(Func<GameObject, bool> predicate){					//predicates are a way of passing in a query that can be true or false into a function
		List<GameObject> inactiveObjects = pooledList.Where (go => go.activeInHierarchy == false).ToList();	//this should give a list of all the inactive objects within the pooled list
		GameObject needle = inactiveObjects.FirstOrDefault(predicate);								//this means find one or return null in there (default)
		if (needle == null) {																		//If all of the objects of the search criteria we're looking for are active
			GameObject prefab = prefabPool.FirstOrDefault (predicate);							//get the first object of the type we are looking for (regardless of active or not)
			if (prefab != null) {																	//if we there does exist some an object of the search criteria, we are okay to add another
				return AddToPoolList (prefab);														//we have copied an object that is active but of the sort we need, so lets clone it to give us one
			} else {																				//else you're trying to get an item that isn't pooled in this pool
				throw new Exception("You've tried getting an unpooled object from the pool: "+predicate.ToString());
			}
		} else {					//there is an inactive object that meets the search critera, so lets return it
			return needle;
		}
	}	

	//PROBLEM: We can get the random Object from the pooled Object, but what part of 
	//			that do we than use as our search criteria for the Get function?
	//			I've tried getObject, getType and GetComponent... no joy. Coolpoints for help. TY x
	public GameObject GetRandomPooledObject(){
		//choose randomly between the different types within the pool
		int randomIndex = UnityEngine.Random.Range (0, prefabPool.Count); //0 4, 0123. Floats is inclusive
		GameObject randomComponent = prefabPool[randomIndex];
		//then get one of that type GET function
		GameObject randomObject = Get(obj=>obj.name.Contains(randomComponent.name));
		return randomObject;
	}	
		
}
