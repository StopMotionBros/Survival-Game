using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Grad 
{
    public BlendMode BlendMode;

    List<ColorKey> _keys = new List<ColorKey>();

    public Grad()
    {
        if (_keys.Count == 0)
        {
            AddKey(Color.white, 0);
            AddKey(Color.black, 1);
        }
    }

    public Color Evaluate(float time)
    {
        ColorKey keyLeft = _keys[0];
        ColorKey keyRight = _keys[_keys.Count - 1];

        for (int i = 0; i < _keys.Count; i++)
        {
            ColorKey key = _keys[i];

            if (key.Time == time) return key.Color;

            if (key.Time < time)
            {
                keyLeft = key;
            }
            if (key.Time > time)
            {
                keyRight = key;
                break;
            }
        }

        if (BlendMode == BlendMode.Linear)
        {
            float blendTime = Mathf.InverseLerp(keyLeft.Time, keyRight.Time, time);
            return Color.Lerp(keyLeft.Color, keyRight.Color, blendTime);
        }
        return keyRight.Color;
    }

    public int AddKey(Color color, float time)
    {
        ColorKey newKey = new ColorKey(color, time);
        for (int i = 0; i < _keys.Count; i++)
        {
            if (newKey.Time < _keys[i].Time)
            {
                _keys.Insert(i, newKey);
                return i;
            }
        }

        _keys.Add(newKey);
        return _keys.Count - 1;
    }

    public int AddKey(ColorKey key)
    {
        ColorKey newKey = key;
        for (int i = 0; i < _keys.Count; i++)
        {
            if (newKey.Time < _keys[i].Time)
            {
                _keys.Insert(i, newKey);
                return i;
            }
        }

        _keys.Add(newKey);
        return _keys.Count - 1;
    }

    public void SetKey(int ind, Color color, float time)
	{
        _keys[ind] = new ColorKey(color, time);
	}

    public void SetKey(int ind, ColorKey key)
	{
        _keys[ind] = key;
	}

    public void ClearKeys()
	{
		for (int i = 0; i < _keys.Count; i++)
		{
            RemoveKey(i);
		}
        _keys[0] = new ColorKey(Color.white, 0f);
	}

    public void RemoveKey(int index)
    {
        if (_keys.Count >= 2)
        {
            _keys.RemoveAt(index);
        }
    }

    public void CopyFrom(Grad gradient)
	{
        ClearKeys();

		for (int i = 0; i < gradient.KeyCount; i++)
		{
            if(i > 0)
			{
                AddKey(gradient.GetKey(i));
			}
			else
			{
                SetKey(i, gradient.GetKey(i));
			}
		}
	}

    public bool Contains(int index)
	{
        if (index < KeyCount)
            return true;
		else
            return false;
	}

    public int UpdateKeyTime(int index, float time)
    {
        Color col = _keys[index].Color;
        RemoveKey(index);
        return AddKey(col, time);
    }

    public void UpdateKeyColor(int index, Color col)
    {
        _keys[index] = new ColorKey(col, _keys[index].Time);
    }

    public int KeyCount
    {
        get
        {
            return _keys.Count;
        }
    }

    public ColorKey GetKey(int i)
    {
        return _keys[i];
    }

    public Texture2D GetTexture(int width)
    {
        Texture2D texture = new Texture2D(width, 1);
        Color[] colours = new Color[width];
        for (int i = 0; i < width; i++)
        {
            colours[i] = Evaluate((float)i / (width - 1));
        }
        texture.SetPixels(colours);
        texture.Apply();
        return texture;
    }

    [System.Serializable]
    public struct ColorKey
    {
        Color _color;
        float _time;

        public ColorKey(Color color, float time)
        {
            _color = color;
            _time = time;
        }

        public Color Color
        {
            get
            {
                return _color;
            }
        }

        public float Time
        {
            get
            {
                return _time;
            }
        }
    }

    public static Color Lerp(Grad a, Grad b, float at, float bt, float t)
	{
        return Color.Lerp(a.Evaluate(at), b.Evaluate(bt), t);
	}
}
public enum BlendMode { Linear, Discrete };
