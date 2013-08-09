using System;
using System.Collections.Generic;
using System.Drawing;

using IHI.Server.Libraries.Subscriptions;
using IHI.Server.Database;
using IHI.Server.Database.Actions;
using IHI.Server.Useful;
using IHI.Server.Rooms.Figure;
using IHI.Server.Habbos.Messenger;
using IHI.Server.Network;
using IHI.Server.Permissions;

namespace IHI.Server.Rooms
{
    [RoomModelAttribute("f")]
    public class RoomModelF : PrivateRoom
    {
        #region Properties
        #region Property:BaseHeightMap
        private static readonly sbyte[,] _baseHeightMap = new sbyte[,]
                                                    {
	                                                    {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, },
	                                                    {-1, -1, -1, -1, -1, -1, -1, 0, 0, 0, 0, -1, },
	                                                    {-1, -1, -1, -1, -1, -1, -1, 0, 0, 0, 0, -1, },
	                                                    {-1, -1, -1, 0, 0, 0, 0, 0, 0, 0, 0, -1, },
	                                                    {-1, -1, -1, 0, 0, 0, 0, 0, 0, 0, 0, -1, },
	                                                    {-1, -1, -1, 0, 0, 0, 0, 0, 0, 0, 0, -1, },
	                                                    {-1, -1, -1, 0, 0, 0, 0, 0, 0, 0, 0, -1, },
	                                                    {-1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -1, },
	                                                    {-1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -1, },
	                                                    {-1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -1, },
	                                                    {-1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -1, },
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

        private static readonly RoomPosition _door = new RoomPosition(2, 5, 0, 0);
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
                return "model_f";
            }
        }
        #endregion
        #endregion

        #region Methods
        #region Method: RoomModelF (Constructor)
        public RoomModelF(int id) : base(id) {}
        #endregion
        #endregion
    }
}