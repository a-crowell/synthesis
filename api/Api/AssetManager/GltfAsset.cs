﻿using System;
using System.IO;
using SynthesisAPI.VirtualFileSystem;
using SharpGLTF.Schema2;
using SynthesisAPI.Utilities;
using SynthesisAPI.EnvironmentManager;
using System.Linq;
using SharpGLTF.Memory;
using MathNet.Spatial.Euclidean;
using SynthesisAPI.Runtime;

namespace SynthesisAPI.AssetManager
{
    public class GltfAsset : Asset
    {
        private ModelRoot model = null;

        public GltfAsset(string name, Permissions perm, string sourcePath)
        {
            Init(name, perm, sourcePath);
        }

        public override IEntry Load(byte[] data)
        {
            var stream = new MemoryStream();
            stream.Write(data, 0, data.Length);
            stream.Position = 0;

            GetModelInfo(stream, true);

            return this;
        }

        private void GetModelInfo(MemoryStream stream, bool tryFix = false)
        {
            try
            {
                var settings = tryFix ? SharpGLTF.Validation.ValidationMode.TryFix : SharpGLTF.Validation.ValidationMode.Strict;

                model = ModelRoot.ReadGLB(stream, settings);
            }
            catch (Exception)
            {
                Logger.Log("GLTF file cannot be read", LogLevel.Warning);
            }
        }

        #region Object Bundle

        public static implicit operator Bundle(GltfAsset gltfAsset) => gltfAsset.Parse();

        public Bundle Parse()
        {
            if (model == null) return null;
            return CreateBundle(model.DefaultScene.VisualChildren.First()); 
        }

        private Bundle CreateBundle(Node node, Node parent = null)
        {
            Bundle bundle = new Bundle();

            AddComponents(bundle, node, parent);

            foreach (Node child in node.VisualChildren)
                bundle.ChildBundles.Add(CreateBundle(child, node));
            return bundle;
        }

        private void AddComponents(Bundle bundle, Node node, Node parent = null)
        {
            if (parent != null) {
                var scale = node.LocalTransform.Scale;
                var parentScale = parent.LocalTransform.Scale;
                scale = new System.Numerics.Vector3(scale.X * parentScale.X, scale.Y * parentScale.Y, scale.Z * parentScale.Z);
                var localTransform = node.LocalTransform;
                localTransform.Scale = scale;
                node.LocalTransform = localTransform;
            }

            bundle.Components.Add(ParseTransform(node.LocalTransform));
            if (node.Mesh != null) bundle.Components.Add(ParseMesh(node.Mesh, node.LocalTransform.Scale.ToMathNet()));
        }

        private EnvironmentManager.Components.Mesh ParseMesh(Mesh nodeMesh, Vector3D scaleFactor)
        {
            EnvironmentManager.Components.Mesh m = new EnvironmentManager.Components.Mesh();
            foreach (MeshPrimitive primitive in nodeMesh.Primitives)
            {
                int c = m.Vertices.Count();
                // checks for POSITION or NORMAL vertex as not all designs have both (TODO: This doesn't trip, if it did would we screw up the triangles?)
                if (primitive.VertexAccessors.ContainsKey("POSITION"))
                {
                    Vector3Array vertices = primitive.GetVertices("POSITION").AsVector3Array();
                    foreach (System.Numerics.Vector3 vertex in vertices)
                        m.Vertices.Add(new Vector3D(vertex.X * scaleFactor.X, vertex.Y * scaleFactor.Y, vertex.Z * scaleFactor.Z));
                }

                var triangles = primitive.GetIndices();
                for (int i = 0; i < triangles.Count; i++)
                    m.Triangles.Add((int)triangles[i] + c);
            }
            return m;
        }

        private EnvironmentManager.Components.Transform ParseTransform(SharpGLTF.Transforms.AffineTransform nodeTransform)
        {
            EnvironmentManager.Components.Transform t = new EnvironmentManager.Components.Transform();

            t.Rotation = new Quaternion(nodeTransform.Rotation.W, nodeTransform.Rotation.X,
                nodeTransform.Rotation.Y, nodeTransform.Rotation.Z);
            t.Position = new Vector3D(nodeTransform.Translation.X * nodeTransform.Scale.X,
                nodeTransform.Translation.Y * nodeTransform.Scale.Y, nodeTransform.Translation.Z * nodeTransform.Scale.Z);
            //scale is applied directly to vertices -> default 1x

            return t;
        }
        #endregion
    }
}