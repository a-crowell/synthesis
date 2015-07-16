﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

[TestClass]
public class BXDAMeshTesting
{
    private static BXDAMesh createRandomMesh()
    {
        Random rand = new Random();

        BXDAMesh.BXDASurface[] surfs = new BXDAMesh.BXDASurface[rand.Next(1, 3)];
        for (int i = 0; i < surfs.Length; i++)
        {
            surfs[i] = new BXDAMesh.BXDASurface();
            surfs[i].hasColor = (i & 1) == 0;
            surfs[i].color = (uint) (0x12345678 ^ rand.Next(0, 0xFFFFFFF));
            surfs[i].translucency = (float) rand.NextDouble();
            surfs[i].transparency = (float) rand.NextDouble();
            surfs[i].indicies = new int[rand.Next(1, 100) * 3];
            for (int j = 0; j < surfs[i].indicies.Length; j++)
            {
                surfs[i].indicies[j] = rand.Next(0,1);
            }
        }
        BXDAMesh.BXDASubMesh[] subMesh = new BXDAMesh.BXDASubMesh[rand.Next(1, 3)];
        for (int i = 0; i < subMesh.Length; i++)
        {
            subMesh[i] = new BXDAMesh.BXDASubMesh();
            subMesh[i].verts = new double[rand.Next(1, 100) * 3];
            if (i == 0)
            {
                subMesh[i].norms = new double[subMesh[i].verts.Length];
            }
            for (int j = 0; j < subMesh[i].verts.Length; j++)
            {
                subMesh[i].verts[j] = rand.NextDouble();
                if (subMesh[i].norms != null)
                {
                    subMesh[i].norms[j] = rand.NextDouble();
                }
            }
            subMesh[i].surfaces.AddRange(surfs);
        }
        BXDAMesh mesh = new BXDAMesh();
        mesh.meshes.AddRange(subMesh);
        mesh.colliders.AddRange(subMesh);
        mesh.physics.centerOfMass.x = (float) rand.NextDouble();
        mesh.physics.centerOfMass.y = (float) rand.NextDouble();
        mesh.physics.centerOfMass.z = (float) rand.NextDouble();
        mesh.physics.mass = (float) rand.NextDouble();
        return mesh;
    }

    private static void AssertSubMeshValid(BXDAMesh.BXDASubMesh a)
    {
        Assert.IsNotNull(a.verts);
        Assert.IsTrue((a.verts.Length % 3) == 0, "Vertex length not multiple of 3");
        if (a.norms != null)
        {
            Assert.AreEqual(a.verts.Length, a.norms.Length, "Normal lengths don't match vertex lengths");
        }
        Assert.IsNotNull(a.surfaces, "Surfaces is null");
        foreach (BXDAMesh.BXDASurface surf in a.surfaces)
        {
            Assert.IsNotNull(surf, "Null surface");
            Assert.IsTrue((surf.indicies.Length % 3) == 0, "Index length not multiple of 3");
            foreach (int i in surf.indicies)
            {
                Assert.IsTrue(i >= 0 && i < a.verts.Length / 3, "Index not in bounds");
            }
            Assert.IsTrue(surf.transparency >= 0 && surf.transparency <= 1.0, "Transparency not [0,1]");
            Assert.IsTrue(surf.translucency >= 0 && surf.translucency <= 1.0, "Translucency not [0,1]");
        }
    }

    private static void AssertSubMeshSetEqual(List<BXDAMesh.BXDASubMesh> a, List<BXDAMesh.BXDASubMesh> b)
    {
        Assert.AreEqual(a.Count, b.Count, "Submesh count doesn't match");
        for (int i = 0; i < a.Count; i++)
        {
            AssertSubMeshValid(a[i]);
            AssertSubMeshValid(b[i]);

            Assert.IsTrue((a[i].norms == null) == (b[i].norms == null), "Normal buffer state doesn't match");

            AssertExtensions.AreEqual(a[i].verts, b[i].verts, 0, "BXDASubMesh Verticies");
            if (a[i].norms != null && b[i].norms != null)
            {
                AssertExtensions.AreEqual(a[i].norms, b[i].norms, 0, "BXDASubMesh Normals");
            }

            Assert.AreEqual(a[i].surfaces.Count, b[i].surfaces.Count, "Surface count doesn't match");
            for (int j = 0; j < a[i].surfaces.Count; j++)
            {
                BXDAMesh.BXDASurface sA = a[i].surfaces[j], sB = b[i].surfaces[j];
                Assert.AreEqual(sA.hasColor, sB.hasColor, "One surface has color, one does not.");
                if (sA.hasColor)
                {
                    Assert.AreEqual(sA.color, sB.color, "Colors don't match");
                }
                Assert.AreEqual(sA.translucency, sB.translucency, "Translucency mismatch");
                Assert.AreEqual(sA.transparency, sB.transparency, "Transparency mismatch");

                AssertExtensions.AreEqual(sA.indicies, sB.indicies, 0, "BXDASurface IndexBuffer");
            }
        }
    }

    private static void AssertMeshEqual(BXDAMesh a, BXDAMesh b)
    {
        Assert.IsTrue((a != null) == (b != null), "Object instance state doesn't match.");
        Assert.AreEqual(a.physics.centerOfMass, b.physics.centerOfMass, "Physics CoM doesn't match");
        Assert.AreEqual(a.physics.mass, b.physics.mass, "Physics Mass doesn't match");
        AssertSubMeshSetEqual(a.meshes, b.meshes);
        AssertSubMeshSetEqual(a.colliders, b.colliders);
    }

    [TestMethod]
    public void BXDAMesh_ReadWriteTest()
    {
        BXDAMesh mesh = createRandomMesh();
        BXDAMesh readInto = TestUtils.WriteReadObject(mesh);
        AssertMeshEqual(readInto, mesh);
    }
}