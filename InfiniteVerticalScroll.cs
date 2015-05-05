using UnityEngine;
using System.Collections;

using UnityEngine.UI;

public class InfiniteVerticalScroll : InfiniteScroll
{
	protected override float GetSize (RectTransform item)
	{
		return item.GetComponent<LayoutElement> ().minHeight;
	}

	protected override float GetDimension (Vector2 vector)
	{
		return vector.y;
	}

	protected override Vector3 GetVector (float value)
	{
		return new Vector3 (0, value);
	}

	protected override float GetPos (RectTransform item)
	{
		return item.localPosition.y + container.localPosition.y;
	}

	protected override int OneOrMinusOne ()
	{
		return 1;
	}
}
