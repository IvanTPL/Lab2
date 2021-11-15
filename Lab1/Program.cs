using System;
using System.Numerics;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace Lab1
{
    struct DataItem
    {
        public double x { get; set; }
        public double y { get; set; }
        public Vector2 vector { get; set; }
        public DataItem(double x, double y, Vector2 vector)
        {
            this.x = x;
            this.y = y;
            this.vector = vector;
        }
        public string ToLongString(string format)
        {
            return $"Coordinates: [{x.ToString(format)}; {y.ToString(format)}]; vector: [{vector.X.ToString(format)}; {vector.Y.ToString(format)}]; Length = {vector.Length().ToString(format)}";
        }
        public override string ToString()
        {
            return $"x = {x}; y = {y}; vector: [{vector}]";
        }
    }

    delegate Vector2 FdblVector2(double x, double y);

    abstract class V3Data : IEnumerable<DataItem>
    {
        public string str { get; set; }
        public DateTime date { get; set; }
        public V3Data(string str, DateTime date)
        {
            this.str = str;
            this.date = date;
        }
        protected int count;
        public abstract int Count { get; }
        protected double maxDistance;
        public abstract double MaxDistance { get; }
        public abstract string ToLongString(string format);
        public override string ToString()
        {
            return $"str: {str}; date: [{date}]; count = {Count}; maxdistance = {MaxDistance}";
        }
        public abstract IEnumerator<DataItem> GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
        IEnumerator<DataItem> IEnumerable<DataItem>.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    class V3DataList : V3Data, IEnumerable<DataItem>
    {
        public List<DataItem> lst { get; }
        public V3DataList(string str, DateTime date) : base(str, date)
        {
            lst = new List<DataItem>();
        }
        public bool Add(DataItem newItem)
        {
            if (lst.Exists(t => t.x == newItem.x && t.y == newItem.y))
                return false;
            else
            {
                lst.Add(newItem);
                return true;
            }
        }
        public int AddDefaults(int nItems, FdblVector2 F)
        {
            int cnt = 0;
            for (int i = 1; i <= nItems; i++)
            {
                DataItem a = new DataItem(i * i, i * i * 2, F(i * i, i * i * 2));
                if (Add(a))
                    cnt++;
            }
            return cnt;
        }
        public override int Count
        {
            get { return lst.Count; }
        }
        public override double MaxDistance
        {
            get
            {
                double md = 0, dx, dy, d;
                for (int i = 0; i < lst.Count; i++)
                {
                    for (int j = i + 1; j < lst.Count; j++)
                    {
                        dx = lst[j].x - lst[i].x;
                        dy = lst[j].y - lst[i].y;
                        d = Math.Sqrt(dx * dx + dy * dy);
                        if (d > md)
                            md = d;
                    }
                }
                return md;
            }
        }
        public override string ToString()
        {
            return $"Typename: {this.GetType()}; {base.ToString()}";
        }
        public override string ToLongString(string format)
        {
            string s = $"Typename: {this.GetType()}; str: {str}; date: [{date}]; count = {Count}; maxdistance = {MaxDistance.ToString(format)}";
            s += "\n" + "List:";
            for (int i = 0; i < lst.Count; i++)
                s += "\n" + lst[i].ToLongString(format);
            return s;
        }
        public override IEnumerator<DataItem> GetEnumerator()
        {
            return lst.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return lst.GetEnumerator();
        }
        IEnumerator<DataItem> IEnumerable<DataItem>.GetEnumerator()
        {
            return lst.GetEnumerator();
        }
    }

    class V3DataArray : V3Data, IEnumerable<DataItem>
    {
        public int n_x { get; }
        public int n_y { get; }
        public double step_x { get; }
        public double step_y { get; }
        public Vector2[,] arr { get; }
        public V3DataArray(string str, DateTime date) : base(str, date)
        {
            arr = new Vector2[0, 0];
        }
        public V3DataArray(string str, DateTime date, int n_x, int n_y, double step_x, double step_y, FdblVector2 F) : base(str, date)
        {
            this.n_x = n_x;
            this.n_y = n_y;
            this.step_x = step_x;
            this.step_y = step_y;
            arr = new Vector2[n_x, n_y];
            for (int i = 0; i < n_x; i++)
            {
                for (int j = 0; j < n_y; j++)
                {
                    arr[i, j] = F(i * step_x, j * step_y);
                }
            }
        }
        public override int Count
        {
            get { return n_x * n_y; }
        }
        public override double MaxDistance
        { 
            get 
            {
                if (Count == 0)
                    return 0;
                else
                    return Math.Sqrt((n_x - 1) * step_x * (n_x - 1) * step_x + (n_y - 1) * step_y * (n_y - 1) * step_y);  
            }
        }
        public override string ToString()
        {
            return $"Typename: {this.GetType()}; {base.ToString()}; n_x = {n_x}; n_y = {n_y}; step_x = {step_x}; step.y = {step_y}";
        }
        public override string ToLongString(string format)
        {
            string s = $"Typename: {this.GetType()}; str: {str}; date: [{date}]; count = {Count}; maxdistance = {MaxDistance.ToString(format)};";
            s += $" n_x = {n_x}; n_y = {n_y}; step_x = {step_x.ToString(format)}; step_y = {step_y.ToString(format)}";
            s += "\n" + "Array:";
            for (int i = 0; i < n_x; i++)
            {
                for (int j = 0; j < n_y; j++)
                {
                    s += $"\nCoordinates: [{(i * step_x).ToString(format)}; {(j * step_y).ToString(format)}]; vector: [{arr[i, j].X.ToString(format)}; {arr[i, j].Y.ToString(format)}]; length = {arr[i, j].Length().ToString(format)}";
                }
            }
            return s;
        }
        public static explicit operator V3DataList(V3DataArray dataArray)
        {
            V3DataList dataList = new V3DataList(dataArray.str, dataArray.date);
            for (int i = 0; i < dataArray.n_x; i++)
            {
                for (int j = 0; j < dataArray.n_y; j++)
                {
                    dataList.Add(new DataItem(i * dataArray.step_x, j * dataArray.step_y, dataArray.arr[i, j]));
                }
            }
            return dataList;
        }
        public override IEnumerator<DataItem> GetEnumerator()
        {
            for (int i = 0; i < n_x; i++)
            {
                for (int j = 0; j < n_y; j++)
                {
                    DataItem dataItem = new DataItem(i * step_x, j * step_y, arr[i, j]);
                    yield return dataItem;
                }
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        IEnumerator<DataItem> IEnumerable<DataItem>.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

    class V3MainCollection
    {
        protected DataItem? ret;
        /*public DataItem? Ret
        {
            get 
            { 

            }
        }*/
        private List<V3Data> lst;
        protected int count;
        public int Count
        {
            get { return lst.Count; }
        }
        public V3MainCollection()
        {
            lst = new List<V3Data>();
        }
        public V3Data this[int index]
        { 
            get { return lst[index]; }
        }
        public bool Contains(string ID)
        {
            if (lst.Exists(t => t.str == ID))
                return true;
            else
                return false;
        }
        public bool Add(V3Data v3Data)
        {
            if (Contains(v3Data.str))
                return false;
            else
            {
                lst.Add(v3Data);
                return true;
            }
        }
        public string ToLongString(string format)
        {
            string s = "";
            for (int i = 0; i < Count - 1; i++)
                s += lst[i].ToLongString(format) + "\n";
            s += lst[Count - 1].ToLongString(format);
            return s;
        }
        public override string ToString()
        {
            string s = "";
            for (int i = 0; i < Count - 1; i++)
                s += lst[i].ToString() + "\n";
            s += lst[Count - 1].ToString();
            return s;
        }
    }

    static class Methods
    {
        public static Vector2 F1(double x, double y)
        {
            return new Vector2((float)(x * 2), (float)(y * 3));
        }
        public static Vector2 F2(double x, double y)
        {
            return new Vector2((float)(x * 4), (float)(y * 5));
        }
        public static Vector2 F3(double x, double y)
        {
            return new Vector2((float)(x * 5), (float)(y * 6));
        }
    }
    
    class Program
    {
        static void Main(string[] args)
        {
            V3DataArray a = new V3DataArray("ID_1", DateTime.Now, 2, 2, 2.5, 2.5, Methods.F1);
            Console.WriteLine(a.ToLongString("F3"));
            V3DataList a_lst = (V3DataList)a;
            Console.WriteLine(a_lst.ToLongString("F3"));
            Console.WriteLine($"Array: count = {a.Count}; maxdistance = {a.MaxDistance}");
            Console.WriteLine($"List from array: count = {a_lst.Count}; maxdistance = {a_lst.MaxDistance}");

            Console.WriteLine("");
            V3DataArray b = new V3DataArray("ID_2", DateTime.Now, 2, 2, 4.5, 4.5, Methods.F1);
            V3DataList c = new V3DataList("ID_3", DateTime.Now);
            V3DataList d = new V3DataList("ID_4", DateTime.Now);
            c.AddDefaults(2, Methods.F2);
            d.AddDefaults(2, Methods.F3);
            V3MainCollection clct = new V3MainCollection();
            clct.Add(a);
            clct.Add(b);
            clct.Add(c);
            clct.Add(d);
            Console.WriteLine(clct.ToLongString("F3"));

            Console.WriteLine("");
            for (int i = 0; i < clct.Count; i++)
            {
                Console.WriteLine($"Collection #{i + 1}: count = {clct[i].Count}; maxdistance = {clct[i].MaxDistance}");
            }
            /*foreach(DataItem dat in a)
            {
                Console.WriteLine(dat);
            }*/

        }
    }
}
