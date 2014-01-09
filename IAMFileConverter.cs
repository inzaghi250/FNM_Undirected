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

        public static Dictionary<string, int> _mapNLabel2ID = new Dictionary<string,int>(),
            _mapELabel2ID = new Dictionary<string, int>();

        public static void GenMutagGraphFile(int[] idLists, string outputDir,string outputName)
        {
            _mapELabel2ID["1"] = 0;
            _mapELabel2ID["2"] = 1;
            _mapELabel2ID["3"] = 2;

            _mapNLabel2ID["C"] = 0;
            _mapNLabel2ID["N"] = 1;
            _mapNLabel2ID["O"] = 2;
            _mapNLabel2ID["H"] = 3;
            _mapNLabel2ID["S"] = 4;
            _mapNLabel2ID["Cl"] = 5;
            _mapNLabel2ID["Br"] = 6;
            _mapNLabel2ID["F"] = 7;
            _mapNLabel2ID["I"] = 8;
            _mapNLabel2ID["K"] = 9;
            _mapNLabel2ID["P"] = 10;
            _mapNLabel2ID["Na"] = 11;
            _mapNLabel2ID["Li"] = 12;
            _mapNLabel2ID["Ca"] = 13;

            Regex nodeReg = new Regex(@"<node id=""(\d+)""><attr name=""chem""><string>(\w+)</string></attr></node>"),
                edgeReg = new Regex(@"<edge from=""(\d+)"" to=""(\d+)""><attr name=""valence""><int>(\w+)</int></attr></edge>");
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
                        nodeLabels.Add(new Tuple<int,int>(localNodeId+nodeOffset,_mapNLabel2ID[match.Groups[2].Value]));
                    }
                    match = edgeReg.Match(line);
                    if (match.Success)
                    {
                        labeledEdges.Add(new Tuple<int,int,int>(
                            int.Parse(match.Groups[1].Value)+nodeOffset,
                            int.Parse(match.Groups[2].Value)+nodeOffset,
                            _mapELabel2ID[match.Groups[3].Value]));
                    }
                }
                nodeOffset += maxLocalNodeId;
            }
            StreamWriter sw = new StreamWriter(outputDir + outputName + ".txt");
            sw.WriteLine(nodeOffset + "\t" + nodeLabels.Count + "\t" + labeledEdges.Count + "\t" + _mapNLabel2ID.Count + "\t" + _mapELabel2ID.Count);
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

        public static void GenAIDSGraphFile(int[] idLists, string outputDir, string outputName)
        {
            _mapELabel2ID["1"] = 0;
            _mapELabel2ID["2"] = 1;
            _mapELabel2ID["3"] = 2;

            _mapNLabel2ID["C"] = 0;
            _mapNLabel2ID["N"] = 1;
            _mapNLabel2ID["O"] = 2;
            _mapNLabel2ID["H"] = 3;
            _mapNLabel2ID["S"] = 4;
            _mapNLabel2ID["Cl"] = 5;
            _mapNLabel2ID["Br"] = 6;
            _mapNLabel2ID["F"] = 7;
            _mapNLabel2ID["I"] = 8;
            _mapNLabel2ID["K"] = 9;
            _mapNLabel2ID["P"] = 10;
            _mapNLabel2ID["Na"] = 11;
            _mapNLabel2ID["Li"] = 12;
            _mapNLabel2ID["Ca"] = 13;
            _mapNLabel2ID["B"] = 14;
            _mapNLabel2ID["Co"] = 15;
            _mapNLabel2ID["Pt"] = 16;
            _mapNLabel2ID["Ho"] = 17;
            _mapNLabel2ID["Si"] = 18;
            _mapNLabel2ID["Se"] = 19;
            _mapNLabel2ID["Mg"] = 20;
            _mapNLabel2ID["Pd"] = 21;
            _mapNLabel2ID["As"] = 22;
            _mapNLabel2ID["Ni"] = 23;
            _mapNLabel2ID["Te"] = 24;
            _mapNLabel2ID["Ru"] = 25;
            _mapNLabel2ID["Tl"] = 26;
            _mapNLabel2ID["Cu"] = 27;
            _mapNLabel2ID["Ga"] = 28;
            _mapNLabel2ID["Tb"] = 29;

            Regex nodeReg = new Regex(@"<node id=""_(\d+)""><attr name=""symbol""><string>(\w+)( )*</string>"),
                edgeReg = new Regex(@"<edge from=""_(\d+)"" to=""_(\d+)""><attr name=""valence""><int>(\d+)</int></attr></edge>");
            List<Tuple<int, int>> nodeLabels = new List<Tuple<int, int>>();
            List<Tuple<int, int, int>> labeledEdges = new List<Tuple<int, int, int>>();
            int nodeOffset = 0;

            string dataDir=@"D:\data\AIDS\data\";

            List<string> fileNames = Directory.EnumerateFiles(dataDir).ToList();

            foreach (int id in idLists)
            {
                int maxLocalNodeId = 0;
                string fileName = fileNames[id - 1];
                string text = File.ReadAllText(fileName);

                MatchCollection matches=nodeReg.Matches(text);
                foreach(Match match in matches)
                {
                    int localNodeId = int.Parse(match.Groups[1].Value);
                    if (localNodeId > maxLocalNodeId)
                        maxLocalNodeId = localNodeId;
                    nodeLabels.Add(new Tuple<int, int>(localNodeId + nodeOffset, _mapNLabel2ID[match.Groups[2].Value]));
                }
                matches = edgeReg.Matches(text);
                foreach(Match match in matches)
                {
                    labeledEdges.Add(new Tuple<int, int, int>(
                        int.Parse(match.Groups[1].Value) + nodeOffset,
                        int.Parse(match.Groups[2].Value) + nodeOffset,
                        _mapELabel2ID[match.Groups[3].Value]));
                }
                nodeOffset += maxLocalNodeId;
            }
            StreamWriter sw = new StreamWriter(outputDir + outputName + ".txt");
            sw.WriteLine(nodeOffset + "\t" + nodeLabels.Count + "\t" + labeledEdges.Count + "\t" + _mapNLabel2ID.Count + "\t" + _mapELabel2ID.Count);
            foreach (var tup in nodeLabels)
                sw.WriteLine(tup.Item1 - 1 + "\tisa\t" + tup.Item2);
            foreach (var tup in labeledEdges)
            {
                if (tup.Item1 < tup.Item2)
                    sw.WriteLine((tup.Item1 - 1) + "\t" + (tup.Item2 - 1) + "\t" + tup.Item3);
                else
                    sw.WriteLine((tup.Item2 - 1) + "\t" + (tup.Item1 - 1) + "\t" + tup.Item3);
            }
            sw.Close();
            return;
        }

        public static void GenProteinGraphFile(int[] idLists, string outputDir, string outputName)
        {
            _mapNLabel2ID["0"] = 0;
            _mapNLabel2ID["1"] = 1;
            _mapNLabel2ID["2"] = 2;

            _mapELabel2ID["1"] = 0;
            _mapELabel2ID["2"] = 1;
            _mapELabel2ID["3"] = 2;
            _mapELabel2ID["4"] = 3;
            _mapELabel2ID["5"] = 4;

            Regex nodeReg = new Regex(@"<node id=""(\d+)""><attr name=""type""><int>(\d+)</int>"),
                edgeReg = new Regex(@"<edge from=""(\d+)"" to=""(\d+)""><attr name=""frequency""><int>\d+</int></attr><attr name=""type0""><double>(\d+)</double>");
            List<Tuple<int, int>> nodeLabels = new List<Tuple<int, int>>();
            List<Tuple<int, int, int>> labeledEdges = new List<Tuple<int, int, int>>();
            int nodeOffset = 0;

            string dataDir = @"D:\data\Protein\data\";
            foreach (int id in idLists)
            {
                int maxLocalNodeId = 0;
                string fileName = "enzyme_" + id + ".gxl";
                foreach (string line in File.ReadAllLines(dataDir + fileName))
                {
                    Match match = nodeReg.Match(line);
                    if (match.Success)
                    {
                        int localNodeId = int.Parse(match.Groups[1].Value);
                        if (localNodeId > maxLocalNodeId)
                            maxLocalNodeId = localNodeId;
                        nodeLabels.Add(new Tuple<int, int>(localNodeId + nodeOffset, _mapNLabel2ID[match.Groups[2].Value]));
                    }
                    match = edgeReg.Match(line);
                    if (match.Success)
                    {
                        labeledEdges.Add(new Tuple<int, int, int>(
                            int.Parse(match.Groups[1].Value) + nodeOffset,
                            int.Parse(match.Groups[2].Value) + nodeOffset,
                            _mapELabel2ID[match.Groups[3].Value]));
                    }
                }
                nodeOffset += maxLocalNodeId;
            }
            StreamWriter sw = new StreamWriter(outputDir + outputName + ".txt");
            sw.WriteLine(nodeOffset + "\t" + nodeLabels.Count + "\t" + labeledEdges.Count + "\t" + _mapNLabel2ID.Count + "\t" + _mapELabel2ID.Count);
            foreach (var tup in nodeLabels)
                sw.WriteLine(tup.Item1 - 1 + "\tisa\t" + tup.Item2);
            foreach (var tup in labeledEdges)
            {
                if (tup.Item1 < tup.Item2)
                    sw.WriteLine((tup.Item1 - 1) + "\t" + (tup.Item2 - 1) + "\t" + tup.Item3);
                else
                    sw.WriteLine((tup.Item2 - 1) + "\t" + (tup.Item1 - 1) + "\t" + tup.Item3);
            }
            sw.Close();
            return;
        }
                
    }
}
