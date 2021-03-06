﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour
{
    // UI variables //
    public Transform PointPrefab;

    // make the inspector use a slider by defining '[Range(start,end)]'
    [Range (10, 50)]
    public int AmountCubes;

    // a simple slider to select which SineFunction to use    
    // replaced the int way of defining the function, by an enum
    public GraphFunctionName Function;

    // Logic variables //

    // a private collection to hold all generated prefabs
    private Transform[] _pointPrefabs;

    static readonly GraphFunction[] GraphFunctions =
    {
        SineFunction,
        CosineFunction,
        TangesFunction,
        Sine2DFunction,
        Sine2DAlternativeFunction,
        MultiSineFunction,
        MultiSine2DFunction,
        RippleFunction,
        CylinderFunction,
        SphereFunction,
        SpherePulsatingFunction,
        TorusFunction,
        TorusPulsatingFunction
    };

    const float pi = Mathf.PI;


    void Awake()
    {
        float step = 2f / this.AmountCubes;
        Vector3 scale = Vector3.one * step;

        // Since we're incorporating the z-axis as well,
        // we have to square the amount of points. Adjust the creation of the 
        // points array in Awake so it's big enough to contain all the points.
        this._pointPrefabs = new Transform[this.AmountCubes * this.AmountCubes];

        #region OLD way of animating relied on a explicit starting position of the prefabs
        //// i = tracker for creating a, i.e. 50x50, grid
        //// x = base for the position on the x-axis
        //// z = base for the position on the z-axis

        //Vector3 position;

        //position.y = 0f;
        //position.z = 0f;
        //for(int i = 0, z = 0; i < this._pointPrefabs.Length; z++)
        //{
        //    position.z = (z + 0.5f) * step - 1f;

        //    for(int x = 0; x < this.AmountCubes; x++, i++)
        //    {
        //        Transform point = Instantiate(this.PointPrefab);
        //        position.x = (x + 0.5f) * step - 1f;

        //        point.localPosition = position;
        //        point.localScale = scale;

        //        // to make the instantiated prefab a child of the Graph (this),
        //        // we can use 'transform', which is inherently available in this object
        //        point.SetParent(this.transform, false);

        //        // add it to the collection for later manipulation
        //        this._pointPrefabs[i] = point;
        //    }
        //}
        #endregion

        // With the new (u, v, time) function, we only need to instantiate the full
        // collection of prefabs to work with. No longer setting the position of them.
        for (int i = 0; i < this._pointPrefabs.Length; i++)
        {
            Transform point = Instantiate(this.PointPrefab);
            point.localScale = scale;
            point.SetParent(transform, false);
            this._pointPrefabs[i] = point;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Time.time returns the time at the moment of the frame,
        // since Update is called once per frame, Time.time remains the same during this function
        float t = Time.time;

        // We'll use the delegate to worry about the exact implementation of the SineFunction
        // By using an array, we can get rid of if-else statements and tie the function
        // to the int the slider in the editor is set to
        GraphFunction graphFunction = GraphFunctions[(int)this.Function];

        #region OLD 1-Dimensional way (Y-axis) of animating
        // we'll use Update to manipulate the .y positions of all the points
        // to make the graph animate
        //for (int i = 0; i < this._pointPrefabs.Length; i++)
        //{
        //    Transform point = this._pointPrefabs[i];
        //    Vector3 position = point.localPosition;

        //    position.y = graphFunction(position.x, position.z, t);

        //    // remember to explicitly set the position to the point taken
        //    // from the array*
        //    point.localPosition = position;
        //}
        #endregion

        // We are now no longer manipulating 1 dimension of a point, but
        // setting a new 3D location instead, this also simplifies the Awake() function
        // because we do not rely on starting positions anymore.
        float step = 2f / this.AmountCubes;
        for(int i = 0, z = 0; z < this.AmountCubes; z++)
        {
            float v = (z + 0.5f) * step - 1f;

            for(int x = 0; x < this.AmountCubes; x++, i++)
            {
                float u = (x + 0.5f) * step - 1f;
                this._pointPrefabs[i].localPosition = graphFunction(u, v, t);
            }
        }
    }

    /// <summary>
    /// * These functions can be static because they have no connection at all
    ///   with the Graph domain or instance. (They rely on parameters, and that's it)
    /// 
    ///   However, we are not using it's static-ness here because they are only used by the
    ///   Graph class at the moment.
    /// </summary>
    static Vector3 SineFunction(float x, float z, float time)
    {
        Vector3 p;
        p.x = x;
        p.y = Mathf.Sin(pi * (x + time));
        p.z = z;

        return p;
    }

    /// <summary>
    /// we're going to create a new function that uses both X and Z as input. 
    /// Create a method for it, named Sine2DFunction. Have it represent the function 
    /// f(x, z, t) = sin(π(x+z+t)), which is the most straightforward way to 
    /// make a sine wave based on both x and z.
    /// </summary>
    static Vector3 Sine2DFunction(float x, float z, float t)
    {
        Vector3 p;
        p.x = x;
        p.y = Mathf.Sin(pi * (x + z + t));
        p.z = z;

        return p;
    }

    /// <summary>
    /// we're going to create a new function that uses both X and Z as input. 
    /// Create a method for it, named Sine2DFunction. Have it represent the function 
    /// f(x, z, t) = sin(π(x+z+t)), which is the most straightforward way to 
    /// make a sine wave based on both x and z.
    /// </summary>
    static Vector3 Sine2DAlternativeFunction(float x, float z, float t)
    {
        Vector3 p;
        p.x = x;
        p.y = Mathf.Sin(pi * (x + t));
        p.y += Mathf.Sin(pi * (z + t));
        p.y *= 0.5f; // halve the result to keep it in the -1 - 1 range
        p.z = z;

        return p;
    }

    static Vector3 CosineFunction(float x, float z, float time)
    {
        Vector3 p;
        p.x = x;
        p.y = Mathf.Cos(pi * (x + time));
        p.z = z;

        return p;
    }

    static Vector3 TangesFunction(float x, float z, float time)
    {
        Vector3 p;
        p.x = x;
        p.y = Mathf.Tan(pi * (x + time));
        p.z = z;

        return p;
    }

    /// <summary>
    /// A duplicate of the original Sinefunction with a slight added complexity.
    /// changes twice as fast, which is done by multiplying the argument of the 
    /// sine function by 2. At the same time, we'll halve the result of this function. 
    /// That keeps the shape of the sine wave the same, just at half size.
    /// 
    /// As both the positive and negative extremes of the sine function are 1 and −1,
    /// the maximum and minimum values of this new function will be 1.5 and −1.5. 
    /// To guarantee that we stay in the −1–1 range, we should divide the entire thing
    /// by 1.5, which is the same as multiplying by 2/3.
    /// 
    /// 
    /// * These functions can be static because they have no connection at all
    ///   with the Graph domain or instance. (They rely on parameters, and that's it)
    /// 
    ///   However, we are not using it's static-ness here because they are only used by the
    ///   Graph class at the moment.
    /// </summary>
    static Vector3 MultiSineFunction(float x, float z, float time)
    {
        Vector3 p;
        p.x = x;
        p.y = Mathf.Sin(pi * (x + time));
        //y += Mathf.Sin(2f * Mathf.PI * (x + this.TimeInfluence * time)) / 2f;
        p.y += Mathf.Sin(2f * pi * (x + time)) / 2f;
        p.y *= 2f / 3f;
        p.z = z;

        return p;
    }

    static Vector3 MultiSine2DFunction(float x, float z, float time)
    {
        Vector3 p;
        p.x = x;
        p.y = 4f * Mathf.Sin(pi * (x + z + time * 0.5f));
        p.y += Mathf.Sin(pi * (x + time));
        p.y += Mathf.Sin(2f * pi * (z + 2f * time)) * 0.5f;
        p.y *= 1f / 5.5f;
        p.z = z;

        return p;
    }


    /// <summary>
    /// What we get is a cone shape that's at zero in the middle and increases 
    /// linearly with the distance from the origin. It ends up highest near the 
    /// corners of the grid, because those points are furthest away from the origin. 
    /// Exactly at the corners, the distance would be √2, which is roughly 1.4142.
    /// </summary>
    static Vector3 RippleFunction(float x, float z, float t)
    {
        Vector3 p;
        p.x = x;

        // by taking the square root in a Pythagorian way, we
        // calculate the distance of the hypotenuse of the right triangle (a^2 * b^2 = c^2)
        float distance = Mathf.Sqrt(x * x + z * z);
        //To create our ripple, we'll have to use f(x, z, t) = sin(πD) where D is the distance.
        p.y = Mathf.Sin(pi * (4f * distance - t));
        // with only the formula above the undulation is far too extreme

        // We can take care of that by reducing the amplitude of the wave. 
        // Instead of doing this uniformly, we can make it depend on the distance as well.
        // However, simply dividing by the distance will result in a division by 
        // zero at the origin, and cause the amplitude to become extreme near the origin.
        // to prevent this, we'll use 1 / 1 + 10D
        p.y /= 1f + 10f * distance;
        p.z = z;

        return p;
    }

    static Vector3 CylinderFunction(float u, float v, float t)
    {
        Vector3 p;
        // star shape (a sinus wave x6 around the cylinder circle on the x-axis)
        //float radius = 1f + Mathf.Sin(6f * pi * u) * 0.2f;
        // using v instead of u creates a 'cylindrical wave' along the y-axis
        //float radius = 1f + Mathf.Sin(2f * pi * v) * 0.2f;

        // we can also use both v + u to create a mix of the 2 formulas above
        float radius = 0.8f + Mathf.Sin(pi * (6f * u + 2f * v + t)) * 0.2f;

        p.x = radius * Mathf.Sin(pi * u);
        p.y = v;
        p.z = radius * Mathf.Cos(pi * u);
        return p;
    }

    static Vector3 SphereFunction(float u, float v, float t)
    {
        Vector3 p;
        // R=cos(πv/2)

        float r = Mathf.Cos(pi * 0.5f * v);
        p.x = r * Mathf.Sin(pi * u);
        p.y = Mathf.Sin(pi * 0.5f * v);
        p.z = r * Mathf.Cos(pi * u);
        return p;
    }

    static Vector3 SpherePulsatingFunction(float u, float v, float t)
    {
        Vector3 p;
        // R=cos(πv/2)

        float r = 0.8f + Mathf.Sin(pi * (6f * u + t)) * 0.1f;
        r += Mathf.Sin(pi * (4f * v + t)) * 0.1f;
        float s = r * Mathf.Cos(pi * 0.5f * v);
        p.x = s * Mathf.Sin(pi * u);
        p.y = r * Mathf.Sin(pi * 0.5f * v);
        p.z = s * Mathf.Cos(pi * u);
        return p;
    }

    static Vector3 TorusFunction(float u, float v, float t)
    {
        Vector3 p;

        float r1 = 1f * t;
        float r2 = 0.5f * t;

        float s = r2 * Mathf.Cos(pi * v) + r1;
        p.x = s * Mathf.Sin(pi * u);
        p.y = r2 * Mathf.Sin(pi * v);
        p.z = s * Mathf.Cos(pi * u);
        return p;
    }

    static Vector3 TorusPulsatingFunction(float u, float v, float t)
    {
        Vector3 p;

        float r1 = 0.65f + Mathf.Sin(pi * (6f * u + t)) * 0.1f;
        float r2 = 0.2f + Mathf.Sin(pi * (4f * v + t)) * 0.05f;

        float s = r2 * Mathf.Cos(pi * v) + r1;
        p.x = s * Mathf.Sin(pi * u);
        p.y = r2 * Mathf.Sin(pi * v);
        p.z = s * Mathf.Cos(pi * u);
        return p;
    }
}
