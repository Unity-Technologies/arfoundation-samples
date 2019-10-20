using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableFaceGeometry
{
//	public byte [] vertices;
//	public byte [] texCoords;
//	public byte [] triIndices;
//
//
//	public SerializableFaceGeometry(byte [] inputVertices, byte [] inputTexCoords, byte [] inputTriIndices)
//	{
//		vertices = inputVertices;
//		texCoords = inputTexCoords;
//		triIndices = inputTriIndices;
//
//	}
//
//	public static implicit operator SerializableFaceGeometry(Mesh faceMesh)
//	{
//		if (faceMesh.vertices.Length != 0 && faceMesh.uv.Length != 0 && faceMesh.triangles.Length != 0)
//		{
//			Vector3 [] faceVertices = faceMesh.vertices;
//			byte [] cbVerts = new byte[faceMesh.vertexCount * sizeof(float) * 3];
//			Buffer.BlockCopy( faceVertices, 0, cbVerts, 0, faceMesh.vertexCount * sizeof(float) * 3 );
//			
//
//			Vector2 [] faceTexCoords = faceMesh.uv;
//			byte [] cbTexCoords = new byte[faceMesh.uv.Length * sizeof(float) * 2];
//			Buffer.BlockCopy( faceTexCoords, 0, cbTexCoords, 0, faceMesh.uv.Length * sizeof(float) * 2 );
//
//
//			int [] triIndices = faceMesh.triangles;
//			byte [] cbTriIndices = new byte[faceMesh.triangles.Length * sizeof(int)];
//			Buffer.BlockCopy( triIndices, 0, cbTriIndices, 0, faceMesh.triangles.Length * sizeof(int));
//
//			return new SerializableFaceGeometry (cbVerts, cbTexCoords, cbTriIndices);
//		}
//		else 
//		{
//			return new SerializableFaceGeometry(null, null, null);
//		}
//	}

    public Vector3[] vertices;
    public Vector2[] texCoords;
    public int[] triIndices;

    public SerializableFaceGeometry(Mesh faceMesh)
    {
        vertices = faceMesh.vertices;
        texCoords = faceMesh.uv;
        triIndices = faceMesh.triangles;
    }

}

