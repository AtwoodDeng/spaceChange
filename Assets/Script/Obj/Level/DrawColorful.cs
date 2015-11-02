using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class DrawColorful : MonoBehaviour {

	public enum DrawType
	{
		Draw1
	}
	[SerializeField] DrawType drawType;

	[SerializeField] bool AVERAGE = false;
	[SerializeField] float diversity = 0.1f;
	[SerializeField] int drawSpeed = 1;
	[SerializeField] int speedMax = 150;
//	[SerializeField] float NUMCOLORS = 16f;
	[SerializeField] int WIDTH = 512;
	[SerializeField] int HEIGHT = 512;
	[SerializeField] int STARTX = 256;
	[SerializeField] int STARTY = 256;
//	[SerializeField] int RANDTEST = 8; 

	[SerializeField] Color centerColor = Color.white;
	[SerializeField] Vector3 colorBoundary = Vector3.one;
	[SerializeField] bool dynamicDrawSpeed = true;
	public float affectRange;

	struct XY
	{
		public int x;
		public int y;
		public XY(int x, int y)
		{
			this.x = x;
			this.y = y;
		}
		public override int GetHashCode()
		{
			return x ^ y;
		}
		public override bool Equals(object obj)
		{
			var that = (XY)obj;
			return this.x == that.x && this.y == that.y;
		}
	}

	// gets the difference between two colors
	 float coldiff(Color c1, Color c2)
	{
		var r = c1.r - c2.r;
		var g = c1.g - c2.g;
		var b = c1.b - c2.b;
		return r * r + g * g + b * b;
	}
	
	// gets the neighbors (3..8) of the given coordinate
	 List<XY> getneighbors(XY xy)
	{
		var ret = new List<XY>(8);
		for (var dy = -1; dy <= 1; dy++)
		{
			if (xy.y + dy == -1 || xy.y + dy == HEIGHT)
				continue;
			for (var dx = -1; dx <= 1; dx++)
			{
				if (xy.x + dx == -1 || xy.x + dx == WIDTH)
					continue;
				ret.Add(new XY(xy.x + dx, xy.y + dy));
			}
		}
		return ret;
	}

	// get neighbor with black point
	 List<XY> getneighbors(XY xy , Color[,] pix , bool withBlack = true)
	{
		List<XY> neigh = getneighbors(xy);
		List<XY> res = new List<XY>();
		foreach(XY n in neigh )
			if( (withBlack && pix[n.x,n.y].Equals(Color.black))
			   || (!withBlack && !pix[n.x,n.y].Equals(Color.black)))
				res.Add(n);
		return res;
	}

	// calculates how well a color fits at the given coordinates
	float calcdiff(Color[,] pixels, XY xy, Color c)
	{
		// get the diffs for each neighbor separately
		List<float> diffs = new List<float>(8);
		foreach (var nxy in getneighbors(xy))
		{
			diffs.Add(coldiff(pixels[nxy.x, nxy.y], c));
		}
		
		// average or minimum selection
		if (AVERAGE)
		{
			float sum = 0 ;
			foreach(float i in diffs )
				sum += i;
			return sum / diffs.Count;
		}
		else
		{
			float min = 9999f;
			foreach(float i in diffs )
				min = (min>i)?i:min;
			return min;
		}
	}

	Color getRandomColor()
	{
		return new Color(UnityEngine.Random.Range(0,1f),
		             UnityEngine.Random.Range(0,1f),
		             UnityEngine.Random.Range(0,1f),1f);
	}

	Color GenerateColorFromNeighbor(XY xy , Color[,] pixels ) 
	{
		List<XY> neigh = getneighbors(xy,pixels,false);
		float sumR =0 , sumG = 0, sumB = 0;
		foreach(XY n in neigh )
		{
			sumR += pixels[n.x,n.y].r;
			sumG += pixels[n.x,n.y].g;
			sumB += pixels[n.x,n.y].b;
		}
		Color res = new Color( sumR / neigh.Count , sumG / neigh.Count , sumB / neigh.Count );
		res.r += UnityEngine.Random.Range(-diversity,diversity);
		res.g += UnityEngine.Random.Range(-diversity,diversity);
		res.b += UnityEngine.Random.Range(-diversity,diversity);

		if ( res.Equals(Color.black))
		{

			res.r = res.g = res.b  = 0.05f;
		}
		return res;
	}

	Color ConstrainColor(Color col)
	{
		Color res = col;
		if ( res.r > Math.Min( centerColor.r + colorBoundary.x , 1f ) )
			res.r = Math.Min( centerColor.r + colorBoundary.x , 1f );
		if ( res.g > Math.Min( centerColor.g + colorBoundary.y , 1f ) )
			res.g = Math.Min( centerColor.g + colorBoundary.y , 1f );
		if ( res.b > Math.Min( centerColor.b + colorBoundary.z , 1f ) )
			res.b = Math.Min( centerColor.b + colorBoundary.z , 1f );
		if ( res.r < Math.Max( centerColor.r - colorBoundary.x , 0 ))
			res.r = Math.Max( centerColor.r - colorBoundary.x , 0 );
		if ( res.g < Math.Max( centerColor.g - colorBoundary.y , 0 ))
			res.g = Math.Max( centerColor.g - colorBoundary.y , 0 );
		if ( res.b < Math.Max( centerColor.b - colorBoundary.z , 0 ))
			res.b = Math.Max( centerColor.b - colorBoundary.z , 0 );
		return res;
	}

	IEnumerator Draw1(Texture2D texture)
	{
		Color[,] pixels = new Color[WIDTH,HEIGHT];
		for(int i = 0 ; i < WIDTH ; ++ i )
			for( int j = 0 ; j < HEIGHT ; ++j )
			{
					pixels[i,j] = Color.black;
				texture.SetPixel(i,j,Color.black);
			}

		{
			pixels[STARTX,STARTY] = ConstrainColor( getRandomColor());
			texture.SetPixel(STARTX,STARTX,pixels[STARTX,STARTY]);
			texture.Apply();
			
			List<XY> available = getneighbors(new XY(STARTX,STARTY) , pixels , true);
			
			int count = 0;
			while(available.Count > 0 )
			{
//				Debug.Log("available" + available.Count.ToString());
				XY temp = available[UnityEngine.Random.Range( 0,available.Count)];
				available.Remove(temp);
				if ( ! pixels[temp.x,temp.y].Equals(Color.black) ) continue;
				pixels[temp.x,temp.y] = ConstrainColor( GenerateColorFromNeighbor(temp,pixels));
				available.AddRange(getneighbors(temp, pixels , true));
				texture.SetPixel(temp.x,temp.y,pixels[temp.x,temp.y]);

				count ++;

				if ( dynamicDrawSpeed )
					drawSpeed = Math.Min( (int) Math.Sqrt( 1.8f *  available.Count) / 3 + 1 , speedMax );

				float raduis =  Math.Min( 1.0f * Math.Abs( temp.x - STARTX ) / WIDTH , 1.0f * Math.Abs( temp.y - STARTY ) / HEIGHT );
				affectRange = Math.Max( raduis , affectRange );

				if ( count % drawSpeed == 0 )
				{
					texture.Apply();
					yield return null;
				}
			}
			
		}
	}

	// Use this for initialization
	void Awake() {
		Texture2D texture = new Texture2D(WIDTH, HEIGHT);

		Renderer renderer = GetComponent<Renderer>();
		
		renderer.material.mainTexture = texture;


//		Color[,] pixels = new Color[WIDTH,HEIGHT];
//		for(int i = 0 ; i < WIDTH ; ++ i )
//			for( int j = 0 ; j < HEIGHT ; ++j )
//				pixels[i,j] = new Color(0,0,0);
//		DateTime timeRecord = System.DateTime.Now;
		UnityEngine.Random.seed = System.DateTime.Now.Second	;

		if ( drawType == DrawType.Draw1 )
			StartCoroutine(Draw1(texture));
	}

	void Update()
	{
//		transform.localScale = Vector3.one *  Mathf.Lerp( transform.localScale.x , 1.2f / affectRange, 0.1f);
	}
}
