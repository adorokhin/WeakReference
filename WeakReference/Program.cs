using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WeakReference
{

    public class MyLittleClass
    {
        public MyLittleClass(uint n)
        {
            this.n = n;
        }
        public uint n;
    }

    public class MyHardClass
    {
        public List<MyLittleClass> hardReferences = new List<MyLittleClass>(4);

        public void Add(MyLittleClass mlc)
        {
            hardReferences.Add(mlc);
        }
        public void Remove(MyLittleClass mlc)
        {
            hardReferences.Remove(mlc);
        }

        public void Dump()
        {
            Console.WriteLine(hardReferences.Count + " elements");
            foreach (var mlc in hardReferences)
            {
                Console.WriteLine(mlc.n);
            }
            Thread.Sleep(3000);
            Console.WriteLine();
        }
    }

    public class MyWeakClass
    {
        public List<WeakReference<MyLittleClass>> weakReferences = new List<WeakReference<MyLittleClass>>();
        public void Add(MyLittleClass mlc)
        {
            weakReferences.Add(new WeakReference<MyLittleClass>(mlc));
        }
        //public void Remove(MyLittleClass mlc)
        //{
        //    l.Remove(new WeakReference<MyLittleClass>(mlc));
        //}

        public void Dump()
        {
            try
            {
                var gcdItems = new List<WeakReference<MyLittleClass>>();   
                
                Console.WriteLine(weakReferences.Count + " elements");
                foreach (WeakReference<MyLittleClass> wr in weakReferences)
                {
                    MyLittleClass target;
                    if (wr.TryGetTarget(out target))
                    {
                        Console.WriteLine(target.n);
                    }
                    else
                    {
                        Console.WriteLine("Reference has been Garbage Collected");
                        //l.Remove(wr);
                        gcdItems.Add(wr);
                    }
                }
                gcdItems.ForEach(d => weakReferences.Remove(d));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally {
                Console.WriteLine();
                Thread.Sleep(3 * 1000);
            }

        }
    }


    class Program
    {

        public static MyHardClass hard = new MyHardClass();
        public static MyWeakClass weak = new MyWeakClass();

        public static void TestMyHardClass()
        {
            var mlc1 = new MyLittleClass(0xAAAAAAAA);
            var mlc2 = new MyLittleClass(2222);
            var mlc3 = new MyLittleClass(3333);
            var mlc4 = new MyLittleClass(4444);

            hard.Add(mlc1);
            hard.Add(mlc2);
            hard.Add(mlc3);
            hard.Add(mlc4);

            #region [ReferenceEquals]
            //Does it point to the same heap location?
            //if (!object.ReferenceEquals(mlc1, hard.hardReferences[0]))
            //    Console.WriteLine("References are different mlc1, hard.hardReferences[0]");
            //else
            //    Console.WriteLine("References are identical mlc1, hard.hardReferences[0]");
            #endregion

            #region [UNSAFE Get memory Locations]
            //unsafe
            //{
            //    TypedReference tr = __makeref(mlc1);
            //    IntPtr ptr1 = **(IntPtr**)(&tr);

            //    MyLittleClass reff = hard.hardReferences[0];
            //    tr = __makeref(reff);
            //    IntPtr ptr2 = **(IntPtr**)(&tr);

            //    Console.WriteLine(ptr1.ToString("X"));
            //    Console.WriteLine(ptr2.ToString("X"));
            //}
            #endregion
        }


        public static void TestMyWeakClass()
        {
            var mlc1 = new MyLittleClass(1111);
            var mlc2 = new MyLittleClass(2222);
            var mlc3 = new MyLittleClass(3333);
            var mlc4 = new MyLittleClass(4444);

            weak.Add(mlc1);
            weak.Add(mlc2);
            weak.Add(mlc3);
            weak.Add(mlc4);

            //mlc1 = null;
            //mlc2 = null;
            //mlc3 = null;
            //mlc4 = null;
        }


        static void Main(string[] args)
        {

            //Enumerable.Range(1, 5).ToList().ForEach(n => u.Add(new MyLittleClass(n)));
            
            //Console.WriteLine("HARD References");
            //TestMyHardClass();
            //Task.Factory.StartNew(() => { while (true) { hard.Dump(); } });

            Console.WriteLine("WEAK References");
            TestMyWeakClass();
            
            for (int i = 0; i < 2; i++)
            {
                weak.Dump();
                Thread.Sleep(1 * 1000);
            }
       
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            Console.WriteLine(">>>> Garbage Collected <<<<");

            Task.Factory.StartNew(() => { while (true) { weak.Dump(); } });
            
            Console.ReadLine();

        }
    }
}
