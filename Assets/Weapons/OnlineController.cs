using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;

public class OnlineController : MonoBehaviourPun
{
    //OUTLINE
    [PunRPC]
    public void EnableOutline(int viewID, bool value)
    {
        if (PhotonView.Find(viewID) == null) return;
        GameObject obj = PhotonView.Find(viewID).gameObject;
        obj.GetComponent<Outline>().enabled = value;
    }

    [PunRPC]
    public void ChangeOutlineColor(int viewID, string color)
    {
        if (PhotonView.Find(viewID) == null) return;
        GameObject obj = PhotonView.Find(viewID).gameObject;
        Outline outline = obj.GetComponent<Outline>();
        string[] colors = color.Split('/');
        outline.OutlineColor = new Color(Convert.ToSingle(colors[0]), Convert.ToSingle(colors[1]), Convert.ToSingle(colors[2]));
    }


    //HOLDER
    [PunRPC]
    public void SetKinematic(int viewID, bool value)
    {
        if (PhotonView.Find(viewID) == null) return;
        GameObject obj = PhotonView.Find(viewID).gameObject;
        obj.GetComponent<Rigidbody>().isKinematic = value;
    }

    //SET MATERIAL
    [SerializeField]
    private SetMaterial _setMaterial;

    [PunRPC]
    public void SetMaterial(int viewID, string shaderName, string mainTexName, string normalMapName, string specularMapName, 
        float offsetX, float offsetY, 
        float tilingX, float tilingY, 
        float specularIntensity, float glossiness, float normalIntensity, float transparency, 
        float r1, float g1, float b1, float a1, 
        float r2, float g2, float b2, float a2)
    {
        if (PhotonView.Find(viewID) == null) return;
        StartCoroutine(SetMaterialCoroutine(viewID, shaderName, mainTexName, normalMapName, specularMapName,
            offsetX, offsetY,
            tilingX, tilingY,
            specularIntensity, glossiness, normalIntensity, transparency,
            r1, g1, b1, a1,
            r2, g2, b2, a2));
    }

    private IEnumerator SetMaterialCoroutine(int viewID, string shaderName, string mainTexName, string normalMapName, string specularMapName,
        float offsetX, float offsetY,
        float tilingX, float tilingY,
        float specularIntensity, float glossiness, float normalIntensity, float transparency,
        float r1, float g1, float b1, float a1,
        float r2, float g2, float b2, float a2)
    {
        PreloadTexturePreview textures = FindObjectOfType<PreloadTexturePreview>();
        Debug.Log(textures.TexturesArray.Find(texture => texture.name == mainTexName));

        GameObject obj = PhotonView.Find(viewID).gameObject;
        Material mat = new Material(Array.Find(_setMaterial.ShaderBases, t => t.name == shaderName));

        if (mainTexName != "no-texture")
        {
            yield return new WaitUntil(() => textures.TexturesArray.Find(texture => texture.name == mainTexName) != null);
            Texture tex = textures.TexturesArray.Find(texture => texture.name == mainTexName);
            mat.SetTexture("_MainTex", tex);
            mat.SetTextureOffset("_MainTex", new Vector2(offsetX, offsetY));
            mat.SetTextureScale("_MainTex", new Vector2(tilingX, tilingY));
        }
        else mat.SetTexture("_MainTex", null);

        if (normalMapName != "no-texture")
        {
            yield return new WaitUntil(() => textures.TexturesArray.Find(texture => texture.name == normalMapName) != null);
            Texture tex = textures.TexturesArray.Find(texture => texture.name == normalMapName);
            mat.SetTexture("_BumpMap", tex);
            mat.SetTextureOffset("_BumpMap", new Vector2(offsetX, offsetY));
            mat.SetTextureScale("_BumpMap", new Vector2(tilingX, tilingY));
        }
        else mat.SetTexture("_BumpMap", null);

        if (specularMapName != "no-texture")
        {
            yield return new WaitUntil(() => textures.TexturesArray.Find(texture => texture.name == specularMapName) != null);
            Texture tex = textures.TexturesArray.Find(texture => texture.name == specularMapName);
            mat.SetTexture("_SpecGlossMap", tex);
            mat.SetTextureOffset("_SpecGlossMap", new Vector2(offsetX, offsetY));
            mat.SetTextureScale("_SpecGlossMap", new Vector2(tilingX, tilingY));
        }
        else mat.SetTexture("_SpecGlossMap", null);

        mat.SetFloat("_SpecularIntensity", specularIntensity);
        mat.SetFloat("_Glossiness", glossiness);
        mat.SetFloat("_NormalIntensity", normalIntensity);
        mat.SetFloat("_Transparency", transparency);

        mat.SetColor("_Color", new Color(r1, g1, b1, a1));
        mat.SetColor("_SpecColor", new Color(r2, g2, b2, a2));

        obj.GetComponent<Renderer>().material = mat;
    }

