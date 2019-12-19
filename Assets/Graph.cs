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
        SineFunction, Sine2DFunction, MultiSineFunction
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
        float toReturn = Mathf.Sin(pi * (x + time));
        //toReturn += Mathf.Sin(2f * Mathf.PI * (x + this.TimeInfluence * time)) / 2f;
        toReturn += Mathf.Sin(2f * pi * (x + time)) / 2f;
        toReturn *= 2f / 3f;

        return toReturn;
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
}
