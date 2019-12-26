using UnityEngine;

namespace RPG.Saving
{
    //Makes this class serizlizable. I.E. Makes all fields serilizable by default
    [System.Serializable]
    public class SerializableVector3 
    {
        float x, y, z;

        public SerializableVector3(Vector3 vector)
        {
            x = vector.x;
            y = vector.y;
            z = vector.z;
        }

        public Vector3 ToVector()
        {
            return new Vector3(x, y,z);
        }
    }
}

