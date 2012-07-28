using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIProject
{
    public class Buffer<T>
    {
        private T[] objects;
        private int count;
        private int counter;

        public Buffer(int count)
        {
            objects = new T[count];
            this.count = count;
            counter = 0;
        }

        public void Add(T obj)
        {
            if (counter > count - 1)
            {
                return;
            }
            objects[counter] = obj;
            counter++;
        }

        public bool Contains(T obj)
        {
            for (int i = 0; i < GetCount(); i++)
            {
                if (obj.Equals(this[i]))
                {
                    return true;
                }
            }

            return false;
        }

        public void Clear()
        {
            counter = 0;
        }

        public int GetCapacity()
        {
            return count;
        }

        public int GetCount()
        {
            return counter;
        }

        public T this[int i]
        {
            get
            {
                return objects[i];
            }
            set
            {
                objects[i] = value;
            }
        }
    }
}
