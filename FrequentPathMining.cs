using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FNM_Undirected
{
    public enum StepType
    {
        LinkTo, Nlabel
    }

    public class Step : Tuple<int, StepType>
    {
        public Step(int id, StepType type) : base(id, type) { }
        public override int GetHashCode()
        {
            return Item1.GetHashCode() | Item2.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            Step step = obj as Step;
            return Item1 == step.Item1 && Item2 == step.Item2;
        }
    }

    public class Path : List<Step>
    {
        public Path()
        {
        }
        public Path(Path path, Step step)
        {
            int i;
            for (i = 0; i < path.Count; i++)
                Add(path[i]);
            Add(step);
        }
        public IndexedGraph ToIndexedGraph()
        {
            IndexedGraph ret = new IndexedGraph();
            if (this[this.Count - 1].Item2 == StepType.Nlabel)
            {
                ret._vertexes = new Vertex[this.Count];
                for (int i = 0; i < ret._vertexes.Length; i++)
                    ret._vertexes[i] = new Vertex();
                ret._vertexes[this.Count - 1]._vLabel = new int[1] { this[this.Count - 1].Item1 };

                ret._edges = new Edge[this.Count - 1];
                for (int i = 0; i < this.Count - 1; i++)
                {
                    Edge e = new Edge();
                    e._eLabel = this[i].Item1;
                    e._v1 = i;
                    e._v2 = i + 1;
                    ret._edges[i] = e;

                    if (i == 0)
                    {
                        ret._vertexes[0]._assoEdge = new int[1] { 0 };
                    }
                    if (i == this.Count - 2)
                    {
                        ret._vertexes[i + 1]._assoEdge = new int[1] { i };
                    }
                    else
                    {
                        ret._vertexes[i + 1]._assoEdge = new int[2] { i,i+1 };
                    }

                }
            }
            else
            {
                ret._vertexes = new Vertex[this.Count + 1];
                for (int i = 0; i < ret._vertexes.Length; i++)
                    ret._vertexes[i] = new Vertex();

                ret._edges = new Edge[this.Count];
                for (int i = 0; i < this.Count; i++)
                {
                    Edge e = new Edge();
                    e._eLabel = this[i].Item1;
                    e._v1 = i;
                    e._v2 = i + 1;
                    ret._edges[i] = e;

                    if (i == 0)
                    {
                        ret._vertexes[0]._assoEdge = new int[1] { 0 };
                    }
                    if (i == this.Count - 1)
                    {
                        ret._vertexes[i + 1]._assoEdge = new int[1] { i };
                    }
                    else
                    {
                        ret._vertexes[i + 1]._assoEdge = new int[2] { i, i + 1 };
                    }
                }
            }
            ret.GenIndex();
            return ret;
        }
    }

    class FrequentPathMining
    {
        public List<Tuple<Path, List<int>>> _resultCache = new List<Tuple<Path, List<int>>>();

        public void Init(Graph g, int minSupp, int maxSize,bool useVIDList,int maxRadius=100)
        {
            int[] constraintVSet = new int[g._vertexes.Length];
            for (int i = 0; i < g._vertexes.Length; i++)
                constraintVSet[i] = i;
            Init(g, minSupp, maxSize, constraintVSet,useVIDList,maxRadius);
        }

        public virtual void Init(Graph g, int minSupp, int maxSize, int[] constraintVSet, bool useVIDList, int maxRadius=100)
        {
            throw new NotImplementedException();
        }

        public List<IndexedGraph> GetPath(int length)
        {
            List<IndexedGraph> ret = new List<IndexedGraph>();
            foreach (var pair in _resultCache.Where(e => e.Item1.Count == length))
                ret.Add(pair.Item1.ToIndexedGraph());
            return ret;
        }

        public List<Tuple<IndexedGraph, int>> GetPathAndCount(int length)
        {
            List<Tuple<IndexedGraph, int>> ret = new List<Tuple<IndexedGraph, int>>();
            foreach (var pair in _resultCache.Where(e => e.Item1.Count == length))
                ret.Add(new Tuple<IndexedGraph, int>(pair.Item1.ToIndexedGraph(), ((List<int>)pair.Item2).Count));
            return ret;
        }

        public List<Tuple<IndexedGraph, List<int>>> GetPathAndVID(int length)
        {
            List<Tuple<IndexedGraph, List<int>>> ret = new List<Tuple<IndexedGraph, List<int>>>();
            foreach (var pair in _resultCache.Where(e => e.Item1.Count == length))
                ret.Add(new Tuple<IndexedGraph, List<int>>(pair.Item1.ToIndexedGraph(), pair.Item2 as List<int>));
            return ret;
        }
    }

    class FrequentPathMiningBreadth:FrequentPathMining
    {
        class GetNextStepGivenNode
        {
            static Path _path;
            static Graph _g;
            static int _nodeId;
            static bool _VLabelOnly;

            static HashSet<Step> _ret = new HashSet<Step>();
            static HashSet<int> _nodeUsed = new HashSet<int>();

            public static List<Step> GetNextStep(Path path, Graph g, int nodeId, bool VLabelOnly=false)
            {
                _path = path; _g = g; _nodeId = nodeId;
                _VLabelOnly = VLabelOnly;
                _ret.Clear();
                _nodeUsed.Clear();
                _nodeUsed.Add(nodeId);
                Search(0, nodeId);
                List<Step> ret = new List<Step>(_ret);
                return ret;
            }

            static void Search(int depth, int nodeId)
            {
                Vertex v = _g._vertexes[nodeId];
                if (depth == _path.Count)
                {
                    if (!_VLabelOnly)
                    {
                        foreach (int eid in v._assoEdge)
                        {
                            Edge e = _g._edges[eid];
                            if (_nodeUsed.Contains(_g.GetOtherVertexID(nodeId, e)))
                                continue;
                            Step step = new Step(e._eLabel, StepType.LinkTo);
                            _ret.Add(step);
                        }
                    }
                    foreach (int lid in v._vLabel)
                    {
                        Step step = new Step(lid, StepType.Nlabel);
                        _ret.Add(step);
                    }
                }
                else
                {
                    Step step = _path[depth];
                    foreach (int eid in v._assoEdge)
                    {
                        Edge e = _g._edges[eid];
                        int otherV=_g.GetOtherVertexID(nodeId,e);
                        if (e._eLabel == step.Item1 && !_nodeUsed.Contains(otherV))
                        {
                            _nodeUsed.Add(otherV);
                            Search(depth + 1, otherV);
                            _nodeUsed.Remove(otherV);
                        }
                    }
                }
                return;
            }
        }

        public override void Init(Graph g, int minSupp, int maxSize, int[] constraintVSet, bool useVIDList, int maxRadius=100)
        {
            _resultCache.Clear();
            List<Tuple<Path, List<int>>> queue = new List<Tuple<Path, List<int>>>();
            int fronti = 0;
            Path emptyPath = new Path();
            queue.Add(new Tuple<Path, List<int>>(emptyPath, constraintVSet.ToList()));

            Dictionary<Step, List<int>> mapStep2VidsOrCounts = new Dictionary<Step, List<int>>();

            while (fronti < queue.Count)
            {
                mapStep2VidsOrCounts.Clear();
                var pair1 = queue[fronti++];
                Path thisPath = pair1.Item1;
                //Console.WriteLine("Expanding...");
                if (thisPath.Count >= maxSize||thisPath.Count > maxRadius)
                    break;
                List<int> scanedVID=null;
                if (useVIDList)
                    scanedVID = pair1.Item2 as List<int>;
                else
                    scanedVID = constraintVSet.ToList();
                foreach (int i in scanedVID)
                {
                    List<Step> steps = null;
                    if(thisPath.Count==maxRadius)
                        steps=GetNextStepGivenNode.GetNextStep(thisPath, g, i, true);
                    else
                        steps = GetNextStepGivenNode.GetNextStep(thisPath, g, i);
                    foreach (Step step in steps)
                    {
                        if (!mapStep2VidsOrCounts.ContainsKey(step))
                            mapStep2VidsOrCounts[step] = new List<int>();
                        mapStep2VidsOrCounts[step].Add(i);
                    }
                }
                foreach (var pair in mapStep2VidsOrCounts)
                {
                    if (((List<int>)pair.Value).Count < minSupp)
                        continue;

                    //为了不在Egonet里引入牵涉自身Label的Feature
                    if (thisPath.Count == 0 && pair.Key.Item2 == StepType.Nlabel)
                        continue;

                    Path newPath = new Path(thisPath, pair.Key);
                    _resultCache.Add(new Tuple<Path, List<int>>(newPath, pair.Value));
                    if (pair.Key.Item2 != StepType.Nlabel)
                        queue.Add(new Tuple<Path, List<int>>(newPath, pair.Value));
                }
            }
        }
    }

}