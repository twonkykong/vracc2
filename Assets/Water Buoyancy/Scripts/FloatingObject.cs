using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WaterBuoyancy
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(MeshFilter))]
    public class FloatingObject : MonoBehaviour
    {
        [SerializeField]
        private bool calculateDensity = false;

        [SerializeField]
        private float density = 0.75f;

        [SerializeField]
        [Range(0f, 1f)]
        private float normalizedVoxelSize = 0.5f;

        [SerializeField]
        private float dragInWater = 1f;

        [SerializeField]
        private float angularDragInWater = 1f;

        public WaterVolume water;
        private new Collider collider;
        public new Rigidbody rigidbody;
        private float initialDrag;
        private float initialAngularDrag;
        private Vector3 voxelSize;
        public Vector3[] voxels;

        Vector3 previousSize;
        Bounds bounds;
        List<Collider> colliders, childrenColliders;

        protected virtual void Awake()
        {
            voxels = null;

            if (GetComponent<Collider>() != null)
            {
                this.collider = this.GetComponent<Collider>();
                if (GetComponents<Collider>().Length != 0) colliders = GetComponents<Collider>().ToList(); 
            }

            if (GetComponentInChildren<Collider>() != null)
            {
                childrenColliders = GetComponentsInChildren<Collider>().ToList();

                foreach (Collider col in childrenColliders) if (col.GetComponent<Rigidbody>() != null) childrenColliders.Remove(col);
            }

            this.rigidbody = this.GetComponent<Rigidbody>();

            this.initialDrag = this.rigidbody.drag;
            this.initialAngularDrag = this.rigidbody.angularDrag;

            previousSize = transform.localScale;
            if (this.calculateDensity)
            {
                float objectVolume = MathfUtils.CalculateVolume_Mesh(this.GetComponent<MeshFilter>().mesh, this.transform);
                this.density = this.rigidbody.mass / objectVolume;
            }
        }

        protected virtual void FixedUpdate()
        {
            if (this.water != null && this.voxels.Length > 0)
            {
                if (previousSize != transform.localScale) voxels = CutIntoVoxels();
                previousSize = transform.localScale;

                Vector3 forceAtSingleVoxel = this.CalculateMaxBuoyancyForce() / this.voxels.Length;

                if (collider != null)
                {
                    bounds = this.collider.bounds;

                    if (colliders != null)
                    {
                        foreach (Collider col in colliders) bounds.Encapsulate(col.bounds);
                    }
                }
                else if (childrenColliders != null) bounds = childrenColliders[0].bounds; 

                if (childrenColliders != null)
                {
                    foreach (Collider col in childrenColliders)
                    {
                        if (col == null)
                        {
                            childrenColliders.Remove(col);
                            continue;
                        }

                        bounds.Encapsulate(col.bounds);
                    }
                }

                float voxelHeight = bounds.size.y * this.normalizedVoxelSize;

                float submergedVolume = 0f;
                for (int i = 0; i < this.voxels.Length; i++)
                {
                    Vector3 worldPoint = this.transform.TransformPoint(this.voxels[i]);

                    Collider waterCollider = water.GetComponent<Collider>();
                    Bounds waterBounds = waterCollider.bounds;
                    float waterLevel = this.water.GetWaterLevel(worldPoint);
                    float deepLevel = waterLevel - worldPoint.y + (voxelHeight / 2f); // How deep is the voxel                    
                    float submergedFactor = Mathf.Clamp(deepLevel / voxelHeight, 0f, 1f); // 0 - voxel is fully out of the water, 1 - voxel is fully submerged
                    submergedVolume += submergedFactor;

                    Vector3 surfaceNormal = this.water.GetSurfaceNormal(worldPoint);
                    Quaternion surfaceRotation = Quaternion.FromToRotation(this.water.transform.up, surfaceNormal);
                    surfaceRotation = Quaternion.Slerp(surfaceRotation, Quaternion.identity, submergedFactor);

                    Vector3 finalVoxelForce = surfaceRotation * (forceAtSingleVoxel * submergedFactor);
                    //if (ColliderUtils.IsPointInsideCollider(worldPoint, waterCollider, ref waterBounds)) this.rigidbody.AddForceAtPosition(finalVoxelForce, worldPoint);
                    if (waterCollider.bounds.Contains(worldPoint)) this.rigidbody.AddForceAtPosition(finalVoxelForce, worldPoint);

                    Debug.DrawLine(worldPoint, worldPoint + finalVoxelForce.normalized, Color.blue);
                }

                submergedVolume /= this.voxels.Length; // 0 - object is fully out of the water, 1 - object is fully submerged

                this.rigidbody.drag = Mathf.Lerp(this.initialDrag, this.dragInWater, submergedVolume);
                this.rigidbody.angularDrag = Mathf.Lerp(this.initialAngularDrag, this.angularDragInWater, submergedVolume);
            }
        }

        /*protected virtual void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(WaterVolume.TAG))
            {
                this.water = other.GetComponent<WaterVolume>();
                if (this.voxels == null)
                {
                    this.voxels = this.CutIntoVoxels();
                }
            }
        }

        protected virtual void OnTriggerStay(Collider other)
        {
            if (other.CompareTag(WaterVolume.TAG))
            {
                if (transform.localScale != previousSize)
                {
                    this.voxels = this.CutIntoVoxels();
                    previousSize = transform.localScale;
                }
            }
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(WaterVolume.TAG))
            {
                this.water = null;
            }
        }*/

        protected virtual void OnDrawGizmos()
        {
            if (this.voxels != null)
            {
                for (int i = 0; i < this.voxels.Length; i++)
                {
                    Gizmos.color = Color.magenta - new Color(0f, 0f, 0f, 0.75f);
                    Gizmos.DrawCube(this.transform.TransformPoint(this.voxels[i]), this.voxelSize * 0.8f);
                }
            }
        }

        private Vector3 CalculateMaxBuoyancyForce()
        {
            float objectVolume = this.rigidbody.mass  / this.density;
            Vector3 maxBuoyancyForce = this.water.Density * objectVolume * -Physics.gravity;

            return maxBuoyancyForce;
        }

        public Vector3[] CutIntoVoxels()
        {
            Quaternion initialRotation = this.transform.rotation;
            this.transform.rotation = Quaternion.identity;

            if (collider != null)
            {
                bounds = this.collider.bounds;

                if (colliders != null)
                {
                    foreach (Collider col in colliders) bounds.Encapsulate(col.bounds);
                }
            }
            else if (childrenColliders != null) bounds = childrenColliders[0].bounds;

            if (childrenColliders != null)
            {
                foreach (Collider col in childrenColliders)
                {
                    if (col == null)
                    {
                        childrenColliders.Remove(col);
                        continue;
                    }

                    bounds.Encapsulate(col.bounds);
                }
            }

            this.voxelSize.x = bounds.size.x * this.normalizedVoxelSize;
            this.voxelSize.y = bounds.size.y * this.normalizedVoxelSize;
            this.voxelSize.z = bounds.size.z * this.normalizedVoxelSize;
            int voxelsCountForEachAxis = Mathf.RoundToInt(1f / this.normalizedVoxelSize);
            List<Vector3> voxels = new List<Vector3>(voxelsCountForEachAxis * voxelsCountForEachAxis * voxelsCountForEachAxis);

            for (int i = 0; i < voxelsCountForEachAxis; i++)
            {
                for (int j = 0; j < voxelsCountForEachAxis; j++)
                {
                    for (int k = 0; k < voxelsCountForEachAxis; k++)
                    {
                        float pX = bounds.min.x + this.voxelSize.x * (0.5f + i);
                        float pY = bounds.min.y + this.voxelSize.y * (0.5f + j);
                        float pZ = bounds.min.z + this.voxelSize.z * (0.5f + k);

                        Vector3 point = new Vector3(pX, pY, pZ);
                        if (bounds.Contains(point))
                        {
                            voxels.Add(this.transform.InverseTransformPoint(point));
                        }
                    }
                }
            }

            this.transform.rotation = initialRotation;

            return voxels.ToArray();
        }
    }
}