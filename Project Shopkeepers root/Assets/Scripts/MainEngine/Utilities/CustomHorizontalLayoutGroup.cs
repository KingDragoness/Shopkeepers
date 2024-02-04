using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class CustomHorizontalLayoutGroup : HorizontalOrVerticalLayoutGroup
{

	public float extraSizeEspeciallyForFuckingLabel = 2f;

	public override void CalculateLayoutInputHorizontal()
	{
		base.CalculateLayoutInputHorizontal();
		CalcAlongAxis(0, false);
	}

	public override void CalculateLayoutInputVertical()
	{
		CalcAlongAxis(1, false);
	}

	public override void SetLayoutHorizontal()
	{
		SetChildrenAlongAxis_Custom(0, false);
	}

	public override void SetLayoutVertical()
	{
		SetChildrenAlongAxis(1, false);
	}


	protected void SetChildrenAlongAxis_Custom(int axis, bool isVertical)
	{
		float size = base.rectTransform.rect.size[axis];
		bool controlSize = (axis == 0) ? m_ChildControlWidth : m_ChildControlHeight;
		bool useScale = (axis == 0) ? m_ChildScaleWidth : m_ChildScaleHeight;
		bool childForceExpandSize = (axis == 0) ? m_ChildForceExpandWidth : m_ChildForceExpandHeight;
		float alignmentOnAxis = GetAlignmentOnAxis(axis);
		if (isVertical ^ (axis == 1))
		{
			float innerSize = size - (float)((axis == 0) ? base.padding.horizontal : base.padding.vertical);
			for (int j = 0; j < base.rectChildren.Count; j++)
			{
				RectTransform child2 = base.rectChildren[j];
				float min2;
				float preferred2;
				float flexible2;
				GetChildSizes(child2, axis, controlSize, childForceExpandSize, out min2, out preferred2, out flexible2);
				float scaleFactor2 = useScale ? child2.localScale[axis] : 1f;
				float requiredSpace = Mathf.Clamp(innerSize, min2, (flexible2 > 0f) ? size : preferred2);
				float startOffset = GetStartOffset(axis, requiredSpace * scaleFactor2);
				if (controlSize)
				{
					SetChildAlongAxisWithScale(child2, axis, startOffset, requiredSpace, scaleFactor2);
					continue;
				}
				float offsetInCell2 = (requiredSpace - child2.sizeDelta[axis]) * alignmentOnAxis;
				SetChildAlongAxisWithScale(child2, axis, startOffset + offsetInCell2, scaleFactor2);
			}
			return;
		}
		float pos = (axis == 0) ? base.padding.left : base.padding.top;
		float itemFlexibleMultiplier = 0f;
		float surplusSpace = size - GetTotalPreferredSize(axis);
		if (surplusSpace > 0f)
		{
			if (GetTotalFlexibleSize(axis) == 0f)
			{
				pos = GetStartOffset(axis, GetTotalPreferredSize(axis) - (float)((axis == 0) ? base.padding.horizontal : base.padding.vertical));
			}
			else if (GetTotalFlexibleSize(axis) > 0f)
			{
				itemFlexibleMultiplier = surplusSpace / GetTotalFlexibleSize(axis);
			}
		}
		float minMaxLerp = 0f;
		if (GetTotalMinSize(axis) != GetTotalPreferredSize(axis))
		{
			minMaxLerp = Mathf.Clamp01((size - GetTotalMinSize(axis)) / (GetTotalPreferredSize(axis) - GetTotalMinSize(axis)));
		}
		for (int i = 0; i < base.rectChildren.Count; i++)
		{
			RectTransform child = base.rectChildren[i];
			float min;
			float preferred;
			float flexible;
			GetChildSizes(child, axis, controlSize, childForceExpandSize, out min, out preferred, out flexible);
			float scaleFactor = useScale ? child.localScale[axis] : 1f;
			float childSize = Mathf.Lerp(min, preferred, minMaxLerp);
			childSize += flexible * itemFlexibleMultiplier;

			if (child.GetComponent<Text>() != null)
			{
				childSize += extraSizeEspeciallyForFuckingLabel;
			}

			if (controlSize)
			{
				SetChildAlongAxisWithScale(child, axis, pos, childSize, scaleFactor);
			}
			else
			{
				float offsetInCell = (childSize - child.sizeDelta[axis]) * alignmentOnAxis;
				SetChildAlongAxisWithScale(child, axis, pos + offsetInCell, scaleFactor);
			}
			pos += childSize * scaleFactor + spacing;
		}
	}

	private void GetChildSizes(RectTransform child, int axis, bool controlSize, bool childForceExpand, out float min, out float preferred, out float flexible)
	{
		if (!controlSize)
		{
			min = child.sizeDelta[axis];
			preferred = min;
			flexible = 0f;
		}
		else
		{
			min = LayoutUtility.GetMinSize(child, axis);
			preferred = LayoutUtility.GetPreferredSize(child, axis);
			flexible = LayoutUtility.GetFlexibleSize(child, axis);
		}
		if (childForceExpand)
		{
			flexible = Mathf.Max(flexible, 1f);
		}
	}

}
