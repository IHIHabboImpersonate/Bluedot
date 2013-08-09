#region Usings

using System;
using System.Collections.Generic;

#endregion

namespace IHI.Server.Rooms.Figure
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


                ushort setId;

                // Parse the set ID.
                if (!ushort.TryParse(setStrings[1], out setId))
                    throw new FormatException("Figure set ID is not a valid ushort in '" + setTypeString + "'.");

                // Make sure the set ID is registered.
                if (!_figureSetIDs.ContainsKey(setId))
                    throw new KeyNotFoundException("Figure set ID " + setId + " is not registered.");

                // Create a new instance of the set type.
                FigureSet set = _figureSetIDs[setId]
                                      .GetConstructor(new Type[0])
                                      .Invoke(new object[0]) as FigureSet;

                // Was a primary colour provided?
                if (setStrings.Length > 2)
                {
                    ushort colourId;

                    // Parse ColourID and validate it.
                    if (!ushort.TryParse(setStrings[2], out colourId))
                        throw new FormatException("Figure Colour ID is not a valid ushort in '" + setTypeString + "'.");

                    //Set PrimaryColour for this set.
                    set.PrimaryColour = colourId;

                    // Was a secondary colour provided?
                    if (setStrings.Length > 3)
                    {
                        // Parse ColourID and validate it.
                        if (!ushort.TryParse(setStrings[3], out colourId))
                            throw new FormatException("Figure ColourID is not a valid ushort in '" + setTypeString + "'.");

                        // Set the SecondaryColour for this set.
                        set.SecondaryColour = colourId;
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
                                throw new InvalidCastException("Figure set ID " + setId +" is a valid figure model but not a valid body set.");

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
                ushort setId = (set.GetConstructors()[0].Invoke(new object[0]) as FigureSet).Id;
                _figureSetIDs.Add(setId, set);
            }
            return this;
        }

        internal HabboFigureFactory UnregisterModelId(Type part)
        {
            if (part.IsSubclassOf(typeof (FigureSet)))
            {
                ushort modelId = (part.GetConstructors()[0].Invoke(new object[0]) as FigureSet).Id;
                if (_figureSetIDs.ContainsKey(modelId))
                    _figureSetIDs.Remove(modelId);
            }
            return this;
        }
    }
}