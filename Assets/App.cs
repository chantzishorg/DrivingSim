using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class App : MonoBehaviour
{
    public class Point
    {
        public float x;
        public float z;
    }
    public class NoEntrace {
        public float x;
        public float z;
        public float width;
        public float vector_x;
        public float vector_z;

        public NoEntrace(float x, float z, float width, float vector_x, float vector_z)
        {
            this.x = x;
            this.z = z;
            this.width = width;
            this.vector_x = vector_x;
            this.vector_z = vector_z;
        }

        // Start is called before the first frame update
        public void setNoEntry(float x, float z, float width, float vector_x, float vector_z)
        {
            this.x = x;
            this.z = z;
            this.width = width;
            this.vector_x = vector_x;
            this.vector_z = vector_z;
    
        }
    }

  
    private List<NoEntrace> noEntraceVector;
    public void SetNoEntrace(float x,float z,float width, float vector_x,float vector_z)
    {
        noEntraceVector.Add(new NoEntrace(x, z, width, vector_x, vector_z));
    }

}
