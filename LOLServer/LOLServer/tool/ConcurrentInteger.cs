using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LOLServer.tool {
    public class ConcurrentInteger {
        int value;
        Mutex tex = new Mutex();
        public ConcurrentInteger() { }
        public ConcurrentInteger(int value) { this.value = value; }
        public int GetAdd() {
            lock(this)
            {
                tex.WaitOne();
                value++;
                tex.ReleaseMutex();
                return value;
            }
        }
        public int GetReduce() {
            lock (this)
            {
                tex.WaitOne();
                value--;
                tex.ReleaseMutex();
                return value;
            }
        }
        public void Reset() {
            lock (this)
            {
                tex.WaitOne();
                value=0;
                tex.ReleaseMutex();
            }
        }
        public int Get() {
            return value;
        }
    }
}