    //DUPLICATE
    [PunRPC]
    public void CopyObject(int viewID, string poolName)
    {
        if (PhotonView.Find(viewID) == null) return;
        var pool = PhotonNetwork.PrefabPool as DefaultPool;
        GameObject copy = PhotonView.Find(viewID).gameObject;
        pool.ResourceCache.Add(poolName, copy);
    }

    //CHANGE WEIGHT
    [PunRPC]
    public void ChangeWeight(int viewID, float newMass)
    {
        if (PhotonView.Find(viewID) == null) return;
        GameObject obj = PhotonView.Find(viewID).gameObject;
        obj.GetComponent<Rigidbody>().mass = newMass;
    }

    //WELDING
    [PunRPC]
    public void Weld(int viewID1, int viewID2, int type, float x, float y, float z, float x1, float y1, float z1)
    {
        if (PhotonView.Find(viewID1) == null || PhotonView.Find(viewID2) == null) return;
        GameObject firstObj = PhotonView.Find(viewID1).gameObject;
        GameObject secondObj = PhotonView.Find(viewID2).gameObject;

        Joint joint = new Joint();

        if (type == 0)
        {
            joint = firstObj.AddComponent<FixedJoint>();
        }
        else if (type == 1)
        {
            joint = firstObj.AddComponent<SpringJoint>();
        }

        Vector3 firstPos = new Vector3(x, y, z);
        Vector3 secondPos = new Vector3(x1, y1, z1);

        joint.connectedBody = secondObj.GetComponent<Rigidbody>();
        joint.anchor = firstPos;
        joint.connectedAnchor = secondPos;

        firstObj.GetComponent<Prop>().JointsList.Add(joint);
        secondObj.GetComponent<Prop>().JointsList.Add(joint);
    }

    [PunRPC]
    public void RemoveWeld(int viewID)
    {
        if (PhotonView.Find(viewID) == null) return;
        Prop obj = PhotonView.Find(viewID).GetComponent<Prop>();
        Destroy(obj.JointsList[obj.JointsList.Count - 1]);
    }


    //SET PHYSICS
    [PunRPC]
    public void SetPhysics(int viewID, float mass, float drag, float angularDrag,
        float dynamicFriction, float staticFriction, float bounciness,
        float xCenter, float yCenter, float zCenter,
        bool useGravity, bool isKinematic,
        bool xFreezePos, bool yFreezePos, bool zFreezePos,
        bool xFreezeRot, bool yFreezeRot, bool zFreezeRot,
        int interpolationType, int collisionDetectionType, int frictionCombine, int bounceCombine)
    {
        if (PhotonView.Find(viewID) == null) return;
        Rigidbody rb = PhotonView.Find(viewID).GetComponent<Rigidbody>();
        PhysicMaterial physMat = new PhysicMaterial();

        rb.mass = mass;
        rb.drag = drag;
        rb.angularDrag = angularDrag;
        rb.useGravity = useGravity;
        rb.isKinematic = isKinematic;
        rb.centerOfMass = new Vector3(xCenter, yCenter, zCenter);

        RigidbodyInterpolation[] rbInterpolations = new RigidbodyInterpolation[3] { RigidbodyInterpolation.None, RigidbodyInterpolation.Interpolate, RigidbodyInterpolation.Extrapolate };
        rb.interpolation = rbInterpolations[interpolationType];

        CollisionDetectionMode[] rbCollisionDetections = new CollisionDetectionMode[4] { CollisionDetectionMode.Discrete, CollisionDetectionMode.Continuous, CollisionDetectionMode.ContinuousDynamic, CollisionDetectionMode.ContinuousSpeculative };
        rb.collisionDetectionMode = rbCollisionDetections[collisionDetectionType];

        PhysicMaterialCombine[] physMatCombine = new PhysicMaterialCombine[4] { PhysicMaterialCombine.Average, PhysicMaterialCombine.Minimum, PhysicMaterialCombine.Multiply, PhysicMaterialCombine.Maximum };
        physMat.frictionCombine = physMatCombine[frictionCombine];
        physMat.bounceCombine = physMatCombine[bounceCombine];

        physMat.dynamicFriction = dynamicFriction;
        physMat.staticFriction = staticFriction;
        physMat.bounciness = bounciness;

        RigidbodyConstraints constraints = RigidbodyConstraints.None;
        if (xFreezePos) constraints = constraints | RigidbodyConstraints.FreezePositionX;
        if (yFreezePos) constraints = constraints | RigidbodyConstraints.FreezePositionY;
        if (zFreezePos) constraints = constraints | RigidbodyConstraints.FreezePositionZ;

        if (xFreezeRot) constraints = constraints | RigidbodyConstraints.FreezeRotationX;
        if (yFreezeRot) constraints = constraints | RigidbodyConstraints.FreezeRotationY;
        if (zFreezeRot) constraints = constraints | RigidbodyConstraints.FreezeRotationZ;
        rb.constraints = constraints;
        rb.GetComponent<Collider>().material = physMat;
    }
}
