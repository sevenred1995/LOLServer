using System;
using System.Collections.Generic;
using System.Text;

namespace NetFrame
{
    /// <summary>
    /// 连接对象池
    /// </summary>
    public class UserTokenPool
    {
        private Stack<UserToken> pool;
        /// <summary>
        /// 获取当前剩余可连接对象的数量
        /// </summary>
        public int Size
        {
            get { return pool.Count; }
        }
        public UserTokenPool(int max)
        {
            pool = new Stack<UserToken>(max);//实例化集合的大小-可以连接的对象数量
        }
        /// <summary>
        /// 需要使用的时候取出一个连接对象
        /// </summary>
        /// <returns></returns>
        public UserToken pop()
        {
            return pool.Pop();
        }
        /// <summary>
        /// 断开连接时候将连接对象放回对象池
        /// </summary>
        /// <param name="token">断开连接的对象</param>
        public void push(UserToken token)
        {
            if(token!=null)
            {
                pool.Push(token);
            } 
        }
    }
}
