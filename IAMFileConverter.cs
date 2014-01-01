using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace FNM_Undirected
{
    public class IAMFileConverter
    {
        public static string _dataDir = @"D:\data\Mutagenicity\data\";

        public static void GenGraphFile(int[] idLists, string outputDir,string outputName)
        {
            Regex nodeReg = new Regex(@"<node id=""(\d+)""><attr name=""chem""><string>(\w+)</string></attr></node>"),
                edgeReg = new Regex(@"<edge from=""(\d+)"" to=""(\d+)""><attr name=""valence""><int>(\w+)</int></attr></edge>");
            Dictionary<string, int> mapNLabel2ID = new Dictionary<string, int>(),
                mapELabel2ID = new Dictionary<string, int>();
            List<Tuple<int, int>> nodeLabels = new List<Tuple<int, int>>();
            List<Tuple<int, int, int>> labeledEdges = new List<Tuple<int, int, int>>();
            int nodeOffset = 0;

            foreach (int id in idLists)
            {
                int maxLocalNodeId = 0;
                string fileName = "molecule_" + id + ".gxl";
                foreach (string line in File.ReadAllLines(_dataDir + fileName))
                {
                    Match match = nodeReg.Match(line);
                    if (match.Success)
                    {
                        int localNodeId = int.Parse(match.Groups[1].Value);
                        if (localNodeId > maxLocalNodeId)
                            maxLocalNodeId = localNodeId;
                        if (!mapNLabel2ID.ContainsKey(match.Groups[2].Value))
                            mapNLabel2ID[match.Groups[2].Value] = mapNLabel2ID.Count;
                        nodeLabels.Add(new Tuple<int,int>(localNodeId+nodeOffset,mapNLabel2ID[match.Groups[2].Value]));
                    }
                    match = edgeReg.Match(line);
                    if (match.Success)
                    {
                        if (!mapELabel2ID.ContainsKey(match.Groups[3].Value))
                            mapELabel2ID[match.Groups[3].Value] = mapELabel2ID.Count;
                        labeledEdges.Add(new Tuple<int,int,int>(
                            int.Parse(match.Groups[1].Value)+nodeOffset,
                            int.Parse(match.Groups[2].Value)+nodeOffset,
                            mapELabel2ID[match.Groups[3].Value]));
                    }
                }
                nodeOffset += maxLocalNodeId;
            }
            StreamWriter sw = new StreamWriter(outputDir + outputName + ".txt");
            sw.WriteLine(nodeOffset + "\t" + nodeLabels.Count + "\t" + labeledEdges.Count + "\t" + mapNLabel2ID.Count + "\t" + mapELabel2ID.Count);
            foreach (var tup in nodeLabels)
                sw.WriteLine(tup.Item1-1 + "\tisa\t" + tup.Item2);
            foreach (var tup in labeledEdges)
            {
                if (tup.Item1 < tup.Item2)
                    sw.WriteLine((tup.Item1-1) + "\t" + (tup.Item2-1) + "\t" + tup.Item3);
                else
                    sw.WriteLine((tup.Item2-1) + "\t" + (tup.Item1-1) + "\t" + tup.Item3);
            }
            sw.Close();
            return;
        }
    }
}
