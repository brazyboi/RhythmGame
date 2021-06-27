using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ListViewBaseAdaptor : MonoBehaviour {
	public delegate void RefreshItems();

	public RefreshItems refreshItems;

	public abstract int getTotalCount ();

	public abstract string getCellPrefab (); 

	public abstract void bindCellData (int index, Transform prefabCell);
}
