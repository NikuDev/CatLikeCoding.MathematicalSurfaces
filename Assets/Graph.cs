using System.Collections;
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
        RippleFunction
    };

    const float pi = Mathf.PI;


    void Awake()
    {
        float step = 2f / AmountCubes;
        Vector3 scale = Vector3.one * step;
        Vector3 position;

        position.y = 0f;
        position.z = 0f;

        // Since we're incorporating the z-axis as well,
        // we have to square the amount of points. Adjust the creation of the 
        // points array in Awake so it's big enough to contain all the points.
        this._pointPrefabs = new Transform[this.AmountCubes * this.AmountCubes];

        // i = 
        // x =
        // z = 

        for(int i = 0, z = 0; i < this._pointPrefabs.Length; z++)
        {
            position.z = (z + 0.5f) * step - 1f;

            for(int x = 0; x < this.AmountCubes; x++, i++)
            {
                Transform point = Instantiate(this.PointPrefab);
                position.x = (x + 0.5f) * step - 1f;

                point.localPosition = position;
                point.localScale = scale;

                // to make the instantiated prefab a child of the Graph (this),
                // we can use 'transform', which is inherently available in this object
                point.SetParent(this.transform, false);

                // add it to the collection for later manipulation
                this._pointPrefabs[i] = point;
            }
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

        // we'll use Update to manipulate the .y positions of all the points
        // to make the graph animate
        for (int i = 0; i < this._pointPrefabs.Length; i++)
        {
            Transform point = this._pointPrefabs[i];
            Vector3 position = point.localPosition;

            position.y = graphFunction(position.x, position.z, t);

            // remember to explicitly set the position to the point taken
            // from the array*
            point.localPosition = position;
        }
    }

    /// <summary>
    /// * These functions can be static because they have no connection at all
    ///   with the Graph domain or instance. (They rely on parameters, and that's it)
    /// 
    ///   However, we are not using it's static-ness here because they are only used by the
    ///   Graph class at the moment.
    /// </summary>
    static float SineFunction(float x, float z, float time)
    {
        return Mathf.Sin(pi * (x + time));
    }

    static float CosineFunction(float x, float z, float time)
    {
        return Mathf.Cos(pi * (x + time));
    }

    static float TangesFunction(float x, float z, float time)
    {
        return Mathf.Tan(pi * (x + time));
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
    static float MultiSineFunction(float x, float z, float time)
    {
        float y = Mathf.Sin(pi * (x + time));
        //y += Mathf.Sin(2f * Mathf.PI * (x + this.TimeInfluence * time)) / 2f;
        y += Mathf.Sin(2f * pi * (x + time)) / 2f;
        y *= 2f / 3f;

        return y;
    }

    static float MultiSine2DFunction(float x, float z, float time)
    {
        float y = 4f * Mathf.Sin(pi * (x + z + time * 0.5f));
        y += Mathf.Sin(pi * (x + time));
        y += Mathf.Sin(2f * pi * (z + 2f * time)) * 0.5f;
        y *= 1f / 5.5f;

        return y;
    }

    /// <summary>
    /// we're going to create a new function that uses both X and Z as input. 
    /// Create a method for it, named Sine2DFunction. Have it represent the function 
    /// f(x, z, t) = sin(π(x+z+t)), which is the most straightforward way to 
    /// make a sine wave based on both x and z.
    /// </summary>
    static float Sine2DFunction(float x, float z, float t)
    {
        return Mathf.Sin(pi * (x + z + t));
    }

    /// <summary>
    /// we're going to create a new function that uses both X and Z as input. 
    /// Create a method for it, named Sine2DFunction. Have it represent the function 
    /// f(x, z, t) = sin(π(x+z+t)), which is the most straightforward way to 
    /// make a sine wave based on both x and z.
    /// </summary>
    static float Sine2DAlternativeFunction(float x, float z, float t)
    {
        float y = Mathf.Sin(pi * (x + t));
        y += Mathf.Sin(pi * (z + t));
        y *= 0.5f; // halve the result to keep it in the -1 - 1 range
        return y;
    }


    /// <summary>
    /// What we get is a cone shape that's at zero in the middle and increases 
    /// linearly with the distance from the origin. It ends up highest near the 
    /// corners of the grid, because those points are furthest away from the origin. 
    /// Exactly at the corners, the distance would be √2, which is roughly 1.4142.
    /// </summary>
    static float RippleFunction(float x, float z, float t)
    {
        // by taking the square root in a Pythagorian way, we
        // calculate the distance of the hypotenuse of the right triangle (a^2 * b^2 = c^2)
        float distance = Mathf.Sqrt(x * x + z * z);
        //To create our ripple, we'll have to use f(x, z, t) = sin(πD) where D is the distance.
        float y = Mathf.Sin(pi * (4f * distance - t));
        // with only the formula above the undulation is far too extreme

        // We can take care of that by reducing the amplitude of the wave. 
        // Instead of doing this uniformly, we can make it depend on the distance as well.
        // However, simply dividing by the distance will result in a division by 
        // zero at the origin, and cause the amplitude to become extreme near the origin.
        // to prevent this, we'll use 1 / 1 + 10D
        y /= 1f + 10f * distance;
        return y;
    }
}
