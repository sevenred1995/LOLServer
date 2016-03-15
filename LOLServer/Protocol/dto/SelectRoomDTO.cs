using System;
using System.Collections.Generic;
using System.Text;

namespace GameProtocol.dto {
    [Serializable]
    public class SelectRoomDTO {
        //该房间的所以信息
        public SelectModel[] teamOne;
        public SelectModel[] teamTwo;
        public int getTeam(int userId) {
            foreach (SelectModel item in teamOne)
            {
                if(item.userID==userId)
                {
                    return 1;
                }
            }
            foreach(SelectModel item in teamTwo)
            {
                if(item.userID==userId)
                {
                    return 2;
                }
            }
            return -1;
        }
    }
}
