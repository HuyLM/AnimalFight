using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

public class FixCellView : MonoBehaviour
{
	public UnityEvent<Transform[]> OnScroll;

	[SerializeField] private Transform pointCenter;
	[SerializeField] private float speedFix = 10;

	private TableView tableView;
	private ScrollRect scrollRect;
	private LinkedList<TableViewCell> visibleCells;
	private Transform[] cellViews;
	private float rowSize;
	private Coroutine coroutine;

	private float Offset{
		get {         
            Transform cell = cellViews[0];
			float offset = 0;
			if (tableView.direction == TableView.Direction.Horizontal)
				offset = cell.position.x + rowSize - pointCenter.position.x;
			else
				offset = cell.position.y - rowSize - pointCenter.position.y;
			return offset;
		}
	}

	private Vector3 Target{
		get{
			Vector3 target = Vector3.zero;
			if (tableView.direction == TableView.Direction.Horizontal)
				target = scrollRect.content.transform.position - new Vector3(Offset, 0, 0);
            else
				target = scrollRect.content.transform.position - new Vector3(0, Offset, 0);
			return target;
		}
	}

	private void Awake()
	{
		tableView = GetComponent<TableView>();
		scrollRect = GetComponent<ScrollRect>();
		if (pointCenter == null)
			pointCenter = transform;
	}

	// Use this for initialization
	void Start()
	{
		scrollRect.movementType = ScrollRect.MovementType.Unrestricted;
		scrollRect.inertia = false;
		rowSize = tableView.datasource.GetRowHeight(tableView, 0, 0) / 2;
	}

	private void OnEnable()
	{
		StopCoroutine("Reload");
		StartCoroutine(ReLoad());
	}

	private IEnumerator ReLoad()
	{
		yield return new WaitForSeconds(0.1f);
		OnScrolled();
		if (cellViews.Length != 0)
		{
			coroutine = StartCoroutine(SmoothMove(scrollRect.content.transform, Target, speedFix));
		}
	}

	private void OnScrolled()
	{
        
		visibleCells = tableView.VisibleCells;
		cellViews = new Transform[visibleCells.Count];
		int i = 0;
		foreach (TableViewCell cell in visibleCells)
		{
			cellViews[i] = cell.transform;
			i++;
		}
		Array.Sort(cellViews, (a, b) => (GetDistanceToPointCenter(a.position, rowSize)
										 .CompareTo(GetDistanceToPointCenter(b.position, rowSize))));

		//for (int j = 0; j < cellViews.Length; j++)
		//{
		//	if (j == 0)
		//	{
		//		cellViews[j].GetComponent<Image>().color = Color.red;
		//	}
		//	else
		//		cellViews[j].GetComponent<Image>().color = Color.white;
		//}
		if (cellViews.Length > 0){
			if (OnScroll != null)
				OnScroll.Invoke(cellViews);
		}
	}

	private float GetDistanceToPointCenter(Vector3 point, float offset)
	{
		float distance = 0;
		if (tableView.direction == TableView.Direction.Horizontal)
			distance = Vector2.Distance(point + new Vector3(offset, 0, 0), pointCenter.position);
        else
			distance = Vector2.Distance(point + new Vector3(0 , offset, 0), pointCenter.position);
		return distance;
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			if (coroutine != null)
				StopCoroutine(coroutine);
		}

		if (Input.GetMouseButtonUp(0))
		{
			if (cellViews.Length == 0)
				return;
			
			coroutine = StartCoroutine(SmoothMove(scrollRect.content.transform, Target, speedFix));
		}

		OnScrolled();
	}

	private IEnumerator SmoothMove(Transform o, Vector3 target, float speed)
	{
		while (Vector3.Distance(o.position, target) > 0.1f)
		{
			o.position = Vector3.Lerp(o.position, target, Time.deltaTime * speed);
			yield return null;
		}
	}
}
