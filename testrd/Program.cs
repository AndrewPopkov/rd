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

            return stopSignal.IsStop ? null : (new Request());
        }


        /* Function from the task specification*/
        public static void ProcessRequest(Request request, Stopper stopSignal)
        {
            if (stopSignal.IsStop)
                return;

            ///* some processig there */
            //const int MAX_WAIT_MS = 1000;
            //int waitMs = rand() % MAX_WAIT_MS + 1;
            //Sleep(waitMs); // emulate long-time operations
        }
    }

    public class Stopper
    {
        public static event Action changeAllStopEvent;
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
            set
            {
                lock (lockStopper)
                {
                    isstop = value;
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
        public Stopper()
        {
            this.isstop = false;
            this.lockStopper = new object();
            changeAllStopEvent += this.changeIsStop;
        }
    }


    abstract class MyThread
    {
        protected Thread thrd;

        public MyThread()
        {

            this.thrd = new Thread(this.Run);
            this.thrd.Start();
        }

        abstract protected virtual void Run();

    }

    public class QueueSynchronization<T>
    {
        private Queue<T> QueueObj;
        private object lockQueue;

        public QueueSynchronization()
        {
            this.lockQueue = new object();
            this.QueueObj = new Queue<T>();
        }


        public void AddQueue(T t)
        {
            lock (lockQueue)
            {
                QueueObj.Enqueue(t);
            }
        }

        public T RemoveQueue()
        {
            lock (lockQueue)
            {
                return QueueObj.Dequeue();
            }
        }

        public int CountQueue()
        {
            lock (lockQueue)
            {
                return QueueObj.Count;
            }
        }
    }

    struct Element
    {
        public Stopper stopSignal;
        public Request request;
        public Element(Stopper _stopSignal, Request _request)
        {
            this.stopSignal = _stopSignal;
            this.request = _request;
        }
    }



    public class ProducerThread : MyThread, IDisposable
    {
        private QueueSynchronization<Element> QueueCommon;

        private QueueSynchronization<Stopper> QueueStop;

        private bool doing;

        public ProducerThread(QueueSynchronization<Element> qc)
            : base()
        {
            this.QueueCommon = qc;
            this.doing = true;
            this.QueueStop = new QueueSynchronization<Stopper>();
        }

        private override void Run()
        {
            while (doing)
            {
                Request request = null;
                Stopper stop = null;
                if (QueueStop.CountQueue() != 0)
                {
                    stop = QueueStop.RemoveQueue();
                    request = Request.GetRequest(stop);
                }
                else
                {
                    Thread.Sleep(5000);
                }
                if (request != null && stop != null)
                {
                    QueueCommon.AddQueue(new Element(stop, request));
                }
            }
        }
        public void cancel()
        {
            this.doing = false;
        }
        public void addRequest(Stopper stop)
        {
            QueueStop.AddQueue(stop);
        }
        public void Dispose()
        {
            cancel();
        }
    }

    public class ConsumerThread : MyThread, IDisposable
    {
        private QueueSynchronization<Element> QueueCommon;

        private bool doing;

        ConsumerThread(QueueSynchronization<Element> qc)
            : base()
        {
            this.QueueCommon = qc;
            this.doing = true;
        }

        private override void Run()
        {
            while (doing)
            {
                Element element;
                if (QueueCommon.CountQueue() != 0)
                {
                    element = QueueCommon.RemoveQueue();
                    Request.ProcessRequest(element.request, element.stopSignal);
                }
                else
                {
                    Thread.Sleep(5000);
                }
            }
        }

        private void cancel()
        {
            this.doing = false;
        }

        public void Dispose()
        {
            cancel();
        }

    }

    public class ManagerThread
    {
        private QueueSynchronization<Element> QueueCommon;

        private int CountConsumerThread;

        private int timeWork;

        ManagerThread(int _CountConsumerThread, int _timeWork)
        {
            this.QueueCommon = new QueueSynchronization<Element>();

            this.CountConsumerThread = _CountConsumerThread;

            this.CountConsumerThread = _timeWork;

        }

        void Work()
        {

        }
    }

    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
