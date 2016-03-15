using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LOLServer.tool {

    public delegate void ExcutorDelegate();
    /// <summary>
    /// 单线程处理事务对象 将所有事物处理调用 同时此处调用
    /// </summary>
    public class ExcutorPool {
        private static ExcutorPool instance;
        /// <summary>
        /// 线程同步锁
        /// </summary>
        Mutex tex = new Mutex();
        public static ExcutorPool Instance {
            get {
                if(instance==null)
                {
                    instance = new ExcutorPool();
                }
                return instance;
            }
        }
        public void excute(ExcutorDelegate d) {
            lock(this)
            {
                tex.WaitOne();//锁住当前信号
                d();
                tex.ReleaseMutex();//释放当前信号量
            }
        }

    }
}
