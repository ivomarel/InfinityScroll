using UnityEngine;
using System.Collections;

using UnityEngine.UI;

public class InfiniteHorizontalScroll : InfiniteScroll
{
	protected override float GetSize (RectTransform item)
	{
		return item.GetComponent<LayoutElement> ().minWidth;
	}

	protected override float GetDimension (Vector2 vector)
	{
		return vector.x;
	}

	protected override Vector2 GetVector (float value)
	{
		return new Vector2 (-value, 0);
	}

	protected override float GetPos (RectTransform item)
	{
		return -item.localPosition.x - content.localPosition.x;
	}

	protected override int OneOrMinusOne ()
	{
		return -1;
	}
}
