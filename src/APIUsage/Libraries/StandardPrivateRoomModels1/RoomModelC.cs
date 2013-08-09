using System;
using System.Collections.Generic;
using System.Drawing;

using Bluedot.HabboServer.ApiUsage.Libraries.Subscriptions;
using Bluedot.HabboServer.Database;
using Bluedot.HabboServer.Database.Actions;
using Bluedot.HabboServer.Useful;
using Bluedot.HabboServer.Rooms.Figure;
using Bluedot.HabboServer.Habbos.Messenger;
using Bluedot.HabboServer.Network;
using Bluedot.HabboServer.Permissions;

namespace Bluedot.HabboServer.Rooms
{
    [RoomModelAttribute("c")]
    public class RoomModelC : PrivateRoom
    {
        #region Properties
        #region Property:BaseHeightMap
        private static readonly sbyte[,] _baseHeightMap = new sbyte[,]
                                                    {
                                                        {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, },
                                                        {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, },
                                                        {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, },
                                                        {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, },
                                                        {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, },
                                                        {-1, -1, -1, -1, -1, 0, 0, 0, 0, 0, 0, -1, },
                                                        {-1, -1, -1, -1, -1, 0, 0, 0, 0, 0, 0, -1, },
                                                        {-1, -1, -1, -1, -1, 0, 0, 0, 0, 0, 0, -1, },
                                                        {-1, -1, -1, -1, -1, 0, 0, 0, 0, 0, 0, -1, },
                                                        {-1, -1, -1, -1, -1, 0, 0, 0, 0, 0, 0, -1, },
                                                        {-1, -1, -1, -1, -1, 0, 0, 0, 0, 0, 0, -1, },
                                                        {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, },
                                                        {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, },
                                                        {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, },
                                                        {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, },
                                                        {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, }
                                                    };
        public override sbyte[,] BaseHeightMap
        {
            get
            {
                return _baseHeightMap;
            }
        }
        #endregion

        #region Property: Size
        private static readonly Size _size = new Size(_baseHeightMap.GetLength(1), _baseHeightMap.GetLength(0));
        public override Size Size
        {
            get
            {
                return _size;
            }
        }
        #endregion

        #region Property: Door
        private static readonly RoomPosition _door = new RoomPosition(4, 7, 0, 0);
        public override RoomPosition Door
        {
            get
            {
                return _door;
            }
        }
        #endregion

        #region Property: ModelName
        public override string ModelName
        {
            get
            {
                return "model_c";
            }
        }
        #endregion
        #endregion

        #region Methods
        #region Method: RoomModelC (Constructor)
        public RoomModelC(int id) : base(id) {}
        #endregion
        #endregion
    }
}