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
            g.Read(@"D:\Users\v-jiahan\HORM\Data\ex3_graph.txt");
            FrequentPathMining fpm=new FrequentPathMiningBreadth();
            fpm.Init(g, 1000, 3,true);
            ShowingNames showname = new ShowingNames(@"D:\Users\v-jiahan\HORM\Data\ex3_graph");
            ShowResult(fpm._resultCache, showname);
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
            //StreamWriter sw = new StreamWriter(@"D:\Users\v-jiahan\HORM\Data\chem_graph.txt");
            //ChemFileConverter.Convert(Console.In, sw, 0);
            //ChemFileConverter.Convert(Console.In, sw, 21);
            //sw.Close();
            //return;

            IndexedGraph g = new IndexedGraph();
            g.Read(@"D:\Users\v-jiahan\HORM\Data\5star_graph.txt");

            //TestCocitation(g);
            //return;
            ////TestPattern(g);
            ////return;

            ////List<Tuple<IndexedGraph, double, double>> ret = fnMiner.Mine(0.9, 3, 1);

            ////conf 0~6712, author 6713~923691, paper 923692~2495971

            //int[] authors = new int[923692 - 6713];
            //for (int i = 6713; i < 923692; i++)
            //    authors[i-6713] = i;

            //int[] conf = new int[6713];
            //for (int i = 0; i < 6713; i++)
            //    conf[i] = i;

            //int[] paper = new int[2495972 - 923692];
            //for (int i = 923692; i < 2495972; i++)
            //    paper[i - 923692] = i;

            //Shuffle(paper);

            //int fold=1;
            //while (fold <= 10)
            //{
                //Console.WriteLine("Fold: {0}", fold);
                //int nPaper = 100000 * fold;
                //int[] selectedPaper = new int[nPaper];
                //for (int i = 0; i < nPaper; i++)
                //    selectedPaper[i] = paper[i];
                //Array.Sort(selectedPaper);

                FrequentNeighborhoodMining fnMiner = new FrequentNeighborhoodMining(g);

                //Console.WriteLine(fnMiner.GetSimilarity(9, 26, 4));
                //return;
                for (int i = 0; i < g._vertexes.Length;i++ )
                {
                    for (int j = 0; j < g._vertexes.Length; j++)
                        Console.Write(fnMiner.GetSimilarity(i,j,4).ToString("0.000")+" ");
                    Console.WriteLine();
                }


                    //List<Tuple<IndexedGraph, double, double>> ret = fnMiner.Mine(0.01, 4, conf);

                    //foreach (var pair in ret.OrderByDescending(edge => edge.Item2))
                    //{
                    //    pair.Item1.Print();
                    //    Console.WriteLine(pair.Item2 + " " + pair.Item3);
                    //    Console.WriteLine();
                    //}

                    //List<Tuple<IndexedGraph, int>> ret = fnMiner.Mine(g._vertexes.Length / 2, 3);

                    //foreach (var pair in ret.OrderByDescending(e => e.Item2))
                    //{
                    //    pair.Item1.Print();
                    //    //Console.WriteLine(pair.Item2);
                    //    Console.WriteLine(pair.Item1.ContainsCycle());
                    //    Console.WriteLine();
                    //}

                    //Console.WriteLine(ret.Count(e => e.Item1.ContainsCycle()) + " cycles.");
                    //    fold++;
                    //}
                    return;
        }
    }
}
