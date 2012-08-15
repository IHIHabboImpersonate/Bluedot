#region GPLv3

// 
// Copyright (C) 2012  Chris Chenery
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General internal License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General internal License for more details.
// 
// You should have received a copy of the GNU General internal License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

#endregion

#region Usings

using System;
using System.Collections.Generic;

#endregion

namespace Bluedot.HabboServer.Habbos.Figure
{
    public class HabboFigureFactory
    {
        private readonly Dictionary<ushort, Type> _figureSetIDs;

        public HabboFigureFactory()
        {
            _figureSetIDs = new Dictionary<ushort, Type>();
        }

        public HabboFigure Parse(string figureString, bool gender)
        {
            // TODO: Possible optimisation: Store unparsed string. Don't parse till needed. (Lazy parsing)

            // Create a new instance of HabboFigure to work on.
            HabboFigure figureInProgress = new HabboFigure
                                               {
                                                   Gender = gender
                                               };
            
            // Split the input string into set types.
            string[] setTypeStrings = figureString.Split(new[] {'.'});

            // For each set type...
            foreach (string setTypeString in setTypeStrings)
            {
                // Split it into set type ID and colours.
                string[] setStrings = setTypeString.Split(new[] { '-' });


                ushort setID;

                // Parse the set ID.
                if (!ushort.TryParse(setStrings[1], out setID))
                    throw new FormatException("Figure set ID is not a valid ushort in '" + setTypeString + "'.");

                // Make sure the set ID is registered.
                if (!_figureSetIDs.ContainsKey(setID))
                    throw new KeyNotFoundException("Figure set ID " + setID + " is not registered.");

                // Create a new instance of the set type.
                FigureSet set = _figureSetIDs[setID]
                                      .GetConstructor(new Type[0])
                                      .Invoke(new object[0]) as FigureSet;

                // Was a primary colour provided?
                if (setStrings.Length > 2)
                {
                    ushort colourID;

                    // Parse ColourID and validate it.
                    if (!ushort.TryParse(setStrings[2], out colourID))
                        throw new FormatException("Figure Colour ID is not a valid ushort in '" + setTypeString + "'.");

                    //Set PrimaryColour for this set.
                    set.PrimaryColour = colourID;

                    // Was a secondary colour provided?
                    if (setStrings.Length > 3)
                    {
                        // Parse ColourID and validate it.
                        if (!ushort.TryParse(setStrings[3], out colourID))
                            throw new FormatException("Figure ColourID is not a valid ushort in '" + setTypeString + "'.");

                        // Set the SecondaryColour for this set.
                        set.SecondaryColour = colourID;
                    }
                }


                // This set is a...
                switch (setStrings[0])
                {
                        // Shirt
                    case "hd":
                        {
                            // Verify this model is a shirt.
                            if (!(set is Body))
                                throw new InvalidCastException("Figure set ID " + setID +" is a valid figure model but not a valid body set.");

                            // Apply the set to the HabboFigure
                            figureInProgress.Body = set as Body;
                            break;
                        }
                    default:
                        continue; // Not a valid set? Ignore it.
                }
            }

            return figureInProgress;
        }

        internal HabboFigureFactory RegisterSet(Type set)
        {
            if (set.IsSubclassOf(typeof (FigureSet)))
            {
                ushort setID = (set.GetConstructors()[0].Invoke(new object[0]) as FigureSet).Id;
                _figureSetIDs.Add(setID, set);
            }
            return this;
        }

        internal HabboFigureFactory UnregisterModelID(Type part)
        {
            if (part.IsSubclassOf(typeof (FigureSet)))
            {
                ushort modelID = (part.GetConstructors()[0].Invoke(new object[0]) as FigureSet).Id;
                if (_figureSetIDs.ContainsKey(modelID))
                    _figureSetIDs.Remove(modelID);
            }
            return this;
        }
    }
}