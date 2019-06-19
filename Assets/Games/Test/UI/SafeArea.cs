using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SafeArea : MonoBehaviour
{
    public float top;
    public float bottom;

    Vector2? _originalPosition;
    Vector2 OriginalPosition
    {
        get
        {
            if (_originalPosition == null)
            {
                _originalPosition = (transform as RectTransform).anchoredPosition;
            }

            return _originalPosition.Value;
        }
    }

    Vector2? _originalOffsetMin;
    Vector2 OriginalOffsetMin
    {
        get
        {
            if (_originalOffsetMin == null)
            {
                _originalOffsetMin = (transform as RectTransform).offsetMin;
            }

            return _originalOffsetMin.Value;
        }
    }

    Vector2? _originalOffsetMax;
    Vector2 OriginalOffsetMax
    {
        get
        {
            if (_originalOffsetMax == null)
            {
                _originalOffsetMax = (transform as RectTransform).offsetMax;
            }

            return _originalOffsetMax.Value;
        }
    }

    void OnEnable()
    {
        if (Device.iPhoneX)
        {
            ApplyX();
        }
    }

    void ApplyX()
    {
        var rt = transform as RectTransform;

        Vector2 min = rt.anchorMin;
        Vector2 max = rt.anchorMax;

        // If anchors are together
        if (Vector2.SqrMagnitude(min - max) < 0.0001f)
        {
            Vector2 position = OriginalPosition;

            if (Mathf.Abs(min.y) < 0.001f)
            {
                // If anchor to bottom
                position.y += bottom;
            }
            else
            {
                // If anchor to top
                position.y -= top;
            }

            rt.anchoredPosition = position;
        }
        else
        {
            Vector2 offset = OriginalOffsetMin;
            offset.y += bottom;
            rt.offsetMin = offset;

            offset = OriginalOffsetMax;
            offset.y -= top;
            rt.offsetMax = offset;
        }
    }

    void RestoreX()
    {
        var rt = transform as RectTransform;

        Vector2 min = rt.anchorMin;
        Vector2 max = rt.anchorMax;

        // If anchors are together
        if (Vector2.SqrMagnitude(min - max) < 0.0001f)
        {
            rt.anchoredPosition = OriginalPosition;
        }
        else
        {
            rt.offsetMin = OriginalOffsetMin;
            rt.offsetMax = OriginalOffsetMax;
        }
    }

    public void Apply()
    {
        if (Device.iPhoneX)
        {
            ApplyX();
        }
        else
        {
            RestoreX();
        }
    }

}
