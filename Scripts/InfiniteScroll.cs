using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.UI;

public abstract class InfiniteScroll : ScrollRect
{
	[HideInInspector]
	public bool
		initOnAwake;

	protected RectTransform t {
		get {
			if (_t == null)
				_t = GetComponent<RectTransform> ();
			return _t;
		}
	}

	private RectTransform _t;

	private RectTransform[] prefabItems;
	private int itemTypeStart = 0;
	private int itemTypeEnd = 0;

	private bool init;

	private Vector2 dragOffset = Vector2.zero;

	#region abstracts	
	protected abstract float GetSize (RectTransform item);
	
	protected abstract float GetDimension (Vector2 vector);
	
	protected abstract Vector2 GetVector (float value);

	protected abstract float GetPos (RectTransform item);

	protected abstract int OneOrMinusOne ();
	#endregion

	#region core
	new void Awake ()
	{
		if (!Application.isPlaying)
			return;

		if (initOnAwake)
			Init ();
	}

	public void Init ()
	{
		init = true;

		//Creating an array of prefab items and disabling them

		var tempStack = new Stack<RectTransform> ();
		foreach (RectTransform child in content) {
			if (!child.gameObject.activeSelf)
				continue;
			tempStack.Push (child);
			child.gameObject.SetActive (false);
		}
		prefabItems = tempStack.ToArray ();

		float containerSize = 0;
		//Filling up the scrollview with initial items
		while (containerSize < GetDimension(t.sizeDelta)) {
			RectTransform nextItem = NewItemAtEnd ();
			containerSize += GetSize (nextItem);
		}


	}
	private void Update ()
	{
		if (!Application.isPlaying || !init)
			return;
		if (GetDimension (content.sizeDelta) - (GetDimension (content.localPosition) * OneOrMinusOne ()) < GetDimension (t.sizeDelta)) {
			NewItemAtEnd ();
			//margin is used to Destroy objects. We add them at half the margin (if we do it at full margin, we continuously add and delete objects)
		} else if (GetDimension (content.localPosition) * OneOrMinusOne () < (GetDimension (t.sizeDelta) * 0.5f)) {
			NewItemAtStart ();
			//Using else because when items get added, sometimes the properties in UnityGUI are only updated at the end of the frame.
			//Only Destroy objects if nothing new was added (also nice performance saver while scrolling fast).
		} else {
			//Looping through all items.
			foreach (RectTransform child in content) {
				//Our prefabs are inactive
				if (!child.gameObject.activeSelf)
					continue;
				//We Destroy an item from the end if it's too far
				if (GetPos (child) > GetDimension (t.sizeDelta)) {
					Destroy (child.gameObject);
					//We update the container position, since after we delete something from the top, the container moves all of it's content up
					content.localPosition -= (Vector3)GetVector (GetSize (child));
					dragOffset -= GetVector (GetSize (child));
					Add (ref itemTypeStart);
				} else if (GetPos (child) < -(GetDimension (t.sizeDelta) + GetSize (child))) {
					Destroy (child.gameObject);
					Subtract (ref itemTypeEnd);
				}
			}
		}
	}

	private RectTransform NewItemAtStart ()
	{
		Subtract (ref itemTypeStart);
		RectTransform newItem = InstantiateNextItem (itemTypeStart);
		newItem.SetAsFirstSibling ();

		content.localPosition += (Vector3)GetVector (GetSize (newItem));
		dragOffset += GetVector (GetSize (newItem));
		return newItem;
	}

	private RectTransform NewItemAtEnd ()
	{
		RectTransform newItem = InstantiateNextItem (itemTypeEnd);
		Add (ref itemTypeEnd);
		return newItem;
	}

	private RectTransform InstantiateNextItem (int itemType)
	{
		RectTransform nextItem = Instantiate (prefabItems [itemType]) as RectTransform;
		nextItem.name = prefabItems [itemType].name;
		nextItem.transform.SetParent (content.transform, false);
		nextItem.gameObject.SetActive (true);
		return nextItem;
	}
	#endregion

	#region overrides
	public override void OnBeginDrag (UnityEngine.EventSystems.PointerEventData eventData)
	{
		dragOffset = Vector2.zero;
		base.OnBeginDrag (eventData);
	}

	public override void OnDrag (UnityEngine.EventSystems.PointerEventData eventData)
	{
		//TEMP method until I found a better solution
		if (dragOffset != Vector2.zero) {
			OnEndDrag (eventData);
			OnBeginDrag (eventData);
			dragOffset = Vector2.zero;
		}

		base.OnDrag (eventData);
	}
	#endregion

	#region convenience


	private void Subtract (ref int i)
	{
		i--;
		if (i == -1) {
			i = prefabItems.Length - 1;
		}
	}

	private void Add (ref int i)
	{
		i ++;
		if (i == prefabItems.Length) {
			i = 0; 
		}
	}
	#endregion
}
