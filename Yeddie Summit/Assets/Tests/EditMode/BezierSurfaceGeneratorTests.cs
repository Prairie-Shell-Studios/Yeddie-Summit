using NUnit.Framework;
using PrairieShellStudios.MountainGeneration;
using System;
using UnityEngine;

namespace PrairieShellStudios.TerrainGenerationTests
{
    public class BezierSurfaceGeneratorTests
    {
        private Vector3[] ctrlPoints = {
        new Vector3(0.174329f, 0f, 0.126437f),
        new Vector3(0.174329f, -0.120153f, 0.126437f),
        new Vector3(0.0799234f, -0.214559f, 0.126437f),
        new Vector3(-0.0402299f, -0.214559f, 0.126437f),
        new Vector3(0.164751f, 0f, 0.146552f),
        new Vector3(0.164751f, -0.114789f, 0.146552f),
        new Vector3(0.0745594f, -0.204981f, 0.146552f),
        new Vector3(-0.0402299f, -0.204981f, 0.146552f),
        new Vector3(0.180077f, 0f, 0.146552f),
        new Vector3(0.180077f, -0.123372f, 0.146552f),
        new Vector3(0.0831418f, -0.220307f, 0.146552f),
        new Vector3(-0.0402299f, -0.220307f, 0.146552f),
        new Vector3(0.189655f, 0f, 0.126437f),
        new Vector3(0.189655f, -0.128736f, 0.126437f),
        new Vector3(0.0885057f, -0.229885f, 0.126437f),
        new Vector3(-0.0402299f, -0.229885f, 0.126437f)
    };
        private BezierSurfaceGenerator generator;
        private Tuple<Vector3[], int[]> meshInfo;
        private const int meshResolution = 10;

        [SetUp]
        public void Setup()
        {
            generator = new BezierSurfaceGenerator();
            meshInfo = generator.GenerateSurface(ctrlPoints, meshResolution, meshResolution);
        }

        [Test]
        public void NullTest()
        {
            Assert.NotNull(generator);
            Assert.NotNull(meshInfo);
            Assert.NotNull(meshInfo.Item1);
            Assert.NotNull(meshInfo.Item2);
        }

        [Test]
        public void SizeTest()
        {
            Assert.AreEqual((meshResolution + 1) * (meshResolution + 1), meshInfo.Item1.Length);
            Assert.AreEqual(meshResolution * meshResolution * 6, meshInfo.Item2.Length);
        }

        [Test]
        public void VerticesNotNullTest()
        {
            foreach (Vector3 vertex in meshInfo.Item1)
            {
                Assert.NotNull(vertex);
            }
        }

        [Test]
        public void TrianglesNotNullTest()
        {
            foreach (int triangle in meshInfo.Item2)
            {
                Assert.NotNull(triangle);
            }
        }
    }
}