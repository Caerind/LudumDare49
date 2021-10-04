using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Habrador_Computational_Geometry;

//Attach this to go with mesh and it should display its bounding box
public class DisplayBoundingBox : MonoBehaviour
{

    private void OnDrawGizmosSelected()
	{
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            return;
        }

        Mesh mesh = meshFilter.sharedMesh;
        if (mesh == null)
        {
            return;
        }

        //mesh.bounds is in local space
        DisplayMeshBoundingBox(mesh);

        //Renderer.bounds in in world space, so this becomes AABB
        MeshRenderer mr = GetComponent<MeshRenderer>();
        if (mr == null)
        {
            return;
        }

        DisplayMeshRendererAABB(mr);
    }


    //Renderer.bounds are AABB in world space
    private void DisplayMeshRendererAABB(MeshRenderer mr)
    {
        Bounds bounds = mr.bounds;
        AABB3 aabb = new AABB3(bounds);

        Gizmos.color = Color.black;

        List<Edge3> edges = aabb.GetEdges();
        foreach (Edge3 e in edges)
        {
            Gizmos.DrawLine(e.p1.ToVector3(), e.p2.ToVector3());
        }
    }

    //Mesh.Bounds are AABB in local space
    //Is taking rotation into account, so we get an oriented bounding box
    private void DisplayMeshBoundingBox(Mesh mesh)
    {
        Box orientedBB = new Box(mesh, transform);

        Gizmos.color = Color.blue;

        List<Edge3> edges = orientedBB.GetEdges();
        foreach (Edge3 e in edges)
        {
            Gizmos.DrawLine(e.p1.ToVector3(), e.p2.ToVector3());
        }
    }
}
