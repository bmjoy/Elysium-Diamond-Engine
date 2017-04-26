﻿using WorldServer.Common;
using WorldServer.Server;

namespace WorldServer.Network {
    public class GamePacket {
        /// <summary>
        /// Envia todos os dados para o GameServer selecionado pelo cliente / servidor.
        /// </summary>
        /// <param name="pIndex"></param>
        /// <param name="worldID"></param>
        public static void Login(string hexID, int serverID) {
            var pData = Authentication.FindByHexID(hexID);
            var buffer = GameNetwork.GameServer[serverID].Socket.CreateMessage();
            buffer.Write((int)PacketList.WS_GS_GameServerLogin);
            buffer.Write(pData.HexID);
            buffer.Write(pData.Account);
            buffer.Write(pData.AccountID);
            buffer.Write(pData.LanguageID);
            buffer.Write(pData.AccessLevel);
            buffer.Write(pData.CharacterID);
            buffer.Write(pData.CharSlot);

            //pega a quantidade de serviços
            var servicesID = pData.Service.GetServicesID();
            buffer.Write(servicesID.Length);

            //escreve cada um no buffer
            foreach(var id in servicesID) buffer.Write(pData.Service.GetService(id));

            GameNetwork.GameServer[serverID].SendData(buffer);  
        }

        /// <summary>
        /// Envia o HexID para o game server.
        /// </summary>
        /// <param name="index"></param>
        public static void HexID(int index) {
            var buffer = GameNetwork.GameServer[index].Socket.CreateMessage();
            buffer.Write((int)PacketList.CL_WS_SendPlayerHexID);
            buffer.Write(Configuration.ID);

            GameNetwork.GameServer[index].SendData(buffer);
        }
    }
}