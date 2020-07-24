﻿using System;
using System.IO;
using System.IO.Compression;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Engine.ModuleLoader;
using Engine.Util;
using MathNet.Spatial.Euclidean;

using Quaternion = MathNet.Spatial.Euclidean.Quaternion;

namespace Tests
{
    public class TestUtilities
    {
        private const float Delta = 0.03f;

        [Test]
        public void TestVectorConversion()
        {
            Vector3 vec3 = new Vector3(1.5f, 2.5f, 3.5f);
            Vector3D vec3dTest = Utilities.MapVector3(vec3);

            Assert.AreEqual(vec3.x, vec3dTest.X, Delta);
            Assert.AreEqual(vec3.y, vec3dTest.Y, Delta);
            Assert.AreEqual(vec3.z, vec3dTest.Z, Delta);

            Vector3 vec3Test = Utilities.MapVector3D(vec3dTest);

            Assert.AreEqual(vec3.x, vec3Test.x, Delta);
            Assert.AreEqual(vec3.y, vec3Test.y, Delta);
            Assert.AreEqual(vec3.z, vec3Test.z, Delta);
        }

        [Test]
        public void TestQuaternionConversion()
        {
            UnityEngine.Quaternion q = new UnityEngine.Quaternion(1.5f, 2.5f, 3.5f, 4.5f);
            Quaternion qTest = Utilities.MapUnityQuaternion(q);

            Assert.AreEqual(q.w, qTest.Real, Delta);
            Assert.AreEqual(q.x, qTest.ImagX, Delta);
            Assert.AreEqual(q.y, qTest.ImagY, Delta);
            Assert.AreEqual(q.z, qTest.ImagZ, Delta);

            UnityEngine.Quaternion qTest2 = Utilities.MapQuaternion(qTest);

            Assert.AreEqual(q.w, qTest2.w, Delta);
            Assert.AreEqual(q.x, qTest2.x, Delta);
            Assert.AreEqual(q.y, qTest2.y, Delta);
            Assert.AreEqual(q.z, qTest2.z, Delta);
        }
    }
}