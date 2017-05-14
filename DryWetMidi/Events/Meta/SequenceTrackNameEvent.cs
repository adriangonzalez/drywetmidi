﻿namespace Melanchall.DryWetMidi.Smf
{
    public sealed class SequenceTrackNameEvent : BaseTextEvent
    {
        #region Constructor

        public SequenceTrackNameEvent()
        {
        }

        public SequenceTrackNameEvent(string text)
            : base(text)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Determines whether the specified event is equal to the current one.
        /// </summary>
        /// <param name="sequenceTrackNameEvent">The event to compare with the current one.</param>
        /// <returns>true if the specified event is equal to the current one; otherwise, false.</returns>
        public bool Equals(SequenceTrackNameEvent sequenceTrackNameEvent)
        {
            return Equals(sequenceTrackNameEvent, true);
        }

        /// <summary>
        /// Determines whether the specified event is equal to the current one.
        /// </summary>
        /// <param name="sequenceTrackNameEvent">The event to compare with the current one.</param>
        /// <param name="respectDeltaTime">If true the <see cref="MidiEvent.DeltaTime"/> will be taken into an account
        /// while comparing events; if false - delta-times will be ignored.</param>
        /// <returns>true if the specified event is equal to the current one; otherwise, false.</returns>
        public bool Equals(SequenceTrackNameEvent sequenceTrackNameEvent, bool respectDeltaTime)
        {
            if (ReferenceEquals(null, sequenceTrackNameEvent))
                return false;

            if (ReferenceEquals(this, sequenceTrackNameEvent))
                return true;

            return base.Equals(sequenceTrackNameEvent, respectDeltaTime);
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Clones event by creating a copy of it.
        /// </summary>
        /// <returns>Copy of the event.</returns>
        protected override MidiEvent CloneEvent()
        {
            return new SequenceTrackNameEvent(Text);
        }

        public override string ToString()
        {
            return $"Sequence/Track Name (text = {Text})";
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as SequenceTrackNameEvent);
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion
    }
}
