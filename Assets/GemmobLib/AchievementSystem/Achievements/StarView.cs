using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarView : MonoBehaviour {

    public Text starReview;

	public void UpdateView (int totalStars, int completedStars)
	{
        starReview.text = totalStars + " / " + completedStars + " levels";
	}
}
