using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CalculationEngine.Models
{
    public class Vertex<T>
    {
        public Vertex(T data)
        {
            Data = data;
            InVertices = new HashSet<Vertex<T>>();
            OutVertices = new HashSet<Vertex<T>>();
        }
        
        public bool Accessed { get; set; }

        public T Data { get; private set; }
            
        public HashSet<Vertex<T>> InVertices { get; private set; }

        public HashSet<Vertex<T>> OutVertices { get; private set; }

        public bool EqualsVertex(Vertex<T> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Data, Data);
        }

        public override int GetHashCode()
        {
            return Data.GetHashCode();
        }

        public void AddOutVertices(Vertex<T> vertex)
        {
            if (Equals(vertex, this))
            {
                return;
            }
            InVertices.Add(vertex);
        }

        public void AddInVertices(Vertex<T> vertex)
        {
            if (Equals(vertex, this))
            {
                return;
            }
            OutVertices.Add(vertex);
        }
    }
}
