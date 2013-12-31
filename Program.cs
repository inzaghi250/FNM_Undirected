using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FNM_Undirected
{
    public class Program
    {
        static void Main(string[] args)
        {
            TestNBMining();
            //TestPathMining();
            return;
        }
        public class ShowingNames
        {
            public Dictionary<int, string> _vertexLabelNames = new Dictionary<int, string>();
            public Dictionary<int, string> _edgeLabelNames = new Dictionary<int, string>();
            public Dictionary<int, string> _vertexNames = new Dictionary<int, string>();
            public ShowingNames(string dir)
            {
                StreamReader sr = new StreamReader(dir + ".names.vertexlabel.txt");
                string line;
                while (true)
                {
                    line = sr.ReadLine();
                    if (line == null)
                        break;
                    string[] parts = line.Split("\t".ToCharArray());
                    _vertexLabelNames[int.Parse(parts[0])] = parts[1];
                }
                sr.Close();
                sr = new StreamReader(dir + ".names.edgelabel.txt");
                while (true)
                {
                    line = sr.ReadLine();
                    if (line == null)
                        break;
                    string[] parts = line.Split("\t".ToCharArray());
                    _edgeLabelNames[int.Parse(parts[0])] = parts[1];
                }
                sr.Close();
                sr = new StreamReader(dir + ".names.vertex.txt");
                while (true)
                {
                    line = sr.ReadLine();
                    if (line == null)
                        break;
                    string[] parts = line.Split("\t".ToCharArray());
                    _vertexNames[int.Parse(parts[0])] = parts[1];
                }
                sr.Close();
            }
        }

        private static void ShowResult(List<Tuple<Path, object>> ret, ShowingNames showName)
        {
            foreach (var pair in ret.OrderByDescending(e => e.Item2))
            {
                Console.Write(pair.Item2 + "\t");
                foreach (Step step in pair.Item1)
                {
                    if (step.Item2 == StepType.Nlabel)
                        Console.Write(showName._vertexLabelNames[step.Item1] + " ");
                    else
                        Console.Write(showName._edgeLabelNames[step.Item1] + " ");
                }
                Console.WriteLine();
            }
        }

        static void TestPathMining()
        {
            IndexedGraph g = new IndexedGraph();
            g.Read(@"D:\data\Mutagenicity\1000.txt");
            FrequentPathMining fpm=new FrequentPathMiningBreadth();
            fpm.Init(g, 5, 3,true);
            return;
        }

        private static void Shuffle(int[] a)
        {
            Random r = new Random(1);
            for (int i = a.Length - 1; i >= 0; i--)
            {
                int j = (int)(i * r.NextDouble());
                int temp = a[i];
                a[i] = a[j];
                a[j] = temp;
            }
            return;
        }

        private static void TestNBMining()
        {
            /*
            int graphNum = 1000;
            int[] idList = new int[graphNum];
            for (int i = 1; i <= graphNum; i++)
                idList[i-1] = i;
            IAMFileConverter.GenGraphFile(idList, @"D:\data\Mutagenicity\", "1000");
            */

            IndexedGraph g = new IndexedGraph();
            g.Read(@"D:\data\Mutagenicity\1000.txt");
            FrequentNeighborhoodMining fnm = new FrequentNeighborhoodMining(g);
            var retLarge = fnm.MineEgonet(2, 9, 1);
            //var retSmall = fnm.Mine(2, 4, true).Where(e=>e.Item1.Is_R_EgoNet(4)).ToList();

            //Console.WriteLine("{0} {1}", retSmall.Count, retLarge.Count);

            //for (int i = 0; i < retLarge.Count; i++)
            //{
            //    SubGraphTest sgt = new SubGraphTest();
            //    bool exist = false;
            //    for (int j = i+1; j < retLarge.Count; j++)
            //    {
            //        var p1 = retLarge[i];
            //        var p2 = retLarge[j];
            //        if (sgt.MatchNeighborhood(p1.Item1, p2.Item1, 0) && sgt.MatchNeighborhood(p2.Item1, p1.Item1, 0))
            //        {
            //            exist = true;
            //            break;
            //        }
            //    }
                
            //}
            return;
        }
    }
}
