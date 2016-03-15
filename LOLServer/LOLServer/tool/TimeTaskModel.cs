using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOLServer.tool {
    public class TimeTaskModel {
        //任务逻辑
        public TimeEvent execut;
        //任务执行时间
        public long time;
        //任务id
        public int id;

        public TimeTaskModel(int id, TimeEvent execut, long time) {
            this.id = id;
            this.execut = execut;
            this.time = time;
        }
        public void run() {
            execut();
        }
    }
}
