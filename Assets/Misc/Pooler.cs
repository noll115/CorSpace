using UnityEngine;
using System.Collections.Generic;

public class Pooler<T> where T : MonoBehaviour  {

	private Queue<T> queue;

	public Pooler(GameObject gameObject,int poolSize) {
		queue = new Queue<T>();
		GameObject parent = new GameObject(gameObject.name);
		for (int i = 0; i < poolSize; i++)
		{
			GameObject go = GameObject.Instantiate(gameObject);
			go.transform.SetParent(parent.transform,true);
			go.SetActive(false);
			queue.Enqueue(go.GetComponent<T>());
			
		}
    }

	public T GetFromPool(){
		T comp = queue.Dequeue();
		queue.Enqueue(comp);
		return comp;
	}
}

