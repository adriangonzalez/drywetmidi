﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Melanchall.DryWetMidi.Smf
{
    internal sealed class MultiSequenceChunksConverter : IChunksConverter
    {
        #region IChunksConverter

        public IEnumerable<MidiChunk> Convert(IEnumerable<MidiChunk> chunks)
        {
            if (chunks == null)
                throw new ArgumentNullException(nameof(chunks));

            var trackChunks = chunks.OfType<TrackChunk>().ToArray();
            if (trackChunks.Length == 0)
                return chunks;

            var sequenceNumbers = trackChunks.Select((c, i) => new { Chunk = c, Number = GetSequenceNumber(c) ?? i })
                                             .ToArray();

            var singleTrackChunksConverter = ChunksConverterFactory.GetConverter(MidiFileFormat.SingleTrack);
            return sequenceNumbers.GroupBy(n => n.Number)
                                  .SelectMany(g => singleTrackChunksConverter.Convert(g.Select(n => n.Chunk)))
                                  .Concat(chunks.Where(c => !(c is TrackChunk)));
        }

        #endregion

        #region Methods

        private static short? GetSequenceNumber(TrackChunk trackChunk)
        {
            if (trackChunk == null)
                throw new ArgumentNullException(nameof(trackChunk));

            return trackChunk.Events
                             .TakeWhile(m => m.DeltaTime == 0)
                             .OfType<SequenceNumberEvent>()
                             .FirstOrDefault()
                             ?.Number;
        }

        #endregion
    }
}
