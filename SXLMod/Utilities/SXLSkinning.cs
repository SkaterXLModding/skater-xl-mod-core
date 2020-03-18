using System;
using System.IO;
using System.Linq;

using UnityEngine;

namespace SXLMod.Skinning
{
    public class SXLSkinning
    {
        public static BoneWeight[] TransferSkinWeights(Mesh oldMesh, Mesh newMesh)
        {

            // USAGE:
            // newMesh.boneWeights = SXLSkinning.TransferSkinWeights(oldMesh, newMesh);
            //
            // For each vertex on newMesh Iterate through old mesh to find the closest world space vertex and grab
            // the index to get weights[i].boneIndex[0,1,2,3] and weight[0,1,2,3] values

            // Set up new emtpy weights
            BoneWeight[] xferWeights = new BoneWeight[newMesh.vertices.Length];

            // Iterate through mesh vertices grabbing the closest relative point and transferring weight
            // information from oldMesh to newMesh - maintaining references to associated bone indices.
            for (int i = 0; i < newMesh.vertices.Length; i++)
            {
                Vector3 point = newMesh.vertices[i];

                var nClosest = oldMesh.vertices.OrderBy(v => (v - point).sqrMagnitude).First();
                int nearestVertexIndex = oldMesh.vertices.Select((item, index) => new
                {
                    ITEM = item,
                    INDEX = index
                }).Where(v => v.ITEM == nClosest).First().INDEX;


                // Set relative bone weights
                BoneWeight weight = new BoneWeight();
                weight.boneIndex0 = oldMesh.boneWeights[nearestVertexIndex].boneIndex0;
                weight.boneIndex1 = oldMesh.boneWeights[nearestVertexIndex].boneIndex1;
                weight.boneIndex2 = oldMesh.boneWeights[nearestVertexIndex].boneIndex2;
                weight.boneIndex3 = oldMesh.boneWeights[nearestVertexIndex].boneIndex3;
                weight.weight0 = oldMesh.boneWeights[nearestVertexIndex].weight0;
                weight.weight1 = oldMesh.boneWeights[nearestVertexIndex].weight1;
                weight.weight2 = oldMesh.boneWeights[nearestVertexIndex].weight2;
                weight.weight3 = oldMesh.boneWeights[nearestVertexIndex].weight3;
                // Set xferWeights index value
                xferWeights[i] = weight;
            }

            return xferWeights;
        }

        public static void WriteWeightsToFile(string filePath, BoneWeight[] weights)
        {
            FileStream stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
            using (StreamWriter writer = new StreamWriter(stream))
            {
                for (int i = 0; i < weights.Length; i++)
                {
                    BoneWeight weight = weights[i];
                    string boneIndexString = $"{weight.boneIndex0} {weight.boneIndex1} {weight.boneIndex2} {weight.boneIndex3}";
                    string weightString = $"{weight.weight0} {weight.weight1} {weight.weight2} {weight.weight3}";
                    writer.Write($"{i} {boneIndexString} {weightString}\n");
                }
                writer.Flush();
            }
        }

        public static BoneWeight[] ReadWeightsFromFile(string filePath)
        {
            StreamReader reader = new StreamReader(filePath);
            int lineCount = File.ReadAllLines(filePath).Length;

            BoneWeight[] newWeights = new BoneWeight[lineCount];

            for (int i = 0; i < lineCount; i++)
            {
                string line = reader.ReadLine();
                string[] vertexData = line.Split(' ');

                BoneWeight weight = new BoneWeight();
                weight.boneIndex0 = Int32.Parse(vertexData[1]);
                weight.boneIndex1 = Int32.Parse(vertexData[2]);
                weight.boneIndex2 = Int32.Parse(vertexData[3]);
                weight.boneIndex3 = Int32.Parse(vertexData[4]);
                weight.weight0 = float.Parse(vertexData[5]);
                weight.weight1 = float.Parse(vertexData[6]);
                weight.weight2 = float.Parse(vertexData[7]);
                weight.weight3 = float.Parse(vertexData[8]);
                newWeights[i] = weight;
            }
            return newWeights;
        }
    }
}
