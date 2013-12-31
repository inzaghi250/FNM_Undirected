﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FNM_Undirected
{
    public class Edge
    {
        public int _v1, _v2, _eLabel;
    }

    public class Vertex : ICloneable
    {
        public static int[] zeroL = new int[0];
        public Vertex()
        {
            _assoEdge = zeroL;
            _vLabel = zeroL;
        }

        public int[] _assoEdge;
        public int[] _vLabel;

        public bool Isolated()
        {
            return _assoEdge.Length == 0 && _vLabel.Length == 0;
        }

        #region ICloneable Members

        public object Clone()
        {
            return MemberwiseClone();
        }

        #endregion
    }

    public class Graph
    {
        public Edge[] _edges;
        public Vertex[] _vertexes;

        public int GetOtherVertexID(int fromV, Edge e)
        {
            if (e._v1 == fromV)
                return e._v2;
            if (e._v2 == fromV)
                return e._v1;
            return -1;
        }

        public int GetOtherVertexID(Vertex fromV, Edge e)
        {
            Vertex v1 = _vertexes[e._v1];
            if (v1 == fromV)
                return e._v2;
            Vertex v2 = _vertexes[e._v2];
            if (v2 == fromV)
                return e._v1;
            return -1;
        }

        /*to be modified*/
        static Comparison<Edge> _edgeCmp = new Comparison<Edge>((e1, e2) =>
            {
                if (e1._eLabel < e2._eLabel)
                    return -1;
                if (e1._eLabel > e2._eLabel)
                    return 1;
                int e1min = Math.Min(e1._v1, e1._v2),
                    e2min = Math.Min(e2._v1, e2._v2),
                    e1max = Math.Max(e1._v1, e1._v2),
                    e2max = Math.Max(e2._v1, e2._v2);
                if (e1min < e2min)
                    return -1;
                if (e1min > e2min)
                    return 1;
                if (e1max < e2max)
                    return -1;
                if (e1max > e2max)
                    return 1;
                return 0;
            });

        public virtual void Read(string file)
        {
            StreamReader sr = new StreamReader(file);
            string line;

            line = sr.ReadLine();
            string[] parts = line.Split("\t".ToCharArray());

            _vertexes = new Vertex[int.Parse(parts[0])];
            _edges = new Edge[int.Parse(parts[2])];

            int nEdge = 0;

            Dictionary<int, HashSet<int>> vertexLabelSets = new Dictionary<int, HashSet<int>>();

            while (true)
            {
                line = sr.ReadLine();
                if (line == null)
                    break;
                parts = line.Split("\t".ToCharArray());
                if (parts[1] == "isa")
                {
                    int vid = int.Parse(parts[0]), vlid = int.Parse(parts[2]);
                    if (!vertexLabelSets.ContainsKey(vid))
                        vertexLabelSets[vid] = new HashSet<int>();
                    vertexLabelSets[vid].Add(vlid);
                }
                else//assert v1<v2
                {
                    int v1 = int.Parse(parts[0]), v2 = int.Parse(parts[1]), elid = int.Parse(parts[2]);
                    if (v1 >= v2)
                        throw new Exception("Edge data format error.");
                    Edge e = new Edge();
                    e._v1 = v1; e._v2 = v2; e._eLabel = elid;
                    _edges[nEdge++] = e;
                }
            }

            Array.Sort(_edges, _edgeCmp);

            for (int i = 0; i < _vertexes.Length; i++)
            {
                _vertexes[i] = new Vertex();
                if (vertexLabelSets.ContainsKey(i))
                {
                    HashSet<int> set = vertexLabelSets[i];
                    _vertexes[i]._vLabel = new int[set.Count];
                    int j = 0;
                    foreach (int vlid in set)
                        _vertexes[i]._vLabel[j++] = vlid;
                    Array.Sort(_vertexes[i]._vLabel);
                }
                else
                    _vertexes[i]._vLabel = new int[0];
            }

            int[] deg = new int[_vertexes.Length];
            foreach (Edge e in _edges)
            {
                deg[e._v1]++;
                deg[e._v2]++;
            }
            for (int i = 0; i < _vertexes.Length; i++)
            {
                _vertexes[i]._assoEdge = new int[deg[i]];
            }
            for (int i = 0; i < _edges.Length; i++)
            {
                Edge e = _edges[i];
                _vertexes[e._v1]._assoEdge[--deg[e._v1]] = i;
                _vertexes[e._v2]._assoEdge[--deg[e._v2]] = i;
            }
            for (int i = 0; i < _vertexes.Length; i++)
            {
                Array.Sort(_vertexes[i]._assoEdge);
            }

        }
        public IndexedGraph ShallowCopy()
        {
            IndexedGraph ret = new IndexedGraph();

            ret._edges = new Edge[_edges.Length];
            _edges.CopyTo(ret._edges, 0);

            ret._vertexes = new Vertex[_vertexes.Length];
            for (int i = 0; i < _vertexes.Length; i++)
            {
                ret._vertexes[i] = _vertexes[i].Clone() as Vertex;
            }
            return ret;
        }
        public virtual bool ContainsEdge(Edge edge)
        {   
            Vertex v1 = _vertexes[edge._v1],
                v2 = _vertexes[edge._v2];
            if (v1._assoEdge.Length < v2._assoEdge.Length)
            {
                foreach (int eid in v1._assoEdge)
                {
                    Edge e = _edges[eid];
                    if (_edgeCmp(e, edge) == 0)
                        return true;
                }
                return false;
            }
            else
            {
                foreach (int eid in v2._assoEdge)
                {
                    Edge e = _edges[eid];
                    if (_edgeCmp(e, edge) == 0)
                        return true;
                }
                return false;
            }
        }
        public virtual void Print()
        {
            for (int i = 0; i < _vertexes.Length; i++)
            {
                Console.Write("v{0}: ", i);
                foreach (int vlid in _vertexes[i]._vLabel)
                    Console.Write("{0} ", vlid);
                Console.WriteLine();
            }
            for (int i = 0; i < _edges.Length; i++)
            {
                Edge e = _edges[i];
                Console.WriteLine("e{0}: {1} {2} {3}", i, e._v1, e._v2, e._eLabel);
            }
        }

        public bool ContainsCycle()//Assert: graph is connected
        {
            Queue<int> queue = new Queue<int>();
            HashSet<int> visitedV=new HashSet<int>();
            HashSet<int> visitedE = new HashSet<int>();
            queue.Enqueue(0);
            visitedV.Add(0);
            while (queue.Count > 0)
            {
                int topV = queue.Dequeue();
                Vertex v = _vertexes[topV];
                foreach (int eid in v._assoEdge)
                {
                    if (visitedE.Contains(eid))
                        continue;
                    visitedE.Add(eid);
                    Edge e = _edges[eid];
                    int otherVID = GetOtherVertexID(v, e);
                    if (visitedV.Contains(otherVID))
                        return true;
                    visitedV.Add(otherVID);
                    queue.Enqueue(otherVID);
                }
            }
            return false;
        }

        public int[] GetDistArray()
        {
            int[] ret = new int[_vertexes.Length];
            Queue<Tuple<int, int>> queue = new Queue<Tuple<int, int>>();
            HashSet<int> visitedV = new HashSet<int>();
            queue.Enqueue(new Tuple<int, int>(0, 0));
            visitedV.Add(0);
            int[] dist = new int[_vertexes.Length];
            while (queue.Count > 0)
            {
                var tup = queue.Dequeue();
                ret[tup.Item1] = tup.Item2;
                Vertex v = _vertexes[tup.Item1];
                foreach (int eid in v._assoEdge)
                {
                    Edge e = _edges[eid];
                    int otherVID = GetOtherVertexID(v, e);
                    if (visitedV.Contains(otherVID))
                        continue;
                    visitedV.Add(otherVID);
                    queue.Enqueue(new Tuple<int, int>(otherVID, tup.Item2 + 1));
                }
            }
            if (visitedV.Count < _vertexes.Length)
                return null;
            return ret;
        }

        public bool Is_R_EgoNet(int radius)//Assert: graph is connected, pivot is node0
        {
            Queue<Tuple<int, int>> queue = new Queue<Tuple<int, int>>();
            HashSet<int> visitedV = new HashSet<int>();
            queue.Enqueue(new Tuple<int, int>(0, 0));
            visitedV.Add(0);
            int[] dist = new int[_vertexes.Length];
            while (queue.Count > 0)
            {
                var tup = queue.Dequeue();
                Vertex v = _vertexes[tup.Item1];
                foreach (int eid in v._assoEdge)
                {
                    Edge e = _edges[eid];
                    int otherVID = GetOtherVertexID(v, e);
                    if (visitedV.Contains(otherVID))
                        continue;
                    visitedV.Add(otherVID);
                    if (tup.Item2 == radius)
                        return false;
                    queue.Enqueue(new Tuple<int, int>(otherVID, tup.Item2 + 1));
                }
            }
            if (visitedV.Count < _vertexes.Length)
                return false;
            return true;
        }
    }

    public class IndexedGraph : Graph
    {
        public Dictionary<int, int[]> _vertexLabelIndex;
        public Dictionary<int, int[]> _edgeLabelIndex;

        public override void Read(string file)
        {
            base.Read(file);
            GenIndex();
        }

        public void GenIndex()
        {
            _vertexLabelIndex = new Dictionary<int, int[]>();
            _edgeLabelIndex = new Dictionary<int, int[]>();

            Dictionary<int, int> vertexLabelCnt = new Dictionary<int, int>();
            Dictionary<int, int> edgeLabelCnt = new Dictionary<int, int>();

            for (int i = 0; i < _vertexes.Length; i++)
            {
                foreach (int vlid in _vertexes[i]._vLabel)
                {
                    if (!vertexLabelCnt.ContainsKey(vlid))
                        vertexLabelCnt[vlid] = 0;
                    vertexLabelCnt[vlid]++;
                }
            }

            foreach (var pair in vertexLabelCnt)
                _vertexLabelIndex[pair.Key] = new int[pair.Value];

            for (int i = 0; i < _vertexes.Length; i++)
            {
                foreach (int vlid in _vertexes[i]._vLabel)
                {
                    _vertexLabelIndex[vlid][--vertexLabelCnt[vlid]] = i;
                }
            }
            foreach (var pair in _vertexLabelIndex)
                Array.Sort(pair.Value);

            foreach (Edge e in _edges)
            {
                if (!edgeLabelCnt.ContainsKey(e._eLabel))
                    edgeLabelCnt[e._eLabel] = 0;
                edgeLabelCnt[e._eLabel]++;
            }
            foreach (var pair in edgeLabelCnt)
            {
                _edgeLabelIndex[pair.Key] = new int[pair.Value];
            }

            for (int i = 0; i < _edges.Length; i++)
            {
                _edgeLabelIndex[_edges[i]._eLabel][--edgeLabelCnt[_edges[i]._eLabel]] = i;
            }

            foreach (var pair in _edgeLabelIndex)
                Array.Sort(pair.Value);
        }
    }
}