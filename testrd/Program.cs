using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace testrd
{
   public class Request
    {
        static private int counElements;
        private int id;
        Request()
        {
            this.id = counElements++;
        }
        public static Request GetRequest(Stopper stopSignal) 
        {
                //const int MAX_WAIT_MS = 100;
                //int waitMs = rand() % MAX_WAIT_MS + 1;
                //Sleep(waitMs); // emulate long-time operations
 
                return stopSignal.IsStop? null : (new Request());
        }
 
 
        /* Function from the task specification*/
        public static void ProcessRequest(Request request, Stopper stopSignal)
        {
                if(stopSignal.IsStop)
                        return;
 
                ///* some processig there */
                //const int MAX_WAIT_MS = 1000;
                //int waitMs = rand() % MAX_WAIT_MS + 1;
                //Sleep(waitMs); // emulate long-time operations
        }
    }
 
    public class Stopper
    {
        private static event Action changeIsStopEvent; 
        private bool isstop;
        private object lockStopper;
        public bool IsStop
        {
            get
            {
                lock (lockStopper)
                {
                    return isstop;
                }
            }
        }

        void changeIsStop()
        {
            lock (lockStopper)
            {
                this.isstop = true;
            }
        }
        Stopper()
        {
            this.isstop = false;
            this.lockStopper = new object();
            changeIsStopEvent +=this.changeIsStop;
        }
    }


    abstract  class MyThread
    {
        protected  Thread thrd;

        public MyThread()
        {
            thrd = new Thread(this.Run);
            thrd.Start();
        }

        abstract  protected virtual void Run();
    }

    struct Book
    {
        public Stopper name;
        public Request author;

    }



    public class listenerThread:MyThread
    {
        private Queue<Dictionary <Stopper,Request>> QueueStop;

        private object lockQueueDictionary;

        listenerThread(Queue<Dictionary<Stopper, Request>> qd, object lockqd)
            : base()
        {
            this.QueueStop = qd;
            this.lockQueueDictionary = lockqd;
        }

        private override void Run()
        {
            while (true)
            {
                Request request=null;
                
                lock (lockQueueStop)
                {
                    if (QueueStop.Count != 0)
                    {
                        request = Request.GetRequest(QueueStop.Dequeue());
                    }
                }
                if (request != null)
                {
                    lock (lockQueueRequest)
                    {
                        QueueRequest.Enqueue(request);
                    }
                }
 
            }
        }

       public void addRequest(Stopper stop)
        {
            Dictionary<Stopper, Request> obj = new Dictionary<Stopper, Request>();
           obj.Add
            lock (lockQueueDictionary)
            {
                QueueStop.Enqueue(new Dictionary<Stopper, Request>().Add(stop,null));
            }
        }
    }
    public class ProcessManager
    {
        private Queue<Stopper> QueueStop;

        private Queue<Request> QueueRequest;

        private object lockQueueRequest;

        private object lockQueueStop = new object();

        ProcessManager(Queue<Request> qr, object lockqr)
        {
            this.QueueStop = new Queue<Stopper>();
            this.QueueRequest = qr;
            this.lockQueueRequest = lockqr;
            Thread thread = new Thread(runThreadProcess);
            thread.Start();
        }

        private void runThreadProcess()
        {
            while (true)
            {
                Request request = null;

                lock (lockQueueStop)
                {
                    if (QueueStop.Count != 0)
                    {
                        request = Request.GetRequest(QueueStop.Dequeue());
                    }
                }
                if (request != null)
                {
                    lock (lockQueueRequest)
                    {
                        QueueRequest.Enqueue(request);
                    }
                }

            }
        }

        public void addRequest(Stopper stop)
        {
            lock (lockQueueStop)
            {
                QueueStop.Enqueue(stop);
            }
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
